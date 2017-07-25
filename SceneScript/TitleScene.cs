using UnityEngine;

/// <summary>
/// 게임 실행시 첫 화면(인트로) 처리 스크립트
/// </summary>
public class TitleScene : MonoBehaviour {
    public CutSceneInfo info;
    private void Start()
    {
        Screen.SetResolution(1080, 1920, false);
        Testing();
    }
    private void Testing()
    {
        for(int i=0;i<info.cuts.Length;i++)
        {
            for (int j = 0; j < info.cuts[i].speech.Length; j++)
                info.cuts[i].speech[j] = "컷신" + (i + 1) + " 대사" + (j + 1);
        }
    }
    public void FirstGo() // 처음하기
    {
        StaticManager.SaveData = new SaveData(true);
        StaticManager.CutScene = info;
        StaticManager.moveScenetoLoading("CutScene");
    }
    public void Continue() // 계속하기
    {
        StaticManager.SaveData = new SaveData(false);
        StaticManager.moveScenetoLoading("MainStage");
    }
    public void Achievements() //업적
    {

    }
}
