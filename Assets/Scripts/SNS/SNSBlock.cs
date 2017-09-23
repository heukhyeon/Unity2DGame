#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SNSBlock : MonoBehaviour {

    [SerializeField]
    RawImage Profile;
    [SerializeField]
    Text Name;
    [SerializeField]
    Text Content;
    [SerializeField]
    Button LikeButton;
    [SerializeField]
    Text LikeList;
    [SerializeField]
    RawImage Picture;
    public Vector2 Position { get { return this.transform.localPosition; } set { this.transform.localPosition = value; } }
    public bool ImageEnable { get; private set; }
    public float Nextpos { get { return Position.y - (GetComponent<RectTransform>().sizeDelta.y + 30); } }
    Vector2 Scale { get { return this.transform.localScale; } set{ this.transform.localScale = value; } }
    public void Set(SNS.Info info)
    {
        if (info.profile != null) Profile.texture = info.profile;
        Name.text = info.name;
        Content.text = info.content;
        LikeButton.interactable = info.likeenble;
        if (LikeButton.interactable) LikeButton.onClick.AddListener(LikeClick);
        if (info.likelist.Length > 0) LikeList.text = info.likelist + "님이 이 게시글을 좋아합니다";
        else LikeList.text = "넌 혼밥하는 찐따주제에 흑역사까지 남기는구나?";
        ImageEnable = info.picture != null;
        if (ImageEnable) Picture.texture = info.picture;
        else
        {
            Destroy(Picture.gameObject);
            Picture = null;
            GetComponent<RectTransform>().sizeDelta = new Vector2(960, 600);
        }
    }
    public IEnumerator IntroEvent()
    {
        RectTransform rect = transform as RectTransform;
        rect.pivot = new Vector2(0.5f, 0.5f);
        Scale = new Vector2(1.5f, 1.5f);
        float speed = 20f;
        Vector2 pos = Position;
        while(pos.y<0)
        {
            pos.y += speed;
            if (pos.y > 0) pos.y = 0;
            speed += speed / 10f;
            Position = pos;
            yield return new WaitForEndOfFrame();
        }
        speed = 2f * Time.deltaTime;
        while(Scale.x>1)
        {
            SmartPhone.SizeSet(transform, -speed, 1);
            speed += speed / 10f;
            yield return new WaitForEndOfFrame();
        }
        rect.pivot = new Vector2(0.5f, 1f);
    }
    void LikeClick()
    {
        SmartPhone.LoadStage(SmartPhone.MainStage);
    }
}
