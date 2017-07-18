using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DictionaryScene : MonoBehaviour {
    public Transform CrossWordPuzzleSpace; //10*10 퍼즐을 가지고있는 객체
    public LayerMask uilayer;
    public GameObject CrossWordBlock;
    private char[,] answer = new char[10, 10];
    private char[,] inputword = new char[10, 10];
    private void Awake()
    {
        DictionaryAppInfo info = StaticManager.Nowappinfo.dictionaryinfo;
        bool[,] space = new bool[10, 10];
        List<Vector2> startpos_s = new List<Vector2>();
        List<Vector2> startpos_c = new List<Vector2>();
        for(int i=0;i<info.Streetwords.Length;i++)
        {
            DictionaryAppInfo.DictionaryWord word = info.Streetwords[i];
            if (!startpos_s.Contains(word.Start_loc)) startpos_s.Add(word.Start_loc);
            for (int j = 0; j < word.word.Length; j++)
                space[(int)word.Start_loc.y,(int)word.Start_loc.x + j] = true;
        }
        for (int i = 0; i < info.Columnwords.Length; i++)
        {
            DictionaryAppInfo.DictionaryWord word = info.Columnwords[i];
            if (!startpos_c.Contains(word.Start_loc)) startpos_c.Add(word.Start_loc);
            for (int j = 0; j < word.word.Length; j++)
                space[(int)word.Start_loc.y+j, (int)word.Start_loc.x] = true;
        }

        for (int i=0;i<10;i++)
            for(int j=0;j<10;j++)
            {
                Transform puzzle = Instantiate(CrossWordBlock, CrossWordPuzzleSpace, false).transform;
                InputField inputsys = puzzle.GetComponent<InputField>();
                puzzle.localPosition = new Vector2(j * 108 - 540, -i * 108);
                inputsys.interactable = space[i,j];
                if(inputsys.interactable)
                {
                    Vector2 startpos = new Vector2(j, i);
                    NumFiledSetting(puzzle.GetComponentsInChildren<Text>()[2], startpos, startpos_s, startpos_c);
                    int index = puzzle.GetSiblingIndex();
                    inputsys.onValueChanged.AddListener(delegate (string ev) { if (ev.Length > 1) inputsys.text = ev[1].ToString(); });
                    inputsys.onEndEdit.AddListener(delegate (string ev)
                    {
                        inputword[index / 10, index % 10] = inputsys.text[0];
                    });

                }
                else
                    Destroy(puzzle.GetChild(3).gameObject);
            }
    }
   
    private void NumFiledSetting(Text text,Vector2 loc,List<Vector2> s_locs,List<Vector2> c_locs)
    {
        for(int i=0;i<s_locs.Count;i++)
            if(loc.Equals(s_locs[i]))
            {
                text.text = i + ")";
                return;
            }
        for (int i = 0; i < c_locs.Count; i++)
            if (loc.Equals(c_locs[i]))
            {
                text.text = i + ")";
                return;
            }
        Destroy(text.transform.GetChild(3).gameObject);
    }
}
