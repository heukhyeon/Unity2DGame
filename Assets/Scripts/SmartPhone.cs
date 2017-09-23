#pragma warning disable 0649
#pragma warning disable 0169
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class SmartPhone
{
    public static bool LoadigSkip;
    public static string NextScene { get; private set; }
    public const string MainStage = "StageSelect";
    public static StageMemory memory;
    public static T GetData<T>() where T:Component
    {
        T ret = (Resources.Load("SaveData") as GameObject).GetComponent<T>();
        return ret;
    }
    public static void LoadStage(string name)
    {
            if (LoadigSkip) SceneManager.LoadScene(name);
            else
            {
                NextScene = name;
                SceneManager.LoadScene("Loading");
            }
    }
    public static RectTransform CreateAndPosition(GameObject obj, RectTransform parent, Vector2 pos)
    {
        GameObject target = UnityEngine.Object.Instantiate(obj, parent, false);
        target.transform.localPosition = pos;
        return target.transform as RectTransform;
    }
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

public struct StageMemory
{
    public enum Status { NotEnter,NotClear,Clear,PerfectClear}
    public Status Message { get; set; }
    public Status Internet { get; set; }
}

