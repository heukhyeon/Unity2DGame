using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutUI : MonoBehaviour {

    private enum CutEffect { None,Throw,Absorbe}
    [SerializeField]
    GameObject TutorialLadder;
    [SerializeField]
    Canvas PlayUI;
    Canvas cutUI;
    Vector2 DEFAULT_CUTLEFT { get { return new Vector2(-920, 0); } }
    Vector2 THROW_CUTLEFTSIZE { get { return new Vector2(20, 20); } }
    Vector2 Position { get { return cut.transform.localPosition; } set { cut.transform.localPosition = value; } }
    Vector2 Scale { get { return cut.transform.localScale; } set { cut.transform.localScale = value; } }
#region 구조체 모음
    [Serializable]
    private struct CutInfo
    {
        public Sprite image;
        public CutEffect effect_left;
        public CutEffect effect_right;
    }
    [SerializeField]
    CutInfo[] Intro;
    [SerializeField]
    CutInfo[] MessageEnter;
    [SerializeField]
    CutInfo[] MessageExit;
    [SerializeField]
    CutInfo[] InternetEnter;
#endregion
    [SerializeField]
    Image cut;
    [SerializeField]
    [Range(1, 80)]
    float speed;
    CutInfo[] Nowcut;
    int index = 0;
    bool clickenable;
    bool dir_left = false;
    private void Start()
    {
        cutUI = GetComponent<Canvas>();
        Judge();
    }
    void Judge()
    {
       bool FirstCut =  SmartPhone.memory.Message == StageMemory.Status.NotEnter;
       bool ThirdCut = SmartPhone.memory.Message == StageMemory.Status.PerfectClear && SmartPhone.memory.Internet == StageMemory.Status.NotEnter;
        TutorialLadder.SetActive(!(FirstCut || ThirdCut));
        if (FirstCut) Nowcut = Intro;
        else if (ThirdCut) Nowcut = MessageExit;
        if (Nowcut != null)
        {
            PlayUI.enabled = false;
            StartCoroutine(CutEvent_Left);
        }
        else EventEnd();
    }
    IEnumerator CutEvent_Left
    {
        get
        {
            if (Scale != new Vector2(1, 1)) Scale = new Vector2(1, 1);
            CutInfo now = Nowcut[index];
            dir_left = true;
            cut.sprite = now.image;
            //이펙트가 없는경우 단순 좌->우로 중앙까지 이동
            if(now.effect_left== CutEffect.None)
            {
                Position = DEFAULT_CUTLEFT;
                while(Position.x<0)
                {
                    Vector2 pos = Position;
                    pos.x += speed;
                    if (pos.x > 0) pos.x = 0;
                    Position = pos;
                    yield return new WaitForEndOfFrame();
                }
            }
            else if(now.effect_left == CutEffect.Throw)
            {
                Position = Vector2.zero;
                Scale = THROW_CUTLEFTSIZE;
                float spd = speed * Time.deltaTime;
                while(Scale.x>1)
                {
                    SmartPhone.SizeSet(cut, -spd, 1);
                    spd += spd / 10f;
                    yield return new WaitForEndOfFrame();
                }
                iTween.ShakePosition(cut.gameObject, new Vector3(100, 100, 0), 1f);
                yield return new WaitForSeconds(0.8f);
            }
            yield return new WaitForEndOfFrame();
            clickenable = true;
        }
    }
    IEnumerator CutEvent_Right
    {
        get
        {
            CutInfo now = Nowcut[index];
            //이펙트가 없는경우 중앙에서 우측으로 이동
            if(now.effect_right== CutEffect.None)
            {
                while (Position.x < 920)
                {
                    Vector2 pos = Position;
                    pos.x += speed;
                    Position = pos;
                    yield return new WaitForEndOfFrame();
                }
            }
            else if(now.effect_right== CutEffect.Throw)
            {
                float spd = speed * Time.deltaTime;
                while(Scale.x>0.1f)
                {
                    SmartPhone.SizeSet(cut, -spd, 0.1f);
                    spd += spd / 10f;
                    yield return new WaitForEndOfFrame();
                }
                while(Scale.x<5f)
                {
                    SmartPhone.SizeSet(cut, spd, 5f);
                    spd += spd / 10f;
                    yield return new WaitForEndOfFrame();
                }
            }
            else if(now.effect_right == CutEffect.Absorbe)
            {
                float spd = speed * Time.deltaTime * Time.deltaTime;
                while(Scale.x>0.05f)
                {
                    SmartPhone.SizeSet(cut, -spd, 0.05f);
                    spd += spd / 10f;
                    yield return new WaitForEndOfFrame();
                }
            }
            index++;
            dir_left = false;
            if (index < Nowcut.Length) StartCoroutine(CutEvent_Left);
            else EventEnd();
        }
    }
    public void CutStart(string stage)
    {
        if (stage == "Message") Nowcut = MessageEnter;
        else if (stage == "Internet") Nowcut = InternetEnter;
        index = 0;
        PlayUI.enabled = false;
        cutUI.enabled = true;
        StartCoroutine(CutEvent_Left);
    }
    public void Click()
    {
        if (!clickenable) return;
        clickenable = false;
        if (dir_left) StartCoroutine(CutEvent_Right);
        else StartCoroutine(CutEvent_Left);
    }
    void EventEnd()
    {
        if (Nowcut == Intro || Nowcut == MessageExit || Nowcut==null)
        {
            cutUI.enabled = false;
            PlayUI.enabled = true;
        }
        else if (Nowcut == MessageEnter)
        {
            SmartPhone.memory.Message = StageMemory.Status.NotClear;
            SmartPhone.LoadStage("Message");
        }
        else if (Nowcut == InternetEnter)
        {
            SmartPhone.memory.Internet = StageMemory.Status.NotClear;
            SmartPhone.LoadStage("Internet");
        }
    }
}
