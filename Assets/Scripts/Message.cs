using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Message : MonoBehaviour,ISpeech {
    public RectTransform blockspace; //메세지 블록들이 잇는 공간(Content)
    public RectTransform bubblespace; //메세지 말풍선들이 잇는공간(Content)
    public GameObject messageblock; // 복제할 메세지 블록
    public GameObject messagebubble; //복제할 메세지 말풍선
    public Image speechspacebackground; //Bubble Space를 감싸는 테두리
    public Text LifeNotice; // 남은 횟수 알림
    public RectTransform LifeValue; //라이프 게이지
    public SpeechEvent speechev;//플레이어 대사 출력
    MessageAppInfo info; //현재 메세지앱 정보
    List<RectTransform> blocklist = new List<RectTransform>(); // 재정렬을 위한 메세지 블록 리스트
    List<RectTransform> bubblelist = new List<RectTransform>(); // 재정렬을 위한 메세지 말풍선 리스트
    Dictionary<string, bool> isEnemySpeak = new Dictionary<string, bool>(); //말풍선 생성시 방향 판가름용 Dictionary
    RectTransform selectblock; //블록 선택-> 드랍시 말풍선 공간내부에 떨어뜨린경우 내용 기억용
    Vector2 selectblocklocation; //블록 선택-> 드랍시 말풍선 공간 외부에 떨어뜨렸을시 해당 블록을 원위치로 돌리기 위한 좌표 기억
    string[] Answers; //재배치되기전 원래의 정답을 가진 배열
    bool isEnter = false; //블록 선택 - > 드랍시 현재 말풍선 공간내부인지를 판별하는 변수
    bool inputenable = false; //블록 선택 이벤트를 필요할때만 발생시킬수있게하는 변수
    int heart = 3; //남은 횟수
    float heartonestep = 620 / 3; //1개의 라이프당 라이프 게이지가 차오르는 가로길이
    private void Start()
    {
        info = (MessageAppInfo)GameManager.appinfo;
        StartCoroutine("StartBlockCreate");
    }
    IEnumerator StartBlockCreate()//앱 시작시 최초로 메세지 블록들을 생성
    {
        Answers = new string[info.speechs.Length];
        for (int i = 0; i < info.speechs.Length; i++)
            Answers[i] = info.speechs[i].speech;
        for (int i = 0; i < info.speechs.Length; i++)
        {
            MessageAppInfo.MessageSpeech temp;
            int loc = UnityEngine.Random.Range(0, info.speechs.Length);
            temp = info.speechs[i];
            info.speechs[i] = info.speechs[loc];
            info.speechs[loc] = temp;
        }
        for (int i = 0; i < info.speechs.Length; i++)
        {
            StartCoroutine("BlockCreate", info.speechs[i]);
            yield return new WaitForSeconds(0.5f);
        }
        speechev.SpeechStart(GameManager.speech.intro, GetComponent<ISpeech>());
    }
    IEnumerator BlockCreate(MessageAppInfo.MessageSpeech speech)
    {
        inputenable = false;
        int index = blocklist.Count;
        Vector2 pos = GetBlockPosition(index);
        RectTransform block = Instantiate(messageblock, blockspace, false).GetComponent<RectTransform>();
        block.localPosition = pos;
        blocklist.Add(block);
        Vector2 size = block.sizeDelta;
        while (size.x < 250)
        {
            size.x += 5;
            size.y += 5;
            block.sizeDelta = size;
            yield return new WaitForEndOfFrame();
        }
        if (!isEnemySpeak.ContainsKey(speech.speech)) isEnemySpeak.Add(speech.speech, speech.isEnemy);
        size = blockspace.sizeDelta;
        size.y = (blocklist.Count / 3) * 320 + 320;
        blockspace.sizeDelta = size;
        block.GetComponentInChildren<Text>().text = speech.speech;
        //문장 블록에 이벤트 추가
        EventTrigger trigger = block.GetComponent<EventTrigger>();
        EventTrigger.Entry selectev = new EventTrigger.Entry();
        selectev.eventID = EventTriggerType.PointerDown;
        selectev.callback.AddListener((eventdata) => { BlockSelect(block, eventdata); });
        EventTrigger.Entry dragev = new EventTrigger.Entry();
        dragev.eventID = EventTriggerType.Drag;
        dragev.callback.AddListener((eventdata) => { BlockMove(block, eventdata); });
        EventTrigger.Entry dropev = new EventTrigger.Entry();
        dropev.eventID = EventTriggerType.PointerUp;
        dropev.callback.AddListener((eventdata) => { BlockDrop(block); });
        trigger.triggers.Add(selectev);
        trigger.triggers.Add(dragev);
        trigger.triggers.Add(dropev);
        inputenable = true;
    }
    public void BlockSelect(RectTransform block, BaseEventData data)
    {
        if (!inputenable) return;
        selectblock = block;
        selectblocklocation = block.position;
    }
    public void BlockDrop(RectTransform block)
    {
        if (!inputenable) return;
        if (isEnter)
        {
            string text = block.GetComponentInChildren<Text>().text;
            blocklist.Remove(block);
            Destroy(block.gameObject);
            BlockSort();
            StartCoroutine("BubbleCreate", text);
        }
        else
        {
            block.localPosition = selectblocklocation;
        }
        selectblock = null;
        SpeechSpaceExit();
    }
    public void BlockMove(RectTransform block, BaseEventData data)
    {
        if (!inputenable) return;
        block.position = (data as PointerEventData).position;
    }
    private void BlockSort()
    {
        for (int i = 0; i < blocklist.Count; i++)
            blocklist[i].localPosition = GetBlockPosition(i);
        Vector2 size = blockspace.sizeDelta;
        size.y = (blocklist.Count / 3) * 320 + 320;
        blockspace.sizeDelta = size;
    }
    private Vector2 GetBlockPosition(int index)
    {
        return new Vector2((index % 3) * 320 + 180, -((index / 3) * 320 + 150));
    }
    public void SpeechSpaceEnter()
    {
        if(selectblock)
        {
            isEnter = true;
            Color enablecolor = new Color(1, 1, 0);
            speechspacebackground.color = enablecolor;
        }
    }
    public void SpeechSpaceExit()
    {
        speechspacebackground.color = new Color(1, 1, 1);
    }
    IEnumerator BubbleCreate(string text)
    {
        inputenable = false;
        int x_pos = isEnemySpeak[text] ? 345 : 615;
        RectTransform bubble = Instantiate(messagebubble, bubblespace, false).GetComponent<RectTransform>();
        Vector2 pos = new Vector2(x_pos, GetBubblePosition_y(bubblelist.Count));
        bubble.localPosition = pos;
        int y_rot = isEnemySpeak[text] ? 180 : 0;
        bubble.rotation = Quaternion.Euler(0, y_rot, 0);
        RectTransform speechbubble = bubble.GetChild(0) as RectTransform;
        Text textcomponent = speechbubble.GetComponentInChildren<Text>();
        textcomponent.text = text;
        textcomponent.transform.rotation = Quaternion.identity;
        Vector2 size = bubble.sizeDelta;
        while(size.x<600)
        {
            size.x += 20;
            size.y += 20;
            bubble.sizeDelta = size;
            yield return new WaitForEndOfFrame();
        }
        speechbubble.anchorMin = new Vector2(0.5f, 1f);
        speechbubble.anchorMax = new Vector2(0.5f, 1f);
        speechbubble.SetParent(bubble.parent);
        speechbubble.localPosition = pos;
        speechbubble.GetComponent<Button>().onClick.AddListener(delegate () { if(inputenable)StartCoroutine("BubbleDelete", speechbubble); });
        Destroy(bubble.gameObject);
        bubblelist.Add(speechbubble);
        size = bubblespace.sizeDelta;
        size.y = bubblelist.Count * 250;
        bubblespace.sizeDelta = size;
        inputenable = true;
    }
    private float GetBubblePosition_y(int index)
    {
        return -((index * 230) + 130);
    }
    public IEnumerator BubbleDelete(RectTransform bubble)
    {
        Destroy(bubble.GetComponent<Button>());
        string text = bubble.GetChild(0).GetComponent<Text>().text;
        RectTransform effect = bubble.GetChild(1) as RectTransform;
        Vector2 size = effect.sizeDelta;
        size = new Vector2(600, 600);
        effect.sizeDelta = size;
        effect.SetParent(bubble.parent);
        bubble.SetParent(effect);
        bubble.anchorMin = new Vector2(0.5f, 0.5f);
        bubble.anchorMax = new Vector2(0.5f, 0.5f);
        bubble.position = effect.position;
        MessageAppInfo.MessageSpeech speech;
        speech.isEnemy = isEnemySpeak[text];
        speech.speech = text;
        while (size.x>0)
        {
            size.x -= 50;
            size.y -= 50;
            if (size.x == 300)StartCoroutine("BlockCreate", speech);
            effect.sizeDelta = size;
            yield return new WaitForEndOfFrame();
        }
        bubblelist.Remove(bubble);
        Destroy(effect.gameObject);
 
        BubbleSort();
    }
    private void BubbleSort()
    {
        for (int i = 0; i < bubblelist.Count; i++)
            bubblelist[i].localPosition = new Vector2(bubblelist[i].localPosition.x, GetBubblePosition_y(i));
    }
    public void Submit()
    {
        bool isAnswer = bubblelist.Count == Answers.Length;
        if(isAnswer)
            for(int i=0;i<blocklist.Count;i++)
                if(!blocklist[i].Equals(Answers[i]))
                {
                    isAnswer = false;
                    break;
                }
        if(isAnswer)
        {
            speechev.SpeechStart(GameManager.speech.clear, GetComponent<ISpeech>());
        }
        else
        {
            heart--;
            LifeNotice.text = "남은 횟수:" + heart + "/3";
            LifeValue.sizeDelta = new Vector2(heartonestep * heart, 50);
            if (heart > 0) speechev.SpeechStart(GameManager.speech.fail, GetComponent<ISpeech>());
            else speechev.SpeechStart(GameManager.speech.gameover, GetComponent<ISpeech>());
        }
    }
    public void SpeechComplete()
    {
        if (!inputenable) inputenable = true;
    }
}
