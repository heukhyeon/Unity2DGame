using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageBlockSpace : MessageScene.Space
{
    RectTransform selectBlock;
    Vector2 backUpPos;
    public override void Create(string text)
    {
        Vector2 pos = new Vector2((items.Count % 3) * 300, -(items.Count / 3) * 300) + new Vector2(-330, -200);
        RectTransform target = SmartPhone.CreateAndPosition(originItem, space, pos);
        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerDown;
        click.callback.AddListener((data) => { if (scene.clickenable) BlockClick(target); });
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { if (scene.clickenable) BlockMove(data as PointerEventData); });
        EventTrigger.Entry drop = new EventTrigger.Entry();
        drop.eventID = EventTriggerType.PointerUp;
        drop.callback.AddListener((data) => { BlockDrop(); });
        target.GetComponent<EventTrigger>().triggers.AddRange(new EventTrigger.Entry[] { click, drag, drop });
        target.GetComponentInChildren<Text>().text = text;
        target.sizeDelta = new Vector2(250, 250);
        items.Add(target);
        space.sizeDelta = new Vector2(1000, (items.Count / 3) * 600);
    }
    public override void ItemSort()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].transform.localPosition = new Vector2((i % 3) * 300, -(i / 3) * 300) + new Vector2(-330, -200);
    }
    void BlockClick(RectTransform block)
    {
        selectBlock = block;
        backUpPos = selectBlock.position;
        selectBlock.SetParent(scene.transform);
        selectBlock.SetSiblingIndex(1);
    }
    void BlockMove(PointerEventData data)
    {
        selectBlock.position = data.position;
    }
    void BlockDrop()
    {
        if (scene.bubblespace.enable)
        {
            scene.clickenable = false;
            string text = selectBlock.GetComponentInChildren<Text>().text;
            scene.bubblespace.Create(text);
            items.Remove(selectBlock);
            DestroyImmediate(selectBlock.gameObject);
            selectBlock = null;
            space.sizeDelta = new Vector2(1000, (items.Count / 3) * 600);
            ItemSort();
        }
        else
        {
            selectBlock.SetParent(space);
            selectBlock.position = backUpPos;
            selectBlock = null;
        }
    }
}
