#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MainStageScene:MonoBehaviour
{
    [SerializeField]
    private SpeechData[] introspeech;
    [SerializeField]
    private SpeechData[] messageclearspeech;
    [SerializeField]
    private SpeechData[] internetclearspeech;
    public SpeechData[] rejectspeech;
    public RectTransform Pad;
    public RectTransform PadStick;
    public RectTransform Battery;
    public RectTransform Upperbar;
    public RectTransform InventoryUI;
    public RectTransform InventoryReturnBlock;
    public SpeechEvent SpeechUI;
    public Inventory Inventory;
    public Player Player;
    public GameObject PrologueRope;
    private Vector2 Stickcenter;
    private Vector2 refVector = new Vector2(0, -1000);
    private bool inputenable = false;
    private float radius;
    private void Awake()
    {
        int value = StaticManager.SaveData.battery;
        Battery.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = value + "%";
        (Battery.GetChild(1) as RectTransform).sizeDelta = new Vector2((Battery.sizeDelta.x / 100) * value, 30);
        Stickcenter = Pad.position; //스틱 위치를 알기위한 원점 지정.
        radius = Pad.sizeDelta.x * 0.5f; //반지름
        if (!StaticManager.SaveData.Prologueclear) PrologueEventProcess();
        Inventory.Setting();
    }
    private void PrologueEventProcess()
    {
        PrologueRope.SetActive(false); //프롤로그 완료전까진 위층으로 이동할수없도록 로프를 비활성화
        //1단계. 메세지 앱이 진입 거부 상태인경우 = 처음하기로 진입한 상태
        if(StaticManager.SaveData.clearstate[AppCategory.Message]==AppState.Rejected)
        {
            StaticManager.SaveData.clearstate[AppCategory.Message] = AppState.NotEnter;
            SpeechUI.SpeechStart(introspeech, this.gameObject, EventState.Intro);
        }
        //2단계. 메세지 앱이 클리어 상태
        else if(StaticManager.SaveData.clearstate[AppCategory.Message]==AppState.Clear)
        {
            //2.5단계 : 인터넷 앱이 진입 거부 상태 = 메세지 앱 클리어 직후
            if(StaticManager.SaveData.clearstate[AppCategory.Internet]==AppState.Rejected)
            {
                StaticManager.SaveData.clearstate[AppCategory.Internet] = AppState.NotEnter;
                StaticManager.SaveData.itemlist.Add("Battery2");
                SpeechUI.SpeechStart(messageclearspeech, this.gameObject, EventState.Intro);
            }
            //3단계. 메세지 앱이 클리어 상태 = 프롤로그 완료
            else if(StaticManager.SaveData.clearstate[AppCategory.Internet]==AppState.Clear)
            {
                PrologueRope.SetActive(true);
                SpeechUI.SpeechStart(internetclearspeech, this.gameObject, EventState.Intro);
                StaticManager.SaveData.Prologueclear = true;
                //나머지 앱들의 진입 금지를 모두 해제한다.
                AppCategory[] temp = new AppCategory[StaticManager.SaveData.clearstate.Count];
                StaticManager.SaveData.clearstate.Keys.CopyTo(temp, 0);
                for (int i = 0; i < temp.Length; i++)
                    if (StaticManager.SaveData.clearstate[temp[i]] == AppState.Rejected) StaticManager.SaveData.clearstate[temp[i]] = AppState.NotEnter;
            }
        }
    }
    public void PadMoveStart(BaseEventData eventdata)
    {
        inputenable = true;
        PadMove(eventdata);
    }
    public void PadMove(BaseEventData eventdata) //패드 터치시 발생하는 이벤트
    {
        if (!inputenable) return;
        PointerEventData pointer = eventdata as PointerEventData;
        Vector2 pos = (pointer.position - Stickcenter).normalized;
        if (Mathf.Abs(pos.x) >= Mathf.Abs(pos.y))
        {
            pos.y = 0f;
            if (pos.x > 0) pos.x = 1;
            else if (pos.x < 0) pos.x = -1;
        }
        else
        {
            pos.x = 0f;
            if (pos.y > 0) pos.y = 1;
            else if (pos.y < 0) pos.y = -1;
        }
        PadStick.localPosition = pos * radius;
        Player.dir = pos;
    }
    public void PadStop()//패드 터치를 해제했을때 발생하는 이벤트
    {
        inputenable = false;
        PadStick.localPosition = Vector3.zero;
        Player.dir = Vector2.zero;
    }
    public void UpperbarDrag(BaseEventData data)
    {
        Vector2 pos = (data as PointerEventData).position;
        if(pos.y>25)InventoryUI.position = new Vector2(InventoryUI.position.x, pos.y);
    }
    public void UpperbarTouchEnd(BaseEventData data)
    {
        Vector2 pos = (data as PointerEventData).position;
        if (pos.y > 100) InventoryUI.localPosition = new Vector2(0, 2000);
        else
        {
            InventoryUI.position = new Vector2(InventoryUI.position.x, 25);
            InventoryReturnBlock.gameObject.SetActive(true);
        }
    }
    public void InventoryReturn(BaseEventData data)
    {
        Vector2 pos = (data as PointerEventData).position;
        if (pos.y > 25) InventoryUI.position = new Vector2(InventoryUI.position.x, pos.y);
    }
    public void InventoryReturnEnd(BaseEventData data)
    {
        Vector2 pos = (data as PointerEventData).position;
        if (pos.y > 640)
        {
            InventoryUI.localPosition = new Vector2(0, 2000);
            InventoryReturnBlock.gameObject.SetActive(false);
        }
        else InventoryUI.localPosition = new Vector2(0, -590); 
    }
   
}

