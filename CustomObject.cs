using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 대부분의 객체에 기본적으로 탑재되는 Monobehavior아래 최상위 상속클래스. 
/// </summary>
public class CustomObject : MonoBehaviour {

    private void Awake()
    {
        SceneObjectManager.Add(this.gameObject);
    }

}


