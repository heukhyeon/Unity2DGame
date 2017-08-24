using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour {

    public enum PlayCut { Prologue,Message };
    public Sprite[] prologue;
    public Sprite[] messagein;
    public Image Cut;
    public PlayCut kind;
    public MessageIcon messageicon;
    public GameObject PlayUI;
    Sprite[] Nowcuts; //플레이하는 종류에 따라 선정된 현재 컷씬모음
    Image background;
    int index = 0;
    bool isGetdark = false;
    bool isMove = false;
    private void Start()
    {
        background = GetComponent<Image>();
        messageicon.stage = this;
        if (!GameManager.prologue.isPlayIntro) CutStart(PlayCut.Prologue);
        else gameObject.SetActive(false);
    }
    IEnumerator BackgroundGetDark()
    {
        Color color = background.color;
        while (color.a < 0.5)
        {
            color.a += Time.deltaTime;
            background.color = color;
            yield return new WaitForEndOfFrame();
        }
        isGetdark = true;
    }
    IEnumerator AppearCut()
    {
        isMove = true;
        if (!isGetdark) yield return StartCoroutine("BackgroundGetDark");
        Vector2 pos = new Vector2(-920, 0);
        Cut.sprite = Nowcuts[index++];
        Cut.transform.localPosition = pos;
        while(pos.x<0)
        {
            pos.x += 75;
            Cut.transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.3f); // 다음 컷으로 넘어가기 위한 최소 대기시간
        isMove = false;
    }
    IEnumerator DisAppearCut()
    {
        isMove = true;
        Vector2 pos = Cut.transform.localPosition;
        while(pos.x<920)
        {
            pos.x += 50;
            Cut.transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        if(index>=Nowcuts.Length)
            switch (kind)
            {
                case PlayCut.Prologue:
                    GameManager.prologue.isPlayIntro = true;
                    background.color = new Color(0, 0, 0, 0);
                    PlayUI.SetActive(true);
                    this.gameObject.SetActive(false);
                    break;
                case PlayCut.Message:
                    GameManager.prologue.isPlayMessage = true;
                    messageicon.Select();
                    break;
            }
        else StartCoroutine("AppearCut");
        yield return new WaitForSeconds(0.3f); // 다음 컷으로 넘어가기 위한 최소 대기시간
        isMove = false;
    }
    public void CutStart(PlayCut cuttype)
    {
        StopAllCoroutines();
        index = 0;
        kind = cuttype;
        isGetdark = false;
        PlayUI.SetActive(false);
        gameObject.SetActive(true);
        switch (kind)
        {
            case PlayCut.Prologue:
                Nowcuts = prologue;
                break;
            case PlayCut.Message:
                Nowcuts = messagein;
                break;
            default:
                Nowcuts = new Sprite[0];
                break;
        }
        StartCoroutine("AppearCut");
    }
    public void Click()
    {
        if (isMove || index>Nowcuts.Length) return;
        StartCoroutine("DisAppearCut");
    }
}
