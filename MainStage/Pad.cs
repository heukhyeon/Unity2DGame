using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플레이어 조이스틱. 이동 이외의 점프 버튼등은 클래스없이 click이벤트만을 끌어오거나 별도 클래스를 작성한다.
/// </summary>
public class Pad : CustomUI,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    private RectTransform stick;
    private float radius;
    private Vector2 stickcenter;
    public bool Moving = false;
    public Vector2 Stickpos = Vector2.zero;
    protected override void UIAwake()
    {
        stick = this.transform.GetChild(0).GetComponent<RectTransform>();
    }
    private void Start()
    {
        radius = recttransform.sizeDelta.x * 0.5f;
        stickcenter = recttransform.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Stickpos = (eventData.position - stickcenter).normalized;
        if (Mathf.Abs(Stickpos.x) < Mathf.Abs(Stickpos.y)) //y절댓값이 x절댓값보다 큰 경우
        {
            Moving = false; //상하 방향키이므로 움직이지 않게 한다.
            Stickpos.x = 0; //작은쪽의 값을 0으로 한다.
            if (Stickpos.y < 0) Stickpos.y = -1; //큰쪽의 절댓값을 1로 한다.
            else if(Stickpos.y>0) Stickpos.y = 1;
        }
        else//x절댓값이 y좌표값보다 작거나 같은경우
        {
            Moving = true;//좌우 방향키이므로 움직이게 한다.
            Stickpos.y = 0; //작은쪽의 값을 0으로 한다.
            if (Stickpos.x < 0) Stickpos.x = -1;//큰쪽의 절댓값을 1로 한다.
            else if(Stickpos.x>0) Stickpos.x = 1;
        }
        stick.localPosition = Stickpos * radius;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Moving = false;
        Stickpos = Vector2.zero;
        stick.localPosition = Vector2.zero;
    }
}
