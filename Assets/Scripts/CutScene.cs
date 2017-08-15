using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CutScene : MonoBehaviour
{
    private CutSceneInfo sceneinfo;
    public GameObject SpeechPanel;
    private int cutindex = 0; //컷 번호
    private int speechindex = 0; //컷 내에서 대화 번호
    private Book book;
    private AutoFlip flip;
    private Text Speech;
    private void Start()
    {
        book = GetComponent<Book>();
        flip = GetComponent<AutoFlip>();
        Speech = SpeechPanel.GetComponentInChildren<Text>();
        Speech.text = null;
        sceneinfo = GameManager.CutScene;
        book.bookPages = sceneinfo.GetSprites();
        book.RightNext.sprite = book.bookPages[0];
        GameManager.CutScene.Dispose();
        StartCoroutine("Printing");
    }
    private IEnumerator Printing() //대사 출력
    {
        Speech.text = null;
        for (int i = 0; i < sceneinfo.cuts[cutindex].speech[speechindex].Length; i++)
        {
            if (flip.isFlipping) StopCoroutine("Printing");
            Speech.text += sceneinfo.cuts[cutindex].speech[speechindex][i];
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public void Click() //화면 터치시
    {
        if (flip.isFlipping) return;
        StopCoroutine("Printing");
        if (speechindex >= sceneinfo.cuts[cutindex].speech.Length - 1)
        {
            if (cutindex == sceneinfo.cuts.Count - 1)
                SceneManager.LoadScene("Loading");
            else
            {
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
        cutindex++;
        SpeechPanel.SetActive(true);
        speechindex = 0;
        Debug.Log(cutindex + "\n" + sceneinfo.cuts[cutindex].speech.Length);
        if (sceneinfo.cuts[cutindex].speech.Length == 0)
        {
            Speech.text = "컷" + cutindex + ": 대사가 없습니다. 6초후 다음 컷으로 이동합니다";
            Invoke("Click", 6f);
        }
        else
            StartCoroutine("Printing");
    }
}