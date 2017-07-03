using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 상단 배터리 객체에 대한 스크립트
/// </summary>
public class Battery : CustomUI {
    [SerializeField]
    private float defaultvalue = 1f; //기본감소량
    private float life = 100f; //현재 배터리양
    private float lifegaugewidth = 0f; //배터리 내부에 채워져있는 객체의 처음 길이
    private RectTransform lifegauge; // 배터리 내부에 채워져있는 객체
    private Text batterytext; //배터리 우측에 출력될 글자
    protected override void UIAwake()
    {
        batterytext = GetComponentInChildren<Text>();
        lifegauge = this.transform.GetChild(1).GetComponent<RectTransform>();
        lifegaugewidth = lifegauge.sizeDelta.x;
    }
    private void Update()
    {
        Vector2 size = lifegauge.sizeDelta;
        size.x = lifegaugewidth * life / 100f;
        lifegauge.sizeDelta = size;
        batterytext.text = life + "%";
    }
    /// <summary>
    /// 기본 감소
    /// </summary>
    public void Decrease()
    {
        life -= defaultvalue;
    }
    /// <summary>
    /// 감소량을 직접 지정한경우
    /// </summary>
    /// <param name="value"></param>
    public void Decrease(float value)
    {
        life -= value;
    }
}
