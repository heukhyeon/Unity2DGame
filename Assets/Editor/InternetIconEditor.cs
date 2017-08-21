using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

[CustomEditor(typeof(InternetIcon))]
public class InternetIconEditor:Editor
{
    public class InternetProperty
    {
        public InternetAppInfo info;
        public int index;
        public bool fold;
        public bool[] elementfold = new bool[5]; // 정답, 연관 검색어, 사이트, 지식인, 사전
        public bool[] fold_site = new bool[1];
        public bool[] fold_knowins = new bool[1];
        public bool[] fold_encyclopedia = new bool[1];
        public string answer_fulltext = "";
        public string relation_fulltext = "";
    }
    InternetIcon app;
    List<InternetProperty> propertys = new List<InternetProperty>();
    private void OnEnable()
    {
        app = (InternetIcon)target;
        if (app.infos == null) app.infos = new InternetAppInfo[1];
        PropertyListModify(app.infos.Length);
    }
    public override void OnInspectorGUI()
    {
        int temp = EditorGUILayout.DelayedIntField("케이스 수", app.infos.Length);
        if (temp != app.infos.Length || app.infos.Length != propertys.Count) PropertyListModify(temp);
        if (temp == 1) AppInfoCreate(propertys[0]);
        else for (int i = 0; i < app.infos.Length; i++)
            {
                propertys[i].fold = EditorGUILayout.Foldout(propertys[i].fold, "케이스" + (i + 1));
                if (propertys[i].fold)
                {
                    EditorGUI.indentLevel++;
                    AppInfoCreate(propertys[i]);
                    EditorGUI.indentLevel--;
                }
            }

    }
    private void PropertyListModify(int size)
    {
        InternetAppInfo[] infos = new InternetAppInfo[size];
        for (int i = 0; i < app.infos.Length; i++)
        {
            if (i == size) break;
            infos[i] = app.infos[i];
        }
        app.infos = infos;
        List<InternetProperty> temp = new List<InternetProperty>(); //작업 완료후 propertys에 옮길 임시 배열
        for(int i=0;i<app.infos.Length;i++)
        {
            InternetProperty property = new InternetProperty();
            property.info = app.infos[i];
            property.index = i;
            if (i < propertys.Count) property.fold = propertys[i].fold; //기존에 있던 숨김처리를 따라감
            else property.fold = true; // 신규추가인경우 자동으로 비숨김 처리
            temp.Add(property);
        }
        propertys = temp;
    }
    private void AppInfoCreate(InternetProperty property)
    {
        StringPropertyModify(ref app.infos[property.index].answers, ref property.answer_fulltext,ref property.elementfold,0,"정답");
        StringPropertyModify(ref app.infos[property.index].relations, ref property.relation_fulltext, ref property.elementfold, 1, "연관 검색어");
        SitePropertyModify(ref app.infos[property.index].sites,property);
        KnowinPropertyModify(ref app.infos[property.index].knowins, property);
        EncyclopediaPropertyModify(ref app.infos[property.index].encyclopedias, property);
    }
    private void ArraySizeModify<T>(ref T[] array, ref bool[] folds,string name)
    {
        int size = EditorGUILayout.DelayedIntField(name+" 수", array.Length);
        if (size != array.Length)
        {
            T[] temparray = new T[size];
            for (int i = 0; i < array.Length; i++)
            {
                if (i == temparray.Length) break;
                temparray[i] = array[i];
            }
            array = temparray;
        }
        if(size!=folds.Length)
        {
            bool[] tempfold = Enumerable.Repeat(true, size).ToArray();
            for (int i = 0; i < folds.Length; i++)
            {
                if (i == tempfold.Length) break;
                tempfold[i] = folds[i];
            }
            folds = tempfold;
        }
    }
    private void StringPropertyModify(ref string[] array,ref string fulltext,ref bool[] elementfold,int index, string name)
    {
        elementfold[index] = EditorGUILayout.Foldout(elementfold[index], name);
        if (!elementfold[index]) return;
        //펼쳐졌을때만 출력
        EditorGUI.indentLevel++;
        if (fulltext == "") SubtoFullText(ref fulltext, array);
        EditorGUILayout.LabelField(name+" 리스트. 쉼표(,)로 구분합니다.");
        StringBuilder sb = new StringBuilder("현재 ");
        sb.AppendFormat("{0} 리스트:", name);
        if (array.Length==0) sb.Append("없음");
        for (int i = 0; i < array.Length; i++)
        {
            if (i == array.Length - 1) sb.Append(array[i]);
            else sb.AppendFormat("{0}/", array[i]);
        }
        EditorGUILayout.LabelField(sb.ToString()); //정답 표시
        string temp = fulltext;
        EditorGUI.BeginChangeCheck();
        temp = EditorGUILayout.TextArea(temp, GUILayout.Height(30)); //정답 기입창 표시
        if (EditorGUI.EndChangeCheck()&& !temp.Equals(fulltext))
        {
            fulltext = temp;
            FulltoSubText(fulltext,ref array);
        }
        EditorGUI.indentLevel--;
    }
    private void SitePropertyModify(ref InternetAppInfo.Site[] sites,InternetProperty property)
    {
        property.elementfold[2] = EditorGUILayout.Foldout(property.elementfold[2], "사이트");
        if (!property.elementfold[2]) return;
        EditorGUI.indentLevel++;
        ArraySizeModify(ref sites, ref property.fold_site,"사이트");
        EditorGUI.indentLevel++;
        for(int i=0;i<sites.Length;i++)
        {
            property.fold_site[i] = EditorGUILayout.Foldout(property.fold_site[i], "사이트" + (i + 1));
            if(property.fold_site[i])
            {
                sites[i].name = EditorGUILayout.DelayedTextField("이름", sites[i].name);
                sites[i].url = EditorGUILayout.DelayedTextField("주소", sites[i].url);
                sites[i].content = EditorGUILayout.DelayedTextField("내용", sites[i].content);
            }
        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }
    private void KnowinPropertyModify(ref InternetAppInfo.Knowin[] knowins, InternetProperty property)
    {
        property.elementfold[3] = EditorGUILayout.Foldout(property.elementfold[3], "지식인");
        if (!property.elementfold[3]) return;
        EditorGUI.indentLevel++;
        ArraySizeModify(ref knowins,ref property.fold_knowins, "지식인 질문");
        EditorGUI.indentLevel++;
        for (int i=0;i<knowins.Length;i++)
        {
            property.fold_knowins[i] = EditorGUILayout.Foldout(property.fold_knowins[i], "질문" + (i + 1));
            if(property.fold_knowins[i])
            {
                EditorGUI.indentLevel++;
                knowins[i].image_enable = EditorGUILayout.Toggle("이미지 여부", knowins[i].image_enable);
                if (knowins[i].image_enable) knowins[i].image = (Sprite)EditorGUILayout.ObjectField("이미지", knowins[i].image, typeof(Sprite), allowSceneObjects: true);
                else knowins[i].image = null;
                knowins[i].question_title = EditorGUILayout.TextField("질문 제목", knowins[i].question_title);
                knowins[i].question_content = EditorGUILayout.TextField("질문 내용", knowins[i].question_content);
                knowins[i].answer_enable= EditorGUILayout.Toggle("답변 여부", knowins[i].answer_enable);
                if (knowins[i].answer_enable) knowins[i].answer_content = EditorGUILayout.TextField("답변 내용", knowins[i].answer_content);
                else knowins[i].answer_content = null;
                EditorGUI.indentLevel--;
            }
        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }
    private void EncyclopediaPropertyModify(ref InternetAppInfo.Encyclopedia[] encyclopedias,InternetProperty property)
    {
        property.elementfold[4] = EditorGUILayout.Foldout(property.elementfold[4], "사전");
        if (!property.elementfold[4]) return;
        EditorGUI.indentLevel++;
        ArraySizeModify(ref encyclopedias, ref property.fold_encyclopedia, "사전 항목");
        EditorGUI.indentLevel++;
        for(int i=0;i<encyclopedias.Length;i++)
        {
            property.fold_encyclopedia[i] = EditorGUILayout.Foldout(property.fold_encyclopedia[i], "항목" + (i + 1));
            if(property.fold_encyclopedia[i])
            {
                EditorGUI.indentLevel++;
                encyclopedias[i].name = EditorGUILayout.TextField("제목", encyclopedias[i].name);
                encyclopedias[i].content = EditorGUILayout.TextField("내용", encyclopedias[i].content);
                EditorGUI.indentLevel--;
            }
        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }
    private void FulltoSubText(string fulltext,ref string[] array)
    {
        string word = "";
        List<string> temps = new List<string>();
        for(int i=0;i<fulltext.Length;i++)
        {
            bool isend = fulltext[i] == ',';
            if(!isend) word += fulltext[i];
            if (isend || (i == fulltext.Length-1 && word.Length > 0))//쉼표 = 단어의 끝인경우
            {
                temps.Add(word);
                word = "";
            }
        }
        array = temps.ToArray();
    }
    private void SubtoFullText(ref string fulltext, string[] array)
    {
        if (array == null) return;
        StringBuilder sb = new StringBuilder();
        for(int i=0;i<array.Length;i++)
        {
            if (i == array.Length - 1) sb.Append(array[i]);
            else sb.AppendFormat("{0}",array[i]);
        }
        fulltext = sb.ToString();
    }
}

