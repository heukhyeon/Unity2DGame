using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public string[] tips;
    public RectTransform value;
    public float LoadingDelay = 3f;
    private bool isLoading = false;
    private Vector2 size;
    // Use this for initialization
    void Start()
    {
        //중복 스테이지 호출 방지
        if (!isLoading)
        {
            size = value.sizeDelta;
            size.x = 0;
            value.sizeDelta = size;
            isLoading = true;
            StartCoroutine(LoadingAsync());
        }
    }
    IEnumerator LoadingAsync()
    {
        AsyncOperation async;
        //다음 씬을 동기화해서 불러옴.
        async = SceneManager.LoadSceneAsync(SmartPhone.NextScene);
        async.allowSceneActivation = false;//로딩 완료시 바로 가지 않게끔 함.
        float delay = 0;
        float step = (1000 / LoadingDelay) * Time.deltaTime;
        //로딩이 완료됬고, 로딩바가 완전히 찰때까지 반복.
        while (!(async.isDone&&size.x>1000))
        {
            if (async.allowSceneActivation == false && delay >= LoadingDelay) async.allowSceneActivation = true;
            else if (delay < LoadingDelay) delay += Time.deltaTime;
            float onestep = size.x + step;
            if (onestep / 1000 > async.progress && async.progress < 0.9f) size.x = 1000 * async.progress;
            else if (size.x < 1000) size.x = onestep;
            value.sizeDelta = size;
            yield return new WaitForEndOfFrame();
        }
    }
}

