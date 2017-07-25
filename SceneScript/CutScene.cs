using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class CutScene : MonoBehaviour
{
    private CutSceneInfo sceneinfo;
    public GameObject SpeechPanel;
    private bool isFliping = false; //페이지 넘기는중
    private int cutindex = 0; //컷 번호
    private int speechindex = 0; //컷 내에서 대화 번호
    private string next = null; //컷씬 종료후 다음씬으로 갈 이름
    public Book book;
    public AutoFlip flip;
    private Text Speech;
    private void Start()
    {
        Speech = SpeechPanel.GetComponentInChildren<Text>();
        Speech.text = null;
        sceneinfo = StaticManager.CutScene;
        book.bookPages = sceneinfo.GetSprites();
        book.RightNext.sprite = book.bookPages[0];
        next = sceneinfo.nextscene;
        StaticManager.CutScene.Dispose();
        StartCoroutine("Printing");
    }
    private IEnumerator Printing() //대사 출력
    {
        Speech.text = null;
        for (int i = 0; i < sceneinfo.cuts[cutindex].speech[speechindex].Length; i++)
        {
            if (isFliping) yield break;
            Speech.text += sceneinfo.cuts[cutindex].speech[speechindex][i];
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public void Click() //화면 터치시
    {
        if (isFliping) return;
        StopCoroutine("Printing");
        if (speechindex >= sceneinfo.cuts[cutindex].speech.Length - 1)
        {
            if (cutindex == sceneinfo.cuts.Length - 1)
                StaticManager.moveScenetoLoading(next);
            else
            {
                isFliping = true;
                SpeechPanel.SetActive(false);
                flip.FlipRightPage();
            }
        }
        else
        {
            speechindex++;
            StartCoroutine("Printing");
        }
    }
    public void FlipComplete() //책넘기기 효과가 끝났을때
    {
        isFliping = false;
        cutindex++;
        SpeechPanel.SetActive(true);
        speechindex = 0;
        StartCoroutine("Printing");
    }
}