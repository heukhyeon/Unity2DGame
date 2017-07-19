using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DictionaryScene : MonoBehaviour,AppSceneScript {
    public Transform CrossWordPuzzleSpace; //10*10 퍼즐을 가지고있는 객체
    public GameObject CrossWordBlock; //입력블록 한칸
    public Text hintspace; //입력블록 클릭시 힌트가 출력되는 공간
    public Button leftbutton; //왼쪽 블록 이동 버튼
    public Button rightbutton; //오른쪽 블록 이동버튼
    public Button upbutton; //위쪽 블록 이동버튼
    public Button downbutton; //아래쪽 블록 이동 버튼
    public SpeechEvent speechEvent; //대사 이벤트
    public Text Timer; //남은 시간 출력 텍스트
    public float max_time = 0; //남은 시간
    private char[,] answer = new char[10, 10]; //정답 char 배열
    private char[,] inputword = new char[10, 10]; //현재 입력된 char 배열
    private InputField nowselectblock; //현재 선택된 블록. 해당 블록의 상하좌우 블록의 상태에 따라 이동 버튼의 상태가 바뀜.
    private bool isEvent = false; //해당 변수가 true일시 타이머가 정지됨.
    private void Awake()
    {
        DictionaryAppInfo info = StaticManager.Nowappinfo.dictionaryinfo;
        Dictionary<int, string> streethints = new Dictionary<int, string>(); //가로 문제들의 힌트 모음
        Dictionary<int, string> columnhints = new Dictionary<int, string>(); //세로 문제들의 힌트 모음
        StringBuilder sb = new StringBuilder();
        bool[,] space = new bool[10, 10]; //입력 블록들의 on,off 여부를 받음
        int [,] startloc = new int[10, 10]; //입력 블록의 힌트 index를 결정. 0인경우 해당 블록은 힌트 출력을 하지않음.
        for(int i=0;i<info.words.Length;i++) //문제를 순회하며 문제가 쓰여져야 하는 공간(블록들)을 활성화하고, 해당 문제 번호에 맞게 힌트를 추가한다.
        {
            startloc[info.words[i].Start_loc.y, info.words[i].Start_loc.x] = info.words[i].num;
            if(info.words[i].isStreet) //문제가 가로 문제인경우
            {
                streethints.Add(info.words[i].num, info.words[i].hint);
                for (int j = 0; j < info.words[i].word.Length; j++)
                {
                    int index_y = info.words[i].Start_loc.y;
                    int index_x = info.words[i].Start_loc.x + j;
                    space[index_y, index_x] = true;
                    answer[index_y, index_x] = info.words[i].word[j];
                }
            }
            else //문제가 세로 문제인경우
            {
                columnhints.Add(info.words[i].num, info.words[i].hint);
                for (int j = 0; j < info.words[i].word.Length; j++)
                {
                    int index_y = info.words[i].Start_loc.y + j;
                    int index_x = info.words[i].Start_loc.x;
                    space[index_y, index_x] = true;
                    answer[index_y, index_x] = info.words[i].word[j];
                }
                    
            }
        }
        for(int i=0;i<10;i++)
            for(int j=0;j<10;j++)
            {
                Transform puzzle = Instantiate(CrossWordBlock, CrossWordPuzzleSpace, false).transform;
                InputField inputsys = puzzle.GetComponent<InputField>();
                puzzle.localPosition = new Vector2(j * 108 - 540, -i * 108);
                inputsys.interactable = space[i, j];
                if (inputsys.interactable)
                {
                    bool isStart=NumFieldSetting(puzzle.GetChild(2).GetComponent<Text>(), j, i, startloc);
                    int index = inputsys.transform.GetSiblingIndex();
                    if (isStart) HintPrintEventAdd(inputsys, index, streethints, columnhints, startloc);
                    BlockMoveButtonEventAdd(inputsys, index);
                    inputsys.onValueChanged.AddListener(delegate (string ev) 
                    {
                        if (ev.Length > 1) inputsys.text = ev[1].ToString();
                    });
                    inputsys.onEndEdit.AddListener(delegate (string ev)
                    {
                        if (ev.Length > 0) inputword[index / 10, index % 10] = inputsys.text[0];
                        else inputword[index / 10, index % 10] = ' ';
                        inputsys.transform.GetChild(2).GetComponent<Text>().text = inputsys.text;
                    });
                }
                else Destroy(puzzle.GetChild(2).gameObject);
            }
        //처음 진입한경우 인트로 대사 출력
        if (StaticManager.SaveData.clearstate[AppCategory.Dictionary] == AppState.NotEnter)
        {
            speechEvent.SpeechStart(StaticManager.Nowappinfo.speecharray.introspeech, this.gameObject, EventState.Intro);
            StaticManager.SaveData.clearstate[AppCategory.Dictionary] = AppState.NotClear;
        }
    }
    private void Update()
    {
        if (isEvent) return;
        if (max_time > 0)
        {
            max_time -= Time.deltaTime;
            int cnt = (int)max_time;
            Timer.text = (cnt / 60).ToString("D2") + ":" + (cnt % 60).ToString("D2");
        }
        else if (max_time < 0 && max_time > -1)
        {
            isEvent = true;
            Button[] buttons = this.transform.root.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
                btn.interactable = false;
            Timer.text = "Game Over";
            Timer.color = Color.red;
            max_time = -1;
            speechEvent.SpeechStart(StaticManager.Nowappinfo.speecharray.gameoverspeech, this.gameObject, EventState.GameOver);
        }
    }
    //힌트 출력 블록 클릭시 힌트를 출력하도록 이벤트 트리거를 추가하는 메소드
    private void HintPrintEventAdd(InputField inputsys,int index,Dictionary<int,string>streethints,Dictionary<int,string>columnhints,int[,]startloc)
    {
        EventTrigger.Entry hintev = new EventTrigger.Entry();
        hintev.eventID = EventTriggerType.Select;
        hintev.callback.AddListener((eventdata) =>
        {
            StringBuilder sb = new StringBuilder();
            int hintindex = startloc[index / 10, index % 10];
            if (streethints.ContainsKey(hintindex)) sb.AppendFormat("가로\n{0}", streethints[hintindex]);
            if (columnhints.ContainsKey(hintindex))
            {
                if (sb.Length > 0) sb.AppendFormat("\n\n세로\n{0}", columnhints[hintindex]);
                else sb.AppendFormat("세로\n{0}", columnhints[hintindex]);
            }
            hintspace.text = sb.ToString();
            Debug.Log(sb.ToString());
        });
        inputsys.GetComponent<EventTrigger>().triggers.Add(hintev);
    }
    //블록이 선택되었을때 상하좌우 블록의 상태에 따라 상하좌우 블록으로 이동하는 각각의 버튼들의 상태를 조정하는 메소드
    private void BlockMoveButtonEventAdd(InputField inputsys,int index)
    {
        EventTrigger.Entry hintev = new EventTrigger.Entry();
        hintev.eventID = EventTriggerType.Select;
        hintev.callback.AddListener((eventdata) =>
        {
            nowselectblock = inputsys;
            if (index - 10 >= 0) upbutton.interactable = CrossWordPuzzleSpace.GetChild(index - 10).GetComponent<InputField>().interactable;
            else upbutton.interactable = false;
            if (index + 10 < 100) downbutton.interactable = CrossWordPuzzleSpace.GetChild(index + 10).GetComponent<InputField>().interactable;
            else downbutton.interactable = false;
            if (index - 1 >= 0) leftbutton.interactable = CrossWordPuzzleSpace.GetChild(index - 1).GetComponent<InputField>().interactable;
            else leftbutton.interactable = false;
            if (index + 1 < 100) rightbutton.interactable = CrossWordPuzzleSpace.GetChild(index + 1).GetComponent<InputField>().interactable;
            else rightbutton.interactable = false;
        });
        inputsys.GetComponent<EventTrigger>().triggers.Add(hintev);
    }
    //현재 블록이 힌트를 출력하는 블록일시 해당 힌트를 가진 문제 번호를 기입하고, 아닐시 번호를 기입하는 Text 오브젝트를 제거한다.
    private bool NumFieldSetting(Text text,int loc_x,int loc_y,int[,]startlocs)
    {
        if (startlocs[loc_y, loc_x] != 0)
        {
            text.text = startlocs[loc_y, loc_x] + ")";
            return true;
        }
        else
        {
            Destroy(text.gameObject);
            return false;
        }
            
    }
    //전송 버튼 클릭시
    public void Submit()
    {
        if(inputword.Equals(answer))
        {
            isEvent = true;
            speechEvent.SpeechStart(StaticManager.Nowappinfo.speecharray.clearspeech, this.gameObject, EventState.Clear);
        }
        else
        {
            max_time -= 5;
            if (max_time < 0) max_time = 0;
        }
    }
    //상하좌우 4개 블록 클릭시
    public void NextBlock(int value)
    {
        int index = nowselectblock.transform.GetSiblingIndex();
        index += value;
        InputField next = CrossWordPuzzleSpace.GetChild(index).GetComponent<InputField>();
        next.OnPointerDown(new PointerEventData(EventSystem.current));
    }
    public void Clear()
    {
        if (StaticManager.SaveData.clearstate[AppCategory.Dictionary] == AppState.NotClear) StaticManager.SaveData.clearstate[AppCategory.Dictionary] = AppState.Clear;
        StaticManager.SaveData.battery -= 2;
        StaticManager.moveScenetoLoading("MainStage");
    }
    public void GameOver()
    {
        StaticManager.SaveData.battery -= 5;
        StaticManager.moveScenetoLoading("MainStage");
    }
}
