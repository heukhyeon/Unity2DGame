#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SNSScene : MonoBehaviour {

    [SerializeField]
    GameObject snsblock;
    [SerializeField]
    RectTransform ViewPort;
    [SerializeField]
    RectTransform Title;
    [SerializeField]
    RectTransform content;
    [SerializeField]
    SNSLoading loading;
    [SerializeField]
    Scrollbar bar;
    private void Start()
    {
        ViewPort.gameObject.SetActive(false);
        Title.gameObject.SetActive(false);
        StartCoroutine(IntroEvent());
    }
    IEnumerator IntroEvent()
    {
        SNS sns = SmartPhone.GetData<SNS>();
        SNSBlock[] blocks = new SNSBlock[sns.infos.Length];
        for (int i=0;i<blocks.Length;i++)
        {
            blocks[i]= Instantiate(snsblock, this.transform, false).GetComponent<SNSBlock>();
            blocks[i].Position = new Vector2(0, -2000);
            blocks[i].Set(sns.infos[i]);
        }
        loading.gameObject.SetActive(true);
        yield return StartCoroutine(loading.IntroEvent());
        ViewPort.gameObject.SetActive(true);
        Title.gameObject.SetActive(true);
        for (int i=0;i<blocks.Length;i++)
        {
            SNSBlock block = blocks[i];
            int val = block.ImageEnable == true ? 1350 : 650;
            yield return StartCoroutine(block.IntroEvent());
            block.transform.SetParent(content);
            if (i == 0) block.Position = Vector2.zero;
            else block.Position = new Vector2(0, blocks[i - 1].Nextpos);
            content.sizeDelta = new Vector2(1000, i==0?val:-blocks[i-1].Nextpos+650);
            yield return new WaitForEndOfFrame();
            while (bar.value > 0)
            {
                bar.value -= 10f* Time.deltaTime;
                if (bar.value < 0) bar.value = 0;
                if(bar.value>0) yield return new WaitForEndOfFrame();
            }
        }
    }
}
