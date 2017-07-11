using System;
using UnityEngine;

[Serializable]
public class Item:MonoBehaviour
{
    public ItemInfo info;
}
[Serializable]
public class ItemInfo
{
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string name;
    /// <summary>
    /// Resource에서 찾을 아이템 프리팹 이름
    /// </summary>
    public string path;
    /// <summary>
    /// 아이템 설명
    /// </summary>
    public string content;
}
