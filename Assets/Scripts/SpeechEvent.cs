using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class SpeechEvent:MonoBehaviour
{
    public Text scripter;
    public Image character;
    public Sprite[] smiles;
    PlayerSpeech[] speechs;
    ISpeech GearBehaviour;
    int index = 0;
    public void SpeechStart(PlayerSpeech[] script,ISpeech gear)
    {
        speechs = script;
        GearBehaviour = gear;
        this.gameObject.SetActive(true);
        StartCoroutine("Speech");
    }
    public void Click()
    {
        StopCoroutine("Speech");
        index++;
        if (index >= speechs.Length)
        {
            this.gameObject.SetActive(false);
            index = 0;
            GearBehaviour.SpeechComplete();
        }
        else StartCoroutine("Speech");
    }
    IEnumerator Speech()
    {
        scripter.text = "";
        string word = speechs[index].text;
        character.sprite = smiles[speechs[index].imageindex];
        for (int i = 0; i < word.Length; i++)
        {
            scripter.text += word[i];
            yield return new WaitForEndOfFrame();
        }
    }

}

