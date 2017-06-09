using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 객체를 계속해서 뱉어내는 실험용 객체
/// </summary>
public class StartLine : MonoBehaviour {
    [SerializeField]
    private GameObject enemy = null; //생성할 객체
    [SerializeField]
    private bool xFlip = false; //true일시 오른쪽으로 향하게 생성
    [SerializeField]
    private float delay = 1f;
    private int cnt = 0;
    private void Start()
    {
        SpriteRenderer spriterender = enemy.GetComponent<SpriteRenderer>();
        spriterender.flipX = xFlip; //객체 x축 반전여부를 인스펙터에서 지정한 bool변수에 맞춘다. (양산은 기존의 복제로 사용되므로 양산된 객체들도 동일한 x축반전유지)
        StartCoroutine("Creating"); //양산 작업 시작.
    }
    /// <summary>
    /// 지정한 객체를 계속해서 양산한다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Creating()
    {
        for(int i=0;i<1000;i++)
        {
            GameObject obj= Instantiate(enemy, this.transform, false);
            obj.name = enemy.name + cnt;
            cnt++;
            yield return new WaitForSeconds(delay);
        }
    }
}
