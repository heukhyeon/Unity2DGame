using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GalleryIcon : MonoBehaviour, AppIcon
{
    public GalleryAppInfo[] infos = new GalleryAppInfo[0];
    public void Select()
    {
        GameManager.appinfo = infos[UnityEngine.Random.Range(0, infos.Length)];
        GameManager.speech = GetComponent<PlayerSpeechScript>();
        GetComponent<SceneWarp>().goScene();
    }
}


[Serializable]
public class GalleryAppInfo
{
    public Texture2D image;
    public int PuzzleNumber;

}
