#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    [SerializeField]
    RawImage now;
    [SerializeField]
    RawImage next;
    Ending.CutInfo[] cutinfo;
    AudioSource effect;
    int index = 0;
    bool input = false;
    private void Start()
    {
        effect = GetComponent<AudioSource>();
        cutinfo = SmartPhone.GetData<Ending>().infos;
        now.texture = cutinfo[0].image;
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (input && index < cutinfo.Length - 1) StartCoroutine(ImageShow());
            else if (input) Application.Quit();
            else input = true;
        }
    }
    IEnumerator ImageShow()
    {
        effect.Play();
        index++;
        next.texture = cutinfo[index].image;
        Vector2 backup = next.transform.position;
        StartCoroutine(Routine1());
        yield return StartCoroutine(Routine2());
        RectTransform NextTr = next.rectTransform;
        Vector2 scale = NextTr.localScale;
        float speed = Time.deltaTime;
        while (scale.x < 1)
        {
            scale.x += speed;
            scale.y += speed;
            speed += speed / 10f;
            NextTr.localScale = scale;
            yield return new WaitForEndOfFrame();
        }
        var temp = now;
        now = next;
        next = temp;
        next.transform.position = backup;
        next.transform.localScale = new Vector2(0.9f, 0.9f);
    }
    IEnumerator Routine1()
    {
        RectTransform NowTr = now.rectTransform;
        Vector2 pos = NowTr.position;
        float speed = 10f * Time.deltaTime;
        while (pos.x > -600)
        {
            pos.x -= speed;
            speed += speed / 3f;
            NowTr.position = pos;
            if (pos.x > -600) yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator Routine2()
    {
        RectTransform NextTr = next.rectTransform;
        Vector2 nextpos = NextTr.position;
        float speed = 10f * Time.deltaTime;
        while (nextpos.y < 960)
        {
            nextpos.y += speed;
            if (nextpos.y > 960) nextpos.y = 960;
            speed += speed / 3f;
            NextTr.position = nextpos;
            yield return new WaitForEndOfFrame();
        }
    }
}

