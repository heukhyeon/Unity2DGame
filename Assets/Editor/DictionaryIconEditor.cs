﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DictionaryIcon))]
public class DictionaryIconEditor:Editor
{
    public class DictionaryProperty
    {
        public DictionaryAppinfo info;
        public List<QuestProperty> street = new List<QuestProperty>();
        public List<QuestProperty> column = new List<QuestProperty>();
        public List<char[]> answerfield = new List<char[]>();
        public List<int[]> spaces = new List<int[]>();
        public bool fold;
        public bool street_fold;
        public bool column_fold;
        public bool example;
        public bool answer_noinput = true;
        public DictionaryProperty()
        {
            for (int i = 0; i < 10; i++) answerfield.Add(new char[10]);
            for (int i = 0; i < 10; i++) spaces.Add(new int[10]);
        }
    }
    public class QuestProperty
    {
        public DictionaryAppinfo.Quest quest;
        public bool fold;
    }
    List<DictionaryProperty> propertys = new List<DictionaryProperty>();
    DictionaryIcon app;
    int cases;
    bool valuechange = false;
    char empty = '\0';
    GUIStyle wordstyle = new GUIStyle();
    private void OnEnable()
    {
        app = (DictionaryIcon)target;
        if (app.infos == null) app.infos = new DictionaryAppinfo[1];
        wordstyle.normal.background = MakeTex(100,100,new Color(255, 255, 255, 255));
    }
    public override void OnInspectorGUI()
    {
        cases = app.infos.Length;
        ExtendEditor.CreateDelayField(ref cases, "케이스 수");
        ExtendEditor.ArraySizeModify(ref app.infos, cases);
        ExtendEditor.ArraySizeModify(propertys, cases);
        if(cases==1)
        {
            if (propertys[0].info == null)propertys[0].info = app.infos[0];
            CreateProperty(propertys[0]);
        }
        else
        {
            for(int i=0;i<cases;i++)
            {
                if (propertys[i].info == null) propertys[i].info = app.infos[i];
                ExtendEditor.FoldOut(ref propertys[i].fold, "케이스", i + 1, CreateProperty, propertys[i]);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    /// <summary>
    /// 하나의 case에 대한 프로퍼티를 작성.
    /// </summary>
    /// <param name="property"></param>
    void CreateProperty(DictionaryProperty property)
    {
        int street_cnt = 0;
        int column_cnt = 0;
        if(property.info.street==null) property.info.street = new DictionaryAppinfo.Quest[0];
        if(property.info.column==null) property.info.column = new DictionaryAppinfo.Quest[0];
        if (valuechange) AnswerFieldModify(property);
        valuechange = false;
        street_cnt = property.info.street.Length;
        column_cnt = property.info.column.Length;
        ExtendEditor.FoldOut(ref property.example, "미리보기", ShowExampleField, property);
        Action street = new Action(delegate () {
            ExtendEditor.CreateDelayField(ref street_cnt, "가로 문제 수");
            ExtendEditor.ArraySizeModify(ref property.info.street, street_cnt, ref valuechange);
            ExtendEditor.ArraySizeModify(property.street, street_cnt);
            for (int i = 0; i < street_cnt; i++)
            {
                property.street[i].quest = property.info.street[i];
                ExtendEditor.FoldOut(ref property.street[i].fold, "가로 문제", i + 1, CreateQuest, property.street[i].quest);
            }
        });
        Action column = new Action(delegate ()
        {
            ExtendEditor.CreateDelayField(ref column_cnt, "세로 문제 수");
            ExtendEditor.ArraySizeModify(ref property.info.column, column_cnt, ref valuechange);
            ExtendEditor.ArraySizeModify(property.column, column_cnt);
            for (int i = 0; i < column_cnt; i++)
            {
                property.column[i].quest = property.info.column[i];
                EditorGUI.indentLevel++;
                ExtendEditor.FoldOut(ref property.column[i].fold, "세로 문제", i + 1, CreateQuest, property.column[i].quest);
                EditorGUI.indentLevel--;
            }
        });
        ExtendEditor.FoldOut(ref property.street_fold, "가로 문제", street);
        ExtendEditor.FoldOut(ref property.column_fold, "세로 문제", column);
        if (property.answer_noinput)
        {
            property.answer_noinput = false;
            AnswerFieldModify(property);
        }
    }
    /// <summary>
    /// 하나의 질문(+해답)에 대한 프로퍼티를 작성.
    /// </summary>
    /// <param name="quest"></param>
    void CreateQuest(DictionaryAppinfo.Quest quest)
    {
        string answer = quest.answer;
        int pos_x = quest.pos_x;
        int pos_y = quest.pos_y;
        EditorGUI.BeginChangeCheck();
        ExtendEditor.CreateField(ref quest.index, "번호");
        ExtendEditor.CreateField(ref quest.answer, "정답");
        ExtendEditor.CreateField(ref quest.hint, "힌트");
        ExtendEditor.CreateField(ref quest.pos_x, "가로 위치");
        ExtendEditor.CreateField(ref quest.pos_y, "세로 위치");
        if(EditorGUI.EndChangeCheck())
            valuechange = answer.Equals(quest.answer) && pos_x.Equals(quest.pos_x) && pos_y.Equals(quest.pos_y) ? valuechange : true;
    }
    /// <summary>
    /// 정답 2차원 배열 (Answers)를 수정한다.
    /// </summary>
    /// <param name="property"></param>
    void AnswerFieldModify(DictionaryProperty property)
    {
        for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) property.answerfield[i][j] = '\0';
        for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) property.spaces[i][j] = -1;
        foreach (DictionaryAppinfo.Quest quest in property.info.street)
        {
            property.spaces[quest.pos_y][quest.pos_x] = quest.index;
            for (int i = 0; i < quest.answer.Length; i++)
            {
                property.answerfield[quest.pos_y][quest.pos_x + i] = quest.answer[i];
                if(i>0)property.spaces[quest.pos_y][quest.pos_x + i] = 0;
            }
        }
        foreach (DictionaryAppinfo.Quest quest in property.info.column)
        {
            property.spaces[quest.pos_y][quest.pos_x] = quest.index;
            for (int i = 0; i < quest.answer.Length; i++)
            {
                property.answerfield[quest.pos_y + i][quest.pos_x] = quest.answer[i];
                if (i > 0) property.spaces[quest.pos_y+i][quest.pos_x] = 0;
            }
        }
        for(int i=0;i<10;i++)for(int j=0; j<10;j++)
            {
                property.info.answers[i, j] = property.answerfield[i][j];
                property.info.space[i, j] = property.spaces[i][j];
            }
    }
    /// <summary>
    /// 미리보기 활성화시 미리보기를 보여준다.
    /// </summary>
    /// <param name="property"></param>
    void ShowExampleField(DictionaryProperty property)
    {
        Rect pos = EditorGUILayout.GetControlRect();
        pos.x += 75;
        pos.y -= 20;
        GUILayout.Space(170); //충분한 여백을 확보한다.
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                Rect rectangle = new Rect(pos.x + (j * 20f), pos.y + (i * 20f), 20, 20);
                Rect textangle = new Rect(rectangle.x- EditorGUI.indentLevel*15, rectangle.y, 35, 20); //GetControlRect는 IndenetLevel이 섞일경우 제대로된 값이 나오지않으므로 수정.
                if (property.answerfield[i][j] == empty) EditorGUI.DrawRect(rectangle, Color.yellow); //입력되지않은공간인경우 노란 사각형을
                else EditorGUI.LabelField(textangle, " "+property.answerfield[i][j].ToString(), wordstyle);//입력이 된경우 배경이 하얀 라벨을 만든다.
            }
    }
    Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
