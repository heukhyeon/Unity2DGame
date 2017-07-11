using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 메세지 어플에서 화면 중단, 메세지 블록을 드래그하는 창의 스크립트. 메세지어플의 작동과정을 총체적으로 관리한다.
/// </summary>
public class MessageBox : CustomUI,HeartbarController {

    /// <summary>
    /// 메인 스테이지의 MessageAppInfo 난수로부터 전달된 현재 정답.
    /// </summary>
    private string Answer = "메세지 어플 문자열 정답을 입력해주세요";
    /// <summary>
    /// 복제할(메세지바 안에 넣어질 투명한) 메세지 블록
    /// </summary>
    [SerializeField]
    private GameObject inmessage = null;
    /// <summary>
    /// 메세지 공간에 배치될 불투명한 블록
    /// </summary>
    [SerializeField]
    private GameObject Messageblock = null;
    /// <summary>
    /// MessageSpace의 Viewport
    /// </summary>
    [SerializeField]
    private RectTransform Viewport = null;
    /// <summary>
    /// 메세지 바를 감싸는 테두리
    /// </summary>
    [SerializeField]
    private RectTransform Centerbar = null;
    /// <summary>
    /// 전송 버튼
    /// </summary>
    [SerializeField]
    private RectTransform SendButton = null;
    /// <summary>
    /// 전송되지 않은(불투명한)메세지 블록들이 나열되있는 메세지 바 아래의 장소
    /// </summary>
    [SerializeField]
    private RectTransform MessageSpace = null;
    /// <summary>
    /// 메세지 스페이스 오른쪽의 스크롤바
    /// </summary>
    [SerializeField]
    private RectTransform Scrollbar = null;
    /// <summary>
    /// 자기 자신의 Recttransform
    /// </summary>
    private RectTransform Messagebar;
    /// <summary>
    /// 라이프바
    /// </summary>
    [SerializeField]
    private Lifebar lifebar = null;
    public string nowword = "";
    private List<GameObject> words = new List<GameObject>();
    private short cnt = 0;
    private const short MESSAGEBLOCKHEIGHT = 45; //메세지 박스에 등록되는 메세지 블록의 기본 높이
    private const short MESSAGEBLOCKWIDTH = 150; //메세지 박스에 등록되는 메세지 블록의 기본 너비
    private Vector2 DEFAULT_POS;

