using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
[CustomEditor(typeof(Ending))]
public class EndingDataEditor : Editor {

    string path = "Assets/Images/Ending";
    List<Texture> list;
    Ending app;
    private void OnEnable()
    {
        list = new List<Texture>();
        app = (Ending)target;
        string[] paths = Directory.GetFiles(path);
        foreach (var item in paths) if (Path.GetExtension(item) == ".tga") list.Add(AssetDatabase.LoadAssetAtPath<Texture>(item));
        foreach (var item in list) if (item == null) list.Remove(item);
        app.infos = new Ending.CutInfo[list.Count];
        for (int i = 0; i < app.infos.Length; i++) app.infos[i].image = list[i];
    }
}
