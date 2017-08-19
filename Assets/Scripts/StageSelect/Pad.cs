#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pad : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler {

    [SerializeField]
    RectTransform stick;
    [SerializeField]
    Player player;
    Vector2 center;
    float radius;
    private void Start()
    {
        center = transform.position;
        radius = GetComponent<RectTransform>().sizeDelta.x * 0.5f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position - center;
        float abs_x = Mathf.Abs(pos.x);
        float abs_y = Mathf.Abs(pos.y);
        if(abs_x>abs_y)
        {
            pos.x = pos.x / abs_x;
            pos.y = 0;
        }
        else
        {
            pos.y = pos.y / abs_y;
            pos.x = 0;
        }
        stick.localPosition = pos * radius;
        player.x_dir = pos.x;
        player.y_dir = pos.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stick.localPosition = Vector3.zero;
        player.x_dir = 0;
        player.y_dir = 0;
    }
}