    protected override void UIAwake()
    {
        MessageAppInfo info = CustomSceneManager.apps.message;
        Answer = info.Answer;
        for(int i=0;i<info.Words.Length;i++) // 무작위로 섞는다.
        {
            int loc1 = UnityEngine.Random.Range(0, info.Words.Length);
            int loc2 = UnityEngine.Random.Range(0, info.Words.Length);
            string temp = info.Words[loc1];
            info.Words[loc1] = info.Words[loc2];
            info.Words[loc2] = temp;
        }

        for(int i=0;i<info.Words.Length;i++) //섞인 문자열배열에서 순서대로 문자열을 뽑는다.
        {
            GameObject obj = Instantiate(Messageblock, MessageSpace, false);
            obj.GetComponentInChildren<UnityEngine.UI.Text>().text = info.Words[i];
            int h_loc = -((i / 3) * 150 + 50); 
            int w_loc = (i % 3) * 230 + 110;
            obj.GetComponent<RectTransform>().localPosition = new Vector2(w_loc, h_loc);
        }
        Messagebar = GetComponent<RectTransform>();
    }
    /// <summary>
    /// OnDrop시 호출되는 메소드 
    /// </summary>
    public void Add()
    {
        //OnDrop은 꼭 메세지 블록을 드래그 앤 드랍할때만 호출되지않으므로, 블록을 잡았을때 nowword가 설정되는걸 이용해 nowword가 null이 아닐때만 진행한다.
        if (nowword == null) return; 
        GameObject obj = Instantiate(inmessage, this.transform);
        obj.GetComponentInChildren<UnityEngine.UI.Text>().text = nowword;
        Vector2 pos = Vector2.zero;
        pos.y = DEFAULT_POS.y - ((cnt / 4) * MESSAGEBLOCKHEIGHT);
        pos.x = DEFAULT_POS.x + ((cnt % 4) * MESSAGEBLOCKWIDTH);
        obj.transform.position = pos;
        obj.name = "InMessage" + cnt;
        words.Add(obj);
        UnityEngine.UI.Button btn = obj.GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() => { Delete(obj.name); }); //Onclick 이벤트 추가
        cnt++;
        if (cnt % 4 == 0) HeightChange(true);
        nowword = null;
    }
    /// <summary>
    /// 메세지박스에 등록된 메세지블록을 클릭시 호출되는 메소드
    /// </summary>
    /// <param name="name"></param>
    public void Delete(string name)
    {
        Debug.Log(words.Count);
        int rate = cnt / 4;
        int loc = int.Parse(System.Text.RegularExpressions.Regex.Replace(name, @"\D", ""));
        Vector2 pos = words[loc].transform.localPosition;
        Destroy(words[loc]);
        words.Remove(words[loc]);
        for(int i=0;i<words.Count;i++)
        {
            words[i].name = "InMessage" + i; //인덱스를 이름끝의 번호로 삼으므로 중간 블록을 삭제한경우를 위한 전체 인덱스 번호를 재지정한다.
            if (i < loc) continue; // 삭제하기 이전 블록은 위치변경을 하지 않는다.
            Vector2 backup = words[i].transform.localPosition;
            words[i].transform.localPosition = pos;
            pos = backup;
        }
        cnt--;
        //현재 남은 블록이 존재하는 상태에서 전체블록수를 4로 나눈수(행의 수)가 달라진경우 메세지 박스를 줄인다.
        if (cnt / 4 < rate) HeightChange(false); 
    }
    /// <summary>
    /// 한줄이 늘어나거나 또는 지워져야할때 메세지바의 세로길이를 변화시킨다.
    /// </summary>
    /// <param name="increase"></param>
    private void HeightChange(bool increase)
    {
        RectHeightChange(Messagebar, increase, true);
        RectHeightChange(Centerbar, increase, true);
        RectHeightChange(SendButton, increase, true);
        //ViewPort와 스크롤바의 증가량은 위 3개와 반비례한다.  또한 스크롤바는 90도 회전한 상태이므로 x축을 편집하기위해 3번째 매개변수를 false로 둔다.
        RectHeightChange(Viewport, !increase, true);
        RectHeightChange(Scrollbar, !increase, false);
        float rate = MESSAGEBLOCKHEIGHT / 2f;
        rate = increase == true ? rate : -rate;
        for (int i = 0; i < words.Count; i++)
        {
            Vector2 size = words[i].transform.position;
            size.y += rate;
            words[i].transform.position = size;
        }
    }
    /// <summary>
    /// 높이 변화에서 각 Recttransform의 높이 변화는 거의 동일한 스크립트를 돌려쓰므로 가독성을 위해 하나의 메소드를 작성해 호출하는 방식을 차용한다.
    /// </summary>
    /// <param name="recttransform"></param>
    /// <param name="increase"></param>
    /// <param name="y_change"></param>
    private void RectHeightChange(RectTransform recttransform,bool increase,bool y_change)
    {
        Vector2 size = recttransform.sizeDelta;
        int value = increase == true ? MESSAGEBLOCKHEIGHT : -MESSAGEBLOCKHEIGHT;
        if (y_change) size.y += value;
        else size.x += value;
        recttransform.sizeDelta = size;
    }
    /// <summary>
    /// "전송"버튼 클릭시 호출되는 메소드
    /// </summary>
    public void MessageSend()
    {
        StringBuilder sb = new StringBuilder();
        foreach (GameObject obj in words)
            sb.Append(obj.GetComponentInChildren<UnityEngine.UI.Text>().text);
        Debug.Log(sb.ToString());
        if (sb.ToString() == Answer) Debug.Log("맞았습니다");
        else
        {
            lifebar.HeartDecrease();
            Debug.Log("틀렸습니다");
        }
    }
    private void Start()
    {
        DEFAULT_POS.y = Messagebar.position.y - Messagebar.sizeDelta.y / 2;
        DEFAULT_POS.x = Messagebar.position.x - Messagebar.sizeDelta.x / 2.65f;
    }

    public void HeartCompareComplete()
    {
        SendButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }
    public void MissionFail()
    {
        Debug.Log("죽었습니다!");
    }
}
