using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{

    public GameObject PuzzleBlock;
    public GameObject PuzzleLine;
    public RectTransform PuzzleStart;
    public Texture2D test;
    public Image CompleteImage;
    Sprite LastPiece;
    bool clickEnable = false;
    Transform EmptyBlock;
    GalleryAppInfo info;
    float puzzleMovespeed = 25f;
    // Use this for initialization
    void Start()
    {
        try //정보가 제대로 존재하는경우(로딩이 된경우)
        {
            info = (GalleryAppInfo)GameManager.appinfo;
            Debug.Log(info.image.name);
        }
        catch //정보가 없는경우(로딩없이 테스트용으로 해당 어플에서 바로 시작햇을시)
        {
            info.image = test;
            info.PuzzleNumber = 16;
        }
        StartCoroutine(PuzzleSwap());
        float columncnt = Mathf.Sqrt(info.PuzzleNumber);
        float onePieceWidth = info.image.width / columncnt;
        float onePieceHeight = info.image.height / columncnt;
        Vector2 puzzleSize = PuzzleBlock.GetComponent<RectTransform>().sizeDelta;
        for (int i = 0; i < columncnt; i++)
        {
            for (int j = 0; j < columncnt; j++)
            {
                GameObject obj = Instantiate(PuzzleBlock, PuzzleStart, false);
                obj.name = "Puzzle" + ((i * columncnt) + (j + 1));
                Sprite sp = Sprite.Create(info.image, new Rect(onePieceWidth * j, onePieceHeight * i, onePieceWidth, onePieceHeight), new Vector2(0f, 0f));
                sp.name = "Puzzle" + ((i * columncnt) + j + 1);
                obj.GetComponent<Image>().sprite = sp;
                obj.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
        }
    }

    private IEnumerator PuzzleSwap()//시작하기전 퍼즐을 섞는다
    {
        CompleteImage.sprite = Sprite.Create(info.image, new Rect(0, 0, info.image.width, info.image.height), new Vector2(0.5f, 0.5f));
        float oneStep = 5 / 255f; //하나의 while 사이클에서 완성된 이미지가 온/오프 되는 속도
        float oneLine = 25f; //하나의 while 사이클에서 퍼즐 줄이 나아가는 속도
        RectTransform[] widthline = new RectTransform[3];
        RectTransform[] heightline = new RectTransform[3];
        while (true)
        {
            Color color = CompleteImage.color;
            color.a += oneStep;
            CompleteImage.color = color;
            if (color.a > 1) break;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        widthline[0] = PuzzleLineCreate(0, true);
        bool widthComplete = false;
        bool heightComplete = false;
        while (!widthComplete || !heightComplete)
        {
            if (!widthComplete) for (int i = 0; i < widthline.Length; i++)
                {
                    if (!widthline[i]) continue;
                    float width = widthline[i].sizeDelta.x;
                    if (width >= 1000)
                    {
                        if (i < 2) continue;
                        else
                        {
                            widthComplete = true;
                            break;
                        }
                    }
                    width += oneLine;
                    widthline[i].sizeDelta = new Vector2(width, 5);
                    if (width > 500)
                    {
                        if (i < widthline.Length - 1 && !widthline[i + 1]) widthline[i + 1] = PuzzleLineCreate(i + 1, true);
                        else if (i == widthline.Length - 1 && !heightline[0]) heightline[0] = PuzzleLineCreate(0, false);
                    }
                }
            if (!heightComplete) for (int i = 0; i < heightline.Length; i++)
                {
                    if (!heightline[i]) continue;
                    float height = heightline[i].sizeDelta.y;
                    if (height >= 1000)
                    {
                        if (i < heightline.Length - 1) continue;
                        else
                        {
                            heightComplete = true;
                            break;
                        }
                    }
                    height += oneLine;
                    heightline[i].sizeDelta = new Vector2(5, height);
                    if (height > 500 && i < widthline.Length - 1 && !heightline[i + 1]) heightline[i + 1] = PuzzleLineCreate(i + 1, false);
                }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        while (true)
        {
            Color color = CompleteImage.color;
            color.a -= oneStep;
            CompleteImage.color = color;
            if (color.a < 0) break;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Destroy(CompleteImage.gameObject);
        Image[] puzzles = PuzzleStart.GetComponentsInChildren<Image>();
        Shuffle<Image>(puzzles);
        Vector2 puzzleSize = PuzzleBlock.GetComponent<RectTransform>().sizeDelta;
        int isEmpty = UnityEngine.Random.Range(0, puzzles.Length);
        for (int i = 0; i < puzzles.Length; i++)
        {
            puzzles[i].transform.SetSiblingIndex(i);
            puzzles[i].transform.localPosition = new Vector2(i % 4 * puzzleSize.x, i / 4 * puzzleSize.y);
            puzzles[i].color = new Color(255, 255, 255, 1);
            if (i == isEmpty)
            {
                puzzles[i].name = "EmptyBlock";
                EmptyBlock = puzzles[i].transform;
                LastPiece = puzzles[i].sprite;
                puzzles[i].sprite = null;
                puzzles[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                Transform target = puzzles[i].transform;
                puzzles[i].GetComponent<Button>().onClick.AddListener(delegate { PuzzleClick(target); });
            }
        }
        clickEnable = true;
    }
    private IEnumerator PuzzleMove(Transform target)//퍼즐 클릭시 빈칸으로 퍼즐을 움직인다
    {
        Transform afterempty = Instantiate(EmptyBlock, EmptyBlock, false).transform;
        afterempty.name = "EmptyBlock";
        afterempty.position = target.position;
        Vector3 dir = (EmptyBlock.position - target.position).normalized;
        int beforeindex = target.GetSiblingIndex();
        target.SetParent(EmptyBlock);
        while (true)
        {
            target.position += puzzleMovespeed * dir;
            Vector3 afterdir = (EmptyBlock.position - target.position).normalized;
            if (target.position == EmptyBlock.position) break;
            else if (!dir.Equals(afterdir))
            {
                target.position = EmptyBlock.position;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        target.SetParent(EmptyBlock.parent);
        afterempty.SetParent(EmptyBlock.parent);
        afterempty.SetSiblingIndex(beforeindex);
        target.SetSiblingIndex(EmptyBlock.GetSiblingIndex());
        Destroy(EmptyBlock.gameObject);
        EmptyBlock = afterempty;
        clickEnable = true;
        Debug.Log("이동완료");
    }
    private RectTransform PuzzleLineCreate(int cnt, bool isWidth)//PuzzleSwap에서 칸별 테두리 작성.
    {
        RectTransform ret = Instantiate(PuzzleLine, CompleteImage.transform, false).GetComponent<RectTransform>();
        ret.localPosition = isWidth == true ? new Vector2(0, ((cnt + 1) * -250) - 5) : new Vector2(((cnt + 1) * 250) - 5, 0);
        ret.parent = this.transform;
        return ret;
    }
    private System.Random rng = new System.Random();
    private void Shuffle<T>(IList<T> list)//리스트 섞기
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void PuzzleClick(Transform target)//퍼즐 클릭 이벤트
    {
        if (!clickEnable) return;
        int index = target.GetSiblingIndex();
        int EmptyIndex = EmptyBlock.GetSiblingIndex();
        if (index + 1 == EmptyIndex || index - 1 == EmptyIndex || index + 4 == EmptyIndex || index - 4 == EmptyIndex)
        {
            clickEnable = false;
            StartCoroutine(PuzzleMove(target));
        }
    }
    public void Submit()
    {
        Image[] puzzles = PuzzleStart.GetComponentsInChildren<Image>();
        for (int i = 0; i < puzzles.Length; i++)
        {
            string answer = "Puzzle" + ((i % 4) + (i * 4) + 1);
            if (puzzles[i].sprite != null && !puzzles[i].sprite.name.Equals(answer))
            {
                Debug.Log(answer + "\n" + puzzles[i].sprite.name);
                return;
            }
        }

    }
}
