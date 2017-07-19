using System.Collections.Generic;
using UnityEngine;

public class AppIcon:MonoBehaviour
{
    public AppCategory category;
    public List<Item> getitem; //어플 클리어시 획득할 아이템.
    public bool GetandDestory = true; //아이템을 획득시 해당 아이템을 아이템리스트에서 제거함으로써 중복 획득을 방지한다.
    public bool RandomGet = false; //아이템이 2개 이상일때 앞에서부터 아이템을 획득할지 여부. 
    [SerializeField]
    public AppSpeechArray appspeecharray;
    [SerializeField]
    public MessageAppInfo[] messageinfo;
    [SerializeField]
    public InternetAppInfo[] internetinfo;
    [SerializeField]
    public DictionaryAppInfo[] dictionaryinfo;
}
