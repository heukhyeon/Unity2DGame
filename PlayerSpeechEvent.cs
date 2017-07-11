#pragma warning disable 0649
// 구조체가 기본값을 사용한다는 경고를 무시한다.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum SpeechStatus { Entry,Fail,GameOver,Success }
public enum PlayerStatus { Idle,Angry,Sadness,Embrassment,Surprised }
public class PlayerSpeechEvent : CustomUI,IPointerDownHandler {

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
    [SerializeField]
    private PlayerSpeechs EntryScript; // "스테이지 첫 진입"시 출력할 스크립트
    [SerializeField]
    private PlayerSpeechs FailScript; // 문제를 "틀렸을 때" 출력할 스크립트
    [SerializeField]
    private PlayerSpeechs GameOverscript; //하트가 0이 되었을때 출력할 스크립트
    [SerializeField]
    private PlayerSpeechs SuccessScript; //문제를 "성공했을 때" 출력할 스크립트
    private PlayerSpeechs Nowscript; //현재 스크립트.
    private Color playercolor;
    private bool inputenable = false;
    private int cnt = 0;
    private Text speechscripter;
    protected override void UIAwake()
    {
        speechscripter = CharacterSprite.GetComponentInChildren<Text>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (inputenable)
        {
            StopCoroutine("SpeechEvent");
            cnt++;
            StartCoroutine("SpeechEvent");
        }
    }
    public void SetScript(SpeechStatus status)
    {
        switch(status)
        {
            case SpeechStatus.Entry:
                Nowscript = EntryScript;
                break;
            case SpeechStatus.Fail:
                Nowscript = FailScript;
                break;
            case SpeechStatus.GameOver:
                Nowscript = GameOverscript;
                break;
            case SpeechStatus.Success:
                Nowscript = SuccessScript;
                break;
        }
        switch (Nowscript.Speechs[0].status)
        {
            case PlayerStatus.Angry:
                CharacterSprite.sprite = AngrySprite;
                break;
            case PlayerStatus.Embrassment:
                CharacterSprite.sprite = EmbrassmentSprite;
                break;
            case PlayerStatus.Idle:
                CharacterSprite.sprite = IdleSprite;
                break;
            case PlayerStatus.Sadness:
                CharacterSprite.sprite = SadnessSprite;
                break;
            case PlayerStatus.Surprised:
                CharacterSprite.sprite = SurprisedSprite;
                break;
        }
        // 저장소 구현후, 저장소에서 해당 앱에 대한 엔트리 이벤트를 본 경우 entry를 true로 둬서 entry 이벤트를 발생하지 않게한다.
        if (Nowscript.Equals(EntryScript))
        {
            playercolor = CharacterSprite.color;
            playercolor.a = 0;
            CharacterSprite.color = playercolor;
            FadeEffect();
        }
        else
        {
            inputenable = true;
            StartCoroutine("SpeechEvent");
        }
    }
    private void FadeEffect()
    {
        playercolor.a += Time.deltaTime;
        CharacterSprite.color = playercolor;
        if (playercolor.a >= 1)
        {
            Nowscript = EntryScript;
            StartCoroutine("SpeechEvent");
            inputenable = true;
        }
        else Invoke("FadeEffect", Time.deltaTime);
    }
    private IEnumerator SpeechEvent()
    {
        if(cnt>=Nowscript.Speechs.Length)
        {
            cnt = 0;
            inputenable = false;
            speechscripter.text = null;
            this.gameObject.SetActive(false);
            StopCoroutine("SpeechEvent");
            if(Nowscript.Equals(GameOverscript) || Nowscript.Equals(SuccessScript))
            {
                if (Nowscript.Equals(GameOverscript)) CustomSceneManager.batterylife -= 5;
                else CustomSceneManager.batterylife -= 2;
                CustomSceneManager.goMainScene();
            }
        }
        else
        {
            switch (Nowscript.Speechs[cnt].status)
            {
                case PlayerStatus.Angry:
                    CharacterSprite.sprite = AngrySprite;
                    break;
                case PlayerStatus.Embrassment:
                    CharacterSprite.sprite = EmbrassmentSprite;
                    break;
                case PlayerStatus.Idle:
                    CharacterSprite.sprite = IdleSprite;
                    break;
                case PlayerStatus.Sadness:
                    CharacterSprite.sprite = SadnessSprite;
                    break;
                case PlayerStatus.Surprised:
                    CharacterSprite.sprite = SurprisedSprite;
                    break;
            }
            speechscripter.text = null;
            for (int i = 0; i < Nowscript.Speechs[cnt].word.Length; i++)
            {
                speechscripter.text += Nowscript.Speechs[cnt].word[i];
                yield return new WaitForSeconds(Time.deltaTime);
            }
            StopCoroutine("SpeechEvent");
        }
    }
}

[Serializable]
public struct PlayerSpeechs
{
    public Speech[] Speechs;
}

[Serializable]
public struct Speech
{
    public PlayerStatus status;
    public string word;
}
