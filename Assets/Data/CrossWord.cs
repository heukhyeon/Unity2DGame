using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossWord : MonoBehaviour {
    [Serializable]
    public class PuzzleInfo
    {
        public int index;
        public string answer;
        public string hint;
        public int pos_x;
        public int pos_y;
    }
    public PuzzleInfo[] Row = new PuzzleInfo[0];
    public PuzzleInfo[] Column = new PuzzleInfo[0];
}
