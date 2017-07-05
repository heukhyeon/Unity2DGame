using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 메세지 어플리케이션에서 화면 하단에 나열된 문자열 블록
/// </summary>
public class MessageBlock : CustomUI,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    [SerializeField]
    private MessageBox messagebox = null; //드랍시 블록이 생성될 메세지 박스
    private Vector3 defaultpos;
    private Text messageword;
    public void OnPointerDown(PointerEventData eventData)
    {
        messagebox.nowword = messageword.text;
        OnDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        this.transform.localPosition= defaultpos;
    }
    protected override void UIAwake()
    {
        defaultpos = this.transform.localPosition;
        messageword = this.GetComponentInChildren<Text>();
    }
}
