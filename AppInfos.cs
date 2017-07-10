using UnityEngine;
using System;
using UnityEngine.UI;
//Message, Internet등 각 어플들에 대한 Info클래스들이 모인 클래스

/// <summary>
/// 메세지 앱에 대한 정답. 이미지 분포 등을 저장하는 클래스
/// </summary>
[Serializable]
public class MessageAppInfo
{
    public string Answer;
    public string[] Words;
}

/// <summary>
/// 인터넷 앱에 대한 정답. 이미지 분포 등을 저장하는 클래스. 메인 스테이지에서 해당 클래스의 배열을 만들어둔뒤 난수(또는 임의의수) 인덱스로 전달
/// </summary>
[Serializable]
public class InternetAppInfo
{
    /// <summary>
    /// 인터넷 앱 "사전"에서 하나의 블록을 담당하는 구조체
    /// </summary>
    [Serializable]
    public struct DictionaryInfo
    {
        public string name;
        public string content;
    }

    /// <summary>
    /// 인터넷 앱에서 "지식인"부분에 대한 하나의 블록을 담당하는 구조체
    /// </summary>
    [Serializable]
    public struct KnowInInfo
    {
        public bool EnableImage;
        public Image image;
        public string QuestionerTitle;
        public string QuestionerContent;
        public string AnswerContent;
    }
    /// <summary>
    /// 인터넷 앱에서 "사이트"부분에 대한 하나의 블록을 담당하는 구조체
    /// </summary>
    [Serializable]
    public struct SiteInfo
    {
        public string name;
        public string url;
        public string content;
    }
    public string answer; //정답
    public string[] relations; //연관 검색어들 배열.
    public DictionaryInfo[] dictionarys; //사전 배열
    public Image[] images;
    public KnowInInfo[] knowins;
    public SiteInfo[] sites;
}
