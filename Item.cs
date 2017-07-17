using UnityEngine;
using UnityEngine.UI;

public class Item:MonoBehaviour
{
    public ItemType type;
    public string name;
    public string content;
    public Image border;
    private void Start()
    {
        border = this.transform.parent.GetComponent<Image>();
    }
}