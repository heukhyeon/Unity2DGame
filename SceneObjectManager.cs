using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 씬 내부의 게임오브젝트들을 관리하고 필요시 해당 오브젝트 내부의 스크립트 컴포넌트를 반환하는 정적 클래스
/// </summary>
public static class SceneObjectManager
{
    private static Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
    private static string nextscene;
    public static string NextScene
    {
        get
        {
            return nextscene;
        }
        set
        {
            nextscene = value;
        }
    }
    /// <summary>
    /// 호출자를 게임 매니저에 추가한다.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool Add(GameObject obj)
    {
        try
        {
            objects.Add(obj.name, obj);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
            return false;
        }
    }
    /// <summary>
    /// 클래스 이름과 동일한 이름을 가진 GameObject로부터 해당 클래스 컴포넌트를 반환한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetComponent<T>()
    {
        T obj = objects[typeof(T).ToString()].GetComponent<T>();
        if (obj == null)
        {
            StringBuilder sb = new StringBuilder(typeof(T).ToString());
            sb.Append(": 해당 클래스를 가진 객체가 없습니다.");
            Debug.Log(sb.ToString());
        }
        return obj;
    }
    /// <summary>
    /// 주어진 매개변수 key를 이름으로 갖는 GameObject로부터 해당 클래스 컴포넌트를 반환한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T GetComponent<T>(string key)
    {
        T obj = objects[key].GetComponent<T>();
        if (obj == null)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.AppendFormat(": 해당 객체에 {0} 스크립트가 탑재되어있지않습니다. 인스펙터를 확인해주세요.", typeof(T).ToString());
            Debug.Log(sb.ToString());
        }
        return obj;
    }
    /// <summary>
    /// 씬에서 다음 씬으로 넘어갈때 사용. 기존에 부여되었던 오브젝트들을 모두 제거한다.
    /// </summary>
    public static void Dispose()
    {
        objects.Clear();
        GC.Collect();
    }
}