using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 프로그램을 첫 실행시 출력되는 인트로화면 객체들에 대한 스크립트
/// </summary>
public class PrologObjects : CustomUI
{
    protected override void UIAwake()
    {
        Screen.SetResolution(540, 960, false);
    }
    /// <summary>
    /// 새로하기
    /// </summary>
    public void Button1Click()
    {
        CustomSceneManager.savedata = new SaveData(false);
        CustomSceneManager.goMainScene();
    }
    /// <summary>
    /// 이어하기. 현재 마땅한 기능이 없으므로 레스토랑스가 빙의해있다.
    /// </summary>
    public void Button2Click()
    {
        CustomSceneManager.savedata = new SaveData(true);
        CustomSceneManager.goMainScene();
    }
    /// <summary>
    /// 업적
    /// </summary>
    public void Button3Click()
    {

    }
}

