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
        Resize(stick);
        radius = recttransform.sizeDelta.x * 0.5f;
        stickcenter = this.transform.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Stickpos = (eventData.position - stickcenter).normalized;
        stick.localPosition = Stickpos * radius;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Moving = true;
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Moving = false;
        stick.localPosition = Vector2.zero;
    }
}
