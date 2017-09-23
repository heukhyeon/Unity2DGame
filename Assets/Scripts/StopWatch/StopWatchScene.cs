using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class StopWatchScene : MonoBehaviour
{
    public Text InputNotice;
    public Text AnswerNotice;
    public Button StopButton;
    private bool isClick = false;
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
        Invoke("TimeOver", Answer + 2);
        watch.Start();
    }
    void TimeOver()
    {
        StopButton.interactable = false;
        StopButton.GetComponentInChildren<Text>().text = "타임 오버!";
        InputNotice.text = "정답 시간:" + Answer;
        StartCoroutine("Clear", false);
    }
    void BackStage()
    {
        SmartPhone.LoadStage("StageSelect");
    }
    public void Stop()
    {
        if (isClick) return;
        isClick = true;
        StopButton.interactable = false;
        watch.Stop();
        float value = (float)watch.ElapsedMilliseconds / 1000;
        bool isAnswer = value < Answer + 1 && value > Answer - 1;
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("현재 시간:{0}\n정답 시간:{1}", value, Answer);
        InputNotice.text = sb.ToString();
        StartCoroutine("Clear",isAnswer);
    }
    IEnumerator Clear(bool isAnswer)
    {
        Color color = AnswerNotice.color;
        color = isAnswer == true ? Color.white : Color.red;
        AnswerNotice.text = isAnswer == true ? "클리어!" : "실패!";
        color.a = 0.2f;
        AnswerNotice.color = color;
        Vector2 pos = AnswerNotice.transform.localPosition;
        pos.x = -1000;
        AnswerNotice.transform.localPosition = pos;
        Vector2 rightpos = pos;
        rightpos.x *= -1;
        RectTransform Leftclear = AnswerNotice.transform as RectTransform;
        RectTransform RightClear = SmartPhone.CreateAndPosition(AnswerNotice.gameObject, this.transform as RectTransform, rightpos);
        int loc = 100;
        float speed = 1;
        while (pos.x < loc)
        {
            pos.x += speed;
            rightpos.x -= speed;
            speed += speed * 0.1f;
            if (pos.x > loc)
            {
                pos.x = loc;
                rightpos.x = -loc;
            }
            Leftclear.localPosition = pos;
            RightClear.localPosition = rightpos;
            yield return new WaitForEndOfFrame();
        }
        loc = 0;
        speed = 0.1f;
        while (pos.x > loc)
        {
            pos.x -= speed;
            rightpos.x += speed;
            speed += speed * 0.5f;
            if (pos.x < loc)
            {
                pos.x = loc;
                rightpos.x = loc;
            }
            Leftclear.localPosition = pos;
            RightClear.localPosition = rightpos;
            yield return new WaitForEndOfFrame();
        }
        color.a = 1;
        AnswerNotice.color = color;
        if(!isAnswer)
        {
            var tr = this.transform.root.GetComponentsInChildren<Transform>();
            StartCoroutine("Clear", false);
            foreach (var t in tr) if (t.name != "Panel") iTween.ShakePosition(t.gameObject, new Vector3(50, 50, 50), 5f);
        }
        yield return new WaitForSeconds(2f);
        SmartPhone.LoadStage("StageSelect");
    }
}
