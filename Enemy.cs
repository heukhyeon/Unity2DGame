using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Enemy:Actor
{
    [SerializeField]
    private float Speed = 3f; //이동속도
    SpriteRenderer spriterender; //캐릭터 방향 획득, 수정
    CharacterController controller;//캐릭터 컨트롤러
    private void Start()
    {
        spriterender = GetComponent<SpriteRenderer>();
        controller = GetComponent<CharacterController>();
        Animator animator = GetComponent<Animator>();
        animator.speed = Speed; //이동속도만큼 애니메이션 재생속도 가속
        Invoke("Dead", 5f);
    }
    private void Update()
    {
        Vector3 pos = Vector3.zero;
        if (spriterender.flipX) pos.x--; //반전되서 바라볼시 x값 감소
        else pos.x++; //제대로 바라볼시 x값 증가
        pos.y = Physics.gravity.y; //벨트스크롤이 아니므로 y값은 그냥 중력값으로.
        controller.Move(pos * Speed* Time.deltaTime);//이동
    }
}
