
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
//계산기 버튼 12개를 관리한다.
public class CalculatorButtonSpace : MonoBehaviour
{
    [HideInInspector]
    public CalculatorScene scene;
    List<Button> buttons;
    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>().ToList();
    }
    public void ValueInput(string val)
    {
        scene.NowValue += val;
    }
    public void Erase()
    {
        scene.NowValue = scene.NowValue.Remove(scene.NowValue.Length - 1);
    }
    //Stage의 AfterIntro에 대응. 
    public IEnumerator ButtonPrepare()
    {
        //각 버튼에 클릭 메소드 부여후 활성화 효과
        for (int i = 0; i < 12; i++)
        {
            buttons[i].interactable = true;
            switch (i)
            {
                case 9:
                    buttons[i].onClick.AddListener(() => { scene.Submit(); });
                    break;
                case 10:
                    buttons[i].onClick.AddListener(() => { ValueInput("0"); });
                    break;
                case 11:
                    buttons[i].onClick.AddListener(() => { Erase(); });
                    break;
                default:
                    string cnt = (i + 1).ToString();
                    buttons[i].onClick.AddListener(() => { ValueInput(cnt); });
                    break;
            }
            yield return new WaitForSeconds(5f * Time.deltaTime);
        }
    }
    public IEnumerator ButtonUnPreapre()
    {
        //임의의 버튼을 비활성화시킴.
        while (buttons.Count > 0)
        {
            if (buttons.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, buttons.Count);
                buttons[index].interactable = false;
                buttons.RemoveAt(index);
            }
            yield return new WaitForEndOfFrame();
        }
        Vector2 scale = this.transform.localScale;
        float spd = Time.deltaTime;
        //버튼 영역 전체를 화면안쪽으로 밀음.
        while(this.transform.localScale.x>0.01f)
        {
            SmartPhone.SizeSet(this.transform, -spd, 0.01f);
            spd += spd;
            yield return new WaitForEndOfFrame();
        }
    }
}

