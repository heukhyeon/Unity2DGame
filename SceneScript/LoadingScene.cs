#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingScene : MonoBehaviour {

    public Text Tip;
    public RectTransform Loadingbar;
    public RectTransform Loadingvalue;
    public float mincnt = 1.5f; //최소 로딩시간
    public string[] TipArray;
    private bool isLoad = false;
    private AsyncOperation sync;
    private Vector2 valuevector;
    private float maxwidth; // 로딩바의 길이. 로딩시 value에 % 계산.
    private float cnt = 0; //경과 시간
    private void Start()
    {
        Tip.text = "팁 : " + TipArray[UnityEngine.Random.Range(0, TipArray.Length - 1)]; //무작위 팁 출력
        valuevector = Loadingbar.sizeDelta;
        Loadingvalue.sizeDelta = new Vector2(0, Loadingvalue.sizeDelta.y);
        maxwidth = valuevector.x;
        StartCoroutine("Loading");
    }
    private void Update()
    {
        cnt += Time.deltaTime;
        if (valuevector.x/maxwidth == 1f) sync.allowSceneActivation = true;
    }
    private IEnumerator Loading()
    {
        if(!isLoad) //중복 실행 방지
        {
            isLoad = true;
            sync = StaticManager.moveScenefromLoading(); //다음 씬 이름은 로딩씬을 불러왔던 씬에서 지정한걸 정적 매니저에서 반환만 하는 방식.
            sync.allowSceneActivation = false; //즉각 로딩을 차단한다.
            while(!sync.isDone)
            {
                float x_value = Loadingvalue.sizeDelta.x + maxwidth / 100f;
                if (x_value / maxwidth > sync.progress && sync.progress<0.9f) valuevector.x = maxwidth * sync.progress;
                else if(Loadingvalue.sizeDelta.x<maxwidth) valuevector.x = x_value;
                Loadingvalue.sizeDelta = valuevector;
                yield return true;
            }
        }
    }
}
