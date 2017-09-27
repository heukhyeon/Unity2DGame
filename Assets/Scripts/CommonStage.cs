using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum MissionType { Count, Timer }
public delegate void VoidDelegate();
public abstract class Stage : MonoBehaviour
{
    //인트로 이벤트 전에 실행할 내용. 하위 Stage는 Awake나 Start를 사용하면 안되므로 해당 Action에 Awake나 Start에 사용할 코드를 넣는다.
    protected abstract Action BeforeIntro { get; }
    //인트로 이벤트 후, 해당 씬에서 자체적으로 재생할 이벤트
    protected abstract IEnumerator AfterIntro { get; }
    //Timer일시 1초마다 자동으로 라이프가 감소한다.
    public abstract MissionType missontype { get; }
    //스테이지 시작시 라이프값.
    public abstract int MaxLife { get; }
    //실패시 깎여나가는 라이프값. missiontype이 Timer여도 매초 깎여나가는값은 이 속성과 관계없다.
    public virtual int WestedLifeFail { get { return 1; } }
    //클리어시 감소할 전체 배터리 값. 
    public abstract int WestedLifeClear { get; }
    //게임오버시 감소할 전체 배터리 값
    public abstract int WestedLifeGameOver { get; }
    //LifeSystem의 버튼 활성화 여부. 
    public bool Submitenable { get { return lifeSystem.lifeUI.submit.interactable; } set { lifeSystem.lifeUI.submit.interactable = value; } }
    public AudioSource BackgroundMusic { get; private set; }
    LifeSystem lifeSystem;
    private void Awake()
    {
        lifeSystem = FindObjectOfType<LifeSystem>();//씬에 속한 LifeSystem을 가져온다.
        lifeSystem.stage = this; //해당 LifeSystem이 자신을 참조할수있도록 한다.
        BackgroundMusic = FindObjectOfType<Camera>().GetComponent<AudioSource>();
    }
    private void Start()
    {
        BeforeIntro(); //하위 클래스에서 정의한 Start메소드 실행.
        //인터페이스를 가져오고, 해당 인터페이스의 null여부에 따른 처리.
        //버튼을 없애고, LifeSystem의 이벤트 호출시점을 하위클래스가 직접 정의
        IDisableButton notbutton = GetComponent<IDisableButton>();
        //Clear이벤트 재생전에 먼저 하위 클래스가 구현한 이벤트를 재생.
        IBeforeClear before = GetComponent<IBeforeClear>();
        //버튼을 없애지 않는경우
        if (notbutton == null)
        {
            IClickDelay delay = GetComponent<IClickDelay>();//클릭 딜레이를 설정한다.
            ICustomButton button = GetComponent<ICustomButton>(); //버튼 이름과 버튼 처리 메소드를 따로 재정의한다.
            //버튼 처리를 기존 메소드에 따를경우
            // 기존 메소드 ->
            //1. 버튼을 클릭
            //2. 현재 씬의 상태가 하위 클래스에서 정의한 정답과 일치하는지를 반환
            //3. true인경우 클리어, false인경우 실패 이벤트 재생.
            if (button == null)
            {
                //ICustomButton이 없는경우 INoramlButton이 반드시 있을거라 가정한다.
                INormalButton normal = GetComponent<INormalButton>();
                //버튼 입력 처리 메소드
                Action action = new Action(() =>
                {
                    //정답인경우
                    if (normal.Answer)
                    {
                        //IBeforeClear와 IAfterClear는 둘 모두를 상속할수도, 모두 상속하지않을수도있으므로 List에 존재하는 이벤트를 담는방식으로 사용.
                        List<IEnumerator> actions = new List<IEnumerator>();
                        IAfterClear after = GetComponent<IAfterClear>();
                        IEnumerator last;//마지막 이벤트후 스테이지 선택으로 돌아가게끔 해야하므로 마지막을 지정해줄 변수를 생성.
                        if (before != null) actions.Add(before.BeforeClear);//BeforeClear가 있는경우 가장 먼저 BeforeClear를 추가.
                        if (after != null)//AfterClear가 있는경우
                        {
                            last = after.AfterClear;//가장 마지막 이벤트를 AfterClear로 지정.
                            actions.Add(lifeSystem.NormalClear);//일반 클리어가 last가 아니므로 직접 추가.
                        }
                        else last = lifeSystem.NormalClear;//AfterClear가 없는경우, 마지막을 일반 클리어 이벤트로 지정.
                        actions.Add(CorutineAction(last, () => { Invoke("ReturnMain", 1.5f); }));//마지막 이벤트 재생후 1.5초 뒤에 스테이지 선택으로 돌아가게끔 지정.
                        CombineCorutine(actions);//선택된 IEnumerator를 순차적으로 재생.
                    }
                    //오답인경우
                    else StartCoroutine(lifeSystem.FailEvent);//틀림 이벤트 재생.
                });
                if (delay == null) lifeSystem.lifeUI.submit.onClick.AddListener(() => { action(); });//딜레이가 없는경우, 누를때마다 Answer 상태를 판정.
                else lifeSystem.lifeUI.submit.onClick.AddListener(() => { if (delay.clickenable) action(); });//딜레이가 있는경우, clickenable상태일때만 Answer상태를 판정.
            }
            //버튼의 이름(Text)과 메소드를 직접 재정의 하는경우
            else
            {
                lifeSystem.lifeUI.submit.GetComponentInChildren<UnityEngine.UI.Text>().text = button.ButtonName; //버튼의 이름을 입력한 속성으로 바꿈.
                lifeSystem.lifeUI.submit.onClick.AddListener(() => { button.ButtonClick(); }); //버튼 클릭시 하위클래스가 정의한 함수를 실행함.
                button.FailEvent += new VoidDelegate(() => { StartCoroutine(lifeSystem.FailEvent); });//하위 클래스가 직접 LifeSystem의 FailEvent를 호출할수있게끔 인터페이스 event에 추가.
                ISectorClear sector = GetComponent<ISectorClear>();
                List<IEnumerator> actions = new List<IEnumerator>(); //button의 clearevent 진행순서를 담음.
                actions.Add(lifeSystem.NormalClear); //일단 기본 클리어를 담음.
                if (sector != null) actions.Add(CorutineAction(lifeSystem.SectorClear, sector.SectorClearAfter));//ISectorClear가 존재하는경우 섹터 클리어 이벤트와 이후 처리 메소드를 담음.
                button.ClearEvent += new VoidDelegate(() => { CombineCorutine(actions); }); //하위클래스에서 클리어이벤트를 자율적으로 호출하게끔 함.
            }
        }
        //버튼을 없애는경우
        else
        {
            Destroy(lifeSystem.lifeUI.submit.gameObject);//버튼 제거
            lifeSystem.lifeUI.submit = null;//LifeSystem에서 Destory된 버튼을 참조하지않도록 null화
            List<IEnumerator> clearlist = new List<IEnumerator>();
            if (before != null) clearlist.Add(before.BeforeClear);//IBeforeClear가 존재하는경우 일단 그것을 먼저 재생하게끔 추가.
            clearlist.Add(CorutineAction(lifeSystem.NormalClear, () => { Invoke("ReturnMain", 1.5f); })); //IDisableButton을 상속하는경우 IAfterClear를 상속하지 않는다 가정.
            //버튼이 없으므로 하위 클래스가 직접 ClearEvent와 FailEvent를 호출할수잇게끔 한다.
            notbutton.ClearEvent += new VoidDelegate(() => { CombineCorutine(clearlist); });
            notbutton.FailEvent += new VoidDelegate(() => { StartCoroutine(lifeSystem.FailEvent); });
        }
        //인트로 이벤트 -> 하위 클래스가 정의한 인트로 이후 이벤트 -> missiontype에 따른 LifeSystem처리.
        CombineCorutine(new IEnumerator[] { lifeSystem.IntroEvent, CorutineAction(AfterIntro, lifeSystem.GameStart) });
    }
    //메세지 등 구조체 내부 정보의 순서 셔플이 필요할때 사용.
    protected T[] InfoShuffle<T>(T[] info) where T : struct
    {
        T[] ret = new T[info.Length];
        for (int i = 0; i < info.Length; i++) ret[i] = info[i];
        for (int i = 0; i < info.Length; i++)
        {
            int index = UnityEngine.Random.Range(0, ret.Length);
            var temp = ret[i];
            ret[i] = ret[index];
            ret[index] = temp;
        }
        return ret;
    }
    protected void ReturnMain()
    {
        SmartPhone.LoadStage(SmartPhone.MainStage);
    }
    void CombineCorutine(List<IEnumerator> corutines)
    {
        StartCoroutine(CorutineConnect(corutines.ToArray()));
    }
    void CombineCorutine(params IEnumerator[] corutines)
    {
        StartCoroutine(CorutineConnect(corutines));
    }
    //전달된 IEnumerator를 순회하며 재생.
    IEnumerator CorutineConnect(IEnumerator[] coroutines)
    {
        foreach (IEnumerator cor in coroutines) yield return StartCoroutine(cor);
    }
    //IEnumerator 재생후 정의된 메소드 실행.
    IEnumerator CorutineAction(IEnumerator corutine, Action action)
    {
        yield return StartCoroutine(corutine);
        action();
    }
}
//버튼의 클릭 처리를 특정 타이밍에 막아야하는경우.
public interface IClickDelay
{
    bool clickenable { get; set; }
}
//버튼의 클릭후 메소드를 일반적인 방식에 따름.
public interface INormalButton
{
    bool Answer { get; }
}
//버튼의 이름(Text)와 클릭후 메소드를 재정의. 메소드가 재정의되므로 FailEvent를 직접 호출한다.
//현재 ISectorClear와 같이 사용되는 RollingBall에서만 사용되므로 ClearEvent는 추가하지않는다.
public interface ICustomButton
{
    string ButtonName { get; }
    event VoidDelegate FailEvent;
    event VoidDelegate ClearEvent;
    Action ButtonClick { get; }
}
//기존 클리어 이벤트전에 재생해야할 이벤트가 있는경우
public interface IBeforeClear
{
    IEnumerator BeforeClear { get; }
}
//클리어 이벤트 재생후에 재생할 이벤트가 있는경우.
//현재 적용되는 씬 없음.
public interface IAfterClear
{
    IEnumerator AfterClear { get; }
}
//클리어 이벤트가 여러번 나오는경우.
public interface ISectorClear
{
    Action SectorClearAfter { get; }
}
//버튼을 아예 사용하지 않는경우
public interface IDisableButton
{
    event VoidDelegate ClearEvent;
    event VoidDelegate FailEvent;
}
