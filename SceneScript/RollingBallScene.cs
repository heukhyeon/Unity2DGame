using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class RollingBallScene : MonoBehaviour {
    public new Transform camera; //공의 회전에 카메라가 대응하지 않게끔 하기위해 카메라 트랜스폼을 받는다.
    public Transform Subcamera;
    public RectTransform SpdGauge; //현재 스피드 게이지
    public RectTransform OverGauge; //현재 과부하 게이지
    public RectTransform PhaseUpNotice; //페이즈 증가시 알림
    public Text OverHeatNotice; //과부하율 표시
    public Text MassNotice; //무게 표시
    public EventTrigger OverHeatTrigger; //과부하 100%시 일시적으로 감속기능 비활성화를 위한 이벤트 트리거
    public Button OverHeatButton; //과부하 100%시 일시적으로 감속 기능 비활성화를 위한 버튼
    public Button JumpButton; //점프 버튼. 바닥에 있을때만 클릭 활성화를 가능케함.
    public GameObject HeartIcon;//복제할 하트 아이콘
    public RectTransform Heartbar; //라이프바
    private Rigidbody2D ball;
    private Vector3 Retrypos; //실패후 재시작 좌표
    private bool isOverheat = false; //감속 상태인지 판단
    private float overheatvalue = 0f; //감속으로 인한 과부하 수치
    private short cnt = 3; //라이프값
    private bool isInvisible = false; //Enemy 접촉으로 인한 투명 상태
    private StringBuilder sb = new StringBuilder();
    private SpriteRenderer sprite;
    private void OnCollisionEnter2D(Collision2D collision)//점프 버튼 활성화를 위한 바닥 충돌감지
    {
        switch(collision.transform.tag)
        {
            case "Bottom":
                if (collision.transform.tag == "Bottom") JumpButton.interactable = true;
                break;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)//점프 버튼 비활성화를 위한 바닥 충돌감지
    {
        if (collision.transform.tag == "Bottom") JumpButton.interactable = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)//재시작, 페이즈 전환 등의 감지를위한 트리거 감지
    {
        switch(collision.transform.tag)
        {
            case "FailTrigger": //추락(실패)
                cnt--;
                Destroy(Heartbar.GetChild(cnt).gameObject);
                Debug.Log("접촉"+collision.transform.name);
                if (cnt > 0) Retry();
                break;
            case "Enemy": //장애물
                if(!isInvisible) //현재 점멸중이 아닐 경우에만
                {
                    isInvisible = true; //일정시간동안 장애물에 맞아도 하트가 감소되지 않게끔 하기위해 bool 변수 활성화
                    cnt--;
                    Destroy(Heartbar.GetChild(cnt).gameObject); //하트 제거
                    StartCoroutine(InvisibleEffect()); //점멸 이펙트 시작
                }
                break;
            case "PhaseUP": //페이즈 전환
                RaycastHit2D hit = Physics2D.Raycast(ball.position, Vector2.down);
                if (hit) //맞은게 있는경우
                {
                    Retrypos = hit.point; //리트라이 좌표 갱신
                    Destroy(collision.gameObject); //중복 감지를 막기 위해 제거
                    StartCoroutine(PhaseUpEvent()); //페이즈 전환 이펙트 출력
                }
                break;
        }
    }
    private void Start()
    {
        ball = GetComponent<Rigidbody2D>();
        Retrypos = ball.position;
        ball.isKinematic = true;
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(HeartCompare());
        StartCoroutine(OverHeatCheck());
    }
    private void Update()
    {
        float abs_velocity = Mathf.Abs(ball.velocity.x); //이동 방향을 알수없으므로 절댓값화
        if (ball.velocity.x == 0) overheatvalue = 0; //속도가 0인경우 (=벽 등에 충돌시) 과부하값 초기화
        if(isOverheat)//감속중인경우
        {
            if(Mathf.Abs(ball.velocity.x)>0.5f)//감속만으로는 0이 될수없도록 함.
            {
                if(ball.velocity.x>0.5f) ball.velocity = new Vector2(ball.velocity.x - 0.5f, ball.velocity.y); //오른쪽으로 이동중인 경우
                else ball.velocity = new Vector2(ball.velocity.x + 0.5f, ball.velocity.y); //왼쪽으로 이동중인경우
                overheatvalue += 0.1f; //과부하 수치 상승
            }
        }
        else//감속버튼 미클릭
        {
            overheatvalue -= 0.05f;
            if (overheatvalue < 0) overheatvalue = 0;
        }
        camera.rotation = Quaternion.identity; //공은 회전하지만 카메라는 회전하지 않게 함.
        float rate = overheatvalue / abs_velocity; //현재 속도와 과부하수치의 비율
        SpdGauge.sizeDelta = new Vector2(10 * abs_velocity, 100);
        OverGauge.sizeDelta = new Vector2(10 * overheatvalue, 100);
        if (rate > 0.9f) OverHeatNotice.color = Color.red;
        else if (rate > 0.6f) OverHeatNotice.color = Color.yellow;
        else OverHeatNotice.color = Color.white;
        if (ball.velocity.x == 0) rate = 0f;
        sb.Length = 0;
        sb.AppendFormat("과부하율: {0}%", (rate*100).ToString("N0"));
        OverHeatNotice.text = sb.ToString();
        sb.Length = 0;
        sb.AppendFormat("무게:{0}", ball.mass.ToString("N1"));
        MassNotice.text = sb.ToString();
    }
    private void Retry()//추락시 위치 재지정
    {
        OverHeatEnd();
        overheatvalue = 0f;
        ball.velocity = Vector2.zero;
        ball.position = Retrypos;

    }
    public void OverHeat()//감속 버튼 클릭시
    {
        isOverheat = true;
    }
    public void OverHeatEnd()//감속 버튼 클릭 해제시 
    {
        isOverheat = false;
    }
    public void Jump()//점프 버튼 클릭시
    {
        ball.AddForce(Vector2.up * 1000f);
    }
    private IEnumerator HeartCompare()//게임 시작시 좌측 하단에 하트 충전
    {
        for(int i=0;i<cnt;i++)
        {
            GameObject obj = Instantiate(HeartIcon, Heartbar, false);
            obj.transform.localPosition = new Vector3((i * 50)+30, 0, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ball.isKinematic = false;
    }
    private IEnumerator OverHeatCheck()//과부하 체크
    {
        while(true)
        {
            if(Mathf.Abs(ball.velocity.x)>0 && overheatvalue>Mathf.Abs(ball.velocity.x))
            {
                overheatvalue = 0f;
                ball.mass += 0.1f;
                OverHeatEnd();
                OverHeatButton.interactable = false;
                OverHeatTrigger.enabled = false;
                yield return new WaitForSeconds(1f);
                OverHeatButton.interactable = true;
                OverHeatTrigger.enabled = true;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator PhaseUpEvent()//페이즈 전환 이벤트
    {
        float loc = PhaseUpNotice.localPosition.x;
        while(true)
        {
            Vector2 pos = PhaseUpNotice.localPosition;
            pos.x += 50;
            PhaseUpNotice.localPosition = pos;
            if (loc < 0 && pos.x >= 0) yield return new WaitForSeconds(2f);
            loc = pos.x;
            yield return new WaitForSeconds(Time.deltaTime);
            if (loc > 850) break;
        }
        PhaseUpNotice.localPosition = new Vector2(-1000, -1600);
    }
    private IEnumerator InvisibleEffect()//피격시 점멸 이벤트
    {
        float time = 0;
        while(true)
        {
            sprite.enabled = !sprite.enabled;
            time += 2f*Time.deltaTime;
            yield return new WaitForSeconds(2f*Time.deltaTime);
            if (time > 2f) break;
        }
        isInvisible = false;
        sprite.enabled = true;
    }
}
