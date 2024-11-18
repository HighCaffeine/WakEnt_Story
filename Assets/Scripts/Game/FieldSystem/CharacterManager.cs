using System;
using System.Collections;
using System.Collections.Generic;
using Devcat;
using UnityEngine;

public class CharacterManager : ObjectPooling<CharacterManager, Character>
{
    public enum CharacterInteractiveState
    {
        None = -1,
        CanInteractive = 0,   //자리에 있음
        CantInteractive = 1,  //상호작용중
    }

    public enum StageCharacterLimit
    {
        FirstStage = 6,
        SecondStage = 8,
        ThirdStage = 10,
    }

    public enum CharacterType
    {
        ISD,        //이세돌
        WAK,        //왁타버스
        Productor,  //작업자
    }

    public enum ISEGYEIDOL
    {
        Ine,
        JingBurger,
        Lilpa,
        Jururu,
        Gosegu,
        Viichan,
        Count,
    }

    public enum PathFindMode
    {
        SwapSeat,
        Random,
        MoveToMySeat,
        Count,
    }

    //현재 캐릭터에 이벤트 등록해서 사용할 예정이였는데,
    //list로 interactive가능한지 아닌지 판단만 하고
    //해당 정보는 캐릭터 데이터 내부에 가지고 있거나 해서 
    //데이터 저장하거나 그럴 때 위치로 ㅇㅇ
    public enum CharacterState
    {
        Fired,
        GoToWork,
        LeaveWork,
        
        
        Count,
    }


    private static int MaxMoveCharacterCount => 2;

    //이벤트 콜백 시 enum to int로 cast해서 list의 index 해당번들 전부 invoke
    public enum CharacterEventType
    {
        IsBroadcastPlanning,
        IsNotBroadcastPlanning,
        Count,
    }

    public enum CharacterSFXType
    {
        StatPopup,
    }

    public interface CharacterMovementEvent
    {
        void RegisterMovementEventToManager();
        void RegisterCharacterStateUpdate(OnUpdateCharacterState OnUpdateCharacterState);
        void RegisterGetPathEvent(OnGetPath OnGetPath);
        void RegisterUpdateSeatIndexEvent(OnUpdateSeatIndex OnUpdateSeatIndex);

        void RegisterCharacterCanInteractiveEvent(OnCharacterCanInteractive OnCharacterCanInteractive);
        void RegisterCharacterInteractiveSenderEvent(OnCharacterInteractiveSenderEvent OnCharacterInteractiveSenderEvent);
        void RegisterCharacterRequestSFXEvent(OnCharacterSFXRequestEvent OnCharacterSFXRequestEvent);
    }

    //좌석 변경 시 본인 좌석 번호를 넘겨줘서 등록/업데이트하는 이벤트
    //좌석 변경
    public delegate void OnUpdateSeatIndex(int seatNum, CharacterInteractiveState characterInteractiveState);
    public delegate Queue<Vector2> OnGetPath(Vector2 myPos, PathFindMode pathFindMode, int characterIndex, out int index, out Vector2 targetPos);
    public delegate void OnUpdateCharacterState(int index, CharacterState characterState);
    public delegate bool OnCharacterCanInteractive(int index);
    public delegate void OnCharacterMovementEvent(); //이벤트 매니저한테 등록해서 전체 콜백용.
    
    public delegate Sprite OnCharacterInteractiveSenderEvent(int index);   //캐릭터가 상호작용 후 interactiveevent측에서 띄울거임.
                                                                // 
    public delegate void OnCharacterSFXRequestEvent(CharacterSFXType sfxType);
    private List<OnCharacterMovementEvent> OnCharacterMovementEvents;







    //캐릭터들 위치 값 기록용인데
    //작업자를 고용하거나 캐릭터들위치를 변경하게 되면 해당되는 index(좌석 번호)에 기록 및 스위칭
    private List<Vector2> charactersPos;    
    [Header("캐릭터 출근 위치")] [SerializeField] private Transform characterStartPos;
    [Header("캐릭터 좌석")] [SerializeField] private Transform seatParent;
    private int[] characterStatusArr;                       //현재 캐릭터가 퇴근한 상태인지 출근한 상태인지
                                                            //해당 정보로 interactive 판단하는 것도 괜찮을 듯
                                                            //
    private List<int> characterSeatInteractable;

