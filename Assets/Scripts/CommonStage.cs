using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    public enum MissionType { Count,Timer}
public delegate void VoidDelegate();
public abstract class Stage : MonoBehaviour
{
    protected abstract Action BeforeIntro { get; }
    protected abstract IEnumerator AfterIntro { get; }
    public abstract MissionType missontype { get; }
    public abstract int MaxLife { get; }
    public virtual int WestedLifeFail { get { return 1; } }
    public abstract int WestedLifeClear { get; }
    public abstract int WestedLifeGameOver { get; }
    public bool Submitenable { get { return lifeSystem.lifeUI.submit.interactable; } set { lifeSystem.lifeUI.submit.interactable = value; } }
    LifeSystem lifeSystem;
    private void Awake()
    {
        lifeSystem = transform.root.GetComponentInChildren<LifeSystem>();
        if (lifeSystem == null) lifeSystem = GameObject.FindObjectOfType<LifeSystem>();
        lifeSystem.stage = this;
    }
    private void Start()
    {
        BeforeIntro();
        IDisableButton notbutton = GetComponent<IDisableButton>();
        if(notbutton==null)
        {
            IClickDelay delay = GetComponent<IClickDelay>();
            ICustomButton button = GetComponent<ICustomButton>();
            if (button == null)
            {
                INormalButton normal = GetComponent<INormalButton>();
                Action action = new Action(() =>
                {
                    if (normal.Answer)
                    {
                        List<IEnumerator> actions = new List<IEnumerator>();
                        IBeforeClear before = GetComponent<IBeforeClear>();
                        IAfterClear after = GetComponent<IAfterClear>();
                        IEnumerator last;
                        if (before != null) actions.Add(before.BeforeClear);
                        if (after != null)
                        {
                            last = after.AfterClear;
                            actions.Add(lifeSystem.NormalClear);
                        }
                        else last = lifeSystem.NormalClear;
                        actions.Add(CorutineAction(last, ReturnMain));
                        CombineCorutine(actions);
                    }
                    else StartCoroutine(lifeSystem.FailEvent);
                });
                if (delay == null) lifeSystem.lifeUI.submit.onClick.AddListener(() => { action(); });
                else lifeSystem.lifeUI.submit.onClick.AddListener(() => { if (delay.clickenable) action(); });
            }
            else
            {
                lifeSystem.lifeUI.submit.GetComponentInChildren<UnityEngine.UI.Text>().text = button.ButtonName;
                lifeSystem.lifeUI.submit.onClick.AddListener(() => { button.ButtonClick(); });
                button.FailEvent += new VoidDelegate(() => { StartCoroutine(lifeSystem.FailEvent); });
            }
            ISectorClear sector = GetComponent<ISectorClear>();
            if (sector != null) sector.ClearEvent += new VoidDelegate(() => {
                CombineCorutine(new IEnumerator[] { lifeSystem.NormalClear, CorutineAction(lifeSystem.SectorClear, sector.SectorClearAfter) });
            });
        }
        else
        {
            Destroy(lifeSystem.lifeUI.submit.gameObject);
            lifeSystem.lifeUI.submit = null;
            notbutton.ClearEvent += new VoidDelegate(() => { StartCoroutine(CorutineAction(lifeSystem.NormalClear, ReturnMain)); });
            notbutton.FailEvent += new VoidDelegate(() => { StartCoroutine(lifeSystem.FailEvent); });
        }
        CombineCorutine(new IEnumerator[] { lifeSystem.IntroEvent,CorutineAction(AfterIntro,lifeSystem.GameStart) });
    }
    protected T[] InfoShuffle<T>(T[] info) where T : struct
    {
        for(int i=0;i<info.Length;i++)
        {
            int index = UnityEngine.Random.Range(0, info.Length);
            var temp = info[i];
            info[i] = info[index];
            info[index] = temp;
        }
        return info;
    }
    protected void ReturnMain()
    {

    }
    void CombineCorutine(List<IEnumerator>corutines)
    {
        StartCoroutine(CorutineConnect(corutines.ToArray()));
    }
    void CombineCorutine(params IEnumerator[] corutines)
    {
        StartCoroutine(CorutineConnect(corutines));
    }
    IEnumerator CorutineConnect(IEnumerator[] coroutines)
    {
        foreach (IEnumerator cor in coroutines) yield return StartCoroutine(cor);
    }
    IEnumerator CorutineAction(IEnumerator corutine,Action action)
    {
        yield return StartCoroutine(corutine);
        action();
    }
}
public interface IClickDelay
    {
        bool clickenable { get; set; }
    }
public interface INormalButton
{
    bool Answer { get; }
}
public interface ICustomButton
{
    string ButtonName { get; }
    event VoidDelegate FailEvent;
    Action ButtonClick { get; }
}
public interface IBeforeClear
{
    IEnumerator BeforeClear { get; }
}
public interface IAfterClear
{
    IEnumerator AfterClear { get; }
}
public interface ISectorClear
{
    Action SectorClearAfter { get;}
    event VoidDelegate ClearEvent;
}
public interface IDisableButton
{
    event VoidDelegate ClearEvent;
    event VoidDelegate FailEvent;
}
