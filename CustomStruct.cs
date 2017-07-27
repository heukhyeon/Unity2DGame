using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
//앱 스테이지 등에 탑재되는 구조체, enum, 인터페이스 들을 모아놓은 스크립트
#region 열거형 모음
public enum AppCategory
{
    Null,
    [Description("메세지")]
    Message,
    [Description("인터넷")]
    Internet,
    [Description("갤러리")]
    Gallery,
    [Description("계산기")]
    Calculator,
    [Description("카메라")]
    Camera,
    [Description("공튀기기")]
    BallMove,
    [Description("톡")]
    Talk,
    [Description("사전")]
    Dictionary,
    [Description("유튜브")]
    Youtube,
    [Description("알림")]
    Notice,
    [Description("음악")]
    Music,
    [Description("휴지통")]
    Garbage
}
public enum AppState
{
    Rejected,
    NotEnter,
    NotClear,
    Clear,
    PerfectClear
}
public enum SpeechState
{
    Angry,
    Fear,
    Just,
    Sorrow,
    Surprise,
}
public enum EventState
{
    Intro,
    Fail,
    GameOver,
    Clear
}
public enum Speaker
{
    Hero,
    Others
}
public enum ItemType
{
    BatteryCharge,
    Other,
}
#endregion

public interface AppSceneScript// ~Scene 스크립트중 App스테이지들에 대한 공통 탑재 스크립트. 탑재 이유는 SpeechEvent에서 클리어와 게임오버시 대사 출력후 행동을 취하게 하기 위함.
{
    void Clear(); //클리어시
    void GameOver(); //게임 오버시
}

[Serializable]
public struct SaveData //세이브 데이터
{
    public Dictionary<AppCategory, AppState> clearstate; //클리어 상태. 해당 Dictionary의 각 앱 별 상태에따라 메인 스테이지 진입 스크립트가 달라짐.
    public List<string> itemlist;
    public int battery;
    public bool Prologueclear;
    public SaveData(bool intro)
    {
        if(intro)
        {
            battery = 100;
            clearstate = new Dictionary<AppCategory, AppState>();
            itemlist = new List<string>();
            Prologueclear = false;
            foreach (AppCategory app in Enum.GetValues(typeof(AppCategory)))
                clearstate.Add(app,AppState.Rejected);
                //clearstate.Add(app, AppState.Rejected);
        }
        else
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(StaticManager.SavePath, FileMode.Open);
            SaveData savedata = (SaveData)formatter.Deserialize(stream);
            clearstate = savedata.clearstate;
            Prologueclear = savedata.Prologueclear;
            battery = savedata.battery;
            itemlist = savedata.itemlist;
            stream.Close();
        }
    }
}

[Serializable]
public struct CutSceneInfo:IDisposable //컷신에 사용될 구조체. 프롤로그/중간씬 호출 객체 /엔딩 객체(아마도 설정어플)에 탑재되 필요시 정적 매니저 객체에 옮기는 방식.
{
    public CutInfo[] cuts;
    public string nextscene;
    public Sprite[] GetSprites()
    {
        int cnt = cuts.Length;
        Sprite[] ret = new Sprite[cnt];
        for (int i = 0; i < cnt; i++)
            ret[i] = cuts[i].sprite;
        return ret;
    }
    public void Dispose()
    {
        foreach (CutInfo cut in cuts)
            cut.Dispose();
        nextscene = null;
    }
}
[Serializable]
public struct CutInfo:IDisposable
{
    public Sprite sprite;
    public string[] speech;

    public void Dispose()
    {
        sprite = null;
        speech = null;
    }
}
[Serializable]
public struct SpeechData //하나의 이벤트 대사에 대한 구조체.
{
    public SpeechState state; //주인공 표정
    public string word; // 출력 대사
}

[Serializable]
public struct AppSpeechArray //각 앱에서 주 이벤트마다 사용될 이벤트 대사 목록
{
    public SpeechData[] introspeech; //진입시 출력할 이벤트 대사
    public SpeechData[] failspeech; //실패시 출력할 이벤트 대사
    public SpeechData[] gameoverspeech; //하트 0 시 출력할 이벤트 대사
    public SpeechData[] clearspeech; //클리어시 출력할 이벤트 대사
}

