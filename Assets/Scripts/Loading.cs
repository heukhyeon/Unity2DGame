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
        async = SceneManager.LoadSceneAsync(SmartPhone.NextScene);
        async.allowSceneActivation = false;
        float delay = 0;
        float step = (1000 / LoadingDelay) * Time.deltaTime;
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

