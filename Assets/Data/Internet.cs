using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Internet : MonoBehaviour {
    [Serializable]
    public class SiteInfo
    {
        public string name;
        public string url;
        public string content;

    }
    [Serializable]
    public class KnowInInfo
    {
        public string title;
        public string content;
        public bool answerenable;
        public string answercontent;
        public bool imageenable;
        public Texture2D image;
    }
    [Serializable]
    public class WikiInfo
    {
        public string name;
        public string content;
    }
    public string Answer;
    public string Relation;
    public SiteInfo[] site;
    public KnowInInfo[] knowin;
    public WikiInfo[] wiki;
}
