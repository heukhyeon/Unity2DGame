using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class CalculatorScene : MonoBehaviour {
    public RectTransform Lifebar;
    public GameObject Life;
    public Text TimerNotice;
    public Text SolveNotice;
    public Text Question;
    public Text Solution;
    public GameObject InputButtons; //입력 버튼들이 모여있는 상위 부모 객체
    private string input = "";
    private int maxTime = 90;
    private int cnt = 3;
    private int problemcnt = 0;
    private string[] ProblemArray = new string[20];
    private short[] Answers = new short[20];
    private void Start()
    {
        StartCoroutine(HeartCompare());
    }
    private IEnumerator HeartCompare()
    {
        for(int i=0;i<cnt;i++)
        {
            GameObject heart = Instantiate(Life, Lifebar, false);
            heart.transform.localPosition = new Vector2(60+(i * 105), 0);
            heart.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            yield return new WaitForSeconds(0.3f);
        }
        ProblemCompare();
    }
    private void ProblemCompare()
    {
        for(int i=0;i<20;i++)
        {
            int n1, n2;
            StringBuilder sb = new StringBuilder();
            int cSwitch = UnityEngine.Random.Range(1, 5); //max-1까지의 값을 반환한다
            NumCompare(out n1, out n2, cSwitch);
            switch(cSwitch)
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
                    sb.AppendFormat("{0}*{1}=", n1, n2);
                    Answers[i] = (short)(n1*n2);
                    break;
                case 4:
                    sb.AppendFormat("{0}/{1}=", n1, n2);
                    Answers[i] = (short)(n1/n2);
                    break;
            }
            ProblemArray[i] = sb.ToString();
        }
        StartCoroutine(ButtonCompare());
    }
    private IEnumerator ButtonCompare()
    {
        Button[] inputbuttons = InputButtons.GetComponentsInChildren<Button>();
        for (int i = 0; i < 12; i++)
        {
            inputbuttons[i].interactable = true;
            switch(i)
            {
                case 9:
                    inputbuttons[i].onClick.AddListener(delegate { Submit(); });
                    break;
                case 10:
                    inputbuttons[i].onClick.AddListener(delegate { ValueInput("0"); });
                    break;
                case 11:
                    inputbuttons[i].onClick.AddListener(delegate { Erase(); });
                    break;
                default:
                    string cnt = (i + 1).ToString();
                    inputbuttons[i].onClick.AddListener(delegate{ValueInput(cnt);});
                    break;
            }
            yield return new WaitForSeconds(5f*Time.deltaTime);
        }
        Question.text = ProblemArray[problemcnt];
        SolveNotice.text = "0/20";
        TimeMeasure();
    }
    private void NumCompare(out int n1,out int n2,int cSwitch)
    {
       switch(cSwitch)
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
    private void TimeMeasure()
    {
        maxTime--;
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0}:{1}", (maxTime / 60).ToString("D2"), (maxTime % 60).ToString("D2"));
        TimerNotice.text = sb.ToString();
        Invoke("TimeMeasure", 1f);
    }
    public void ValueInput(string val)
    {
        Debug.Log(input.Length);
        if (input.Length > 3)
            input= input.Remove(0, 1);
        input += val;
        Solution.text = int.Parse(input).ToString("n0");
    }
    public void Submit()
    {
        StringBuilder sb = new StringBuilder();
        int num = int.Parse(input);
        if (num != Answers[problemcnt])
        {
            cnt--;
            Destroy(Lifebar.GetChild(cnt).gameObject);
        }
        problemcnt++;
        sb.Length = 0;
        sb.AppendFormat("{0}/20", problemcnt);
        SolveNotice.text = sb.ToString();
        Question.text = ProblemArray[problemcnt];
        input = "";
        Solution.text = null;
    }
    public void Erase()
    {
        Debug.Log("클릭");
        if(input.Length>0)
        {
            Debug.Log(input[input.Length - 1]);
            input =input.Remove(input.Length - 1);
            if (input.Length > 0) Solution.text = int.Parse(input).ToString("n0");
            else Solution.text = null;
        }
    }
}
