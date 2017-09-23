using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Talk:MonoBehaviour
{
    [Serializable]
    public struct TalkInfo
    {
        [Serializable]
        public struct TalkSpeech
        {
            public bool isHero;
            public string content;
        }
        [HideInInspector]
        public int index;
        public string comment;//선택지 출현시 해당 선택지로 가는 문구
        public string connectbefore; //해당 TalkInfo가 어느 선택지로부터 파생되는지. TalkScene에서 파생되는 TalkInfo에는 적용되지 않는다.
        public TalkSpeech[] content;
        //public TalkInfo[] nexts; //Talk에서 파생되는 TalkInfo에는 다음 노드가 존재하지않는다. TalkScene에서 파생되는 TalkInfo에 존재.
    }
    public TalkInfo[] info;
}