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
    /// <summary>
    /// 충돌, 공격등에 의해 객체가 사망할때의 메소드. 필요시 오버라이드해 사용.
    /// </summary>
    public void Dead()
    {
        Debug.Log(gameObject.name+"사망");
        Destroy(this.gameObject);
    }
}

