#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBallScene : Stage, ICustomButton,ISectorClear
{
    Rigidbody2D rBody;
    [SerializeField]
    List<Vector2> Re_Pos = new List<Vector2>();
    [SerializeField]
    new Transform camera;
    [SerializeField]
    Transform background;
    [SerializeField]
    AudioClip jump;
    AudioSource au;
    float dis;
    public event VoidDelegate FailEvent;
    public event VoidDelegate ClearEvent;
    protected override Action BeforeIntro
    {
        get
        {
            return () =>
            {
                au = GetComponent<AudioSource>();
                rBody = GetComponent<Rigidbody2D>();
                dis = transform.position.y - camera.position.y;
                rBody.isKinematic = true;
            };
        }
    }
    protected override IEnumerator AfterIntro
    {
        get
        {
            rBody.isKinematic = false;
            yield return new WaitForEndOfFrame();
        }
    }
    public override MissionType missontype { get { return MissionType.Count; } }
    public override int MaxLife { get { return 30; } }
    public override int WestedLifeClear { get { return 3; } }
    public override int WestedLifeGameOver { get { return 5; } }
    //점프 가능 여부를 결정짓기위해 CollisonEnter,Exit 메소드 사용.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("map") || collision.transform.name.Contains("stage"))
        {
            Submitenable = true;
            au.Play();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("map") || collision.transform.name.Contains("stage"))
        {
            Submitenable = false;
        }
    }
    //장애물 충돌, 클리어 여부를 판정짓기위해 TriggerEnter 메소드 사용.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "RollingClear")
        {
            FailEvent();
            Retry();
        }
        else
        {
            Re_Pos.RemoveAt(0);
            au.Stop();
            rBody.isKinematic = true;
            rBody.velocity = Vector2.zero;
            ClearEvent();
        }
    }
    //카메라와 배경의 위치가 같게하고, 두 객체와 공 사이의 거리가 매번 일정하게끔 조정.
    private void Update()
    {
        Vector2 pos = camera.position;
        pos.y = transform.position.y - dis;
        camera.position = pos;
        background.position = new Vector3(background.position.x, pos.y, background.position.z);
    }
    //공 위치 재조정
    void Retry()
    {
        rBody.velocity = Vector2.zero;
        rBody.position = Re_Pos[0];
    }
    //버튼 클릭시 메소드
    public Action ButtonClick
    {
        get
        {
            return () => { au.PlayOneShot(jump); rBody.AddForce(Vector2.up * 7000); };
        }
    }
    //버튼 이름
    public string ButtonName
    {
        get
        {
            return "점프!";
        }
    }
    public Action SectorClearAfter
    {
        get
        {
            return () => 
            {
                if(Re_Pos.Count>0)
                {
                    BackgroundMusic.Play();
                    rBody.position = Re_Pos[0];
                    rBody.isKinematic = false;
                }
                else
                {
                    SmartPhone.memory.Rollingball = StageMemory.Status.PerfectClear;
                    SmartPhone.LoadStage("StageSelect");
                }
            };
        }
    }
}
