#pragma warning disable 0649
#pragma warning disable 0169
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//전체적인 게임 관리. 각 씬에서 자주 쓰이는 메소드 포함.
public static class SmartPhone
{
    public static string NextScene { get; private set; }
    public const string MainStage = "StageSelect";
    public static StageMemory memory;
    public static T GetData<T>() where T:Component
    {
        T ret = (Resources.Load("SaveData") as GameObject).GetComponent<T>();
        return ret;
    }
    public static Cut GetCut()
    {
        Cut[] array = (Resources.Load("SaveData") as GameObject).GetComponents<Cut>();
        return memory.ClearCheck == true ? array[1] : array[0];
    }
    public static void LoadStage(string name)
    {
            NextScene = memory.ClearCheck == true ? "Ending" : name; //모든 씬을 클리어한경우 컷씬으로 이동.
            SceneManager.LoadScene("Loading");
    }
    public static RectTransform CreateAndPosition(GameObject obj, RectTransform parent, Vector2 pos)
    {
        GameObject target = UnityEngine.Object.Instantiate(obj, parent, false);
        target.transform.localPosition = pos;
        return target.transform as RectTransform;
    }
    //입력된 수치에 맞게 매개변수 객체의 localscale을 바꾸고, 목표치를 넘겨버리는경우 목표치로 맞춤.
    public static void SizeSet<T>(T target, float spd, float goal) where T : MonoBehaviour
    {
        Vector2 size = target.transform.localScale;
        bool enable = size.x > goal;
        size.x += spd;
        size.y += spd;
        if ((enable && size.x < goal) || (!enable && size.x > goal))
        {
            size.x = goal;
            size.y = goal;
        }
        target.transform.localScale = size;
    }
    public static void SizeSet(Transform target, float spd, float goal)
    {
        Vector2 size = target.localScale;
        bool enable = size.x > goal;
        size.x += spd;
        size.y += spd;
        if ((enable && size.x < goal) || (!enable && size.x > goal))
        {
            size.x = goal;
            size.y = goal;
        }
        target.localScale = size;
    }
}
//스테이지 상태 체크
//Clear는 멀티엔딩 구현이후 Talk에서 사용.
public struct StageMemory
{
    public enum Status { NotEnter, NotClear, Clear, PerfectClear }
    public Status Message { get; set; }
    public Status Internet { get; set; }
    public Status Dictionary { get; set; }
    public Status Rollingball { get; set; }
    public Status Calculator { get; set; }
    public Status StopWatch { get; set; }
    public Status Gallery { get; set; }
    public bool ClearCheck
    {
        get
        {
            return Message == Status.PerfectClear && Internet == Status.PerfectClear && Dictionary == Status.PerfectClear && Rollingball == Status.PerfectClear &&
                 Calculator == Status.PerfectClear && StopWatch == Status.PerfectClear && Gallery == Status.PerfectClear;
        }
    }
}


