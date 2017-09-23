#pragma warning disable 0649
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class CalculatorQuestionSpace : MonoBehaviour
{
    [SerializeField]
    Text solve;
    [SerializeField]
    Text question;
    [SerializeField]
    Text solution;
    [HideInInspector]
    public CalculatorScene scene;
    [HideInInspector]
    public string[] ProblemArray = new string[20];
    [HideInInspector]
    public short[] Answers = new short[20];
    public string Solve { get { return solve.text; } set{ solve.text = value; } }
    public string Question { get{ return question.text; } set { question.text = value; } }
    public string Solution { get { return solution.text; } set{ solution.text = value; } }
    public void ProblemCompare()
    {
        for (int i = 0; i < 20; i++)
        {
            int n1, n2;
            StringBuilder sb = new StringBuilder();
            int cSwitch = UnityEngine.Random.Range(1, 5); //max-1까지의 값을 반환한다
            NumCompare(out n1, out n2, cSwitch);
            switch (cSwitch)
            {
                case 1:
                    sb.AppendFormat("{0}+{1}=", n1, n2);
                    Answers[i] = (short)(n1 + n2);
                    break;
                case 2:
                    sb.AppendFormat("{0}-{1}=", n1, n2);
                    Answers[i] = (short)(n1 - n2);
                    break;
                case 3:
                    sb.AppendFormat("{0}×{1}=", n1, n2);
                    Answers[i] = (short)(n1 * n2);
                    break;
                case 4:
                    sb.AppendFormat("{0}/{1}=", n1, n2);
                    Answers[i] = (short)(n1 / n2);
                    break;
            }
            ProblemArray[i] = sb.ToString();
        }
    }
    private void NumCompare(out int n1, out int n2, int cSwitch)
    {
        switch (cSwitch)
        {
            case 1:
                n1 = UnityEngine.Random.Range(1, 25);
                n2 = UnityEngine.Random.Range(1, 25);
                break;
            case 2:
                n1 = UnityEngine.Random.Range(1, 25);
                n2 = UnityEngine.Random.Range(1, n1);
                break;
            case 3:
                n1 = UnityEngine.Random.Range(1, 25);
                n2 = UnityEngine.Random.Range(1, 25);
                break;
            case 4:
                n1 = UnityEngine.Random.Range(1, 25);
                n2 = UnityEngine.Random.Range(1, n1);
                break;
            default:
                n1 = 0;
                n2 = 0;
                break;
        }
    }
    public void QuestionSet(int index)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("현재 문제:{0}/20", index);
        Solve = sb.ToString();
        Question = ProblemArray[index];
        scene.NowValue = null;
    }
}

