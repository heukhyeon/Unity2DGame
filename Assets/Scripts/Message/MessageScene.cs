using commonStage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageScene : Stage,IClickDelay
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
    string[] answers;
    public Image background;
    public override Action BeforeIntro
    {
        get
        {
            return () =>
            {
                background = bubblespace.transform.parent.GetComponent<Image>();
                Message message = SmartPhone.GetData<Message>();
                bubblespace.Init(message);
                answers = new string[message.items.Length];
                for (int i = 0; i < answers.Length; i++) answers[i] = message.items[i].content;
                var infos = InfoShuffle(message.items);
                for (int i = 0; i < infos.Length; i++) blockspace.Create(infos[i].content);
                for (int i = 0; i < infos.Length; i++) blockspace.items[i].gameObject.SetActive(false);
            };
        }
    }
    public override IEnumerator AfterIntro {
        get
        {
            foreach(var block in blockspace.items)
            {
                block.gameObject.SetActive(true);
                iTween.ShakeScale(block.gameObject, new Vector2(5, 5), 2f);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    public override MissionType missiontype { get { return MissionType.Count; } }
    public override int MaxLife{get { return 3; }}
    public override bool Answer {
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
    public override int WestedLifeClear { get { return 3; } }
    public override int WestedLifeGameover { get { return 5; } }
    public bool clickenable { get; set; }
}


