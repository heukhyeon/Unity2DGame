using System;
using UnityEngine;

public class InternetIcon:MonoBehaviour,AppIcon
{
    public InternetAppInfo[] infos;
    public void Select()
    {
        GameManager.appinfo = infos[UnityEngine.Random.Range(0, infos.Length)];
        GameManager.speech = GetComponent<PlayerSpeechScript>();
        GetComponent<SceneWarp>().goScene();
    }
}

[Serializable]
public struct InternetAppInfo
{
    [Serializable]
    public struct Site
    {
        public string name;
        public string url;
        public string content;
    }
    [Serializable]
    public struct Knowin
    {
        public string question_title;
        public string question_content;
        public bool image_enable;
        public Sprite image;
        public bool answer_enable;
        public string answer_content;
    }
    [Serializable]
    public struct Encyclopedia
    {
        public string name;
        public string content;
    }
    public Site[] sites;
    public Knowin[] knowins;
    public Encyclopedia[] encyclopedias;
    [SerializeField]
    public string[] relations;
    [SerializeField]
    public string[] answers;
}
