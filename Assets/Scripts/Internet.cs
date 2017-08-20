using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class Internet : MonoBehaviour,ISpeech
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
        info = (InternetAppInfo)GameManager.appinfo;
        PlaceLocation = Vector2.zero;
        PlaceLocation.y = 10;
        CreateRealationBlock();
        CreateSiteBlock();
        CreateKnowInBlock();
        CreateEncyclopediaBlock();
        InfoSpace.sizeDelta = new Vector2(InfoSpace.sizeDelta.x, PlaceLocation.y + 100f);
    }
    #region 블록 생성
    private void CreateRealationBlock()
    {
        Transform block = Instantiate(RelationWordblock, InfoSpace, false).transform;
        block.localPosition = new Vector2(0, -PlaceLocation.y);
        StringBuilder sb = new StringBuilder();
        foreach (string word in info.relations)
            sb.AppendFormat("{0} , ", word);
        block.GetComponentsInChildren<Text>()[1].text = sb.ToString();
        PlaceLocation.y += 120f;
    }
    private Transform CreateInfoBlock(string name, string content)
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
        if (info.encyclopedias.Length == 0) return;
        float height = encyclopediablock.GetComponent<RectTransform>().sizeDelta.y;
        Transform parentblock = CreateInfoBlock("EncyclopediaBlock", "지식백과");
        for (int i = 0; i < info.encyclopedias.Length; i++)
        {
            Transform encyclopedia = Instantiate(encyclopediablock, parentblock, false).transform;
            encyclopedia.name = "Encyclopedia" + i;
            encyclopedia.localPosition = new Vector2(0, -(55 + height * i));
            Text[] blocktexts = encyclopedia.GetComponentsInChildren<Text>();
            blocktexts[0].text = info.encyclopedias[i].name;
            blocktexts[1].text = info.encyclopedias[i].content;
            PlaceLocation.y += height;
        }
        PlaceLocation.y += 10f;
    }
    private void CreateSiteBlock()
    {
        if (info.sites.Length == 0) return;
        float height = siteblock.GetComponent<RectTransform>().sizeDelta.y;
        Transform parentblock = CreateInfoBlock("SiteBlock", "사이트");
        for (int i = 0; i < info.sites.Length; i++)
        {
            Transform site = Instantiate(siteblock, parentblock, false).transform;
            site.name = "Site" + i;
            site.localPosition = new Vector2(0, -(55 + height * i));
            Text[] blocktexts = site.GetComponentsInChildren<Text>();
            blocktexts[0].text = info.sites[i].name;
            blocktexts[1].text = info.sites[i].url;
            blocktexts[2].text = info.sites[i].content;
            PlaceLocation.y += height;
        }
        PlaceLocation.y += 10f;
    }
    private void CreateKnowInBlock()
    {
        if (info.knowins.Length == 0) return;
        float height = knowinblock.GetComponent<RectTransform>().sizeDelta.y;
        Transform parentblock = CreateInfoBlock("KnowInBlock", "지식IN");
        for (int i = 0; i < info.knowins.Length; i++)
        {
            RectTransform knowin = Instantiate(knowinblock, parentblock, false).transform as RectTransform;
            knowin.name = "KnowIn" + i;
            if (info.knowins[i].image_enable == false)
            {
                Destroy(knowin.GetChild(0).gameObject);
                knowin.sizeDelta = new Vector2(650, 200);
                knowin.localPosition = new Vector2(0, -(55 + 200 * i));
            }
            else
            {
                knowin.GetComponentsInChildren<Image>()[1].sprite = info.knowins[i].image;
                knowin.localPosition = new Vector2(60, -(55 + 200 * i));
            }
            if (info.knowins[i].answer_enable)
            {
                Text[] blocktexts = knowin.GetComponentsInChildren<Text>();
                blocktexts[1].text = info.knowins[i].question_title;
                blocktexts[2].text = info.knowins[i].question_content;
                blocktexts[4].text = info.knowins[i].answer_content;
                PlaceLocation.y += 200f;
            }
            else
            {
                Text[] blocktexts = knowin.GetComponentsInChildren<Text>();
                blocktexts[1].text = info.knowins[i].question_title;
                blocktexts[2].text = info.knowins[i].question_content;
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
        foreach (string word in info.answers)
            if (Keyword.text.Equals(word))
            {
                isAnswer = true;
                break;
            }
        if (isAnswer)
            speechevent.SpeechStart(GameManager.speech.clear, GetComponent<ISpeech>());
        else
        {
            Destroy(Lifebar.GetChild(cnt-- - 1).gameObject);
            if (cnt == 0) speechevent.SpeechStart(GameManager.speech.gameover, GetComponent<ISpeech>());
            else speechevent.SpeechStart(GameManager.speech.fail, GetComponent<ISpeech>());
        }
    }
    public void SpeechComplete()
    {
        
    }
}

