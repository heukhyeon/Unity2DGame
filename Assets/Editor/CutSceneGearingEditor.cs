using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutSceneGearing))]
public class CutSceneGearingEditor : Editor
{
    string[] scenelist = new string[1];
    List<bool> cutfolds = new List<bool>();
    List<string> fulltexts = new List<string>();
    SerializedProperty sceneindex;
    SerializedProperty cuts;
    private void OnEnable()
    {
        sceneindex = serializedObject.FindProperty("sceneindex");
        cuts = serializedObject.FindProperty("cuts");
        scenelistrefresh();
    }
    public override void OnInspectorGUI()
    {
        if (sceneindex.intValue > scenelist.Length - 1) sceneindex.intValue = EditorGUILayout.Popup("컷씬 종료후 이동씬", 0, scenelist);
        else sceneindex.intValue = EditorGUILayout.Popup("컷씬 종료후 이동 씬", sceneindex.intValue, scenelist);
        cuts.arraySize = EditorGUILayout.DelayedIntField("컷 수", cuts.arraySize);
        if (cutfolds.Count != cuts.arraySize)
        {
            FoldSizeEdit();
        }
        for (int i = 0; i < cuts.arraySize; i++)
        {
            cutfolds[i] = EditorGUILayout.Foldout(cutfolds[i], "컷" + (i + 1));
            if (cutfolds[i])
            {
                SerializedProperty cut = cuts.GetArrayElementAtIndex(i);
                SerializedProperty image = cut.FindPropertyRelative("sprite");
                SerializedProperty speechs = cut.FindPropertyRelative("speech");
                SerializedProperty fulltext = cut.FindPropertyRelative("fulltext");
                image.objectReferenceValue = EditorGUILayout.ObjectField("컷 이미지",image.objectReferenceValue,typeof(Sprite),allowSceneObjects : true);
                speechs.arraySize = EditorGUILayout.DelayedIntField("해당 컷의 대사 수", speechs.arraySize);
                EditorGUILayout.LabelField("해당 컷 대사 전문. 엔터키로 대사 인덱스를 바꿀수있습니다.");
                string temp = EditorGUILayout.TextArea(fulltext.stringValue, GUILayout.Height(100));
                if(temp!=fulltext.stringValue)
                {
                    fulltext.stringValue = temp;
                    FullTexttoSubText(speechs, temp);
                }
                for (int j = 0; j < speechs.arraySize; j++)
                {
                    SerializedProperty speech = speechs.GetArrayElementAtIndex(j);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("대사" + (j + 1),GUILayout.Width(180));
                    string temp2 = speech.stringValue;
                    EditorGUI.BeginChangeCheck();
                    speech.stringValue = EditorGUILayout.TextArea(speech.stringValue, GUILayout.Height(50));
                    if (EditorGUI.EndChangeCheck() && temp2 != speech.stringValue) SubTexttoFullText(speechs,fulltext);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
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
    private void FoldSizeEdit()
    {
        while(cutfolds.Count!=cuts.arraySize)
        {
            if (cutfolds.Count > cuts.arraySize)
            {
                cutfolds.RemoveAt(cutfolds.Count - 1);
                fulltexts.RemoveAt(fulltexts.Count - 1);
            }
            else
            {
                cutfolds.Add(false);
                fulltexts.Add(null);
            }
        }
    }
    private void FullTexttoSubText(SerializedProperty speechs,string fulltext)
    {
        StringReader reader = new StringReader(fulltext);
        string word = reader.ReadLine();
        speechs.arraySize = 0;
        int cnt = 0;
        while (word != null)
        {
            speechs.arraySize += 1;
            speechs.GetArrayElementAtIndex(cnt++).stringValue = word;
            word = reader.ReadLine();
        }
    }
    private void SubTexttoFullText(SerializedProperty speechs,SerializedProperty fulltext)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < speechs.arraySize; i++)
            sb.AppendFormat("{0}\n", speechs.GetArrayElementAtIndex(i).stringValue);
        fulltext.stringValue = sb.ToString();
    }
}

