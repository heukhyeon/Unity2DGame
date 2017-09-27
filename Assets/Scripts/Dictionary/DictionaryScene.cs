#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryScene : Stage,IBeforeClear,INormalButton
{
    public struct CrossWordBlockInfo
    {
        public int index;
        public char answer;
        public string hint;
        public AudioClip effect;
    }
    [SerializeField]
    Text HintText;
    [SerializeField]
    AudioClip blockselect;
    AudioSource au;
    RectTransform HintSpace;
    List<CrossWordBlock> blocklist;
    protected override Action BeforeIntro
    {
        get
        {
            return () =>
            {
                au = GetComponent<AudioSource>();
                var temp = this.transform.GetComponentsInChildren<CrossWordBlock>();
                foreach (var info in temp) info.gameObject.SetActive(false);
                blocklist = new List<CrossWordBlock>(temp);
            };
        }
    }
    protected override IEnumerator AfterIntro
    {
        get
        {
            CrossWord info = SmartPhone.GetData<CrossWord>();
            List<CrossWordBlock> temp = blocklist.ToList();
            CrossWordBlockInfo[,] infos = new CrossWordBlockInfo[10, 10];
            foreach (var word in info.Row)
            {
                int y = word.pos_y;
                int x = word.pos_x;
                infos[y, x].index = word.index;
                infos[y, x].effect = blockselect;
                for (int i = 0; i < word.answer.Length; i++)
                {
                    infos[y, x + i].answer = word.answer[i];
                    infos[y, x + i].hint = "가로\n"+word.hint;
                }
            }
            foreach (var word in info.Column)
            {
                int y = word.pos_y;
                int x = word.pos_x;
                infos[y, x].index = word.index;
                infos[y, x].effect = blockselect;
                for (int i = 0; i < word.answer.Length; i++)
                {
                    infos[y+i, x].answer = word.answer[i];
                    infos[y+i, x].hint += infos[y + i, x].hint == null ? "세로\n"+word.hint : "\n세로\n" + word.hint;
                }
            }
            float delay = 3f * Time.deltaTime;
            while (temp.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, temp.Count);
                var block = temp[index];
                block.Setting(this, temp, infos[block.Y, block.X]);
                delay /= 1.05f;
                yield return new WaitForSeconds(delay);
            }
        }
    }
    public override MissionType missontype { get { return MissionType.Timer; } }
    public override int MaxLife { get { return 300; } }
    public bool Answer {
        get
        {
            foreach (var block in blocklist) if (block.isEnable&&!block.Judge) return false;
            return true;
        }
    }
    public override int WestedLifeFail
    {
        get
        {
            return 5;
        }
    }
    public override int WestedLifeClear { get { return 3; } }
    public override int WestedLifeGameOver { get { return 5; } }
    public IEnumerator BeforeClear
    {
        get
        {
            int right_index = 99;
            int left_index = 0;
            int left_dir = 1;
            int right_dir = -1;
            int cnt = 0;
            float delay = 5 * Time.deltaTime;
            au.Play();
            while(cnt<100)
            {
                blocklist[right_index].gameObject.SetActive(false);
                blocklist[left_index].gameObject.SetActive(false);
                blocklist[right_index] = null;
                blocklist[left_index] = null;
                right_dir = DirReturn(right_index, right_dir);
                left_dir = DirReturn(left_index, left_dir);
                right_index += right_dir;
                left_index += left_dir;
                cnt += 2;
                delay -= delay * 0.1f;
                yield return new WaitForSeconds(delay);
            }
            au.Stop();
        }
    }
    //BeforeClear시 현재 블록이 나아갈 방향을 알려준다.
    int DirReturn(int index, int dir)
    {
        bool condition;
        int ret;
        switch(dir)
        {
            case 1:
                condition = index % 10 < 9 && blocklist[index + dir] != null;
                ret = 10;
                break;
            case -1:
                condition = index % 10 > 0 && blocklist[index + dir] != null;
                ret = -10;
                break;
            case 10:
                condition = index + dir < 99 && blocklist[index + dir] != null;
                ret = -1;
                break;
            case -10:
                condition = index + dir > 0 && blocklist[index + dir] != null;
                ret = 1;
                break;
            default:
                condition = true;
                ret = -1;
                break;
        }
        return condition == true ? dir : ret;
    }
    public void ShowHint(string hint)
    {
        HintText.text = hint;
        if (HintSpace == null) HintSpace = HintText.transform as RectTransform;
        HintSpace.sizeDelta = GetSizeOfWord(hint);
    }
    //힌트 출력시 힌트 출력 공간을 적절히 조정.
    Vector2 GetSizeOfWord(string word)
    {
        float width = 0.0f;
        int cnt = 0;
        CharacterInfo charInfo;
        foreach (char c in word)
        {
            if (c == '\n')
            {
                cnt++;
                continue;
            }
            HintText.font.GetCharacterInfo(c, out charInfo, HintText.fontSize);
            width += charInfo.advance;
        }
        cnt += (int)(width / 1080);
        return new Vector2(1080, cnt * 200);
    }
}