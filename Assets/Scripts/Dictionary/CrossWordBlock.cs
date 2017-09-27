using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//사전앱에서 입력 블록 한칸마다 삽입되는 클래스
public class CrossWordBlock : MonoBehaviour {
    string Answer;
    InputField field;
    AudioSource au;
    public int X { get { return transform.GetSiblingIndex() % 10; } }
    public int Y { get { return transform.GetSiblingIndex() / 10; } }
    //현재 입력값이 최초 입력된 정답과 같은지를 반환.
    public bool Judge { get { return field.text.Equals(Answer); } }
    public bool isEnable;
    public void Setting(DictionaryScene scene,List<CrossWordBlock>list, DictionaryScene.CrossWordBlockInfo info)
    {
        field = GetComponent<InputField>();
        au = gameObject.AddComponent<AudioSource>();
        Transform num = transform.Find("NumField");
        if (info.index== 0) Destroy(num.gameObject); //숫자가 0임 = 인덱스 표시 블록이 아님
        else
        {
            num.GetComponent<Text>().text = info.index + ")";
            EventTrigger ev= gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry trigger = new EventTrigger.Entry();
            trigger.eventID = EventTriggerType.Select;
            trigger.callback.AddListener((data) => { scene.ShowHint(info.hint); });
            ev.triggers.Add(trigger);
        }
        if (info.answer == '\0') field.interactable = false; //입력된 글자가 없음 = 비활성화 블록임
        else Answer = info.answer.ToString();
        isEnable = field.interactable; //정답체크시 해당 블록을 체크할지 여부
        if (isEnable)
        {
            field.onValueChanged.AddListener((data) => { ValueChange(); });
            EventTrigger.Entry trigger = new EventTrigger.Entry();
            trigger.eventID = EventTriggerType.Select;
            trigger.callback.AddListener((data) => { au.PlayOneShot(info.effect); scene.ShowHint(info.hint); });
            GetComponent<EventTrigger>().triggers.Add(trigger);
        }
        else Destroy(GetComponent<EventTrigger>());
        list.Remove(this);//다시 이 블록을 호출하지않도록 리스트에서 자신을 제거
        gameObject.SetActive(true);//블록을 렌더링함
        au.PlayOneShot(info.effect);
    }
    public void ValueChange()
    {
        if (field == null) field = GetComponent<InputField>();
        if (field.text.Length > 1) field.text = field.text[field.text.Length - 1].ToString();
    }
}
