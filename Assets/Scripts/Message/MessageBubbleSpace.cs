using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBubbleSpace : MessageScene.Space
{
    public bool enable = false;
    Dictionary<string, bool> speech = new Dictionary<string, bool>();
    public void Init(Message message)
    {
        foreach (var info in message.items) speech.Add(info.content, info.speaker);
    }
    public void MessageEnter()
    {
        enable = true;
    }
    public void MessageExit()
    {
        enable = false;
    }
    public override void Create(string text)
    {
        bool isHero = speech[text];
        Vector2 pos = new Vector2(isHero == true ? 95 : -95, items.Count * -260);
        RectTransform target = SmartPhone.CreateAndPosition(originItem, space, pos);
        if (!isHero) target.rotation = Quaternion.Euler(0, 180, 0);
        Text speechcom = target.GetComponentInChildren<Text>();
        speechcom.transform.rotation = Quaternion.identity;
        speechcom.text = text;
        target.GetComponent<Button>().onClick.AddListener(() => { MessageRemove(target); });
        items.Add(target);
        scene.clickenable = true;
        space.sizeDelta = new Vector2(space.sizeDelta.x, items.Count * 300);
    }
    void MessageRemove(RectTransform target)
    {
        string text = target.GetComponentInChildren<Text>().text;
        items.Remove(target);
        DestroyImmediate(target.gameObject);
        scene.blockspace.Create(text);
        space.sizeDelta = new Vector2(space.sizeDelta.x, items.Count * 300);
        ItemSort();
    }
    public override void ItemSort()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Vector2 pos = items[i].localPosition;
            pos.y = i * -260;
            items[i].localPosition = pos;
        }
    }
    public Text[] GetMessage()
    {
        return space.GetComponentsInChildren<Text>();
    }
}