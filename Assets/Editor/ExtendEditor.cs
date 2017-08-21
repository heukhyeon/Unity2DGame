using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Edtior대신 상속하는 커스텀에디터용 클래스
/// </summary>
public class ExtendEditor : Editor
{
    /// <summary>
    /// TextField를 생성한다.
    /// </summary>
    /// <param name="content">텍스트필드 변동에 반응하는 문자열 참조</param>
    /// <param name="name">텍스트필드 제목</param>
    protected void CreateField(ref string content,string name)
    {
        content = EditorGUILayout.TextField(name, content);
    }
    /// <summary>
    /// IntField를 생성한다.
    /// </summary>
    /// <param name="content">정수 필드 변동에 반응하는 정수 참조</param>
    /// <param name="name">정수필드 제목</param>
    protected void CreateField(ref int content, string name)
    {
        content = EditorGUILayout.IntField(name, content);
    }
    /// <summary>
    /// DelayedIntField를 생성한다.
    /// </summary>
    /// <param name="content">정수 필드 변동에 반응하는 정수 참조</param>
    /// <param name="name">정수필드 제목</param>
    protected void CreateDelayField(ref int content, string name)
    {
        content = EditorGUILayout.DelayedIntField(name, content);
    }
    /// <summary>
    /// 배열을 사이즈에 맞게 재조정.
    /// </summary>
    /// <typeparam name="T">변수 타입</typeparam>
    /// <param name="array">변경할 배열</param>
    /// <param name="size">변경할 크기</param>
    protected void ArraySizeModify<T>(ref T[] array, int size)where T:new()
    {
        T[] temp;
        if (size == array.Length) return;
        temp = Enumerable.Repeat(new T(), size).ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            if (i == temp.Length) break;
            temp[i] = array[i];
        }
        array = temp;
    }
    /// <summary>
    /// List를 사이즈에 맞게 재조정.
    /// </summary>
    /// <typeparam name="T">변수 타입</typeparam>
    /// <param name="list">변경할 리스트</param>
    /// <param name="size">변경할 크기</param>
    protected void ArraySizeModify<T>(List<T> list, int size) where T:new()
    {
        while(list.Count!=size)
        {
            if (list.Count > size) list.RemoveAt(list.Count - 1);
            else list.Add(new T());
        }
    }
    /// <summary>
    /// 배열의 크기가 사이즈와 다를경우, bool 매개변수를 true로 하고 배열의 크기를 재조정.
    /// </summary>
    /// <typeparam name="T">변수 타입</typeparam>
    /// <param name="array">재조정할 배열</param>
    /// <param name="size">변경 크기</param>
    /// <param name="change">배열의 크기가 재조정될경우 활성화될 bool 변수</param>
    protected void ArraySizeModify<T>(ref T[] array, int size,ref bool change)
    {
        if (size == array.Length) return;
        change = true;
        T[] temp = new T[size];
        for (int i = 0; i < array.Length; i++)
        {
            if (i == temp.Length) break;
            temp[i] = array[i];
        }
        array = temp;
    }
    /// <summary>
    /// List를 사이즈에 맞게 재조정.
    /// </summary>
    /// <typeparam name="T">변수 타입</typeparam>
    /// <param name="list">변경할 리스트</param>
    /// <param name="size">변경할 크기</param>
    /// <param name="change">리스트의 크기가 재조정될경우 활성화될 bool 변수</param>
    protected void ArraySizeModify<T>(List<T> list, int size,ref bool change) where T : new()
    {
        if(list.Count!=size)
        {
            change = true;
            while (list.Count != size)
            {
                if (list.Count > size) list.RemoveAt(list.Count - 1);
                else list.Add(new T());
            }
        }

    }
    /// <summary>
    /// 숨김, 활성화 창을 만들고, 활성화될시 매개변수가 없는 함수를 실행.
    /// </summary>
    /// <param name="toggle">숨김, 활성화 여부를 관리할 bool 변수</param>
    /// <param name="name">숨김, 활성화 창의 이름 </param>
    /// <param name="action">활성화시 실행될 함수</param>
    protected void FoldOut(ref bool toggle,string name, Action action)
    {
        toggle = EditorGUILayout.Foldout(toggle, name);
        if (toggle)
        {
            EditorGUI.indentLevel++;
            action();
            EditorGUI.indentLevel--;
        }
    }
    /// <summary>
    /// 숨김, 활성화 창을 만들고, 활성화될시 매개변수가 없는 함수를 실행.
    /// </summary>
    /// <param name="toggle">숨김, 활성화 여부를 관리할 bool 변수</param>
    /// <param name="name">숨김, 활성화 창의 이름 </param>
    /// <param name="index">연속적으로 생성 등에 사용될, 이름 뒤에 붙을 인덱스 번호</param>
    /// <param name="action">활성화시 실행될 함수</param>
    protected void FoldOut(ref bool toggle, string name,int index, Action action)
    {
        toggle = EditorGUILayout.Foldout(toggle, name+index);
        if (toggle)
        {
            EditorGUI.indentLevel++;
            action();
            EditorGUI.indentLevel--;
        }
    }
    /// <summary>
    /// 숨김, 활성화 창을 만들고, 활성화될시 매개변수를 가진 함수를 실행.
    /// </summary>
    /// <typeparam name="T">매개변수 타입</typeparam>
    /// <param name="toggle">숨김, 활성화 여부를 관리할 bool 변수</param>
    /// <param name="name">숨김, 활성화 창의 이름 </param>
    /// <param name="action">활성화시 실행될 함수</param>
    /// <param name="parameter">전송할 매개변수</param>
    protected void FoldOut<T>(ref bool toggle,string name,Action<T> action,T parameter)
    {
        toggle = EditorGUILayout.Foldout(toggle, name);
        if (toggle)
        {
            EditorGUI.indentLevel++;
            action(parameter);
            EditorGUI.indentLevel--;
        }
    }
    /// <summary>
    /// 숨김, 활성화 창을 만들고, 활성화될시 매개변수가 없는 함수를 실행.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="toggle">숨김, 활성화 여부를 관리할 bool 변수</param>
    /// <param name="name">숨김, 활성화 창의 이름 </param>
    /// <param name="index">연속적으로 생성 등에 사용될, 이름 뒤에 붙을 인덱스 번호</param>
    /// <param name="action">활성화시 실행될 함수</param>
    /// <param name="parameter">전송할 매개변수</param>
    protected void FoldOut<T>(ref bool toggle, string name, int index, Action<T> action, T parameter)
    {
        toggle = EditorGUILayout.Foldout(toggle, name + index);
        if (toggle)
        {
            EditorGUI.indentLevel++;
            action(parameter);
            EditorGUI.indentLevel--;
        }
    }
}
