using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerSpeechScript))]
public class PlayerSpeechScriptEditor:Editor
{
    bool introfold = false;
    bool failfold = false;
    bool clearfold = false;
    bool gameoverfold = false;
    string[] fulltexts = new string[4];
    string[] smilelist = new string[] { "화남", "공포", "평범", "슬픔", "놀람" }; 
    SerializedProperty intro;
    SerializedProperty fail;
    SerializedProperty clear;
    SerializedProperty gameover;
    List<bool> introelementfolds = new List<bool>();
    List<bool> failelementfolds = new List<bool>();
    List<bool> clearelementfolds = new List<bool>();
    List<bool> gameoverelementfolds = new List<bool>();
    private void OnEnable()
    {

        intro = serializedObject.FindProperty("intro");
        fail = serializedObject.FindProperty("fail");
        clear = serializedObject.FindProperty("clear");
        gameover = serializedObject.FindProperty("gameover");
    }
    public override void OnInspectorGUI()
    {
        introfold = EditorGUILayout.Foldout(introfold, "진입 대사");
        if (introfold) SpeechDraw(intro, introelementfolds,0);
        failfold = EditorGUILayout.Foldout(failfold, "실패 대사");
        if (failfold) SpeechDraw(fail, failelementfolds,1);
        clearfold = EditorGUILayout.Foldout(clearfold, "클리어 대사");
        if (clearfold) SpeechDraw(clear, clearelementfolds,2);
        gameoverfold = EditorGUILayout.Foldout(gameoverfold, "게임오버 대사");
        if (gameoverfold) SpeechDraw(gameover, gameoverelementfolds,3);
        serializedObject.ApplyModifiedProperties();
    }
    /// <summary>
    /// 대사 작성창을 그린다.
    /// </summary>
    /// <param name="property">인트로,실패,클리어,게임오버등의 대사 이벤트 발생요소 </param>
    /// <param name="elementfolds">하위 대사들의 숨김여부를 관리하는 배열 </param>
    /// <param name="index">전체 문장 관리를 위한 index </param>
    private void SpeechDraw(SerializedProperty property,List<bool>elementfolds,int index)
    {
        string fulltext = fulltexts[index];
        EditorGUILayout.LabelField("대사 전문, 세미콜론으로 표정과 대사를 구분합니다.");
        EditorGUILayout.LabelField("표정: 0=화남,1=공포,2=평범,3=슬픔,4=놀람, 숫자가 아니거나 세미콜론없을시 0고정");
        fulltext = EditorGUILayout.TextArea(fulltext, GUILayout.Height(50));
        if(fulltext!=fulltexts[index])//내용이 변경된경우
        {
            fulltexts[index] = fulltext;
            FulltoSubText(property, fulltext);
        }
        property.arraySize = EditorGUILayout.DelayedIntField("대사 수",property.arraySize);//대사 수 결정하는 IntField
        if (!property.arraySize.Equals(elementfolds.Count))//사이즈가 변동된경우
        {
            while(!property.arraySize.Equals(elementfolds.Count))
            {
                if (property.arraySize > elementfolds.Count)
                    elementfolds.Add(true);
                else
                    elementfolds.RemoveAt(elementfolds.Count - 1);
            }
        }
        for(int i=0;i<property.arraySize;i++)
        {
            
            elementfolds[i] = EditorGUILayout.Foldout(elementfolds[i], "대사" + (i + 1));
            if(elementfolds[i])
            {
                SerializedProperty speech = property.GetArrayElementAtIndex(i); //하나의 대사에 대한 프로퍼티
                SerializedProperty smile = speech.FindPropertyRelative("imageindex"); //대사의 표정을 관리하는 int값
                SerializedProperty text = speech.FindPropertyRelative("text"); //대사의 내용을 관리하는 string값
                smile.intValue = EditorGUILayout.Popup("표정", smile.intValue, smilelist);
                string temp = text.stringValue;
                EditorGUI.BeginChangeCheck();
                text.stringValue = EditorGUILayout.TextField("대사", text.stringValue);
                if (EditorGUI.EndChangeCheck() && !temp.Equals(text.stringValue)) SubtoFullText(property,index);
            }
        }
    }
    /// <summary>
    /// 전문이 변동되었을시, 전문의 내용에 따라 대사 수, 값등을 바꾼다
    /// </summary>
    /// <param name="property">인트로,실패,클리어,게임오버등의 대사 이벤트 발생요소</param>
    /// <param name="fulltext">전문 내용</param>
    private void FulltoSubText(SerializedProperty property,string fulltext)
    {
        StringReader reader = new StringReader(fulltext);
        string word = reader.ReadLine();
        property.arraySize = 0;
        int index = 0;
        while(word!=null)
        {
            property.arraySize += 1;
            SerializedProperty speech = property.GetArrayElementAtIndex(index++);
            SerializedProperty smile = speech.FindPropertyRelative("imageindex");
            SerializedProperty text = speech.FindPropertyRelative("text");
            int smileindex = 0;
            int semiindex = word.IndexOf(":");
            if(semiindex!=-1) //세미콜론 인덱스가 존재하는경우
            {
                if(int.TryParse(word.Substring(0,semiindex),out smileindex))
                {
                    smile.intValue = smileindex;
                    text.stringValue = word.Substring(semiindex + 1);
                }
                else
                {
                    smile.intValue = 0;
                    text.stringValue= word.Substring(semiindex + 1);
                }
            }
            else //세미콜론 인덱스가 없는경우=세미콜론이없는경우
            {
                smile.intValue = 0;
                text.stringValue = word;
            }
            word = reader.ReadLine();
        }
    }
    /// <summary>
    /// 하위 내용이 변동되었을시, 전문을 수정한다. 이것으로 인한 FulltoSubText는 발생하지않는다.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="index"></param>
    private void SubtoFullText(SerializedProperty property,int index)
    {
        StringBuilder sb = new StringBuilder();
        for(int i=0;i<property.arraySize;i++)
        {
            SerializedProperty speech = property.GetArrayElementAtIndex(i);
            SerializedProperty smile = speech.FindPropertyRelative("imageindex");
            SerializedProperty text = speech.FindPropertyRelative("text");
            sb.AppendFormat("{0}:{1}\n", smile.intValue, text.stringValue);
        }
        fulltexts[index] = sb.ToString();
    }
}

