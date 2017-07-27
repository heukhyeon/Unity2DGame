using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TalkScene:MonoBehaviour
{
    public GameObject SpeechBubble;
    public GameObject WordBlock;
    public RectTransform SpeechBubbleSpace;
    public RectTransform WordBlockSpace;
    public TalkAppInfo test;
    private TalkAppInfo info;
    private TalkAppInfo.Talk[] NowTalk;
    private int speechindex = 0;
    private int totalbubblenum = 0;
    private bool selectdelay = false; //선택지 출현중임을 알림.
    private void Start()
    {
        WordBlock.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);
        info = StaticManager.Nowappinfo.talkinfo;
        try
        {
            Debug.Log(info.introtalk.Length);
        }
        catch //info가 제대로 전달되지 않은경우
        {
            info = test;
        }
        NowTalk = info.introtalk;
        Speech();
    }
    private void Speech()
    {
        TalkAppInfo.Talk talk = NowTalk[speechindex];
        GameObject bubble = Instantiate(SpeechBubble, SpeechBubbleSpace, false);
        Destroy(bubble.GetComponent<Button>());
        Vector2 loc = new Vector2(90, -(85 + totalbubblenum++ * 200));
        Vector3 rot = Vector3.zero;
        loc.x = talk.speaker == Speaker.Hero ? loc.x : -loc.x;
        rot.y = talk.speaker == Speaker.Hero ? 0 : 180;
        bubble.transform.localPosition = loc;
        bubble.transform.localRotation = Quaternion.Euler(rot);
        bubble.transform.localScale = new Vector3(2, 2, 0);
        bubble.GetComponentInChildren<Text>().text = talk.content;
        bubble.GetComponentInChildren<Text>().transform.rotation = Quaternion.identity;
    }
    public void Click()
    {
        if (selectdelay) return;
        if (speechindex == NowTalk.Length-1 || NowTalk[speechindex].commonroute)
        {
            if(NowTalk[speechindex].choicelist.Length==0)
            {
                // 마지막 대사인경우 종료.
            }
            WordBlockSpace.gameObject.SetActive(true);
            selectdelay = true;
            int num = NowTalk[speechindex].choicelist.Length;
            for(int i=0;i<NowTalk[speechindex].choicelist.Length;i++)
            {
                GameObject obj = Instantiate(WordBlock, WordBlockSpace, false);
                Text quarter = obj.GetComponentInChildren<Text>();
                quarter.text = NowTalk[speechindex].choicelist[i];
                obj.AddComponent<Button>().onClick.AddListener(delegate { QuarterSelect(quarter); });
                Vector2 pos = new Vector2(0, -10);
                switch(i)
                {
                    case 0:
                        pos.x = -340;
                        break;
                    case 1:
                        pos.x = 340;
                        break;
                }
                obj.transform.localPosition = pos;
            }
        }
        else
        {
            speechindex++;
            Speech();
        }
    }
    public void QuarterSelect(Text quarter)
    {
        selectdelay = false;
        if(NowTalk[speechindex].commonroute)
        {
            speechindex++;
            NowTalk[speechindex].content = quarter.text;
        }
        else
        {
            int choiceindex = NowTalk[speechindex].Goindex[quarter.transform.GetSiblingIndex()];
            NowTalk = info.choicetalk[choiceindex];
            speechindex = 0;
        }
        foreach (Transform tr in WordBlockSpace)
            Destroy(tr.gameObject);
        WordBlockSpace.gameObject.SetActive(false);
        Speech();

    }
}
