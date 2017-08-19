#pragma warning disable 0649
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 컷씬으로 이동하는 경우에 대한 클래스. 하나의 씬에는 SceneWarp와 CutSceneGearing중 어느 하나만이 있어야한다.
/// </summary>
public class CutSceneGearing:MonoBehaviour,SceneWarp
 {
    [SerializeField]
    private CutInfo[] cuts;
    [SerializeField]
    private int sceneindex;
    public void goScene()
    {
        GameManager.CutScene.Dispose();
        CutSceneInfo info;
        info.cuts = new List<CutInfo>();
        info.goCutscene = true;
        for (int i = 0; i < cuts.Length; i++)
            info.cuts.Add(cuts[i]);
        GameManager.CutScene = info;
        GameManager.NextScene = sceneindex;
        SceneManager.LoadScene("Loading");
    }
}

