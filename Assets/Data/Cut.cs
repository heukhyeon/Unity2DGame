using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Cut : MonoBehaviour
{
    [Serializable]
    public struct CutInfo
    {
        public Texture sprite;
        public string content;
    }
    public CutInfo[] cuts = new CutInfo[0];
}

