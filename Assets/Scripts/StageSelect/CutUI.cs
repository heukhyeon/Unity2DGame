#pragma warning disable 0649
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
    [SerializeField]
    AudioSource au;
    [SerializeField]
    AudioClip defaultsound;
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
        public AudioClip sound; //null이 아닌경우 현재 컷이 나올때 음악이 출력됨.
        public CutEffect effect_left; //왼쪽->중앙으로 올때의 이펙트
        public CutEffect effect_right;//중앙->퇴장할때 이펙트
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
       bool FirstCut =  SmartPhone.memory.Message == StageMemory.Status.NotEnter; //메인스테이지에 처음 진입한경우
       bool ThirdCut = SmartPhone.memory.Message == StageMemory.Status.PerfectClear && SmartPhone.memory.Internet == StageMemory.Status.NotEnter; //메세지를 깨고 아직 인터넷을 진입하지않은경우
        TutorialLadder.SetActive(!(FirstCut || ThirdCut)); //메세지, 인터넷을 모두 클리어한경우 사다리 개방.
        if (FirstCut) Nowcut = Intro;
        else if (ThirdCut) Nowcut = MessageExit;
        //재생할 컷이 존재하는경우 PlayUI 비활성.
        if (Nowcut != null)
        {
            PlayUI.enabled = false;
            au.clip = Nowcut[0].sound; 
            StartCoroutine(CutEvent_Left);
            au.Play();
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
    //Player가 앱 진입시 컷씬을 재생하는경우 호출.
    public void CutStart(string stage)
    {
        if (stage == "Message") Nowcut = MessageEnter;
        else if (stage == "Internet") Nowcut = InternetEnter;
        au.clip = Nowcut[0].sound;
        au.Play();
        index = 0;
        PlayUI.enabled = false;
        cutUI.enabled = true;
        StartCoroutine(CutEvent_Left);
    }
    public void Click()
    {
        if (!clickenable) return;//아직 컷이 이동중인경우 리턴.
        clickenable = false;//컷이 이동시작
        //현재 이동방향에 따른 역방향으로 재생.
        if (dir_left) StartCoroutine(CutEvent_Right);
        else StartCoroutine(CutEvent_Left);
    }
    //컷씬 재생이 끝난경우
    void EventEnd()
    {
        //다음 씬 이동이 없는경우
        if (Nowcut == Intro || Nowcut == MessageExit || Nowcut==null)
        {
            cutUI.enabled = false;
            PlayUI.enabled = true;
            au.clip = defaultsound;
            au.Play();
        }
        //메세지 진입
        else if (Nowcut == MessageEnter)
        {
            SmartPhone.memory.Message = StageMemory.Status.NotClear;
            SmartPhone.LoadStage("Message");
        }
        //인터넷 진입
        else if (Nowcut == InternetEnter)
        {
            SmartPhone.memory.Internet = StageMemory.Status.NotClear;
            SmartPhone.LoadStage("Internet");
        }
    }
}
