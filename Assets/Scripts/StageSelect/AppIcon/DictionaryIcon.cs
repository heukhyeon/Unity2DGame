using System;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryIcon:MonoBehaviour,AppIcon
{
    [SerializeField]
    public DictionaryAppinfo[] infos = new DictionaryAppinfo[1];
    public void Select()
    {
        GameManager.appinfo = infos[UnityEngine.Random.Range(0, infos.Length)];
        GameManager.speech = GetComponent<PlayerSpeechScript>();
        GetComponent<SceneWarp>().goScene();
    }
}

[Serializable]
public class DictionaryAppinfo
{
    [Serializable]
    public class Quest
    {
        public int index = 0;
        public int pos_x = 0;
        public int pos_y = 0;
        public string answer = "";
        public string hint = "";
    }
    [SerializeField]
    public Quest[] street = new Quest[0];
    [SerializeField]
    public Quest[] column = new Quest[0];
    [SerializeField]
    public char[,] answers = new char[10, 10];
    [SerializeField]
    public int[,] space = new int[10, 10];
}