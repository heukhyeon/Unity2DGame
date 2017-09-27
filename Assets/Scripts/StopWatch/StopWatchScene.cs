using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
//당초에는 Stage를 상속했으나 현재 시간을 표시하지않는게 더 좋다는 기획의 의견을 받아들여 Stage 비상속.
public class StopWatchScene : MonoBehaviour
{
    public Text InputNotice;
    public Text AnswerNotice;
    public Button StopButton;
    AudioSource au;
    [SerializeField]
    AudioClip clear;
    [SerializeField]
    AudioClip gameover;
    private bool isClick = false;
    private int Answer;
    private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    private void Start()
    {
        au = GetComponent<AudioSource>();
        StartCoroutine(AnswerShow());
    }
    private IEnumerator AnswerShow()
    {
        Answer = UnityEngine.Random.Range(1, 31); //정답을 1에서 30중 임의의 수중 하나로 선정한다.
        AnswerNotice.text = Answer.ToString();
        bool isTurn = false; //투명하게 할지 불투명하게 할지 결정하는 bool 변수. false 일시 불투명하게한다.
        while (true)
        {
            Vector2 pos = AnswerNotice.transform.localPosition;
            Color color = AnswerNotice.color;
            pos.x += 20; //오른쪽으로 이동시킨다
            AnswerNotice.transform.localPosition = pos;
            if (!isTurn) color.a += Time.deltaTime;
            else color.a -= 2f * Time.deltaTime;
            if (color.a > 1 && !isTurn) isTurn = true;
            if (color.a < 0 && isTurn) break; //완전히 투명해진경우 반복문 종료
            AnswerNotice.color = color;
            yield return new WaitForSeconds(0.01f * Time.deltaTime);
        }
        StopButton.interactable = true;//정지 버튼 활성화
        Invoke("TimeOver", Answer + 2); //정답 시간이후 2초전까지 버튼을 누르지않은경우 게임오버되도록 Invoke
        watch.Start();
        au.Play();
    }
    void TimeOver()
    {
        StopButton.interactable = false;
        StopButton.GetComponentInChildren<Text>().text = "타임 오버!";
        InputNotice.text = "정답 시간:" + Answer;
        StartCoroutine("Clear", false);
    }
    //버튼을 누른경우
    public void Stop()
    {
        if (isClick) return; //중복 클릭을 막음.
        isClick = true;
        StopButton.interactable = false; 
        watch.Stop();//스탑워치 정지.
        float value = (float)watch.ElapsedMilliseconds / 1000;
        bool isAnswer = value < Answer + 1 && value > Answer - 1;
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("현재 시간:{0}\n정답 시간:{1}", value, Answer);
        InputNotice.text = sb.ToString();
        StartCoroutine("Clear",isAnswer);
    }
    //유효 구간내에 클릭했던 말던 메인 스테이지로 돌아가므로, 하나의 IEnumerator내에서 정답, 오답에 따른 다른 이벤트를 재생.
    IEnumerator Clear(bool isAnswer)
    {
        CancelInvoke();
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
        au.PlayOneShot(isAnswer == true ? clear : gameover);
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
        //정답이 아닌경우, 모든 객체를 뒤흔들음.
        if(!isAnswer)
        {
            var tr = this.transform.root.GetComponentsInChildren<Transform>();
            StartCoroutine("Clear", false);
            foreach (var t in tr) if (t.name != "Panel") iTween.ShakePosition(t.gameObject, new Vector3(50, 50, 50), 5f);
        }
        yield return new WaitForSeconds(2f);
        if (isAnswer) SmartPhone.memory.StopWatch = StageMemory.Status.PerfectClear;
        SmartPhone.LoadStage("StageSelect");
    }
}
