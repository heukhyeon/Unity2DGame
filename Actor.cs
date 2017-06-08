using System;
using UnityEngine;

/// <summary>
/// 플레이어, 적, NPC등 캐릭터들에 대한 공통 상속 클래스
/// </summary>
public class Actor : CustomObject
{
    /// <summary>
    /// 기본 스테이터스. 추후 더 추가예정
    /// </summary>
    public struct Status
    {
        public int Hp;
        public int Mp;
        public int Atk;
    }
    /// <summary>
    /// 현재 객체의 타입을 나타낸다. 해당값에 따라 처리가 달라지게 할 예정.
    /// </summary>
    public enum CharacterType { Player,Enemy,NPC};
}

