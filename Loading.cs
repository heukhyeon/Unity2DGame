﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

/// <summary>
/// 로딩 씬
/// </summary>
public class Loading : MonoBehaviour {
    [SerializeField]
    private float minTime = 3f; //로딩씬이 유지되는 최소 시간
    [SerializeField]
    private Slider sliderbar = null; //하단 슬라이더바
    [SerializeField]
    private Text Tip = null; //상단 팁 텍스트
    [SerializeField]
    private Transform wheel = null;//중앙 회전 이미지
    [SerializeField]
    private string[] Tips = null;//팁 텍스트 모음
    private bool isLoad = false; //중복 실행 방지
    private float timer = 0; //시간 측정
    private Vector3 wheeleuler;//중앙 회전 이미지 오일러 각도측정용
    AsyncOperation async;
    // Use this for initialization
    private void Awake()
    {
        int loc = UnityEngine.Random.Range(0, Tips.Length - 1); //배열내에서 무작위로 인덱스를 얻는다.
        StringBuilder sb = new StringBuilder("팁 : ");
        sb.Append(Tips[loc]); //배열 내 무작위 요소를 출력한다.
        Tip.text = sb.ToString();
    }
    void Start () {
        StartCoroutine("LoadingScene");
	}
    private void Update()
    {
        wheeleuler = wheel.rotation.eulerAngles;
        wheeleuler.z -= 3f;
        wheel.rotation = Quaternion.Euler(wheeleuler);
        timer += Time.deltaTime;
        if (timer > minTime) async.allowSceneActivation = true;
    }
    IEnumerator LoadingScene()
    {
        if(isLoad==false)
        {
            isLoad = true;
            async = SceneManager.LoadSceneAsync(SceneObjectManager.NextScene);
            SceneObjectManager.NextScene = null; //다시 다음씬이 지정될때까지 null로 둔다.
            async.allowSceneActivation = false; //다음 씬의 준비가 완료되더라도 바로 로딩되는걸 막는다.
            while (async.progress<1f)
            {
                sliderbar.value = async.progress;
                yield return true;
            }
        }
    }
}
