using UnityEngine;

/// <summary>
/// 게임 실행시 첫 화면(인트로) 처리 스크립트
/// </summary>
public class TitleScene : MonoBehaviour {

    private void Start()
    {
        Screen.SetResolution(1080, 1920, false);
    }
    public void FirstGo() // 처음하기
    {
        StaticManager.SaveData = new SaveData(true);
        StaticManager.moveScenetoLoading("MainStage");
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
