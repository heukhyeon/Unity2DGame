using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 공굴리기, 계산기, 스탑워치등 AppInfo를 전달할 필요가 없는 앱아이콘에 삽입하는 컴포넌트
/// </summary>
public class CommonIcon : MonoBehaviour, AppIcon
{
    public void Select()
    {
        GameManager.speech = GetComponent<PlayerSpeechScript>();
        GetComponent<SceneWarp>().goScene();
    }
}
