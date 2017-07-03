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
        
    }
    /// <summary>
    /// 새로하기
    /// </summary>
    public void Button1Click()
    {
        SceneObjectManager.NextScene = "Stage1";
        SceneObjectManager.Dispose();
        SceneManager.LoadScene("Loading");
    }
    /// <summary>
    /// 이어하기. 현재 마땅한 기능이 없으므로 레스토랑스가 빙의해있다.
    /// </summary>
    public void Button2Click()
    {
        Application.OpenURL("http://kr.battle.net/heroes/ko/");
    }
    /// <summary>
    /// 업적
    /// </summary>
    public void Button3Click()
    {

    }
}

