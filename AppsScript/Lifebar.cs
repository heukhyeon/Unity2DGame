using System;
using System.Collections;
using UnityEngine;
public class Lifebar:CustomUI
{
    /// <summary>
    /// 생명바를 컨트롤할 객체. 생명바에 하트가 꽉 채워지면 무언의 동작을 취하기 위함.
    /// </summary>
    [SerializeField]
    private GameObject controllobject = null;
    /// <summary>
    /// 표시될 하트 객체
    /// </summary>
    [SerializeField]
    private GameObject Heart = null;
    /// <summary>
    /// 탑재한 오브젝트로부터 분리한 HeartbarController 인터페이스.
    /// </summary>
    private HeartbarController controller = null;
    private RectTransform panel;
    [SerializeField]
    private PlayerSpeechEvent speecheventer = null;
    private byte cnt = 0;
    protected override void UIAwake()
    {
        controller = controllobject.GetComponent(typeof(HeartbarController)) as HeartbarController; //인터페이스 분리
        if (controller == null) Debug.LogError("HeartbarController 인터페이스 미탑재 :" + controllobject.name);
    }
    private void Start()
    {
        StartCoroutine("Heartcompare");
    }
    private IEnumerator Heartcompare()
    {
        panel = GetComponent<RectTransform>();
        Vector2 size = panel.sizeDelta;
        size.y = ((CustomSceneManager.MAXHEARTS / 4) + 1) * 70; //한줄당 하트3개. 4개째가 되는순간 늘리는 방식을 사용.
        cnt = CustomSceneManager.MAXHEARTS;
        for(int i=0;i<CustomSceneManager.MAXHEARTS;i++)
        {
            GameObject obj = Instantiate(Heart, this.transform, false);
            obj.name = "Heart" + (i + 1);
            size.x = (i % 3) * 80 - 80;
            size.y = -((i / 3) * 50 + 35);
            obj.transform.localPosition = size;
            yield return new WaitForSeconds(0.5f);
        }
        controller.HeartCompareComplete();
        speecheventer.SetScript(SpeechStatus.Entry);
        speecheventer.gameObject.SetActive(true);
    }
    public void HeartDecrease()
    {
        Destroy(this.transform.GetChild(cnt - 1).gameObject);
        cnt--;
        if(cnt<=0)
        {
            speecheventer.gameObject.SetActive(true);
            speecheventer.SetScript(SpeechStatus.GameOver);
        }
        else
        {
            speecheventer.gameObject.SetActive(true);
            speecheventer.SetScript(SpeechStatus.Fail);
        }
    }
}
