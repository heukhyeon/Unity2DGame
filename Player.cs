using UnityEngine;
using UnityEngine.EventSystems;

public class Player:MonoBehaviour
{
    private SpriteRenderer spriterenderer;
    public Vector2 dir = Vector2.zero;
    public MainStageScene PlayUI;
    private bool isBottom = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isBottom = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isBottom = false;
    }
    private void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (dir.x != 0&&isBottom) Moving();
        if(Mathf.Abs(dir.y)==1f)
        {
            RaycastHit hit;
            if(Physics.Raycast(this.transform.position,this.transform.forward,out hit))
            {
                switch (hit.transform.tag)
                {
                    case "App":
                        if (dir.y == -1f) return;
                        TryWarp(hit.transform.GetComponent<AppIcon>());
                        break;
                    case "Rope":
                        Vector2 after = this.transform.position;
                        after.x = hit.transform.position.x;
                        after.y += dir.y * Time.deltaTime;
                        if (Physics.Raycast(after, this.transform.forward, out hit) && hit.transform.tag=="Rope") this.transform.position = after;
                        break;
                }
            }
        }
    }
    private void Moving()
    {
        spriterenderer.flipX = dir.x > 0 ? true : false;
        Vector2 pos = this.transform.position;
        pos.x += dir.x * Time.deltaTime;
        this.transform.position = pos;
    }
    private void TryWarp(AppIcon app)
    {
        if (StaticManager.SaveData.clearstate[app.category] == AppState.Rejected)
        {
            PlayUI.PadStop();
            PlayUI.SpeechUI.SpeechStart(PlayUI.rejectspeech, PlayUI.gameObject, EventState.Fail);
            return;
        }
        StaticManager.Nowappinfo.speecharray = app.appspeecharray;
        StaticManager.Nowappinfo.Dispose();
        switch (app.category)
        {
            case AppCategory.Message:
                StaticManager.Nowappinfo.messageinfo = app.messageinfo[Random.Range(0, app.messageinfo.Length - 1)];
                break;
            case AppCategory.Internet:
                StaticManager.Nowappinfo.internetinfo = app.internetinfo[Random.Range(0, app.internetinfo.Length - 1)];
                break;
        }
        StaticManager.moveScenetoLoading(app.category.ToString());
    }
}