using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SNS:MonoBehaviour
{
    [Serializable]
    public struct Info
    {
        public string name;
        public Texture profile;
        public string content;
        public bool likeenble;
        public string likelist;
        public Texture picture;
    }
    public Info[] infos;
}
