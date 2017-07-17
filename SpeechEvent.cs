using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpeechEvent:MonoBehaviour,IPointerDownHandler
{
    [SerializeField]
    private Image CharacterSprite = null; //화면에 출력되는 캐릭터 이미지 객체. 표정 전환은 내부 Sprite를 바꾸는 방식 사용.
    [SerializeField]
    private Sprite IdleSprite = null; //무표정 이미지
    [SerializeField]
    private Sprite AngrySprite = null; // 화남 이미지
    [SerializeField]
    private Sprite SadnessSprite = null; //슬픔 이미지
    [SerializeField]
    private Sprite EmbrassmentSprite = null; //당황 이미지
    [SerializeField]
    private Sprite SurprisedSprite = null; //놀람 이미지
    private SpeechData[] Nowscript; //현재 스크립트.
    private Color playercolor;
    private bool inputenable = false;
    private int cnt = 0;
    private Text speechscripter;
    private GameObject PlayUI;
    private EventState eventtype;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (inputenable)
        {
            StopCoroutine("Speech");
            cnt++;
            StartCoroutine("Speech");
        }
    }
    /// <summary>
    /// 대화 시작
    /// </summary>
    /// <param name="data">출력할 대사 모음</param>
    /// <param name="UI">비활성화 후 대사 출력 완료시 다시 활성화할 PlayUI</param>
    /// <param name="FadeEffect">페이드 이펙트 적용 여부</param>
    public void SpeechStart(SpeechData[] data,GameObject UI,EventState eventstate)
    {
        eventtype = eventstate;
        cnt = 0;
        Nowscript = data;
        if(speechscripter==null) speechscripter = CharacterSprite.GetComponentInChildren<Text>();
        speechscripter.text = null;
        ChangeCharacterState();
        PlayUI = UI;
        this.gameObject.SetActive(true);
        if(eventstate==EventState.Intro)
        {
            playercolor = CharacterSprite.color;
            playercolor.a = 0;
            CharacterSprite.color = playercolor;
            Fade();
        }
        else
        {
            inputenable = true;
            StartCoroutine("Speech");
        }
    }
    private void Fade()
    {
        playercolor = CharacterSprite.color;
        playercolor.a += 1.5f* Time.deltaTime;
        CharacterSprite.color = playercolor;
        if (playercolor.a <= 1) Invoke("Fade", Time.deltaTime);
        else
        {
            inputenable = true;
            StartCoroutine("Speech");
        }
    }
    /// <summary>
    /// 얼굴 변화
    /// </summary>
    private void ChangeCharacterState()
    {
        switch (Nowscript[cnt].state)
        {
            case SpeechState.Angry:
                CharacterSprite.sprite = AngrySprite;
                break;
            case SpeechState.Fear:
                CharacterSprite.sprite = EmbrassmentSprite;
                break;
            case SpeechState.Just:
                CharacterSprite.sprite = IdleSprite;
                break;
            case SpeechState.Sorrow:
                CharacterSprite.sprite = SadnessSprite;
                break;
            case SpeechState.Surprise:
                CharacterSprite.sprite = SurprisedSprite;
                break;
        }
    }
    private IEnumerator Speech()
    {
        if (!inputenable) yield break;
        if(cnt>=Nowscript.Length)
        {
            Nowscript = null;
            inputenable = false;
            if (eventtype == EventState.GameOver || eventtype == EventState.Clear)
            {
                AppSceneScript appscript = PlayUI.GetComponent<AppSceneScript>();
                switch (eventtype)
                {
                    case EventState.Clear:
                        appscript.Clear();
                        break;
                    case EventState.GameOver:
                        appscript.GameOver();
                        break;
                }
            }
            this.gameObject.SetActive(false);
        }
        else
        {
            ChangeCharacterState();
            speechscripter.text = null;
            for (int i = 0; i < Nowscript[cnt].word.Length; i++)
            {
                speechscripter.text += Nowscript[cnt].word[i];
                yield return new WaitForSeconds(Time.deltaTime);
            }
            StopCoroutine("SpeechEvent");
        }
    }

}
