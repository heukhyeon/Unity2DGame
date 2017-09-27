using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField]
    AudioSource background;
    public RectTransform FadeCircle; //시작버튼 클릭시 줄어들 원
    public bool isClick = false;
    public float fadespeed = 200f;
    public Camera scenecamera;
    public Material NormalMaterial; //마우스를 올리지않았을때 색깔
    public Material OverMaterial; //마우스를 올렸을때 색깔
    public bool LoadingIgnore = false; //테스트용으로 시작한경우, 로딩시간을 무시한다.
    private SpriteRenderer buttoncolor;
    private void Start()
    {
        Screen.SetResolution(1080, 1920, true);
        buttoncolor = GetComponent<SpriteRenderer>();
    }
    private void OnMouseOver()
    {
        if (isClick) return;
        buttoncolor.material = OverMaterial;
    }
    private void OnMouseExit()
    {
        if (isClick) return;
        buttoncolor.material = NormalMaterial;
    }
    private void OnMouseDown()
    {
        if (!isClick)
        {
            isClick = true;
            StartCoroutine(StartEvent());
        }
    }
    IEnumerator StartEvent()
    {
        GetComponent<AudioSource>().Play();
        background.Stop();
        while (scenecamera.orthographicSize < 10)
        {
            scenecamera.orthographicSize += 10f * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(0.1f);
        Vector2 size = FadeCircle.sizeDelta;
        while (true)
        {
            scenecamera.orthographicSize -= 10f * Time.deltaTime;
            size = new Vector2(size.x - fadespeed, size.x - fadespeed);
            FadeCircle.sizeDelta = size;
            if (size.x < 0) break;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        SmartPhone.LoadStage("CutScene");
    }
}
