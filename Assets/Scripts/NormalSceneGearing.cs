using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 로딩씬을 제외한 나머지 씬에서 다른 씬으로의 이동이 발생할때 사용.
/// </summary>
public class NormalSceneGearing : MonoBehaviour,SceneWarp {

    [SerializeField]
    private int sceneindex;

    public void goScene()
    {
        GameManager.NextScene = sceneindex;
        SceneManager.LoadScene("Loading");
    }
}
