
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
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
    public IEnumerator ButtonPrepare()
    {
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
        while(this.transform.localScale.x>0.01f)
        {
            SmartPhone.SizeSet(this.transform, -spd, 0.01f);
            spd += spd;
            yield return new WaitForEndOfFrame();
        }
    }
}

