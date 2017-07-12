using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 인터넷 어플에서 사용될 클래스. Helps_InternetBox에 삽입된다.
/// </summary>
public class InternetBox : CustomUI,HeartbarController {
    //~Block은 프리팹에서, 그렇지 않은건 현재 씬에 있는 오브젝트를 집어넣는다.
    /// <summary>
    /// 입력창에 입력된 Text 게임오브젝트
    /// </summary>
    [SerializeField]
    private Text input = null; //유저 입력창 글자
    [SerializeField]
    private Text relation = null; //연관 검색어
    [SerializeField]
    private GameObject KnowIn = null;
    [SerializeField]
    private GameObject KnowInBlock = null;
    [SerializeField]
    private GameObject Dictionary = null;
    [SerializeField]
    private GameObject DictionaryBlock = null;
    [SerializeField]
    private GameObject Site = null;
    [SerializeField]
    private GameObject SiteBlock = null;
    [SerializeField]
    private GameObject RefferenceImages = null;
    [SerializeField]
    private GameObject RefferenceImage = null;
    private Vector2 pos = Vector2.zero;

    public void HeartCompareComplete()
    {
        
    }

    public void MissionFail()
    {
        
    }
    private void Start()
    {
        // 연관 검색어 -> 사이트 -> 사전-> 이미지 -> 지식인 순으로 처리.
        // 모든 컴포넌트는 표시할 필요가 없는경우 (연관검색어의 경우 연관검색어가 없다던지) 해당 객체를 지운다.
        //연관검색어 처리
        InternetAppInfo internet = CustomSceneManager.apps.internet;
        Text[] objtexts;
        if (internet.relations.Length == 0) Destroy(relation.transform.parent.gameObject);
        else
        {
            StringBuilder sb = new StringBuilder();
            foreach (string word in internet.relations)
                sb.AppendFormat("{0} ", word);
            relation.text = sb.ToString();
            pos.y = 110f;
        }
        //사이트 처리
        if (internet.sites.Length == 0) Destroy(Site);
        else
        {
            setPos(Site);
            for (int i = 0; i < internet.sites.Length; i++)
            {
                GameObject obj = Instantiate(SiteBlock, Site.transform, false);
                obj.name = "Site" + (i + 1);
                obj.transform.localPosition = new Vector2(0, -(i * 130 + 60));
                objtexts = obj.GetComponentsInChildren<Text>();
                objtexts[0].text = internet.sites[i].name;
                objtexts[1].text = internet.sites[i].content;
                objtexts[2].text = internet.sites[i].url;
                pos.y += 130;
            }
        }
        //사전 처리
        if (internet.dictionarys.Length == 0) Destroy(Dictionary);
        else
        {
            setPos(Dictionary);
            Debug.Log(Dictionary.GetComponent<RectTransform>().localPosition);
            for (int i = 0; i < internet.dictionarys.Length; i++)
            {
                GameObject obj = Instantiate(DictionaryBlock, Dictionary.transform, false);
                obj.name = "Dictionary" + (i + 1);
                obj.transform.localPosition = new Vector2(0, -(i * 130 + 60));
                Debug.Log(obj.transform.localPosition);
                objtexts = obj.GetComponentsInChildren<Text>();
                objtexts[0].text = internet.dictionarys[i].name;
                objtexts[1].text = internet.dictionarys[i].content;
                pos.y += 130;
            }
        }
        //이미지 처리
        if (internet.images.Length == 0) Destroy(RefferenceImages);
        else
        {
            setPos(RefferenceImages);
            for (int i = 0; i < internet.images.Length; i++)
            {
                GameObject obj = Instantiate(RefferenceImage, RefferenceImages.transform, false);
                obj.name = "RefferenceImage" + (i + 1);
                obj.transform.localPosition = new Vector2(i % 3 * 235f - 350f, -(30 + i/3 * 235f));
                obj.GetComponent<Image>().overrideSprite = internet.images[i];
                pos.y = i % 3 == 0 ? pos.y + 220f : pos.y;
            }
        }
        //지식인 처리
        if (internet.knowins.Length == 0) Destroy(KnowIn);
        else
        {
            setPos(KnowIn);
            for (int i = 0; i < internet.knowins.Length; i++)
            {
                GameObject obj = Instantiate(KnowInBlock, KnowIn.transform, false);
                obj.name = "KnowIn" + (i + 1);
                obj.transform.localPosition = new Vector2(0, -(60 + i * 190f));
                objtexts = obj.transform.GetChild(1).GetComponentsInChildren<Text>();
                objtexts[1].text = internet.knowins[i].QuestionerTitle;
                objtexts[2].text = internet.knowins[i].QuestionerContent;
                obj.transform.GetChild(2).GetComponentsInChildren<Text>()[1].text = internet.knowins[i].AnswerContent;
                if (internet.knowins[i].EnableImage)//현재 지식인 항목이 이미지를 쓰는경우
                {
                    obj.GetComponentsInChildren<Image>()[1].sprite = internet.knowins[i].image;
                }
                else //현재 지식인 항목이 이미지를 쓰지 않는경우
                {
                    Vector2 size = new Vector2(700, 85);
                    obj.transform.GetChild(1).localPosition = Vector2.zero;
                    Debug.Log(obj.transform.GetChild(1).name);
                    obj.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = size;
                    size.y += 15;
                    obj.transform.GetChild(2).localPosition = new Vector2(0, -85f);
                    obj.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = size;
                    Destroy(obj.transform.GetChild(0).gameObject); //이미지 제거
                }
                pos.y += 190;
            }
        }
        if(pos.y<1060) this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(720, 1060);
        else this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(720, pos.y + 100);
    }
    protected override void UIAwake()
    {
    }
    public void KewordSend()
    {

    }
    private void setPos(GameObject obj)
    {
        obj.GetComponent<RectTransform>().localPosition = new Vector2(0, -pos.y);
        pos.y += 60f;
    }
}


