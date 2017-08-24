using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class StopWatch : MonoBehaviour
{
    public Text InputNotice;
    public Text AnswerNotice;
    public Button StopButton;
    private float cnt = 0;
    private bool isClick = false;
    private bool TimeOver = false;
    private int Answer;
    private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    private void Start()
    {
        StartCoroutine(AnswerShow());
    }
    private IEnumerator AnswerShow()
    {
        Answer = UnityEngine.Random.Range(1, 31);
        AnswerNotice.text = Answer.ToString();
        bool isTurn = false;
        while (true)
        {
            Vector2 pos = AnswerNotice.transform.localPosition;
            Color color = AnswerNotice.color;
            pos.x += 20;
            AnswerNotice.transform.localPosition = pos;
            if (!isTurn) color.a += Time.deltaTime;
            else color.a -= 2f * Time.deltaTime;
            if (color.a > 1 && !isTurn) isTurn = true;
            if (color.a < 0 && isTurn) break;
            AnswerNotice.color = color;
            yield return new WaitForSeconds(0.01f * Time.deltaTime);
        }
        StopButton.interactable = true;
        watch.Start();
    }
    public void Stop()
    {
        watch.Stop();
        float value = (float)watch.ElapsedMilliseconds / 1000;
        TimeOver = false;
        if (value < Answer + 1 && value > Answer - 1) Debug.Log("정답:" + value);
        else Debug.Log("정답:" + Answer + "\n현재:" + value);
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("현재 시간:{0}\n정답 시간:{1}", value, Answer);
        InputNotice.text = sb.ToString();
    }
}
