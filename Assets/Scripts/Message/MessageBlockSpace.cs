using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//메세지 블록이 모여있는 공간.
public class MessageBlockSpace : MessageScene.Space
{
    RectTransform selectBlock;
    Vector2 backUpPos;
    //블록 생성
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
        scene.Submitenable = false;
    }
    //블록 제거에 따른 블록 재정렬
    public override void ItemSort()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].transform.localPosition = new Vector2((i % 3) * 300, -(i / 3) * 300) + new Vector2(-330, -200);
    }
    //블록 클릭시 호출
    void BlockClick(RectTransform block)
    {
        selectBlock = block;
        backUpPos = selectBlock.position;
        selectBlock.SetParent(scene.transform);
        selectBlock.SetSiblingIndex(1);
    }
    //블록을 움직일시 호출
    void BlockMove(PointerEventData data)
    {
        selectBlock.position = data.position;
    }
    //블록을 놓았을때, BubbleSpace 내부인경우 말풍선을 만들고, 아닌경우 선택되었던 블록을 원래 위치로 돌린다.
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
            if (items.Count == 0) scene.Submitenable = true;
        }
        else
        {
            selectBlock.SetParent(space);
            selectBlock.position = backUpPos;
            selectBlock = null;
        }
    }
}
