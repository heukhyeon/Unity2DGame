#pragma warning disable 0649
using UnityEngine;
using UnityEngine.SceneManagement;
public static class StaticManager
{
    private static string nextscene;
    private static string savepath;
    private static AppSpeechArray appspeecharray;
    public static AppInfo Nowappinfo;
    public static string SavePath
    {
        get { return savepath; }
    }
    public static SaveData SaveData;
    public static AppSpeechArray AppSpeechArray
    {
        get { return appspeecharray; }
        set { appspeecharray = value; }
    }
#region 로딩, 씬 이동 관련
    /// <summary>
    /// 일반씬 -> 로딩 -> 일반씬에서 지정한 스테이지로 이동
    /// </summary>
    /// <param name="scenename"> BuildSetting에서 지정된 씬 이름</param>
    public static void moveScenetoLoading(string scenename)
    {
        nextscene = scenename;
        SceneManager.LoadScene("Loading");
    }
    /// <summary>
    /// 로딩씬에서 지정된 다음 스테이지로 이동
    /// </summary>
    /// <returns>moveScenetoLoading에서 지정된 씬</returns>
    public static AsyncOperation moveScenefromLoading()
    {
        return SceneManager.LoadSceneAsync(nextscene);
    }
#endregion

}
