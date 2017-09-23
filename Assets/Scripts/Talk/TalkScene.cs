#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TalkScene : MonoBehaviour {
    private struct TalkInfo
    {
        [HideInInspector]
        public int index;
        public string comment;//선택지 출현시 해당 선택지로 가는 문구
        public Talk.TalkInfo.TalkSpeech[] content;
        public TalkInfo[] nexts;
    }
    [SerializeField]
    GameObject button;
    [SerializeField]
    GameObject bubble;
    [SerializeField]
    RectTransform bubblespace;
    [SerializeField]
    RectTransform choicespace;
    TalkInfo talk = new TalkInfo();
    [SerializeField]
    Scrollbar bar;
    readonly Vector2 defaultpos = new Vector2(670, 1536);
    List<GameObject> bubbles = new List<GameObject>();
    int bubblecount = 0;
    bool clickenable = true;
	// Use this for initialization
	void Start () {
        InfoConnect();
        StartCoroutine("BubbleCreate");
	}
    void InfoConnect()
    {
        List<Talk.TalkInfo> info = new List<Talk.TalkInfo>(SmartPhone.GetData<Talk>().info);
        //처음 씬에 대한 노드를 잇는다.
        Talk.TalkInfo first= info.Find((x) => { return x.connectbefore == "기본"; });
        info.Remove(first);
        talk.comment = first.comment;
        talk.content = first.content;
        talk.nexts = FindNode(talk, info);
    }
    TalkInfo[] FindNode(TalkInfo run,List<Talk.TalkInfo>info)
    {
        Talk.TalkInfo[] nodes = info.FindAll((x) => { return x.connectbefore == run.comment; }).ToArray();
        foreach (var node in nodes) info.Remove(node);
        if (nodes.Length > 0)
        {
            TalkInfo[] ret = new TalkInfo[nodes.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new TalkInfo();
                ret[i].comment = nodes[i].comment;
                ret[i].content = nodes[i].content;
                ret[i].nexts = FindNode(ret[i], info);
            }
            return ret;
        }
        else return null;
    }
	// Update is called once per frame
    IEnumerator BubbleCreate()
    {
        Debug.Log(bubblecount);
        bool dir = talk.content[talk.index].isHero;
        GameObject bub = Instantiate(bubble, bubblespace, false);
        Vector2 pos = new Vector2(dir == true ? 1500 : -1000, 0);
        if (bubbles.Count > 0) pos.y = bubbles[bubbles.Count - 1].transform.localPosition.y - 300;
        else pos.y = 0;
        Text content = bub.GetComponentInChildren<Text>();
        content.text = null;
        bub.transform.localPosition = pos;
        float speed = 1f;
        bubblecount++;
        bubblespace.sizeDelta = new Vector2(1080, bubblecount * 320);
        bar.value = 0;
        if (dir)
        {
            float goal = 520;
            while(pos.x>goal)
            {
                pos.x -= speed;
                speed += speed / 10f;
                bub.transform.localPosition = pos;
                yield return new WaitForEndOfFrame();
            }
            goal = 670;
            while (pos.x < goal)
            {
                pos.x += speed;
                if (pos.x > goal) pos.x = goal;
                speed += speed / 10f;
                bub.transform.localPosition = pos;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            bub.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
            content.transform.rotation = Quaternion.identity;
            float goal = 520;
            while(pos.x<goal)
            {
                pos.x += speed;
                speed += speed / 10f;
                bub.transform.localPosition = pos;
                yield return new WaitForEndOfFrame();
            }
            goal = 410;
            while (pos.x > goal)
            {
                pos.x -= speed;
                if (pos.x < goal) pos.x = goal;
                speed += speed / 10f;
                bub.transform.localPosition = pos;
                yield return new WaitForEndOfFrame();
            }
        }
        for(int i=0;i<talk.content[talk.index].content.Length;i++)
        {
            content.text += talk.content[talk.index].content[i];
            yield return new WaitForEndOfFrame();
        }
        bubbles.Add(bub);
        clickenable = true;
    }
    IEnumerator ChoiceCreate()
    {
        int cnt = talk.nexts.Length;
        if(cnt==0)
        {
            yield return new WaitForEndOfFrame();
            //스테이지 복귀
        }
        else
        {
            Vector2 start = new Vector2(-350, -2000);
            float plus = cnt == 2 ? 700 : 350; //선택지는 2가지 또는 3가지로만 제한.
            Transform[] blocks = new Transform[cnt];
            for(int i=0;i<cnt;i++)
            {
                GameObject block = Instantiate(button, choicespace, false);
                block.transform.localPosition = start + new Vector2(plus*i, 0);
                blocks[i] = block.transform;
            }
            Vector2 size = choicespace.sizeDelta;
            float speed = 1f;
            while(true)
            {
                bool pass = true;
                if(size.y<400)
                {
                    size.y += speed;
                    if (size.y > 400) size.y = 400;
                    choicespace.sizeDelta = size;
                }
                foreach(var block in blocks)
                {
                    if(block.localPosition.y<150)
                    {
                        pass = false;
                        Vector2 pos = block.localPosition;
                        pos.y += speed;
                        block.localPosition = pos;
                    }
                }
                if (size.y == 400&&pass) break;
                speed += speed / 10f;
                yield return new WaitForEndOfFrame();
            }
            speed /= 3f;
            while(true)
            {
                bool pass = true;
                foreach(var block in blocks)
                    if(block.localPosition.y>200)
                    {
                        pass = false;
                        Vector2 pos = block.localPosition;
                        pos.y -= speed;
                        if (pos.y < 200) pos.y = 200;
                        block.localPosition = pos;
                    }
                if (pass == true) break;
                yield return new WaitForEndOfFrame();
            }
            for(int i=0;i<cnt;i++)
            {
                RawImage image = blocks[i].GetComponent<RawImage>();
                TalkInfo next = talk.nexts[i];
                blocks[i].GetComponentInChildren<Text>().text = talk.nexts[i].comment;
                EventTrigger.Entry sel = new EventTrigger.Entry();
                sel.eventID = EventTriggerType.PointerDown;
                sel.callback.AddListener((data) => { if(clickenable)StartCoroutine(ChoiceSelect(image,next)); });
                blocks[i].gameObject.AddComponent<EventTrigger>().triggers.Add(sel);
            }
        }
        clickenable = true;
    }
    IEnumerator ChoiceSelect(RawImage image, TalkInfo next)
    {
        clickenable = false;
        Color backup = image.color;
        float cnt = 0;
        while(true)
        {
            backup.a = backup.a == 1 ? 0 : 1;
            image.color = backup;
            cnt += Time.deltaTime;
            if (cnt >= 1) break;
            yield return new WaitForEndOfFrame();
        }
        backup.a = 1;
        image.color = backup;
        cnt = 0;
        Vector2 size = choicespace.sizeDelta;
        Vector2 pos = Vector2.zero;
        while(true)
        {
            size.y += 3;
            foreach(RectTransform tr in choicespace)
            {
                pos = tr.localPosition;
                pos.y += 3;
                tr.localPosition = pos;
            }
            choicespace.sizeDelta = size;
            cnt += Time.deltaTime;
            if (cnt >= 1) break;
            yield return new WaitForEndOfFrame();
        }
        float speed = 5;
        while(pos.y>-1000)
        {
            foreach(RectTransform tr in choicespace)
            {
                pos = tr.localPosition;
                pos.y -= speed;
                tr.localPosition = pos;
            }
            if(size.y>0)
            {
                size.y -= speed;
                if (size.y < 0) size.y = 0;
                choicespace.sizeDelta = size;
            }
            speed += speed / 10f;
            yield return new WaitForEndOfFrame();
        }
        talk = next;
        RawImage[] blocks = choicespace.GetComponentsInChildren<RawImage>();
        for (int i = 0; i < blocks.Length; i++) Destroy(blocks[i].gameObject);
        StartCoroutine("BubbleCreate");
    }
    public void Click()
    {
        if (!clickenable) return;
        if(talk.index<talk.content.Length-1)
        {
            talk.index++;
            clickenable = false;
            StartCoroutine(BubbleCreate());
        }
        else if(talk.index==talk.content.Length-1)
        {
            clickenable = false;
            StartCoroutine(ChoiceCreate());
        }
    }
}
