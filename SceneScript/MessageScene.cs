using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System;

public class MessageScene : MonoBehaviour,AppSceneScript {

    public GameObject WordBlock; //하단에 나열된 문장 블록
    public GameObject SpeechBubble; //상단에 출력될 말풍선
    public RectTransform WordBlockSpace; //WordBlock이 위치할 공간
    public RectTransform SpeechBubbleSpace; //SpeechBubble이 위치할 공간
    public Image SpeechBubbleViewport; //말풍선이 표시될 공간을 감싸는 테두리
    public SpeechEvent speechEvent;
    public Transform Lifebar;
    public GameObject Life;
    private GameObject nowWordblock; //현재 선택된 문장 블록
    private Vector2 nowWordblocklocation; //선택된 문장이 드랍되지않았을시 돌아갈 좌표
    private Dictionary<string, Speaker> blockdictionary = new Dictionary<string, Speaker>(); //해당 문장이 타인의 문장인지 본인의 문장인지 판별
    private List<string> Talks = new List<string>(); //전체 대화 배열
    private List<string> Answer = new List<string>(); //정답 대화 배열
    private List<GameObject> WordBlocks = new List<GameObject>();
    private bool inBubbleSpace = false; // 현재 블록이 말풍선 공간안에 위치해있는지 판별
    private int cnt = 3;
    private void Start()
    {
        MessageAppInfo.MessageStructure[] messages = StaticManager.Nowappinfo.messageinfo.messagestruct;
        foreach (MessageAppInfo.MessageStructure message in messages)
            if(message.isAnswer)Answer.Add(message.speech); //정답으로서 등록된 문장만 판별 List에 추가
        //문장 블록 배치를 무작위로 한다.
        for (int i = 0; i < messages.Length; i++)
        {
            int loc = UnityEngine.Random.Range(0, messages.Length - 1);
            MessageAppInfo.MessageStructure temp = messages[loc];
            messages[loc] = messages[i];
            messages[i] = temp;
        }
        //화면 하단에 문장 블록을 추가
        for (int i = 0; i < messages.Length; i++)
        {
            GameObject obj = Instantiate(WordBlock, WordBlockSpace, false);
            obj.name = "WordBlock" + i;
            obj.GetComponent<RectTransform>().localPosition = WordBlockSort(i);
            obj.GetComponentInChildren<Text>().text = messages[i].speech;
            EventAdd(obj);
            blockdictionary.Add(messages[i].speech, messages[i].speaker);
            WordBlocks.Add(obj);
        }
        for (int i = 0; i < 3; i++)
        {
            Transform heart = Instantiate(Life, Lifebar, false).transform;
            heart.localPosition = new Vector2(i * 80 +50, 0);
        }
        //처음 진입한경우 인트로 대사 출력
        if (StaticManager.SaveData.clearstate[AppCategory.Message] == AppState.NotEnter)
        {
            speechEvent.SpeechStart(StaticManager.Nowappinfo.speecharray.introspeech, this.gameObject, EventState.Intro);
            StaticManager.SaveData.clearstate[AppCategory.Message] = AppState.NotClear;
        }
        
    }
#region 문장 블록 선택 이벤트
    public void WordBlockSelect(GameObject obj)
    {
        nowWordblock = obj;
        nowWordblocklocation = obj.transform.localPosition;
    }
    public void WordBlockDrag(BaseEventData data,GameObject obj)
    {
        PointerEventData pointer = data as PointerEventData;
        obj.transform.position = pointer.position;
    }
    public void WordBlockDeSelect(GameObject obj)
    {
        if (obj != null) obj.transform.localPosition = nowWordblocklocation;
        if(!inBubbleSpace)nowWordblock = null; //말풍선 공간 이외의 장소에서 놓은경우 선택 블록을 null로 둠.
    }
    #endregion

#region 말풍선 블록 공간 효과
    public void SpeechBubbleSpaceEffect()
    {
        if (nowWordblock)
        {
            inBubbleSpace = true; //블록을 가진 상태에서 진입후 클릭(터치)를 해제했을때 선택블록이 해제되지않도록 bool 변수를 참으로 둠.
            SpeechBubbleViewport.color = Color.red;
        }
    }
    public void SpeechBubbleSpaceDeEffect()
    {
        inBubbleSpace = false;
        SpeechBubbleViewport.color = Color.white;
    }
    #endregion

#region 말풍선 생성. 제거
    public void WordBlockDrop()
    {
        if (nowWordblock == null) return;
        SpeechBubbleSpaceDeEffect();
        WordBlocks.Remove(nowWordblock);
        Destroy(nowWordblock);
        string nowword = nowWordblock.GetComponentInChildren<Text>().text;
        float rotate = blockdictionary[nowword] == Speaker.Hero ? 0 : 180f;
        float x_loc = blockdictionary[nowword] == Speaker.Hero ? 129f : -139f;
        float y_loc = -110 * Talks.Count;
        GameObject obj = Instantiate(SpeechBubble, SpeechBubbleSpace, false);
        obj.name = "Bubble" + Talks.Count;
        Text bubbletext = obj.GetComponentInChildren<Text>();
        Vector3 rot = Vector3.zero;
        rot.y = rotate;
        obj.GetComponent<RectTransform>().localPosition = new Vector3(x_loc, y_loc, 0);
        obj.GetComponent<Button>().onClick.AddListener(delegate () { SpeechBubbleDelete(obj); });
        obj.transform.rotation = Quaternion.Euler(rot);
        bubbletext.text = nowword;
        bubbletext.gameObject.transform.localRotation = Quaternion.Euler(rot);
        Talks.Add(nowword);
        nowword = null;
        for (int i = 0; i < WordBlocks.Count; i++)
        {
            WordBlocks[i].name = "WordBlock" + i;
            WordBlocks[i].GetComponent<RectTransform>().localPosition = WordBlockSort(i);
        }
        SpeechBubbleSpace.sizeDelta = new Vector2(720, Talks.Count * 150);
    }
    public void SpeechBubbleDelete(GameObject obj)
    {
        string name = obj.name;
        string nowword = obj.GetComponentInChildren<Text>().text;
        Vector2 pos = Vector2.zero;
        Talks.Remove(nowword);
        Destroy(obj);
        int cnt = 0;
        foreach(RectTransform tr in SpeechBubbleSpace)
        {
            if (tr.name.Equals(name)) continue;
            tr.name = "Bubble" + cnt;
            pos = tr.localPosition;
            pos.y = -110 * cnt++;
            tr.localPosition = pos;
        }
        cnt = WordBlocks.Count;
        for (int i = 0; i < cnt; i++)
        {
            WordBlocks[i].name = "WordBlock" + i;
            WordBlocks[i].GetComponent<RectTransform>().localPosition = WordBlockSort(i);
        }
        obj = Instantiate(WordBlock, WordBlockSpace, false);
        obj.name = "WordBlock" + cnt;
        pos.x = cnt % 3 * 235 - 230;
        pos.y = -(cnt / 3 * 165 + 85);
        obj.GetComponent<RectTransform>().localPosition = pos;
        obj.GetComponentInChildren<Text>().text = nowword;
        EventAdd(obj);
        WordBlocks.Add(obj);
        SpeechBubbleSpace.sizeDelta = new Vector2(720, Talks.Count * 110);
    }
#endregion

