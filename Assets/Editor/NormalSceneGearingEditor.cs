using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(NormalSceneGearing))]
public class NormalSceneGearingEditor : Editor
{

    string[] scenelist = new string[1];
    private void OnEnable()
    {
        scenelistrefresh();
    }
    public override void OnInspectorGUI()
    {
        SerializedProperty sceneindex = serializedObject.FindProperty("sceneindex");
        if (sceneindex.intValue > scenelist.Length - 1) sceneindex.intValue = EditorGUILayout.Popup("이동 씬", 0, scenelist);
        else sceneindex.intValue = EditorGUILayout.Popup("이동 씬", sceneindex.intValue, scenelist);
        serializedObject.ApplyModifiedProperties();
    }
    private void scenelistrefresh()
    {
        if (scenelist.Length == EditorBuildSettings.scenes.Length) return;
        List<string> temp = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            if (scene.enabled)
            {
                string name = scene.path.Substring(scene.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                temp.Add(name);
            }
        scenelist = temp.ToArray();
    }
}
