using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;


[CustomEditor(typeof(MessageIcon))]
public class MessageIconEditor : Editor
{
    
    SerializedProperty infos;
    List<bool> casefolds = new List<bool>();
    List<string> fulltexts = new List<string>();
    List<List<bool>> elementfolds = new List<List<bool>>();
    string[] speechtypes = new string[] { "주인공", "타인" };
    private void OnEnable()
    {
        infos = serializedObject.FindProperty("infos");
    }
    public override void OnInspectorGUI()
    {
        infos.arraySize=EditorGUILayout.DelayedIntField("케이스 수", infos.arraySize);
        if(casefolds.Count!=infos.arraySize)
        {
            while(casefolds.Count != infos.arraySize)
            {
                if (casefolds.Count > infos.arraySize)
                {
                    elementfolds.RemoveAt(elementfolds.Count - 1);
                    casefolds.RemoveAt(casefolds.Count - 1);
                    fulltexts.RemoveAt(fulltexts.Count - 1);
                }
                else
                {
                    elementfolds.Add(new List<bool>());
                    casefolds.Add(false);
                    fulltexts.Add(null);
                }
            }
        }
        if(casefolds.Count!=elementfolds.Count)
        {
            while (elementfolds.Count != casefolds.Count)
            {
                if (casefolds.Count < elementfolds.Count)
                {
                    elementfolds.RemoveAt(elementfolds.Count - 1);
                    fulltexts.RemoveAt(fulltexts.Count - 1);
                }
                else
                {
                    fulltexts.Add(null);
                    elementfolds.Add(new List<bool>());
                }
            }
        }
        if (infos.arraySize == 1) //단일 케이스인경우
            SpeechEdit(infos, 0);
        else if(infos.arraySize>1)//다중 케이스인경우
        {
            for(int i=0;i<infos.arraySize;i++)
            {
                casefolds[i] = EditorGUILayout.Foldout(casefolds[i], "케이스" + (i + 1));
                if (casefolds[i]) SpeechEdit(infos, i);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    
    private void SpeechEdit(SerializedProperty infos,int cnt)
    {
        SerializedProperty speechs = infos.GetArrayElementAtIndex(cnt).FindPropertyRelative("speechs");
        EditorGUILayout.LabelField("대화 전문, 세미콜론으로 화자를, 엔터키로 대사 인덱스를 구분합니다.");
        EditorGUILayout.LabelField("0일시 주인공, 그 이외의 숫자일시 타인으로 취급됩니다");
        SubTexttoFullText(speechs, cnt);
        string temp = EditorGUILayout.TextArea(fulltexts[cnt], GUILayout.Height(100));
        if(temp!=fulltexts[cnt])
        {
            fulltexts[cnt] = temp;
            FullTexttoSubText(speechs, temp);
        }
        speechs.arraySize = EditorGUILayout.DelayedIntField("대화 수", speechs.arraySize);
        if (speechs.arraySize != elementfolds[cnt].Count)
        {
            while (speechs.arraySize != elementfolds[cnt].Count)
            {
                if (speechs.arraySize > elementfolds[cnt].Count) elementfolds[cnt].Add(false);
                else elementfolds[cnt].RemoveAt(elementfolds[cnt].Count - 1);
            }
        }
        for(int i=0;i<elementfolds[cnt].Count;i++)
        {
            SerializedProperty speech = speechs.GetArrayElementAtIndex(i);
            elementfolds[cnt][i] = EditorGUILayout.Foldout(elementfolds[cnt][i], "대사" + (i + 1));
            if(elementfolds[cnt][i])
            {
                SerializedProperty speechtype = speech.FindPropertyRelative("isEnemy");
                SerializedProperty speechword = speech.FindPropertyRelative("speech");
                int temp1 = speechtype.intValue;
                string temp2 = speechword.stringValue;
                EditorGUI.BeginChangeCheck();
                speechtype.intValue = EditorGUILayout.Popup("화자", speechtype.intValue, speechtypes);
                speechword.stringValue = EditorGUILayout.TextField("내용", speechword.stringValue);
                //if (EditorGUI.EndChangeCheck() && (temp1 != speechtype.intValue || !temp2.Equals(speechword.stringValue))) SubTexttoFullText(speechs, cnt);
            }
        }
    }
    private void FullTexttoSubText(SerializedProperty speechs, string fulltext)
    {
        StringReader reader = new StringReader(fulltext);
        string word = reader.ReadLine();
        speechs.arraySize = 0;
        int cnt = 0;
        while (word != null)
        {
            speechs.arraySize += 1;
            int index = -1;
            index = word.IndexOf(":");
            SerializedProperty speech = speechs.GetArrayElementAtIndex(cnt++);
            SerializedProperty speechtype = speech.FindPropertyRelative("isEnemy");
            SerializedProperty speechword = speech.FindPropertyRelative("speech");
            if(index==-1 || index==0)
            {
                speechtype.boolValue = true;
                speechword.stringValue = word;
            }
            else
            {
                int temp = -1;
                if (int.TryParse(word.Substring(0, index), out temp) && temp == 0) speechtype.boolValue = false;
                else speechtype.boolValue = true;
                speechword.stringValue = word.Substring(index + 1);
            }
            word = reader.ReadLine();
        }
    }
    private void SubTexttoFullText(SerializedProperty speechs, int index)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < speechs.arraySize; i++)
        {
            bool isEnemy = speechs.GetArrayElementAtIndex(i).FindPropertyRelative("isEnemy").boolValue;
            int speechtype = isEnemy ? 1 : 0;
            sb.AppendFormat("{0}:{1}\n", speechtype, speechs.GetArrayElementAtIndex(i).FindPropertyRelative("speech").stringValue);
        }
           
        fulltexts[index] = sb.ToString();
    }
}

