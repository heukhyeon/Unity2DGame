using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//메세지, 인터넷등의 어플들에 대해 "메인 스테이지에서" Warp시 호출되는, 즉 ~App객체들에 담기는 스크립트.
//Apps에서 enum을 선택한것에 따라 나머지 속성이 보인다.
//Apps는 커스텀 에디터를 사용한다.
public interface MainStageApps
{
    void goApp();
}
[Serializable]
public enum AppKind { Null,Message, Internet, Gallery, Calculator, Camera, BallMove, SNS, Dictionary, Youtube, Notice, Music, Garbage }
[Serializable]
public enum AppStatus { NotOpen,NotClear,Clear }
[Serializable]
public class Apps:MonoBehaviour
{
    public AppKind kind;
    public InternetApp internet = null;
    public MessageApp message = null;
}

[Serializable]
public class InternetApp : MainStageApps
{
    public List<InternetAppInfo> cases;
    public void goApp()
    {
        CustomSceneManager.apps.internet = cases[UnityEngine.Random.Range(0, cases.Count)];
    }
}

[Serializable]
public class MessageApp : MainStageApps
{
    public List<MessageAppInfo> cases;
    public void goApp()
    {
        CustomSceneManager.apps.message = cases[UnityEngine.Random.Range(0, cases.Count)];
    }
}

