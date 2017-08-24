using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CrossWordPuzzle : MonoBehaviour
{
    public GameObject CrosswordPuzzleblock;
    public RectTransform PuzzleSpace;
    public Button Up;
    public Button Down;
    public Button Left;
    public Button Right;
    DictionaryAppinfo info;
    Vector2 defaultpos;
    GameObject[] blocks = new GameObject[100];
    List<int[]> spaces = new List<int[]>();
    List<char[]> answers = new List<char[]>();
    int selectindex = 0;
    private void Start()
    {
        char[] temp2 = new char[10];
        answers.AddRange(new List<char[]> { temp2, temp2, temp2, temp2, temp2, temp2, temp2, temp2, temp2, temp2 });
        info = (DictionaryAppinfo)GameManager.appinfo;
        for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) answers[i][j] = info.answers[i, j];
        for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) spaces[i][j] = info.space[i, j];
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++) sb.Append(answers[i][j] == '\0' ? ' ' : answers[i][j]);
            sb.Append("\n");
        }
        Debug.Log(sb.ToString());
        defaultpos = CrosswordPuzzleblock.transform.position;
        for (int i = 0; i < 100; i++)
        {
            blocks[i] = PuzzleSpace.GetChild(i).gameObject;
            SetBlockField(i % 10, i / 10, blocks[i]);
        }
        int[] temp = new int[3] { 0, 0, 0 };
        StartCoroutine("EnableBlock",temp);
    }
    IEnumerator EnableBlock(int[] arr)
    {
        int x = arr[0];
        int y = arr[1];
        int dir = arr[2];
        int index = y * 10 + x;
        blocks[index].SetActive(true);
        if(dir==0) yield return new WaitForSeconds(12f*Time.deltaTime);
        else yield return new WaitForSeconds(4f * Time.deltaTime);
        if (x < 9 && y < 9 && dir == 0) StartCoroutine("EnableBlock", new int[] { x + 1, y + 1, 0 });
        if (x < 9 && dir != 2) StartCoroutine("EnableBlock", new int[] { x + 1, y, 1 });
        if (y < 9 && dir !=1) StartCoroutine("EnableBlock", new int[] { x, y + 1, 2 });
    }
    Vector2 GetBlockPosition(int x,int y)
    {
        Vector2 pos = new Vector2(x * 108, -y * 108);
        return defaultpos + pos;
    }
    void SetBlockField(int x,int y,GameObject block)
    {
        int index = spaces[y][x];
        Text numfield = block.transform.GetChild(2).GetComponent<Text>();
        InputField inputsys = block.GetComponent<InputField>();
        EventTrigger eventtrigger = block.GetComponent<EventTrigger>();
        if (index > -1)
        {
            EventTrigger.Entry select = new EventTrigger.Entry();
            select.eventID = EventTriggerType.Select;
            select.callback.AddListener(delegate { selectindex = block.transform.GetSiblingIndex(); });
            eventtrigger.triggers.Add(select);
            inputsys.onValueChanged.AddListener(delegate
            {
                if (inputsys.text.Length > 1) inputsys.text = inputsys.text[1].ToString();
            });
            inputsys.onEndEdit.AddListener(delegate
            {
                answers[y][x] = inputsys.text[0];
            });
            if (index == 0) Destroy(numfield);
        }
        else
        {
            inputsys.interactable = false;
            Destroy(numfield);
            Destroy(eventtrigger);
        }
    }
    public void Move(int dir)
    {
        int index = selectindex + dir;
        int pos_y = index / 10;
        int pos_x = index % 10;
        bool enable = spaces[pos_y][pos_x] != -1;
        if (index >= 0 && index < 100 && enable)PuzzleSpace.GetChild(index).GetComponent<InputField>().Select();
        Up.interactable = pos_y + 1 < 10 && spaces[pos_y + 1][pos_x] != -1;
        Down.interactable = pos_y - 1 > -1 && spaces[pos_y - 1][pos_x] != -1;
        Left.interactable = pos_x + 1 < 10 && spaces[pos_y][pos_x + 1] != -1;
        Right.interactable = pos_x - 1 > -1 && spaces[pos_y][pos_x - 1] != -1;
    }
    public void Submit()
    {
        if(answers.Equals(info.answers))
        {

        }
        else
        {

        }
    }
}

