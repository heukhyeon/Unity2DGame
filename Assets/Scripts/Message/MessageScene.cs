using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageScene : Stage,IBeforeClear,IClickDelay,INormalButton
{
    public abstract class Space:MonoBehaviour
    {
        [SerializeField]
        protected GameObject originItem;
        [SerializeField]
        protected RectTransform space;
        [HideInInspector]
        public List<RectTransform> items;
        public MessageScene scene;
        public abstract void Create(string text);
        public abstract void ItemSort();
    }
    public MessageBlockSpace blockspace;
    public MessageBubbleSpace bubblespace;
    [SerializeField]
    Message.MessageInfo[] info = new Message.MessageInfo[0];
    string[] answers;
    public Image background;
    protected override Action BeforeIntro
    {
        get
        {
            return () =>
            {
                background = bubblespace.transform.parent.GetComponent<Image>();
                blockspace.scene = this;
                bubblespace.scene = this;
                bubblespace.Init(info);
                answers = new string[info.Length];
                for (int i = 0; i < answers.Length; i++) answers[i] = info[i].content;
                var infos = InfoShuffle(info);
                for (int i = 0; i < infos.Length; i++) blockspace.Create(infos[i].content);
                for (int i = 0; i < infos.Length; i++) blockspace.items[i].gameObject.SetActive(false);
            };
        }
    }
    protected override IEnumerator AfterIntro {
        get
        {
            foreach(var block in blockspace.items)
            {
                block.gameObject.SetActive(true);
                iTween.ShakeScale(block.gameObject, new Vector2(5, 5), 2f);
                yield return new WaitForSeconds(0.5f);
            }
            clickenable = true;
        }
    }
    public override int MaxLife{get { return 3; }}
    public bool Answer {
        get
        {
            Text[] nowbubbles = bubblespace.GetMessage();
            for (int i = 0; i < answers.Length; i++)
            {
                string now = nowbubbles[i].text;
                string answer = answers[i];
                if (now != answer) return false;
            }
            return true;
        }
    }
    public override MissionType missontype
    {
        get { return MissionType.Count; }
    }
    public override int WestedLifeClear { get { return 3; } }
    public override int WestedLifeGameOver { get { return 5; } }
    public bool clickenable { get; set; }
    public IEnumerator BeforeClear
    {
        get
        {
            while(true)
            {
                if(blockspace.transform.localScale.x>0) SmartPhone.SizeSet(blockspace, -Time.deltaTime, 0);
                if (bubblespace.transform.localScale.x > 0)  SmartPhone.SizeSet(bubblespace, -Time.deltaTime, 0);
                if (blockspace.transform.localScale.x == 0 && bubblespace.transform.localScale.x == 0) break;
                else yield return new WaitForEndOfFrame();
            }
        }
    }
}


