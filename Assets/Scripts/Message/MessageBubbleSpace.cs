using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBubbleSpace : MessageScene.Space
{
    public bool enable = false;
    Dictionary<string, bool> speech = new Dictionary<string, bool>(); //말풍선의 방향을 판정하기위한 Dictionary
    AudioSource au;
    public void Init(Message.MessageInfo[] infos)
    {
        au = GetComponent<AudioSource>();
        foreach (var info in infos) speech.Add(info.content, info.speaker);//전달받은 정보에 따라 Dictionary에 항목 추가
    }
    //해당 객체 구역에 진입시
    public void MessageEnter()
    {
        enable = true;
    }
    //해당 객체 구역을 벗어낫을시.
    public void MessageExit()
    {
        enable = false;
    }
    public override void Create(string text)
    {
        bool isHero = speech[text];
        au.PlayOneShot(scene.kwang);
        Vector2 pos = new Vector2(isHero == true ? 95 : -95, items.Count * -260-115);
        RectTransform target = SmartPhone.CreateAndPosition(originItem, space, pos);
        if (!isHero) target.rotation = Quaternion.Euler(0, 180, 0);
        Text speechcom = target.GetComponentInChildren<Text>();
        speechcom.transform.rotation = Quaternion.identity;
        speechcom.text = text;
        target.GetComponentInChildren<Button>().onClick.AddListener(() => { MessageRemove(target); });
        items.Add(target);
        scene.clickenable = true;
        space.sizeDelta = new Vector2(space.sizeDelta.x, items.Count * 300);
    }
    //말풍선을 클릭한경우, 말풍선을 블록으로 변환.
    void MessageRemove(RectTransform target)
    {
        string text = target.GetComponentInChildren<Text>().text;
        items.Remove(target);
        DestroyImmediate(target.gameObject);
        scene.blockspace.Create(text);
        space.sizeDelta = new Vector2(space.sizeDelta.x, items.Count * 300);
        ItemSort();
    }
    //말풍선 제거에 따른 말풍선 재정렬
    public override void ItemSort()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Vector2 pos = items[i].localPosition;
            pos.y = i * -260 - 115;
            items[i].localPosition = pos;
        }
    }
    //Answer에서 현재 메세지 목록을 요구하므로 현재 말풍선 내용들을 반환.
    public Text[] GetMessage()
    {
        return space.GetComponentsInChildren<Text>();
    }
}