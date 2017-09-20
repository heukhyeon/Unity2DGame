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
#region 파생 구조체 
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
        public Image Frame;
        public Button submit;
        public Vector2 Value { get { return lifevalue.sizeDelta; }  set { lifevalue.sizeDelta = value; } }
        public Vector2 FaceFieldSize { get { return FaceField.sizeDelta; } set { FaceField.sizeDelta = value; } }
        public Texture Face { get { return face.texture; } set { face.texture = value; } }
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

    public LifeInterface lifeUI;
    [SerializeField]
    SpeedControl speed;
    [SerializeField]
    NoticeInterface notice;
    #endregion
    [HideInInspector]
    public Stage stage;
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
                pos.x += 5;
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
            Color color = notice.ClearColor;
            color.a = 0.2f;
            notice.ClearColor = color;
            Vector2 pos = notice.Clear;
            Vector2 rightpos = pos;
            rightpos.x *= -1;
            int loc = 100;
            float spd = speed.Clear;
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
            loc = 0;
            spd = speed.Clear * 0.1f;
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
            color.a = 1;
            notice.ClearColor = color;
        }
    }
    public IEnumerator SectorClear
    {
        get
        {
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
            life -= stage.WestedLifeFail;
            lifeUI.Value = new Vector2(step * life, 120);
            lifeUI.Notice = stage.missontype == MissionType.Count ? NowLife : NowTime;
            iTween.ShakePosition(lifeUI.Frame.gameObject, new Vector2(20, 20), 1f);
            for (int i = 0; i < 10; i++)
            {
                lifeUI.Frame.color = lifeUI.Frame.color == Color.red ? Color.white : Color.red;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
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
    void Timer()
    {
        life--;
        lifeUI.Value= new Vector2(step * life, 120);
        lifeUI.Notice = NowTime;
        Invoke("Timer", 1f);
    }
}
