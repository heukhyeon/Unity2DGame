using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ending:MonoBehaviour
{
    [Serializable]
    public struct CutInfo
    {
        public Texture image;
    }
    public CutInfo[] infos;
}
