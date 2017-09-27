#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryScene : Stage, IDisableButton, IClickDelay
{
    //퍼즐 공간을 관리한다.(Click과 이동효과는 GalleryScene에서 관리)
    [Serializable]
    private struct GalleryPuzzle
    {
        [SerializeField]
        GameObject block; //복제할 블록
        public GameObject line; //복제할 선
        public RectTransform start;//시작할 위치
        public Image complete;//완성된 이미지를 보여줄 컴포넌트
        GalleryScene scene;
        public Transform Empty;//현재 비어있는(다른 블록이 이동가능한)블록
        [Range(1, 50)]
        public float speed; //이동 속도
        //에디터에서 설정한 퍼즐수에 맞게 적당히 퍼즐을 배치.
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
        //퍼즐을 섞고, 임의의 한 블록을 텅 비게 만듬.
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
        //현재 상태가 정답인지 체크
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
            puzzle.complete.GetComponent<AudioSource>().Play();
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
    public event VoidDelegate FailEvent; //타이머형 이벤트에 버튼을 쓰지않으므로 FailEvent는 미사용.
    public Gallery info;
    [SerializeField]
    GalleryPuzzle puzzle;
    AudioSource au;
    public void Click(Transform block)
    {
        if (!clickenable) return;
        if (au == null) au = GetComponent<AudioSource>();//오디오 출력기를 아직 얻지않은경우 얻어옴.
        int empty = puzzle.Empty.GetSiblingIndex();//빈 블록의 번짓수를 받음.
        int index = block.GetSiblingIndex();//선택한 블록의 번짓수를 받음.
        bool enable1 = (block.position.y == puzzle.Empty.position.y) && (index + 1 == empty || index - 1 == empty);//선택한 블록과 빈 블록의 열 위치가 같고, 두 블록이 좌우로 붙어있는경우
        bool enable2 = (block.position.x == puzzle.Empty.position.x) && (index + info.PuzzleNumber == empty || index - info.PuzzleNumber == empty);//선택한 블록과 빈 블록의 행 위치가 같고, 두 블록이 세로로 붙어있는 경우
        //퍼즐을 움직일수 잇는경우
        if (enable1 || enable2)
        {
            clickenable = false;
            StartCoroutine(BlockMove(block));
        }
    }
    IEnumerator BlockMove(Transform target)//블록 움직임 효과
    {
        au.Play();
        Transform afterempty = Instantiate(puzzle.Empty, puzzle.Empty, false).transform;
        afterempty.position = target.position;
        //현재 블록과 빈 블록의 방향을 파악.
        Vector3 dir = (puzzle.Empty.position - target.position).normalized;
        int beforeindex = target.GetSiblingIndex();
        target.SetParent(puzzle.Empty);
        while (true)
        {
            target.position += puzzle.speed * dir;
            Vector3 afterdir = (puzzle.Empty.position - target.position).normalized;//매 호출마다 방향 파악
            if (target.position == puzzle.Empty.position) break;//목적지에 도달시 중단.
            else if (!dir.Equals(afterdir)) //방향이 바뀐경우(목적지를 지나친경우)
            {
                target.position = puzzle.Empty.position;//목적지 고정
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
        if (puzzle.Check) ClearEvent(); //정답인경우 클리어 이벤트 출력.
        clickenable = true;
    }
}