[Serializable]
public struct AppInfo:IDisposable
{
    public AppCategory beforeapp;
    public AppSpeechArray speecharray;
    public MessageAppInfo messageinfo;
    public InternetAppInfo internetinfo;
    public DictionaryAppInfo dictionaryinfo;
    public GalleryAppInfo galleryinfo;
    public TalkAppInfo talkinfo;
    public void Dispose()
    {
        switch(beforeapp)
        {
            case AppCategory.Message:
                messageinfo.Dispose();
                break;
            case AppCategory.Internet:
                internetinfo.Dispose();
                break;
            case AppCategory.Dictionary:
                dictionaryinfo.Dispose();
                break;
            case AppCategory.Gallery:
                galleryinfo.Dispose();
                break;
            case AppCategory.Talk:
                talkinfo.Dispose();
                break;
        }
    }
}
[Serializable]
public struct TalkAppInfo:IDisposable
{
    [Serializable]
    public struct Talk
    {
        public Speaker speaker; //화자 (말풍선 방향)
        public string content; //대화 내용
        public bool commonroute; //해당값이 true인경우 배열의 끝이 아니여도 선택지를 출현시킨다. 대신 분기가 이루어지지않고 다음대사의 내용만이 바뀐다.
        public string[] choicelist;
        public int[] Goindex;
    }
    public Talk[] introtalk;
    public List<Talk[]> choicetalk;

    public void Dispose()
    {
        introtalk = null;
        choicetalk.Clear();
    }
}
[Serializable]
public struct MessageAppInfo:IDisposable
{
    [Serializable]
    public struct MessageStructure:IDisposable //MessageApp에서 배치될 대화 목록
    {
        public Speaker speaker; //메세지 방향 (Other인경우 왼쪽, Hero인경우 오른쪽)
        public string speech; //대화 내용
        public bool isAnswer; // 배치하지않는것이 정답인경우 false로 둔다.

        public void Dispose()
        {
            speech = null;
            isAnswer = false;
        }
    }
    public MessageStructure[] messagestruct;

    public void Dispose()
    {
        foreach (MessageStructure structure in messagestruct)
            structure.Dispose();
    }
}
[Serializable]
public struct InternetAppInfo:IDisposable
{
    [Serializable]
    public struct EncyclopediaBlock:IDisposable
    {
        public string name;
        public string content;

        public void Dispose()
        {
            name = null;
            content = null;
        }
    }
    [Serializable]
    public struct SiteBlock:IDisposable
    {
        public string name;
        public string url;
        public string content;

        public void Dispose()
        {
            name = null;
            url = null;
            content = null;
        }
    }
    [Serializable]
    public struct KnowInBlock: IDisposable
    {
        public bool ImageEnable; //이미지 존재여부 
        public bool AnswerEnable; //답변 존재여부
        public Sprite image;
        public string questiontitle;
        public string questioncontent;
        public string answercontent;

        public void Dispose()
        {
            image = null;
            questiontitle = null;
            questioncontent = null;
            answercontent = null;
        }
    }
    public string[] answer;
    public string[] relationwords;
    public EncyclopediaBlock[] encyclopediablock;
    public SiteBlock[] siteblock;
    public KnowInBlock[] knowinblock;

    public void Dispose()
    {
        answer = null;
        relationwords = null;
        encyclopediablock = null;
        siteblock = null;
        knowinblock = null;
    }
}
[Serializable]
public struct DictionaryAppInfo:IDisposable
{
    [Serializable]
    public struct DictionaryWord
    {
        public string word; //답 글자
        public string hint;// 힌트
        public int num; //문제 번호
        public bool isStreet; //가로 문제 여부 (false시 세로 문제)
        public CustomVector Start_loc;
    }
    public DictionaryWord[] words;

    public void Dispose()
    {
        words = null;
    }
}

[Serializable]
public struct GalleryAppInfo:IDisposable
{
    public Texture2D image;
    public int PuzzleNumber;

    public void Dispose()
    {
        image = null;
    }
}

[Serializable]
public struct CustomVector
{
    public int x;
    public int y;
    public CustomVector(int loc_x,int loc_y)
    {
        x = loc_x;
        y = loc_y;
    }
}