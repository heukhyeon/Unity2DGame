#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CalculatorScene : Stage, IDisableButton,IBeforeClear
{
    public override MissionType missontype { get { return MissionType.Timer; } }
    public override int MaxLife { get { return 100; } }
    public override int WestedLifeClear { get { return 3; } }
    public override int WestedLifeGameOver { get { return 5; } }
    protected override Action BeforeIntro
    {
        get
        {
            return () =>
            {
                buttonspace.scene = this;
                questionspace.scene = this;
                questionspace.ProblemCompare();
            };
        }
    }
    protected override IEnumerator AfterIntro
    {
        get
        {
            yield return StartCoroutine(buttonspace.ButtonPrepare());
            questionspace.QuestionSet(problemcnt);
        }
    }
    public override int WestedLifeFail
    {
        get
        {
            return 20;
        }
    }
    public IEnumerator BeforeClear
    {
        get
        {
            yield return StartCoroutine(buttonspace.ButtonUnPreapre());
        }
    }
    [SerializeField]
    CalculatorButtonSpace buttonspace;
    [SerializeField]
    CalculatorQuestionSpace questionspace;
    [HideInInspector]
    public int problemcnt = 1;
    public event VoidDelegate ClearEvent;
    public event VoidDelegate FailEvent;
    private string nowvalue = "";
    public string NowValue { get { return nowvalue; }
        set
        {
            nowvalue = value;
            if (nowvalue!=null && nowvalue.Length > 5) nowvalue = nowvalue.Remove(0, 1);
            int ret = 0;
            bool enable = int.TryParse(nowvalue, out ret); 
            questionspace.Solution = String.Format("{0:#,###}", ret);
        }
    }
    public void Submit()
    {
        StringBuilder sb = new StringBuilder();
        int num = int.Parse(nowvalue);
        if (num != questionspace.Answers[problemcnt]) FailEvent();
        problemcnt++;
        if (problemcnt < questionspace.ProblemArray.Length) questionspace.QuestionSet(problemcnt);
        else ClearEvent();
    }

}