    public void Submit() // 확인 버튼 클릭 이벤트
    {
        if(isCorrect()) //정답일시
        {
            if(StaticManager.SaveData.clearstate[AppCategory.Message]==AppState.NotClear) //처음으로 클리어하는경우
            {
                speechEvent.SpeechStart(StaticManager.Nowappinfo.speecharray.clearspeech, this.gameObject, EventState.Clear);
            }
        }
        else // 정답이 아닐시
        {
            Destroy(Lifebar.GetChild(cnt-- -1).gameObject);
            if(cnt==0) speechEvent.SpeechStart(StaticManager.Nowappinfo.speecharray.gameoverspeech, this.gameObject, EventState.GameOver);
            else speechEvent.SpeechStart(StaticManager.Nowappinfo.speecharray.failspeech, this.gameObject, EventState.Fail);
            

        }
    }
    private bool isCorrect() //현재 상태가 정답인지 오답인지 판별
    {
        if (Talks.Count != Answer.Count) return false;
        for (int i = 0; i < Talks.Count; i++)
            if (Talks[i]!= Answer[i]) return false;
        return true;
    }
    public void Clear()//클리어 스크립트 진행후
    {
        if (StaticManager.SaveData.clearstate[AppCategory.Message] == AppState.NotClear) StaticManager.SaveData.clearstate[AppCategory.Message] = AppState.Clear;
        StaticManager.SaveData.battery -= 2;
        StaticManager.moveScenetoLoading("MainStage");
    }
    public void GameOver()//게임오버 스크립트 진행후
    {
        StaticManager.SaveData.battery -= 50;
        StaticManager.moveScenetoLoading("MainStage");
    }
    private void EventAdd(GameObject obj) //PointerDown,PointerUp,Drag에 대한 이벤트 추가
    {
        EventTrigger.Entry wordselectevent = new EventTrigger.Entry();
        wordselectevent.eventID = EventTriggerType.PointerDown;
        wordselectevent.callback.AddListener((eventdata) => { WordBlockSelect(obj); });
        EventTrigger.Entry worddragevent = new EventTrigger.Entry();
        worddragevent.eventID = EventTriggerType.Drag;
        worddragevent.callback.AddListener((eventdata) => { WordBlockDrag(eventdata, obj); });
        EventTrigger.Entry worddeselect = new EventTrigger.Entry();
        worddeselect.eventID = EventTriggerType.PointerUp;
        worddeselect.callback.AddListener((eventdata) => { WordBlockDeSelect(obj); });
        EventTrigger eventsys = obj.GetComponent<EventTrigger>();
        eventsys.triggers.Add(wordselectevent);
        eventsys.triggers.Add(worddragevent);
        eventsys.triggers.Add(worddeselect);
    }
    private Vector2 WordBlockSort(int cnt) //WordBlock위치 지정
    {
        return new Vector2(cnt % 3 * 235 - 230, -(cnt / 3 * 165 + 85));
    }
}
