#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryScene : Stage, IDisableButton, IClickDelay
{
    [Serializable]
    private struct GalleryPuzzle
    {
        [SerializeField]
        GameObject block;
        public GameObject line;
        public RectTransform start;
        public Image complete;
        GalleryScene scene;
        public Transform Empty;
        [Range(1, 50)]
        public float speed;
        public void Create(GalleryScene scen)
        {
            this.scene = scen;
            float width = scene.info.image.width / scene.info.PuzzleNumber;
            float height = scene.info.image.height / scene.info.PuzzleNumber;
            float step = 1000 / scene.info.PuzzleNumber;
            block.GetComponent<RectTransform>().sizeDelta = new Vector2(step, step);
            Vector2 pos = complete.transform.position;
            GalleryPuzzle str = this;
            for (int i = 0; i < scene.info.PuzzleNumber; i++)
            {
                for (int j = 0; j < scene.info.PuzzleNumber; j++)
                {
                    GameObject puzzle = Instantiate(block, start, false);
                    puzzle.name = (i * scene.info.PuzzleNumber + j).ToString("D2");
                    puzzle.transform.position = new Vector2(step * j, step * i) + pos;
                    Image image = puzzle.GetComponent<Image>();
                    image.sprite = Sprite.Create(scene.info.image, new Rect(width * j, height * i, width, height), new Vector2(0.5f, 0.5f));
                    image.sprite.name = puzzle.name;
                    puzzle.GetComponent<Button>().onClick.AddListener(delegate { scen.Click(puzzle.transform); });
                }
            }
        }
        public void Swap()
        {
            Image[] images = start.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                int index = UnityEngine.Random.Range(0, images.Length);
                Sprite temp = images[i].sprite;
                images[i].sprite = images[index].sprite;
                images[index].sprite = temp;
            }
            int empty = UnityEngine.Random.Range(0, images.Length);
            Empty = images[empty].transform;
            images[empty].sprite = null;
            Destroy(Empty.GetComponent<Button>());
        }
        public bool Check
        {
            get
            {
                Image[] now = start.GetComponentsInChildren<Image>();
                for (int i = 0; i < now.Length; i++)
                {
                    bool empty = now[i].sprite == null;
                    if (empty) continue;
                    int num = int.Parse(now[i].sprite.name);
                    if (i != num) return false;
                }
                return true;
            }
        }
    }
    public override MissionType missontype { get { return MissionType.Timer; } }
    public override int MaxLife { get { return 300; } }
    public override int WestedLifeClear { get { return 3; } }
    public override int WestedLifeGameOver { get { return 5; } }
    public bool clickenable
    {
        get;set;
    }
    protected override Action BeforeIntro
    {
        get
        {
            return () =>
            {
                info = SmartPhone.GetData<Gallery>();
                puzzle.Create(this);
            };
        }
    }
    protected override IEnumerator AfterIntro {
        get
        {
            puzzle.complete.sprite = Sprite.Create(info.image, new Rect(0, 0, 500, 500), new Vector2(0.5f, 0.5f));
            Color color = puzzle.complete.color;
            List<RectTransform> width = new List<RectTransform>();
            List<RectTransform> height = new List<RectTransform>();
            float step = 1000 / info.PuzzleNumber;
            Vector2 pos = puzzle.complete.transform.position;
            //완성 이미지의 불투명하게 한다.
            while (color.a < 1)
            {
                color.a += Time.deltaTime;
                puzzle.complete.color = color;
                yield return new WaitForEndOfFrame();
            }
            for (int i = 1; i < info.PuzzleNumber; i++)
            {
                Transform line_w = Instantiate(puzzle.line, new Vector2(0, i * step) + pos, Quaternion.identity).transform;
                Transform line_h = Instantiate(puzzle.line, new Vector2(i * step, 1000) + pos, Quaternion.identity).transform;
                line_w.SetParent(puzzle.start.parent);
                line_h.SetParent(puzzle.start.parent);
                width.Add(line_w as RectTransform);
                height.Add(line_h as RectTransform);
            }
            Vector2 width_l = width[0].sizeDelta;
            width_l.y = 5;
            Vector2 height_l = height[0].sizeDelta;
            height_l.x = 5;
            //가로세로 선을 늘린다.
            while (width_l.x < 1000)
            {
                width_l.x += puzzle.speed;
                height_l.y += puzzle.speed;
                for (int i = 0; i < width.Count; i++)
                {
                    width[i].sizeDelta = width_l;
                    height[i].sizeDelta = height_l;
                }
                yield return new WaitForEndOfFrame();
            }
            //완성 이미지를 투명하게 한다.
            while (color.a > 0)
            {
                color.a -= Time.deltaTime;
                puzzle.complete.color = color;
                yield return new WaitForEndOfFrame();
            }
            puzzle.Swap();
            Destroy(puzzle.complete.gameObject);
            puzzle.start.gameObject.SetActive(true);
        }
    }
    public event VoidDelegate ClearEvent;
    public event VoidDelegate FailEvent;
    public Gallery info;
    [SerializeField]
    GalleryPuzzle puzzle;
    public void Click(Transform block)
    {
        if (!clickenable) return;
        int empty = puzzle.Empty.GetSiblingIndex();
        int index = block.GetSiblingIndex();
        bool enable1 = (block.position.y == puzzle.Empty.position.y) && (index + 1 == empty || index - 1 == empty);
        bool enable2 = (block.position.x == puzzle.Empty.position.x) && (index + info.PuzzleNumber == empty || index - info.PuzzleNumber == empty);
        if (enable1 || enable2)
        {
            clickenable = false;
            StartCoroutine(BlockMove(block));
        }
    }
    IEnumerator BlockMove(Transform target)//블록 움직임 효과
    {
        Transform afterempty = Instantiate(puzzle.Empty, puzzle.Empty, false).transform;
        afterempty.position = target.position;
        Vector3 dir = (puzzle.Empty.position - target.position).normalized;
        int beforeindex = target.GetSiblingIndex();
        target.SetParent(puzzle.Empty);
        while (true)
        {
            target.position += puzzle.speed * dir;
            Vector3 afterdir = (puzzle.Empty.position - target.position).normalized;
            if (target.position == puzzle.Empty.position) break;
            else if (!dir.Equals(afterdir))
            {
                target.position = puzzle.Empty.position;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        target.SetParent(puzzle.Empty.parent);
        afterempty.SetParent(puzzle.Empty.parent);
        afterempty.SetSiblingIndex(beforeindex);
        int index = puzzle.Empty.GetSiblingIndex();
        target.SetSiblingIndex(index);
        DestroyImmediate(puzzle.Empty.gameObject);
        puzzle.Empty = afterempty;
        if (puzzle.Check) ClearEvent();
        clickenable = true;
    }
}
