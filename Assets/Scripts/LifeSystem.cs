#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LifeSystem : MonoBehaviour
{
    //LifeSystem의 각 부분별 속성을 명확히 하기위해 구조체 분할.
#region 파생 구조체 
        //라이프창이 표시되는 틀
    [Serializable]
    public struct LifeInterface
    {
        public RectTransform ValueField;
        [SerializeField]
        RectTransform FaceField;
        [SerializeField]
        RectTransform NoticeField;
        RectTransform lifevalue;
        RawImage face;
        Text notice;
        AudioSource au;
        public Image Frame;
        public Button submit;
        public Vector2 Value { get { return lifevalue.sizeDelta; }  set { lifevalue.sizeDelta = value; } }
        public Vector2 FaceFieldSize { get { return FaceField.sizeDelta; } set { FaceField.sizeDelta = value; } }
        public Texture Face { get { return face.texture; } set { face.texture = value; } }
        public AudioSource Au { get { if (au == null) au = Frame.GetComponent<AudioSource>(); return au; } }
        public float Intro_Goal { get { return NoticeField.transform.position.y; } }
        public string Notice { get { return notice.text; } set { notice.text = value; } }
        public void Init()
        {
            lifevalue = ValueField.GetChild(0) as RectTransform;
            notice = NoticeField.GetComponentInChildren<Text>();
            face = FaceField.GetComponentInChildren<RawImage>();
            lifevalue.sizeDelta = new Vector2(0, lifevalue.sizeDelta.y);
            FaceField.sizeDelta = new Vector2(0, FaceField.sizeDelta.y);
            Frame.gameObject.SetActive(false);
            ValueField.gameObject.SetActive(false);
        }
        public void ValueParentSet(bool enable)
        {
            lifevalue.SetParent(enable == true ? Frame.transform : ValueField);
        }
    }
    //처음 진입문구, 클리어 문구가 표시되는 장소
    [Serializable]
    public struct NoticeInterface
    {
        [SerializeField]
        Text clear;
        RectTransform rightclear;
        public Text Intro;
        public Vector2 Clear { get { return clear.transform.localPosition; } set { clear.transform.localPosition = value; } }
        public Vector2 RightClear
        {
            get
            {
                if(rightclear==null)
                {
                    rightclear= SmartPhone.CreateAndPosition(clear.gameObject, clear.transform.parent as RectTransform, clear.transform.localPosition);
                }
                return rightclear.localPosition;
            }
            set
            {
                if (rightclear == null)
                {
                    rightclear = SmartPhone.CreateAndPosition(clear.gameObject, clear.transform.parent as RectTransform, clear.transform.localPosition);
                }
                rightclear.localPosition = value;
            }
        }
        public Color ClearColor { get { return clear.color; } set{ clear.color = value;} }
        
    }
    //각종 이벤트 진행 속도
    [Serializable]
    public struct SpeedControl
    {
        [SerializeField]
        [Range(1, 50f)]
        float introaccess;
        [SerializeField]
        [Range(1, 50f)]
        float typing;
        [SerializeField]
        [Range(1, 20)]
        float aftertyping;
        [SerializeField]
        [Range(1, 50)]
        float introup;
        [SerializeField]
        [Range(1, 10)]
        float clear;
        public float IntroAccess { get { return introaccess * 2f * Time.deltaTime; } }
        public float Typing { get { return typing * Time.deltaTime; } }
        public float AfterTyping { get { return aftertyping * 3f * Time.deltaTime; } }
        public float IntroUP { get { return introup; } }
        public float Clear { get { return clear; } }
    }

    public LifeInterface lifeUI; // 버튼을 관리하므로 예외적으로 LifeInterface만 Stage가 접근할수있게끔한다.
    [SerializeField]
    SpeedControl speed;
    [SerializeField]
    NoticeInterface notice;
    #endregion
    [HideInInspector]
    public Stage stage;
    [SerializeField]
    AudioClip fail;
    [SerializeField]
    AudioClip clear;
    [SerializeField]
    AudioClip gameover;
    int life;
    string Misson
    {
        get
        {
            string name = SceneManager.GetActiveScene().name;
            switch (name)
            {
                case "Message":
                    return "메세지는 어떻게 배열되야할까?";
                case "Internet":
                    return "뭘 검색했는가?";
                case "Dictionary":
                    return "제한 시간안에 가로세로 퍼즐을 완성!";
                case "RollingBall":
                    return "최대한 죽지않고 골인!";
                case "Calculator":
                    return "시간안에 모든 문제를 해결!";
                case "Gallery":
                    return "시간안에 퍼즐을 맞춰!";
                default:
                    return "아직 설정안됨!";
            }
        }
    }
    string NowTime { get { return "남은 시간:" + (life / 60).ToString("D2") + ":" + (life % 60).ToString("D2"); } }
    string NowLife { get { return "남은 횟수:" + life + "/" + maxlife; } }
    float step = 0f;
    int maxlife;
    public IEnumerator IntroEvent
    {
        get
        {
            lifeUI.Init();
            notice.Intro.gameObject.SetActive(true);
            float spd = speed.IntroAccess;
            Vector2 size =notice.Intro.transform.localScale;
            //미션 타입을 화면 바깥에서 내부로 끌어온다.
            //NoticeEffect.gameObject.SetActive(true);
            while (notice.Intro.transform.localScale.x > 1)
            {
                SmartPhone.SizeSet(notice.Intro, -spd, 1);
                yield return new WaitForEndOfFrame();
            }
            string typestring = Misson;
            for (int i = 0; i < typestring.Length; i++)
            {
               notice.Intro.text += typestring[i];
                yield return new WaitForSeconds(speed.Typing);
            }
            yield return new WaitForSeconds(speed.AfterTyping);
            float cnt = 0;
            Vector2 pos =notice.Intro.transform.position;
            float goal = lifeUI.Intro_Goal;
            float upspd = speed.IntroUP;
            //잠시 미션타입을 아래로 내린다
            while (cnt < 1)
            {
                cnt += Time.deltaTime;
                pos.y -= upspd / 5;
                if (upspd > 2) upspd -= 1;
               notice.Intro.transform.position = pos;
                yield return new WaitForEndOfFrame();
            }
            //위로 올리며 축소한다.
            while (pos.y < goal)
            {
                SmartPhone.SizeSet(notice.Intro, -Time.deltaTime, 0);
                pos.y += upspd;
                if (pos.y > goal) pos.y = goal;
                upspd += 3;
               notice.Intro.transform.position = pos;
                yield return new WaitForEndOfFrame();
            }
            Destroy(notice.Intro.gameObject);
            lifeUI.Frame.gameObject.SetActive(true);
            lifeUI.ValueParentSet(true);
            pos = lifeUI.Value;
            yield return new WaitForSeconds(0.5f);
            while (pos.x < 840)
            {
                pos.x += 20;
                if (pos.x > 840) pos.x = 840;
                lifeUI.Value = pos;
                yield return new WaitForEndOfFrame();
            }
            lifeUI.ValueField.gameObject.SetActive(true);
            lifeUI.ValueParentSet(false);
            pos = lifeUI.FaceFieldSize;
            while (pos.x < 145)
            {
                pos.x += 10;
                if (pos.x > 145) pos.x = 145;
                lifeUI.FaceFieldSize = pos;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public IEnumerator NormalClear
    {
        get
        {
            CancelInvoke();
            if (lifeUI.submit != null) lifeUI.submit.interactable = false;
            stage.BackgroundMusic.Stop();
            lifeUI.Au.PlayOneShot(clear);
            SaveDataSet();
            Color color = notice.ClearColor;
            color.a = 0.2f; //클리어 문구를 투명하게끔한다.
            notice.ClearColor = color;
            Vector2 pos = notice.Clear;
            Vector2 rightpos = pos;
            rightpos.x *= -1;
            int loc = 100; //중앙에서 약간 더 나아가게끔 목적지 지정.
            float spd = speed.Clear;
            //양 옆에서 클리어문구가 중앙으로 밀려오게 한다.
            while (pos.x < loc)
            {
                pos.x += spd;
                rightpos.x -= spd;
                spd += spd * 0.1f;
                if (pos.x > loc)
                {
                    pos.x = loc;
                    rightpos.x = -loc;
                }
                notice.Clear = pos;
                notice.RightClear = rightpos;
                yield return new WaitForEndOfFrame();
            }
            loc = 0; //중앙으로 목적지 지정.
            spd = speed.Clear * 0.1f; //스피드를 초기화하고, 기존값보다 약간 더 줄인다.
            //두 클리어 문구를 중앙에 맞춘다.
            while (pos.x > loc)
            {
                pos.x -= spd;
                rightpos.x += spd;
                spd += spd * 0.5f;
                if (pos.x < loc)
                {
                    pos.x = loc;
                    rightpos.x = loc;
                }
                notice.Clear = pos;
                notice.RightClear = rightpos;
                yield return new WaitForEndOfFrame();
            }
            //클리어 문구를 불투명하게 바꾼다.
            color.a = 1;
            notice.ClearColor = color;
            yield return new WaitForSeconds(3f);
        }
    }
    //NormalClear를 역순으로 진행.
    public IEnumerator SectorClear
    {
        get
        {
            yield return new WaitForSeconds(1f);
            Color color = notice.ClearColor;
            Vector2 pos = notice.Clear;
            Vector2 rightpos = notice.RightClear;
            color.a = 0.2f;
            notice.ClearColor = color;
            int loc = 100;
            float spd = speed.Clear * 0.1f;
            while (pos.x < loc)
            {
                pos.x -= spd;
                rightpos.x += spd;
                spd += spd * 0.5f;
                if (pos.x < loc)
                {
                    pos.x = loc;
                    rightpos.x = loc;
                }
                notice.Clear = pos;
                notice.RightClear = rightpos;
                yield return new WaitForEndOfFrame();
            }
            loc = -2000;
            spd = speed.Clear;
            while (pos.x > loc)
            {
                pos.x -= spd;
                rightpos.x += spd;
                spd += spd * 0.1f;
                if (pos.x < loc)
                {
                    pos.x = loc;
                    rightpos.x = -loc;
                }
                notice.Clear = pos;
                notice.RightClear = rightpos;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public IEnumerator FailEvent
    {
        get {
            lifeUI.Au.PlayOneShot(fail);
            life -= stage.WestedLifeFail; //Stage 하위클래스가 구현한 감소라이프값만큼 현재 라이프값 감소. 
            lifeUI.Value = new Vector2(step * life, 120);//수치 표현
            lifeUI.Notice = stage.missontype == MissionType.Count ? NowLife : NowTime; //MissionType에 따라 문구 갱신.
            iTween.ShakePosition(lifeUI.Frame.gameObject, new Vector2(20, 20), 1f);//LifeInterface가 흔들리게끔 함.
            //LifeInterface가 점등되게끔 함.
            for (int i = 0; i < 10; i++)
            {
                lifeUI.Frame.color = lifeUI.Frame.color == Color.red ? Color.white : Color.red;
                yield return new WaitForSeconds(0.1f);
            }
            //라이프가 0이 된경우 GameOver 재생.
            if (life == 0) yield return StartCoroutine(GameOver);
        }
    }
    //MissionType에 따라 문구를 표시하고, Timer인경우 타이머 작동. Stage 하위 클래스가 IClickDelay를 가진경우 clickenable을 수정.
    public Action GameStart
    {
        get
        {
            return () =>
            {
                maxlife = stage.MaxLife;
                life = maxlife;
                step = (float)840 / maxlife;
                if (stage.missontype == MissionType.Count) lifeUI.Notice = NowLife;
                else
                {
                    lifeUI.Notice = NowTime;
                    Timer();
                }
                IClickDelay delay = stage.GetComponent<IClickDelay>();
                if (delay != null) delay.clickenable = true;
                else if (lifeUI.submit != null) lifeUI.submit.interactable = true;
            };
        }
    }
    IEnumerator GameOver
    {
        get
        {
            stage.BackgroundMusic.Stop();
            if (lifeUI.submit != null) lifeUI.submit.interactable = false; //버튼이 존재하는경우 버튼을 강제로 비활성화
            lifeUI.Au.PlayOneShot(gameover);
            Transform[] trs = this.transform.root.GetComponentsInChildren<Transform>();
            foreach (var tr in trs) iTween.ShakePosition(tr.gameObject, new Vector3(100, 100, 0), 2f);//모든 씬 내 객체를 뒤흔듬
            yield return new WaitForSeconds(3f);
            SmartPhone.LoadStage("StageSelect");//메인 스테이지로
        }
    }
    void Timer()
    {
        life--;
        lifeUI.Value= new Vector2(step * life, 120);
        lifeUI.Notice = NowTime;
        if (life > 0) Invoke("Timer", 1f);
        else StartCoroutine(GameOver);
    }
    // 한번의 클리어 이벤트만을 실행하는 어플의 경우, 여기서 클리어 상태를 체크. 섹터 클리어나 LifeSystem을 사용하지않는 씬은 해당 씬에서 직접 처리.
    void SaveDataSet()
    {
        string name = SceneManager.GetActiveScene().name;
        switch (name)
        {
            case "Message":
                SmartPhone.memory.Message = StageMemory.Status.PerfectClear;
                break;
            case "Internet":
                SmartPhone.memory.Internet= StageMemory.Status.PerfectClear;
                break;
            case "Dictionary":
                SmartPhone.memory.Dictionary = StageMemory.Status.PerfectClear;
                break;
            case "Calculator":
                SmartPhone.memory.Calculator = StageMemory.Status.PerfectClear;
                break;
            case "Gallery":
                SmartPhone.memory.Gallery = StageMemory.Status.PerfectClear;
                break;
        }
    }
}
