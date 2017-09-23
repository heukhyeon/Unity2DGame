#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class InternetScene : Stage,INormalButton,IBeforeClear
{
    [Serializable]
    private struct InternetBlock
    {
        [SerializeField]
        RectTransform Header;
        [SerializeField]
        GameObject Origin;
        public bool Enable { get; private set; }
        public float Prepare<T>(T[] info,float headerpos, Vector2 blockpos,Func<RectTransform,T,float>func)
        {
            Enable = info.Length != 0;
            if (info.Length == 0)
            {
                Destroy(Header.gameObject);
                return 0;
            }
            else
            {
                Header.localPosition = new Vector2(495, headerpos);
                for (int i = 0; i < info.Length; i++)
                {
                    RectTransform block = SmartPhone.CreateAndPosition(Origin, Header, blockpos);
                    blockpos.y -= func(block, info[i]);
                }
                Header.gameObject.SetActive(false);
                return blockpos.y-20;
            }
        }
        public static implicit operator RectTransform(InternetBlock block)
        {
            return block.Header;
        }
    }
    [Serializable]
    private struct RollingBlock
    {
        public Transform block;
        public Transform parent;
        public int dir;
        public IEnumerator RollingThrow(params float[] y_val)
        {
            float y_dir = 10;
            if (y_val.Length > 0) y_dir += y_val[0];
            float cnt = 0;
            float rot_dir = dir == 0 ? 1 : dir;
            block.SetParent(parent);
            while(cnt<10)
            {
                Vector3 pos = block.position;
                pos.x += dir * 5;
                pos.y += y_dir;
                y_dir -= 30 * Time.deltaTime;
                block.position = pos;
                pos = block.rotation.eulerAngles;
                pos.z += rot_dir * 30;
                block.rotation = Quaternion.Euler(pos);
                cnt += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    List<string> answers = new List<string>();

    [SerializeField]
    InternetBlock site;
    [SerializeField]
    InternetBlock knowin;
    [SerializeField]
    InternetBlock wiki;
    [SerializeField]
    RectTransform Content;
    [SerializeField]
    RectTransform Relation;
    [SerializeField]
    InputField AnswerField;
    float SiteSetting(RectTransform block,Internet.SiteInfo site)
    {
        Text[] content = block.GetComponentsInChildren<Text>();
        content[0].text = site.name;
        content[1].text = site.url;
        content[2].text = site.content;
        return 200f;
    }
    float KnowinSetting(RectTransform block, Internet.KnowInInfo knowin)
    {
        Vector2 size = block.sizeDelta;
        Transform question = block.GetChild(1);
        Transform Answer = block.GetChild(2);
        //이미지 존재 질문인경우 이미지를 갱신하고, 아닌경우 이미지를 삭제하고 질문/답변을 이미지공간만큼 당긴다.
        if (knowin.image) block.GetChild(0).GetComponent<RawImage>().texture = knowin.image;
        else
        {
            Vector2 temp = question.localPosition;
            temp.x -= 220;
            question.localPosition = temp;
            temp.y = Answer.localPosition.y;
            Answer.localPosition = temp;
            Destroy(block.GetChild(0).gameObject);
        }
        //질문을 채운다.
        Text[] questions = question.GetComponentsInChildren<Text>();
        questions[1].text = knowin.title;
        questions[2].text = knowin.content;
        //답변이 있는경우 답변을 채우고, 답변이 없는경우 답변란을 지운다.
        if (knowin.answerenable)
        {
            size.y += 210;
            block.sizeDelta = size;
            Text[] answers = Answer.GetComponentsInChildren<Text>();
            answers[1].text = knowin.answercontent;
        }
        else Destroy(Answer.gameObject);
        return size.y;
    }
    float WikiSetting(RectTransform block,Internet.WikiInfo wiki)
    {
        Text[] texts = block.GetComponentsInChildren<Text>();
        texts[0].text = wiki.name;
        texts[1].text = wiki.content;
        return 210;
    }
    public void InputChange()
    {
        if (AnswerField.text.Length > 0 && !Submitenable) Submitenable = true;
        else if (AnswerField.text.Length == 0 && Submitenable) Submitenable = false;
    }
    protected override Action BeforeIntro
    {
        get
        {
            return () =>
            {
                string temp = "";
                Internet info = SmartPhone.GetData<Internet>();
                foreach (char c in info.Answer)
                {
                    if (c == ',')
                    {
                        answers.Add(temp);
                        temp = "";
                    }
                    else temp += c;
                }
                answers.Add(temp);
                Text relations = Relation.GetComponentsInChildren<Text>()[1];
                relations.text = info.Relation;
                Relation.gameObject.SetActive(false);
                float pos = -(Relation.sizeDelta.y + 70);
                pos += site.Prepare(info.site, pos, new Vector2(0, -65), SiteSetting);
                pos += knowin.Prepare(info.knowin, pos, new Vector2(0, -65), KnowinSetting);
                pos += wiki.Prepare(info.wiki, pos, new Vector2(0, -60), WikiSetting);
                Content.sizeDelta = new Vector2(990, -pos);
            };
        }
    }
    protected override IEnumerator AfterIntro
    {
        get
        {
            List<RectTransform> blocks = new List<RectTransform>();
            blocks.Add(Relation);
            if (site.Enable) blocks.Add(site);
            if (knowin.Enable) blocks.Add(knowin);
            if (wiki.Enable) blocks.Add(wiki);
            foreach (RectTransform item in blocks)
            {
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Vector2 deltaPivot = item.pivot - pivot;
                Vector3 deltaPosition = item.localPosition;
                deltaPosition.y -= item.sizeDelta.y / 2f;
                item.pivot = pivot;
                item.localPosition = deltaPosition;
                Vector2 size = new Vector2(10, 10);
                item.localScale = size;
                item.gameObject.SetActive(true);
                float speed = Time.deltaTime;
                while (size.x > 1)
                {
                    size.x -= speed;
                    size.y -= speed;
                    speed += 0.1f * speed;
                    if (size.x < 1)
                    {
                        size.x = 1;
                        size.y = 1;
                    }
                    item.localScale = size;
                    yield return new WaitForEndOfFrame();
                }
            }
            AnswerField.interactable = true;
            AnswerField.text = answers[0];

        }
    }
    public override MissionType missontype { get { return MissionType.Count; } }
    public override int MaxLife { get { return 3; } }
    public bool Answer { get { string text = AnswerField.text; foreach (string answer in answers) if (text == answer) return true; return false; } }
    public override int WestedLifeClear { get { return 3; } }
    public override int WestedLifeGameOver { get { return 5; } }

    public IEnumerator BeforeClear
    {
        get
        {
            List<Transform> blocks = new List<Transform>();
            if (Relation != null) blocks.Add(Relation);
            if (site.Enable) blocks.Add(site);
            if (knowin.Enable) blocks.Add(knowin);
            if (wiki.Enable) blocks.Add(wiki);
            Transform scroll = Relation.parent.parent;
            float speed = 20f;
            int dir = -1;
            while(blocks.Count>0)
            {
                if(blocks[0].position.y<1300)
                {
                    foreach(var block in blocks)
                    {
                        Vector2 pos = block.position;
                        pos.y += speed;
                        block.position = pos;
                    }
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    RollingBlock rolling = new RollingBlock();
                    rolling.block = blocks[0];
                    rolling.dir = dir;
                    rolling.parent = this.transform;
                    dir *= -1;
                    blocks.RemoveAt(0);
                    StartCoroutine(rolling.RollingThrow());
                }
            }
            RollingBlock rolling2 = new RollingBlock();
            rolling2.block = scroll;
            rolling2.dir = 0;
            rolling2.parent = this.transform;
            StartCoroutine(rolling2.RollingThrow(20f));
            yield return new WaitForSeconds(1.5f);
        }
    }
}



