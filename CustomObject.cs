using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 대부분의 객체에 기본적으로 탑재되는 Monobehavior아래 최상위 상속클래스. 
/// </summary>
public class CustomObject : MonoBehaviour {

    protected virtual void Awake()
    {
        SceneObjectManager.Add(this.gameObject);
    }

}

/// <summary>
/// 플레이어, 적, NPC등 캐릭터들에 대한 공통 상속 클래스
/// </summary>
public abstract class Actor : CustomObject
{
    //충돌 이벤트(OnCollisionEnter 등)은 Player등의 상속받은 객체에서 작성한다.
    /// <summary>
    /// 기본 스테이터스. 추후 더 추가예정
    /// </summary>
    public Status status;
    /// <summary>
    /// 현재 객체의 타입을 나타낸다. 해당값에 따라 처리가 달라지게 할 예정.
    /// </summary>
    protected enum CharacterType { Player, Enemy, NPC };
    /// <summary>
    /// 모든 Actor는 SpriteRenderer를 가진다.
    /// </summary>
    protected SpriteRenderer Sprite;
    /// <summary>
    /// 모든 Actor는 강체를 가진다.
    /// </summary>
    protected Rigidbody2D Rbody;
    /// <summary>
    /// 모든 Actor는 애니메이터를 가지며, 반드시 Bottom,Moving파라미터를 가지고있어야한다. (적이 발사하는 탄환 등은 Actor를 상속하지 않는다.)
    /// </summary>
    protected Animator Ani;
    /// <summary>
    /// true일시 스프라이트 이미지 기준의 방향을 유지하고, false시 반전시킨다.
    /// </summary>
    protected bool Flip
    {
        get { return Sprite.flipX; }
        set { Sprite.flipX = !value; }
    }
    /// <summary>
    /// 애니메이터의 Bottom파라미터를 조정한다.
    /// </summary>
    protected bool Bottom
    {
        get { return Ani.GetBool("Bottom"); }
        set { Ani.SetBool("Bottom",value); }
    }
    /// <summary>
    /// 애니메이터의 Moving 파라미터를 조절한다.
    /// </summary>
    protected bool Moving
    {
        get { return Ani.GetBool("Moving"); }
        set { Ani.SetBool("Moving", value); }
    }
    protected override sealed void Awake()
    {
        base.Awake();
        Sprite = GetComponent<SpriteRenderer>();
        Rbody = GetComponent<Rigidbody2D>();
        Ani = GetComponent<Animator>();
        ActorAwake();
    }
    protected abstract void ActorAwake();
    protected abstract void Move();
    /// <summary>
    /// 충돌, 공격등에 의해 객체가 사망할때의 메소드. 필요시 오버라이드해 사용.
    /// </summary>
    protected void Dead()
    {
        Debug.Log(gameObject.name + "사망");
        Destroy(this.gameObject);
    }
}

/// <summary>
/// UI 오브젝트들이 상속하는 클래스. 해상도 비율에 따른 크기 재조정.
/// </summary>
public abstract class CustomUI : CustomObject
{
    protected float RATE;
    protected RectTransform recttransform;
    protected override sealed void Awake()
    {
        base.Awake();
        recttransform = GetComponent<RectTransform>();
        RATE = Screen.width / 720f > Screen.height / 1280f ? Screen.width / 720f : Screen.height / 1280f;
        Resize(recttransform);
        Reposition();
        UIAwake();
    }
    protected abstract void UIAwake();
    /// <summary>
    /// 해상도에 맞게 UI 크기를 조정한다.
    /// </summary>
    /// <param name="rect"></param>
    protected void Resize(RectTransform rect)
    {
        rect.sizeDelta=new Vector2(rect.sizeDelta.x * RATE, rect.sizeDelta.y * RATE);
    }
    /// <summary>
    /// 해상도에 맞게 UI 위치를 재조정한다.
    /// </summary>
    /// <param name="rect"></param>
    protected void Reposition()
    {
        this.transform.position = new Vector2(this.transform.position.x * RATE, this.transform.position.y * RATE);
    }
}
[Serializable]
public class Status
{
    public int Hp;
    public int Mp;
    public int Atk;
    public float defaultspeed;
    public float defaultjumping;
    public float Dashspeed;
}

[Serializable]
public class StateStruct
{
    public enum ParameterType { Int,Float,Bool};
    public AnimationClip clip = null;
    public AnimationClip afterclip = null;
    public ParameterType parametertype = ParameterType.Bool;
    public string parametername;
    public float parameter = 0f;
}