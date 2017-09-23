using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SNSLoading : MonoBehaviour {
    [SerializeField]
    Text emblem;
    [SerializeField]
    Text content;
    [SerializeField]
    RectTransform value;
    int cnt = 0;
    public IEnumerator IntroEvent()
    {
        Color color = emblem.color;
        while(color.a<1)
        {
            color.a += Time.deltaTime;
            emblem.color = color; ;
            yield return new WaitForEndOfFrame();
        }
        content.color = color;
        value.sizeDelta = new Vector2(0, 48);
        value.parent.gameObject.SetActive(true);
        Vector2 pos = value.sizeDelta;
        float speed = Time.deltaTime / 10f;
        string jum = "....";
        int cnt2 = 0;
        while(pos.x<1000)
        {
            pos.x += speed;
            if (pos.x > 1000) pos.x = 1000;
            value.sizeDelta = pos;
            cnt++;
            if (cnt > 10)
            {
                cnt2++;
                speed += speed;
                if (cnt2 > 4) cnt2 = 0;
                content.text = "당신의 흑역사를 로딩중" + jum.Substring(0, cnt2);
                cnt = 0;
            }
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
