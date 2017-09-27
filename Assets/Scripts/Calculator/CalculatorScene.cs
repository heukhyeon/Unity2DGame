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
    //입력 버튼 클릭시
    public void Submit()
    {
        StringBuilder sb = new StringBuilder();
        int num = int.Parse(nowvalue);
        if (num != questionspace.Answers[problemcnt]) FailEvent(); //현재 입력값이 현재 정답과 다른경우 일단 틀렸다는 이벤트 재생
        problemcnt++;//문제 인덱스 증가
        if (problemcnt < questionspace.ProblemArray.Length) questionspace.QuestionSet(problemcnt); //아직 문제가 남은경우 다음 문제로
        else ClearEvent();//문제가 더이상 없는경우 클리어 이벤트.
    }
}
