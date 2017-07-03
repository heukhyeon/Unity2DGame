using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Timer : CustomUI {

    private Battery battery;
    private UnityEngine.UI.Text TimerText;
    private StringBuilder builder = new StringBuilder();
    private short cnt = 0;
    protected override void UIAwake()
    {
        TimerText = GetComponent<UnityEngine.UI.Text>();
    }

    // Use this for initialization
    void Start () {
        battery = SceneObjectManager.GetComponent<Battery>();
        StartCoroutine("StopWatch");
	}
    private void Update()
    {
        int hour = cnt / 60;
        int min = cnt % 60;
        builder.Length = 0;
        builder.AppendFormat("{0}:{1}", hour.ToString("D2"), min.ToString("D2"));
        TimerText.text = builder.ToString();
    }
    IEnumerator StopWatch()
    {
        while(true)
        {
            cnt++;
            battery.Decrease();
            yield return new WaitForSeconds(1f);
        }
    }
}