    private List<Vector2> characterSeatList;                //각 좌석 위치
    private List<int> seatNumberList;                       //맵에 추가된 캐릭터들의 각 좌석 번호
    private int currentStageCharacterCount = 0;             //현재 스테이지 캐릭터 수
                                                            //해고 기능의 경우 -처리 해야 함

    private StageCharacterLimit currentStageCharacterLimit; //현재 스테이지 캐릭터 수 제한
    private int interactiveCharacterCount;                  //현재 상호작용하려고 하는 캐릭터의 수
                                                            //해당 캐릭터가 자리에 앉을 때까지 오래걸릴 수도 있기 때문에(맵크기가 좀 클 수도 있음)
                                                            //그래서 미리빼고 characterseatinteractable 값을
                                                            //target이였던 애는 풀어주고 본인자리 복귀중인 캐릭터는 자리 앉을 때 돌려주는 걸로

    private new void Awake()
    {
        currentStageCharacterLimit = StageCharacterLimit.FirstStage;

        Init();

        base.Awake();


        //해당 List는 json파일에 캐릭터들 좌석 위치를 정할 거임.
        //좌석(캐릭터)에 interactive할 시 캐릭터 interactive에 본인 위치도 넣어둘 거임.

        //charactersPos
    }

    private new void Start()
    {
        StartCoroutine(SetCharacterInfo());
    }

    private void Init()
    {
        OnCharacterMovementEvents = new List<OnCharacterMovementEvent>();

        charactersPos = new List<Vector2>();
        characterSeatList = new List<Vector2>();
        characterSeatInteractable = new List<int>();
        seatNumberList = new List<int>();

        //스테이지 구분 넣어서 스테이지마다 다르게
        int characterArrSize = ValueCastTo<int>.From(ISEGYEIDOL.Count) + ValueCastTo<int>.From(StageCharacterLimit.FirstStage);

        characterStatusArr = new int[characterArrSize];

        for (int i = 0; i < characterArrSize; i++)
        {
            characterStatusArr[i] = -1;
        }

        UpdateSeatList();
    }

    //스테이지 변경 시 실행
    private void UpdateSeatList()
    {
        //현재 스테이지 캐릭터 수 제한 - 현재 캐릭터 위치 리스트 수 -> 추가되는 좌석  
        int listCount = charactersPos.Count;
        int count = ValueCastTo<int>.From(currentStageCharacterLimit) - listCount;

        for (int i = 0; i < count; i++)
        {
            charactersPos.Add(Vector2.zero);
            characterSeatInteractable.Add(ValueCastTo<int>.From(CharacterInteractiveState.None));
        }
    }

    private void SetSeatPos()
    {
        for (int i = 0; i < ValueCastTo<int>.From(StageCharacterLimit.FirstStage); i++)
        {
            Transform seat = seatParent.GetChild(i);

            if (seat)
            {
                Vector2 nodePos = PathFinding.Instance.GetNodePos(seat.position);

                characterSeatList.Add(nodePos);
            }
        }
    }

    //test
    public List<Action> testaction = new List<Action>();

    public void Test_ResetCharacterState()
    {
        foreach (var action in testaction)
        {
            action?.Invoke();
        }
    }
    //test


