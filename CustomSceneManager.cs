using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 이동을 처리하는 정적 클래스
/// </summary>
public static class CustomSceneManager
{

    /// <summary>
    /// 다음 씬으로 넘어갈 씬 인덱스
    /// </summary>
    private static byte sceneindex;
    /// <summary>
    /// 메인 스테이지의 빌드 인덱스. 빌드 순서가 바뀔때마다 변경하는 방식을 사용한다.
    /// </summary>
    private const byte MAINSCENEINDEX = 1;
    /// <summary>
    /// 로딩 씬의 빌드 인덱스. 빌드 순서가 바뀔때마다 변경하는 방식을 사용한다.
    /// </summary>
    private const byte LOADINGINDEX = 2;
    /// <summary>
    /// 씬 빌드 목록에서, 어플리케이션 목록 이전에 빌드된 씬(메인 스테이지, 인트로 등)의 수. 
    /// 해당 변수 + goScene에서 받은 Transform의 부모에 대한 자신의 index를 더한것이 실제 이동씬 index가 된다.
    /// </summary>
    private const byte BEFOREAPPCOUNT = 3;
    /// <summary>
    /// 최대 하트수. 난이도에 따라 메인 스테이지에서 조정가능.
    /// </summary>
    public static byte MAXHEARTS = 3;
    /// <summary>
    /// 각 어플에 대한 정답등을 보관한다. 어플 진입시 각 어플에서 해당 변수에 자신들이 가진 클래스를 대입하는 방식.
    /// </summary>
    public static AppManager apps;
    /// <summary>
    /// 메인 스테이지로 복귀시 플레이어 객체 위치
    /// </summary>
    private static Vector2 playerlocation = new Vector2(-2.4f, 0.32f);
    /// <summary>
    /// 로딩씬에서 이전씬에 지정되었던 다음씬으로 넘어간다.
    /// </summary>
    public static AsyncOperation goScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.buildIndex!=LOADINGINDEX)
        {
            Debug.LogError("Loading씬 이외의 씬에서 잘못된 씬 이동이 사용되었습니다 :" + scene.name);
            return null;
        }
        return SceneManager.LoadSceneAsync(sceneindex);
    }
    /// <summary>
    /// 메인 스테이지에서 로딩씬을 거쳐 지정된 씬으로 넘어간다.
    /// </summary>
    /// <param name="obj">레이캐스트된 어플 객체의 Trasnform</param>
    /// <param name="position">현재 플레이어 위치 정보</param>
    public static void goScene(Transform app,Vector2 position)
    {
        sceneindex = BEFOREAPPCOUNT;
        foreach (Transform obj in app.parent)
        {
            if (obj == app) break;
            sceneindex++;
        }
        Debug.Log(sceneindex);
        playerlocation = position;
        SceneManager.LoadSceneAsync(LOADINGINDEX);
    }
    /// <summary>
    /// 어플리케이션에서 메인 스테이지로 복귀할때 사용하는 메소드
    /// </summary>
    public static void goMainScene()
    {
        sceneindex = MAINSCENEINDEX;
        SceneManager.LoadSceneAsync(MAINSCENEINDEX);
    }
    /// <summary>
    /// 어플리케이션에서 메인 스테이지로 복귀시 기존 플레이어 위치를 반환한다.
    /// </summary>
    public static Vector2 getPlayerPosition()
    {
        return playerlocation;
    }
}
/// <summary>
/// 각 어플들에 대한 문제등을 담고있는 클래스
/// </summary>
public struct AppManager
{
    public InternetAppInfo internet;
    public MessageAppInfo message;
}