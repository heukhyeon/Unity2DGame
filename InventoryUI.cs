#pragma warning disable 0649
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : CustomUI
{
    /// <summary>
    /// 인벤토리 UI에서 보이는 배터리 바 테두리
    /// </summary>
    [SerializeField]
    private Transform Batterybar;
    /// <summary>
    /// 아이템 칸이 모인 상자
    /// </summary>
    [SerializeField]
    private Transform Items;
    /// <summary>
    /// 어플 클리어 현황이 모인 상자
    /// </summary>
    [SerializeField]
    private Transform AppStatuss;
    /// <summary>
    /// 아이템 클릭시 상세설명이 보일 텍스트 객체
    /// </summary>
    [SerializeField]
    private Text ItemExplain;
    [SerializeField]
    private GameObject PlayUI;
    //아이템 획득은 어플 내부에서만 발생하므로 Awake시에만 세이브 데이터를 불러온다. 
    protected override void UIAwake()
    {
        SaveData savedata = CustomSceneManager.savedata;
        for (int i = 0; i < savedata.getItem.Length; i++)
            if (savedata.getItem[i] != null)Instantiate(Resources.Load(savedata.getItem[i].path), Items.GetChild(i), false);
        int cnt = 0;
        foreach(KeyValuePair<AppKind, AppStatus> status in savedata.Appstatus)
        {
            Text[] statustext = AppStatuss.GetChild(cnt++).GetComponentsInChildren<Text>();
            switch(status.Key)
            {
                case AppKind.Message:
                    statustext[0].text = "메세지";
                    break;
                case AppKind.Internet:
                    statustext[0].text = "인터넷";
                    break;
                default:
                    statustext[0].text = "미확인";
                    break;
            }
            statustext[1].text = status.Value.ToString();
            switch(status.Value)
            {
                case AppStatus.Clear:
                    statustext[1].color = Color.yellow;
                    break;
                case AppStatus.NotClear:
                    statustext[1].color = Color.white;
                    break;
                case AppStatus.NotOpen:
                    statustext[1].color = Color.red;
                    break;
            }
        }
        RectTransform batteryrect = Batterybar.GetChild(0).GetComponent<RectTransform>();
        Vector2 batteryvalue = batteryrect.sizeDelta;
        batteryvalue.x = batteryvalue.x * savedata.battery / 100f;
        batteryrect.sizeDelta = batteryvalue;
        Batterybar.GetChild(1).GetComponent<Text>().text = savedata.battery + "%";
        this.gameObject.SetActive(false);
    }
    public void UIChange(bool Play)
    {
        PlayUI.SetActive(Play);
        this.gameObject.SetActive(!Play);
    }
}
