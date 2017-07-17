using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class InternetScene:MonoBehaviour,AppSceneScript
{
    private int cnt = 3;
    public RectTransform InfoSpace;
    public GameObject RelationWordblock;
    public GameObject Infoblock;
    public GameObject encyclopediablock;
    public GameObject siteblock;
    public GameObject knowinblock;
    public SpeechEvent speechevent;
    public Sprite testimage;
    public Text Keyword;
    public Transform Lifebar;
    public GameObject Life;
    private Vector2 PlaceLocation;
    private InternetAppInfo info;
    private void Start()
    {
        info = StaticManager.Nowappinfo.internetinfo;
        if (StaticManager.SaveData.clearstate[AppCategory.Internet] == AppState.NotEnter)
        {
            StaticManager.SaveData.clearstate[AppCategory.Internet] = AppState.NotClear;
            speechevent.SpeechStart(StaticManager.Nowappinfo.speecharray.introspeech, this.gameObject, EventState.Intro);
        }
        //test();
        PlaceLocation = Vector2.zero;
        PlaceLocation.y = 10;
        CreateRealationBlock();
        CreateSiteBlock();
        CreateKnowInBlock();
        CreateEncyclopediaBlock();
        for (int i = 0; i < 3; i++)
        {
            Transform heart = Instantiate(Life, Lifebar, false).transform;
            heart.localPosition = new Vector2(i * 80 + 50, 0);
        }
        InfoSpace.sizeDelta = new Vector2(InfoSpace.sizeDelta.x, PlaceLocation.y + 100f);
    }
    /*private void test()
    {
        info.encyclopediablock = new InternetAppInfo.EncyclopediaBlock[1];
        info.encyclopediablock[0].name = "사전 테스트1";
        info.encyclopediablock[0].content = "사전 테스트테스테테스트데트스스트트트데트스트";
        info.relationwords = new string[10];
        for (int i = 0; i < 10; i++) info.relationwords[i] = "검색어 테스트" + i;
        info.knowinblock = new InternetAppInfo.KnowInBlock[2];
        for(int i=0;i<2;i++)
        {
            info.knowinblock[i].ImageEnable = i == 0 ? true : false;
            info.knowinblock[i].image = testimage;
            info.knowinblock[i].questiontitle = "질문 제목" + i;
            info.knowinblock[i].questioncontent = "질문 내용이다이이아아아앙다다다다";
            info.knowinblock[i].answercontent = "답변 내용이다아아아앙아아아아";
        }
        info.siteblock = new InternetAppInfo.SiteBlock[3];
        for(int i=0;i<3;i++)
        {
            info.siteblock[i].name = "사이트" + i;
            info.siteblock[i].url = "wwwwwwwwwwwwwwwwwwwwwwwwwwwww";
            info.siteblock[i].content = "테스트" + i;
        }
    }*/
#region 블록 생성
    private void CreateRealationBlock()
    {
        Transform block = Instantiate(RelationWordblock, InfoSpace, false).transform;
        block.localPosition = new Vector2(0, -PlaceLocation.y);
        StringBuilder sb = new StringBuilder();
        foreach (string word in info.relationwords)
            sb.AppendFormat("{0} , ", word);
        block.GetComponentsInChildren<Text>()[1].text = sb.ToString();
        PlaceLocation.y += 120f;
    }
    private Transform CreateInfoBlock(string name,string content)
    {
        GameObject obj = Instantiate(Infoblock, InfoSpace, false);
        obj.name = name;
        obj.transform.localPosition = new Vector2(-20, -PlaceLocation.y);
        obj.GetComponentInChildren<Text>().text = content;
        PlaceLocation.y += 55f;
        return obj.transform;
    }
    private void CreateEncyclopediaBlock()
    {
        if (info.encyclopediablock.Length == 0) return;
        float height = encyclopediablock.GetComponent<RectTransform>().sizeDelta.y;
        Transform parentblock = CreateInfoBlock("EncyclopediaBlock","지식백과");
        for (int i=0;i<info.encyclopediablock.Length;i++)
        {
            Transform encyclopedia = Instantiate(encyclopediablock, parentblock, false).transform;
            encyclopedia.name = "Encyclopedia" + i;
            encyclopedia.localPosition = new Vector2(0, -(55 + height * i));
            Text[] blocktexts = encyclopedia.GetComponentsInChildren<Text>();
            blocktexts[0].text = info.encyclopediablock[i].name;
            blocktexts[1].text = info.encyclopediablock[i].content;
            PlaceLocation.y += height;
        }
        PlaceLocation.y += 10f;
    }
    private void CreateSiteBlock()
    {
        if (info.siteblock.Length == 0) return;
        float height = siteblock.GetComponent<RectTransform>().sizeDelta.y;
        Transform parentblock = CreateInfoBlock("SiteBlock","사이트");
        for (int i=0;i<info.siteblock.Length;i++)
        {
            Transform site = Instantiate(siteblock, parentblock, false).transform;
            site.name = "Site" + i;
            site.localPosition = new Vector2(0, -(55 + height * i));
            Text[] blocktexts = site.GetComponentsInChildren<Text>();
            blocktexts[0].text = info.siteblock[i].name;
            blocktexts[1].text = info.siteblock[i].url;
            blocktexts[2].text = info.siteblock[i].content;
            PlaceLocation.y += height;
        }
        PlaceLocation.y += 10f;
    }
    private void CreateKnowInBlock()
    {
        if (info.knowinblock.Length == 0) return;
        float height=knowinblock.GetComponent<RectTransform>().sizeDelta.y;
        Transform parentblock = CreateInfoBlock("KnowInBlock","지식IN");
        for (int i=0;i<info.knowinblock.Length;i++)
        {
            RectTransform knowin = Instantiate(knowinblock, parentblock, false).transform as RectTransform;
            knowin.name = "KnowIn" + i;
            if (info.knowinblock[i].ImageEnable == false)
            {
                Destroy(knowin.GetChild(0).gameObject);
                knowin.sizeDelta = new Vector2(650, 200);
                knowin.localPosition = new Vector2(0, -(55 + 200 * i));
            }
            else
            {
                knowin.GetComponentsInChildren<Image>()[1].sprite = info.knowinblock[i].image;
                knowin.localPosition = new Vector2(60, -(55 + 200 * i));
            }
            if(info.knowinblock[i].AnswerEnable)
            {
                Text[] blocktexts = knowin.GetComponentsInChildren<Text>();
                blocktexts[1].text = info.knowinblock[i].questiontitle;
                blocktexts[2].text = info.knowinblock[i].questioncontent;
                blocktexts[4].text = info.knowinblock[i].answercontent;
                PlaceLocation.y += 200f;
            }
            else
            {
                Text[] blocktexts = knowin.GetComponentsInChildren<Text>();
                blocktexts[1].text = info.knowinblock[i].questiontitle;
                blocktexts[2].text = info.knowinblock[i].questioncontent;
                Destroy(blocktexts[3].gameObject);
                Destroy(blocktexts[4].gameObject);
                knowin.sizeDelta = new Vector2(knowin.sizeDelta.x, 120);
                PlaceLocation.y += 120f;
            }
        }
        PlaceLocation.y += 10f;
    }
    #endregion
    public void KeywordSend()
    {
        bool isAnswer = false;
        foreach (string word in StaticManager.Nowappinfo.internetinfo.answer)
            if (Keyword.text.Equals(word))
            {
                isAnswer = true;
                break;
            }
        if(isAnswer)
            speechevent.SpeechStart(StaticManager.Nowappinfo.speecharray.clearspeech, this.gameObject, EventState.Clear);
        else
        {
            Destroy(Lifebar.GetChild(cnt-- - 1).gameObject);
            if (cnt == 0) speechevent.SpeechStart(StaticManager.Nowappinfo.speecharray.gameoverspeech, this.gameObject, EventState.GameOver);
            else speechevent.SpeechStart(StaticManager.Nowappinfo.speecharray.failspeech, this.gameObject, EventState.Fail);
        }
    }

    public void Clear()
    {
        StaticManager.SaveData.clearstate[AppCategory.Internet] = AppState.Clear;
        StaticManager.SaveData.battery -= 2;
        StaticManager.moveScenetoLoading("MainStage");
    }

    public void GameOver()
    {
        StaticManager.SaveData.battery -= 5;
        StaticManager.moveScenetoLoading("MainStage");
    }
}
