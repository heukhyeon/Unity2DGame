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
    float dis;
    public event VoidDelegate FailEvent;
    public event VoidDelegate ClearEvent;
    protected override Action BeforeIntro
    {
        get
        {
            return () =>
            {
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("map") || collision.transform.name.Contains("stage")) Submitenable = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("map") || collision.transform.name.Contains("stage")) Submitenable = false;
    }
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
            rBody.isKinematic = true;
            rBody.velocity = Vector2.zero;
            ClearEvent();
        }
    }
    private void Update()
    {
        Vector2 pos = camera.position;
        pos.y = transform.position.y - dis;
        camera.position = pos;
        background.position = new Vector3(background.position.x, pos.y, background.position.z);
    }
    void Retry()
    {
        rBody.velocity = Vector2.zero;
        rBody.position = Re_Pos[0];
    }
    public Action ButtonClick
    {
        get
        {
            return () => { rBody.AddForce(Vector2.up * 7000); };
        }
    }
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
                    rBody.position = Re_Pos[0];
                    rBody.isKinematic = false;
                }
                else
                {
                    //클리어
                }
            };
        }
    }
}
