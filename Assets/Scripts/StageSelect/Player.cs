using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    bool isBottom = false;
    bool isSelect = false;
    public float x_dir;
    public float y_dir;
    public Sprite bottomstop;
    public Sprite RopeStop;
    Animator ani;
    SpriteRenderer render;
    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bottom") isBottom = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Bottom") isBottom = false;
    }
    private void Update()
    {
        if (x_dir == 0 && y_dir == 0)
        {
            ani.enabled = false;
            if (isBottom) render.sprite = bottomstop;
            else render.sprite = RopeStop;
        }
        else
        {
            ani.enabled = true;
            if (x_dir != 0 && isBottom)
            {
                if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Run")) ani.CrossFade("Run", 0);
                Move();
            }
            else if (y_dir != 0) Cmd_Vertical();
        }


    }
    public void Move()
    {
        render.flipX = x_dir == -1 ? true : false;
        Vector2 pos = transform.position;
        pos.x += x_dir * Time.deltaTime;
        transform.position = pos;
    }
    public void Cmd_Vertical()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit))
        {
            switch(hit.transform.tag)
            {
                case "Rope":
                    Vector2 pos = transform.position;
                    pos.x = hit.transform.position.x;
                    pos.y += y_dir*Time.deltaTime;
                    hit = new RaycastHit();
                    if (Physics.Raycast(pos, transform.forward, out hit) && hit.transform.tag.Equals("Rope"))
                    {
                        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Climb")) ani.CrossFade("Climb", 0);
                        transform.position = pos;
                    }
                    else
                    {
                        ani.enabled = false;
                        if (isBottom) render.sprite = bottomstop;
                        else render.sprite = RopeStop;
                    }
                    break;
                case "App":
                    if(y_dir==1&&!isSelect)
                    {
                        x_dir = 0;
                        y_dir = 0;
                        isSelect = true;
                        hit.transform.GetComponent<AppIcon>().Select();
                    }
                    break;
            }
        }
    }
}
