using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System.Collections;

public class Inventory:MonoBehaviour
{
    public RectTransform battery;
    public RectTransform Appstatus;
    public RectTransform ItemExplain;
    public RectTransform Items;
    public RectTransform PlayUIbattery;
    public Image SelectItemborder;
    public Item SelectItem;
    private Text[] explains;
    private string nowitemtext;
    private void Start()
    {
        explains = ItemExplain.GetComponentsInChildren<Text>();
        explains[0].text = null;
        explains[1].text = null;
    }
    public void Setting()
    {
        int cnt = 0;
        foreach(KeyValuePair<AppCategory,AppState> pair in StaticManager.SaveData.clearstate)
        {
            Text[] statustext = Appstatus.GetChild(cnt).GetComponentsInChildren<Text>();
            statustext[0].text = GetDescription(pair.Key);
            statustext[1].text = pair.Value.ToString();
            Color textcolor = new Color();
            switch(pair.Value)
            {
                case AppState.Clear:
                    textcolor = Color.green;
                    break;
                case AppState.NotClear:
                    textcolor = Color.magenta;
                    break;
                case AppState.NotEnter:
                    textcolor = Color.gray;
                    break;
                case AppState.PerfectClear:
                    textcolor = Color.cyan;
                    break;
                case AppState.Rejected:
                    textcolor = Color.red;
                    break;
            }
            statustext[1].color = textcolor;
            cnt++;
        }
        (battery.GetChild(0) as RectTransform).sizeDelta = new Vector2(battery.sizeDelta.x / 100 * StaticManager.SaveData.battery, 45);
        battery.GetChild(1).GetComponent<Text>().text = StaticManager.SaveData.battery + "%";
        for(int i=0;i<StaticManager.SaveData.itemlist.Count;i++)
        {
            GameObject origin = Resources.Load(StaticManager.SaveData.itemlist[i]) as GameObject;
            GameObject obj = Instantiate(origin, Items.GetChild(i), false);
            obj.name = origin.name;
            Item item = obj.GetComponent<Item>();
            EventTrigger.Entry itemselectevent = new EventTrigger.Entry();
            itemselectevent.eventID = EventTriggerType.PointerDown;
            itemselectevent.callback.AddListener((eventdata) => { ItemSelect(item); });
            obj.GetComponent<EventTrigger>().triggers.Add(itemselectevent);
        }
    }
    public string GetDescription(AppCategory category)
    {
        FieldInfo fi = category.GetType().GetField(category.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length > 0)
            return attributes[0].Description;
        else
            return category.ToString();
    }
    public void ItemSelect(Item item)
    {
        if(SelectItem!=null && SelectItem.Equals(item))
        {
            StopCoroutine("PrintItemExplain");
            switch(item.type)
            {
                case ItemType.BatteryCharge:
                    int val = int.Parse(Regex.Replace(item.gameObject.name, @"\D", ""));
                    StaticManager.SaveData.battery += val;
                    if (StaticManager.SaveData.battery > 100) StaticManager.SaveData.battery = 100;
                    int value = StaticManager.SaveData.battery;
                    (battery.GetChild(0) as RectTransform).sizeDelta = new Vector2((battery.sizeDelta.x / 100) * value, 45);
                    battery.GetChild(1).GetComponent<Text>().text = value + "%";
                    PlayUIbattery.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = value + "%";
                    (PlayUIbattery.GetChild(1) as RectTransform).sizeDelta = new Vector2((PlayUIbattery.sizeDelta.x / 100) * value, 30);
                    break;
            }
            StaticManager.SaveData.itemlist.Remove(item.gameObject.name);
            SelectItem.border.color = Color.white;
            SelectItem = null;
            explains[0].text = null;
            explains[1].text = null;
            Destroy(item.gameObject);
        }
        else
        {
            if (SelectItem != null) SelectItem.border.color = Color.white;
            SelectItem = item;
            SelectItem.border.color = Color.yellow;
            Text[] explains = ItemExplain.GetComponentsInChildren<Text>();
            explains[0].text = SelectItem.name;
            nowitemtext = SelectItem.content;
            StartCoroutine("PrintItemExplain");
        }
    }
    public void ItemDeselect(Image panel)
    {
        panel.color = Color.white;
    }
    private IEnumerator PrintItemExplain()
    {
        explains[1].text = null;
        for(int i=0;i<nowitemtext.Length;i++)
        {
            explains[1].text += nowitemtext[i];
            yield return new WaitForSeconds(Time.deltaTime);
        }
        StopCoroutine("PrintItemExplain");
    }
}