    /// <summary>
    /// 
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="pathFindMode"></param>
    /// <param name="characterIndex">특정위치(본인위치 / 캐릭터 자리 변경(상대index))로 갈 경우에만 사용</param>
    /// <param name="index">타겟리스트 번호</param>
    /// <returns></returns>
    public Queue<Vector2> GetPath(Vector2 startPos, PathFindMode pathFindMode, int characterIndex, out int index, out Vector2 targetPos)
    {
        targetPos = Vector2.zero;

        index = -1;

        switch(pathFindMode)
        {
            case PathFindMode.SwapSeat:
            //swapseat함수에서 자리 변경 시 해당 선택캐릭터를 대상캐릭터의 인덱스로 넣어서 사용
            //대상캐릭터에게도 선택 캐릭터의 index보내서 이동
            targetPos = GetRandomCharacterPos(characterIndex, out index);

            return PathFinding.Instance.CurvedPathFind(startPos, targetPos);

            case PathFindMode.Random:
            //현재 상호작용 요청 캐릭터가 둘 이상이거나 캐릭터 자체가 둘 미만이거나 내가 누군가에게 상호작용 요청을 받았을 경우
            if (MaxMoveCharacterCount * 2 - 1 <= interactiveCharacterCount || (currentStageCharacterCount - interactiveCharacterCount) < 2
                || characterSeatInteractable[characterIndex] == ValueCastTo<int>.From(CharacterInteractiveState.CantInteractive))
            {
                return null;
            }

            targetPos = GetRandomCharacterPos(characterIndex, out index);
            
            UpdateSeatInterState(index + 1, CharacterInteractiveState.CantInteractive);

            //테스트용
            targetedList.Add(new KeyValuePair<Vector2, Vector2>(startPos, targetPos));

            return PathFinding.Instance.CurvedPathFind(startPos, targetPos);

            case PathFindMode.MoveToMySeat:
            targetPos = characterSeatList[characterIndex];

            return PathFinding.Instance.CurvedPathFind(startPos, targetPos);

            default:
            return null;
        }
    }


    //내부에서 2 캐릭터가 동일 타겟 또는 이동하는 캐릭터로 안 가도록 내부 체크 추가
    private Vector2 GetRandomCharacterPos(int characterIndex, out int targetIndex)
    {
        Vector2 newPos = Vector2.zero;
        targetIndex = -1;

        while (true)
        {
            int index = seatNumberList[UnityEngine.Random.Range(0, currentStageCharacterCount)];
            newPos = charactersPos[index];

            targetIndex = index;

            if (targetIndex == characterIndex 
                || characterSeatInteractable[index] == ValueCastTo<int>.From(CharacterInteractiveState.CantInteractive))
            {
                continue;
            }

            if (characterSeatInteractable[index] == ValueCastTo<int>.From(CharacterInteractiveState.CanInteractive))
            {
                break;
            }
        } 

        return newPos;
    }

    public bool IsCanMoveForInteractive(int index)
    {
        return characterSeatInteractable[index - 1] == ValueCastTo<int>.From(CharacterInteractiveState.CanInteractive);
    }

    //캐릭터 상호작용 가능 여부
    public void UpdateSeatInterState(int seatNum, CharacterInteractiveState characterInteractiveState)
    {
        characterSeatInteractable[seatNum - 1] = ValueCastTo<int>.From(characterInteractiveState);

        switch (characterInteractiveState)
        {
            case CharacterInteractiveState.CanInteractive:
            interactiveCharacterCount--;
            break;
            case CharacterInteractiveState.CantInteractive:
            interactiveCharacterCount++;
            break;
        }
    }

    private void SelectTargetSeat()
    {
        //gamemanager에게 게임 pause 요청
        GameManager.Instance.PauseGame();

        //플레이어가 좌석을 선택을 할 거임.
        //Ray쏴서 체크하지 않을까 싶음
    }
    
    //자리 변경
    //좌석위치에 따라 인덱스 관리를 할 거고
    //해당 swap으로 위치(characterPos), 작업자들 정보(characterInfo), 작업자들 이벤트정보 (characterEvent)바꿔줄 거
    //바꾸면서 경로 지정해줄 거임.
    private void SwitchPos(int before, int after)
    {
        QuickSort.Swap(charactersPos, before, after);
    }

    /*
        기본적으로 캐릭터들의 자리 위치들을 알고 있어야 함.
        방송 제작중이 아닐 경우 게임 로딩 후 시작지점에서 출근 시킴
        방송 제작중일 경우 기존 본인 자리에서 시작.

        데이터 테이블에 작업자 목록들을 ProductorManager가 캐싱하면서 
        여기에다가 이동에 필요한 애들만 캐싱할 예정

        캐릭터가 자체로 이동 요청을 보낼거라 본인을 보낼건데 나머지의 출근 여부 및 이동중인지 체크해서
        타겟 정해주는 방식으로 진행하면 될

        데이터들 캐싱 방식만 정하면 될 듯
        ->Productor와 Movement 둘 다 관리하는 객체를 만들거나
        Movement랑 Productor 둘 다 캐싱하고 보내주거나 ㅇㅇ
    */


    //캐릭터무브먼트의 이벤트들을 받아서 
    //알려줄 것들(방송이 제작중이거나)등등 필요없나 싶기도 하고


