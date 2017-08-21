using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CrossWordPuzzle : MonoBehaviour
{
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
        DictionaryAppinfo info = (DictionaryAppinfo)GameManager.appinfo;
        Dictionary<int, string> streethints = new Dictionary<int, string>(); //가로 문제들의 힌트 모음
        Dictionary<int, string> columnhints = new Dictionary<int, string>(); //세로 문제들의 힌트 모음
        for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) answer[i, j] = info.answers[i][j];
        StringBuilder sb = new StringBuilder();
        bool[,] space = new bool[10, 10]; //입력 블록들의 on,off 여부를 받음
        int[,] startloc = new int[10, 10]; //입력 블록의 힌트 index를 결정. 0인경우 해당 블록은 힌트 출력을 하지않음.
        //가로 문제 순회
        foreach(DictionaryAppinfo.Quest quest in info.street)
        {
            if (startloc[quest.pos_y, quest.pos_x] == 0) startloc[quest.pos_y, quest.pos_x] = quest.index;
            streethints.Add(quest.index, quest.hint);
            int length = quest.answer.Length;
            for (int i = 0; i < length; i++) space[quest.pos_y, quest.pos_x + i] = true;
        }
        //세로 문제 순회
        foreach(DictionaryAppinfo.Quest quest in info.column)
        {
            if (startloc[quest.pos_y, quest.pos_x] == 0) startloc[quest.pos_y, quest.pos_x] = quest.index;
            columnhints.Add(quest.index, quest.hint);
            int length = quest.answer.Length;
            for (int i = 0; i < length; i++) space[quest.pos_y+i, quest.pos_x] = true;
        }
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                Transform puzzle = Instantiate(CrossWordBlock, CrossWordPuzzleSpace, false).transform;
                InputField inputsys = puzzle.GetComponent<InputField>();
                puzzle.localPosition = new Vector2(j * 108 - 540, -i * 108);
                inputsys.interactable = space[i, j];
                if (inputsys.interactable)
                {
                    bool isStart = NumFieldSetting(puzzle.GetChild(2).GetComponent<Text>(), j, i, startloc);
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
            speechEvent.SpeechStart(GameManager.speech.gameover, GetComponent<ISpeech>());
        }
    }
    //힌트 출력 블록 클릭시 힌트를 출력하도록 이벤트 트리거를 추가하는 메소드
    private void HintPrintEventAdd(InputField inputsys, int index, Dictionary<int, string> streethints, Dictionary<int, string> columnhints, int[,] startloc)
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
    private void BlockMoveButtonEventAdd(InputField inputsys, int index)
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
    private bool NumFieldSetting(Text text, int loc_x, int loc_y, int[,] startlocs)
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
        if (inputword.Equals(answer))
        {
            isEvent = true;
            speechEvent.SpeechStart(GameManager.speech.clear, GetComponent<ISpeech>());
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
}

