#pragma warning disable 0649
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class MessageIcon : MonoBehaviour, AppIcon
{
    [SerializeField]
    private MessageAppInfo[] infos = new MessageAppInfo[1];
    public StageSelect stage;
    public void Select()
    {
        if(GameManager.prologue.isPlayMessage)
        {
            GameManager.appinfo = infos[UnityEngine.Random.Range(0, infos.Length)];
            GameManager.speech = GetComponent<PlayerSpeechScript>();
            GetComponent<SceneWarp>().goScene();
        }
        else stage.CutStart(StageSelect.PlayCut.Message);
    }
}

[Serializable]
public struct MessageAppInfo
{
    [Serializable]
    public struct MessageSpeech
    {
        public bool isEnemy;
        public string speech;
    }
    public MessageSpeech[] speechs;
}