    public delegate bool OnCharacterIsMove();
    //캐릭터에게 받은 요청 메세지 매니저에게 넘겨줌, callback => 메세지 종료 때 부를 함수들
    public void RequestPopupMessage(string message, Transform target, OnCharacterIsMove OnCharacterIsMove, params Action[] callback)
    {
        CharacterMessageManager.Instance.SetMessage(message, target, OnCharacterIsMove, callback);
    }
    
    //pooling 값 가져와서 필드 캐릭터들 수 만큼 불러와서 각자 좌석 번호 배정

    public IEnumerator SetCharacterInfo()
    {
        /*
        데이터 테이블에서
        좌석번호랑, 이세돌인지, 필드에 나와있는지, 데이터가 저장 기능에 사용할 각 캐릭터별 ID를 포함하고 있음.

        데이터 변경 시 해당 ID로 해쉬테이블 변경하고, 불러올 경우 데이터 매니저에게 해당 ID로 데이터 받아옴.

        저장 시 업데이트 된 Json파일로 Copy
        */

        List<CharacterData> characterDataList = JsonManager.Instance.GetCharacterData();

        SetSeatPos();

        foreach (var data in characterDataList)
        {
            if (data.isFieldCharacter == "O")
            {
                Character character = GetPool();

                character.transform.name = data.Name;

                character.transform.position = characterStartPos.position;

                SetCharacterData(character, data.CharacterID, data);

                //캐릭터 데이터 내부에 캐릭터가 마지막에 어떤 상태였는지등 적용해줄거
                //위치(이동 중 종료할 수도 있기 때문에)값 현재 타겟으로 이동중인 곳 위치(POS) 저장
                //fever 상태였을 경우 계산된 fever카운트, 현재 fever진행 수치
                //characterInteractiveState.Add(ValueCastTo<int>.From(CharacterInteractiveState.None));

                //방송 제작중이 아니라면 다시 출근시킴 -> 나중에 출근여부 확인해서 본인 좌석번호에서 시작하는 걸로 수정
                character.ReturnMySeat();

                //본인좌석에 해당하는 위치값 넣어줌
                charactersPos[data.SeatNumber - 1] = characterSeatList[data.SeatNumber - 1];

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    private void SetCharacterData(Character character, int characterID, CharacterData characterData)
    {
        ResourceID resourceID = ValueCastTo<ResourceID>.From(characterID);

        Sprite sprite = DataManager.Instance.GetSpriteFromID(resourceID, ResourceType.DefaultSprite);

        character.SetData(sprite, characterData, characterID);
        seatNumberList.Add(characterData.SeatNumber - 1);

        currentStageCharacterCount++;
    }

    public void RegisterCharacterEvent(params OnCharacterMovementEvent[] OnCharacterMovementEvent)
    {

        foreach (var eventData in OnCharacterMovementEvent)
        {
            this.OnCharacterMovementEvents.Add(eventData);
        }
    }
    
    //스테이지에 허용된 캐릭터 수 만큼 배열을 처음에 세팅할 거 -1로
    //Fired         -   해고하면
    //GoToWork      -   출근하면 배열에 해당 캐릭터에 배정한 index의 값 1로 변경
    //LeaveWork     -   퇴근하면 배열에 해당 캐릭터에 배정한 index의 값 2로 변경 

    public void SetCharacterStatus(int index, CharacterState characterState)
    {
        switch (characterState)
        {
            case CharacterState.Fired:
            Fired(index);
            break;
            case CharacterState.GoToWork:
            GoToWork(index);
            break;
            case CharacterState.LeaveWork:
            LeaveWork(index);
            break;
        }
    }

    private void Fired(int index)
    {
        characterStatusArr[index] = ValueCastTo<int>.From(CharacterState.Fired);
    }
    private void GoToWork(int index)
    {
        characterStatusArr[index] = ValueCastTo<int>.From(CharacterState.GoToWork);
    }
    private void LeaveWork(int index)
    {
        characterStatusArr[index] = ValueCastTo<int>.From(CharacterState.LeaveWork);
    }

    public AnimationClip GetAnimationClip(long characterID, ResourceType resourceType)
    {
        ResourceID resourceID = ValueCastTo<ResourceID>.From(characterID);

        return DataManager.Instance.GetAnimationClipFromID(resourceID, resourceType);
    }

    public Sprite GetSpriteFromID(long characterID, ResourceType resourceType)
    {
        ResourceID resourceID = ValueCastTo<ResourceID>.From(characterID);

        return DataManager.Instance.GetSpriteFromID(resourceID, resourceType);
    }

    //캐릭터 이벤트 함수 호출(방송 제작 상태 업데이트)
    public void CallBackEvent(CharacterEventType characterEventType)
    {
        for (int i = ValueCastTo<int>.From(characterEventType); i < this.OnCharacterMovementEvents.Count; i += ValueCastTo<int>.From(CharacterEventType.Count))
        {
            this.OnCharacterMovementEvents[i]?.Invoke();
        }
    }

    //맵의 경우 프리팹 브러쉬 사용해서 세팅할거고 좌석 세팅은 어쩔 수 없이 수동으로 좀 세팅을 해야할 듯.
    //자체 코드 컴포넌트 붙여서 애니메이션 사용하고 

    //레이 체크
    //아래 방법으로 위치를 찍을건데, 좌석 번호를 받는 방법이 필요함.
    //hit point위치 찍어서 Node를 받아오면
    //Node에 좌석 번호를 세팅하긴 해야할 듯, 그러고 노드 세팅할 때 노드 내부에 값 세팅 
    //->seat layer로 좌석들 바꾸고 해당 레이어일 경우에 좌석 번호 넣고 1씩 증가시키는걸로
    private IEnumerator RaySeat()
    {
        //내부에서 체크해서 본인 자리 아닌지 체크
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                RaycastHit2D hit = Physics2D.Raycast(mousePosition + Vector3.forward * 5f, Vector3.forward, 10f);

                if (hit.collider != null)
                {
                    Debug.Log(hit.point);
                }

            }

            yield return new WaitForFixedUpdate();
        }
    }


    /*
    상호작용 함수들
    캐릭터 info를 받아서 해야함.
    캐릭터가 몇 번 자리에 갔는지를 정하는 걸로 해야할 듯.
    최대치 배율로 관리를 하고 자리 번호를 인덱스로 사용해서 바꾸는 형식으로 사용
    본인이 몇 번 자리인지를 알고 있어야 하고 해당 정보는 데이터 베이스로 가지고 있어야 함.
    변경 시 업데이트 필요
    */
    public void AddBroadcastStat(int index, params Action<int>[] callback)
    {
        //productorManager 호출해주고 메세지 매니저한테도 전달
        //매니저의 콜백함수를 넘겨줄 수도 -> 메세지 딜레이 처리 대신 콜백으로 
        //interactive target 좌석 번호를 넘겨야 됨
        int amount = ProductorManager.Instance.AddStatFieldInteractive(index);

        if (amount < 0)
        {
            return;
        }

        //해당 캐릭터 인덱스 아니깐거기로 넘겨주는걸로 ㅇㅇ
        foreach (var eventData in callback)
        {
            //현재는 수치로 추가하는 것 뿐임.
            
            eventData?.Invoke(amount);
        }
    }

    public void RequestSFX(CharacterSFXType sfxType)
    {

    }

    public Sprite ReqPopupStat(int index)
    {
        return null;
    }

    //productormanager에게 넘겨서 characterData와 ProductorInfo index 오차 없도록
    //

    public void SwapSeat(int a, int b)
    {
        //따로 자리 변경 후 길 찾아가라고 이벤트 호출시킬거임 타겟만 전달
        //pool객체 가져올 때 해당 객체의 길 찾는 
    }


    List<Vector2> test = new List<Vector2>();
    List<KeyValuePair<Vector2, Vector2>> targetedList = new List<KeyValuePair<Vector2, Vector2>>();
    private Color[] t_color = {Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.red};

    private void OnDrawGizmos() 
    {
        if (test != null)
        {
            int index = 0;
            foreach (var pos in targetedList)
            {
                index++;

                index %= t_color.Length;

                Gizmos.color = t_color[index];

                Gizmos.DrawWireSphere(pos.Key, 0.25f);
                Gizmos.DrawWireSphere(pos.Value, 0.5f);
            }
        }
    }

}