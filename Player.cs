using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Actor
{
    private Pad pad;
    public bool Jump
    {
        get { return Ani.GetBool("Jumping"); }
        set
        {
            if (Bottom && value == true)
            {
                Rbody.AddForce(Vector2.up * status.defaultjumping, ForceMode2D.Impulse);
                Ani.SetBool("Jumping", true);
            }
            else if (value == false) Ani.SetBool("Jumping", false);
        }
    }
    private bool Dash
    {
        get { return Ani.GetBool("Dashing"); }
        set { Ani.SetBool("Dashing", value); }
    }
    protected override void ActorAwake()
    {
        if (this.gameObject.name != "Player") Debug.LogError("Player 클래스를 가진 객체의 이름은 Player여야만 합니다! :" + this.gameObject.name);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bottom")
        {
            Bottom = true;
            if (Jump) Jump = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bottom") Bottom = false;
    }
    private void Start()
    {
        pad = SceneObjectManager.GetComponent<Pad>();
    }
    private void FixedUpdate()
    {
        Moving = pad.Moving == true ? true : false;
        Flip = pad.Stickpos.x >= 0 ? true : false;
        if((!Flip && Rbody.velocity.x<1f) || (Flip && Rbody.velocity.x>1f))
        {
            Vector2 spd = Rbody.velocity;
            spd.x = 0f;
            Rbody.velocity = spd;
        }
        if (Moving) Move();
        if (pad.Stickpos.y == 1) Warp();
    }
    protected override void Move()
    {
        Vector2 pos = Vector2.zero;
        pos.x = (Flip == true ? -1f : 1f) * status.defaultspeed * Time.fixedDeltaTime;
        Rbody.position += pos;
    }

    /*public void Dashing()
    {
        if (!Bottom) return;
        if (Rbody.velocity.x > 0)
        {
            Vector2 spd = Rbody.velocity;
            spd.x = 0;
            Rbody.velocity = spd;
        }
        Vector2 pos = Vector2.zero;
        pos.x += (Flip == true ? -1f : 1f) * status.Dashspeed;
        Rbody.AddForce(pos, ForceMode2D.Impulse);
        Debug.Log(Rbody.velocity);
        Ani.SetBool("Dashing", true);
     }*/
     /// <summary>
     /// 위쪽 키가 입력되었을때의 처리
     /// </summary>
    private void Warp()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit)&& hit.transform.tag == "App")
        {
           SceneObjectManager.Playerlocation = this.transform.position;
           SceneObjectManager.NextScene = hit.transform.name;
           SceneManager.LoadSceneAsync("Loading");
        }
    }
    
}
