using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CutSceneInfo : IDisposable //컷신에 사용될 구조체. 프롤로그/중간씬 호출 객체 /엔딩 객체(아마도 설정어플)에 탑재되 필요시 정적 매니저 객체에 옮기는 방식.
{
    public List<CutInfo> cuts;
    public bool goCutscene; //로딩에서 다음씬을 컷씬으로 재생할지 여부.
    public Sprite[] GetSprites()
    {
        int cnt = cuts.Count;
        Sprite[] ret = new Sprite[cnt];
        for (int i = 0; i < cnt; i++)
            ret[i] = cuts[i].sprite;
        return ret;
    }
    public void Dispose()
    {
        if(cuts!=null)
        {
            foreach (CutInfo cut in cuts)
                cut.Dispose();
        }
        goCutscene = false;
    }
}
[Serializable]
public struct CutInfo : IDisposable
{
    public Sprite sprite;
    public string[] speech;
    public string fulltext;
    public void Dispose()
    {
        sprite = null;
        speech = null;
        fulltext = null;
    }
}

public interface SceneWarp
{
    void goScene();
}

