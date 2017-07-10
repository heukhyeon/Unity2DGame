using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Actor
{
    [SerializeField]
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
    private bool isWarp = false; //앱 선택이 중복되서 발생하는것방지
    protected override void ActorAwake()
    {
        if (this.gameObject.name != "Player") Debug.LogError("Player 클래스를 가진 객체의 이름은 Player여야만 합니다! :" + this.gameObject.name);
    }
    private void Start()
    {
        if (pad == null) Debug.LogError("Pad 객체를 삽입해주세요! Player 스크립트 에러");
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
    /// <summary>
    /// 좌우 키가 입력되었을때의 처리
    /// </summary>
    protected override void Move()
    {
        Vector2 pos = Vector2.zero;
        pos.x = (Flip == true ? -1f : 1f) * status.defaultspeed * Time.fixedDeltaTime;
        Rbody.position += pos;
    }
     /// <summary>
     /// 위쪽 키가 입력되었을때의 처리
     /// </summary>
    private void Warp()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit)&& hit.transform.tag == "App" && !isWarp)
        {
            isWarp = true;
            Apps app = hit.transform.GetComponent<Apps>();
            switch(app.kind)
            {
                case AppKind.Message:
                    app.message.goApp();
                    break;
                case AppKind.Internet:
                    app.internet.goApp();
                    break;
                case AppKind.Null:
                    Debug.LogError("앱 종류가 없습니다! :" + app.gameObject.name);
                    break;
            }
            CustomSceneManager.goScene(hit.transform, this.transform.position);
        }
    }
    
}
