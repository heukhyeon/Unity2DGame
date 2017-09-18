using System;
using UnityEngine;

public class Message : MonoBehaviour
{
    [Serializable]
    public struct MessageInfo : ICloneable
    {
        public bool speaker;
        public string content;

        public object Clone()
        {
            MessageInfo ret = new MessageInfo();
            ret.speaker = this.speaker;
            ret.content = this.content;
            return ret;
        }
    }
    public MessageInfo[] items;
}