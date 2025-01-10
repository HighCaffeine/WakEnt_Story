using System;
using System.Collections;
using System.Collections.Generic;
using Devcat;
using UnityEngine;

public class Character : MonoBehaviour, 
                                CharacterManager.CharacterMovementEvent,
                                OnReturnPool<Character>
{
    /*
    statemachine 코루틴 운용
    ㄴ캐릭터가 필드에 나올 때 (출근 시) 내부에 출근 bool같은 거 하나 만들어서
        예외처리 해주고, 동시에 본인 자리로 ㄱ
    Idle - Move - Interactive - Work

    Idle
    컴퓨터에 앉아있는 애니메이션 Idle애니메이션으로 앉아서 주위 가끔씩 돌아보는 느낌
    매니저에게 이동 요청 보내서 다른 캐릭터와 상호작용
    매니저 내부에서 최대이동 캐릭터는 2개로 제한 타겟은 랜덤 지정(안겹치게)

    Move
    매니저 통해서 path받으면 Move상태로 전환
    본인자리인지 아닌지 판단 후 Interactive / Idle상태로 변경

    Interactive
    내부에서 overlap하여 interactiveevent접근해서 이벤트 호출
    interactive overlap layer로 판단 -> interactable layer

    *Astar내부 노드는 다중레이어로 처리

    Work 
    broadcastplanning쪽에서 내부 변수로 현재 방송 제작중인지 아닌지 추가할거.
    해당 정보를 캐릭터매니저 통해서 work 방식 결정
    ->매니저 통하거나 static 변수로 처리로 할 수도

    방송제작이 시작 됐다고 매니저 측에서 확인되면, 이벤트 호출해서 현재 맵에 잇는 
    캐릭터들을 work상태로 변경

    


    */

    public const float MAXFEVERYIELDTIME = 0.5f; 
    public const float MINFEVERYIELDTIME = 0.1f;
    public const float ANISPEED = 1f;

    [Header("Resources")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CharacterManager.ISEGYEIDOL iSEGYEIDOL;
    [SerializeField] private Animator animator;

    private AnimatorOverrideController aoc;

    [SerializeField]private Sprite[] characterSprites;

    private CharacterData characterData;
    private ProductorInfo productorInfo;

    private Transform imageTransform;

    private Queue<Action> callActionAfterSit = new Queue<Action>();

    enum State
    {
        None,
        Idle,
        Move,
        Interactive,
        Work,
        Sit,
    }

    enum SpriteType
    {
        Standing = 0,
        SitFront = 1,
        SitBack = 2,
    }

    enum AniState
    {
        Walk,            //걷는 상태인지 여부
        Interactive,        //상호작용 상태인지 여부
        Front,              //Front 애니메이션 실행 필요 여부
        Back,               //Back 애니메이션 실행 필요 여부
        Work,               //Work 상태인지 여부
        Idle,               //Idle 상태인지 여부
        Sit,                //앉아있는 상태인지 여부
        IdleNum,            //1 -> Stretching, 2 -> LookAround
        ChangeIdleAni,      //Stretching <-> LookAround 전환용

        AniSpeed,           //앉는 애니메이션 반대로 재생할려고 만듦
    }

    enum IdleAniType
    {
        None            = 0,
        Stretching      = 1,
        LookAround      = 2,
        Count,
    }

    enum MoveType
    {
        Target,
        MySeat,
    }

    [SerializeField] private State currentState;

    //캐릭터가 나갈 때가 퇴근하는 거랑 교체하는 거 2개가 있음/
    //퇴근의 경우 그대로 다시 켜서 위치 지정하고 가면 되고,
    //교체의 경우 매니저측에서 할 거니깐 바꾼 대상에게 설정을 해줄거라서 
    //여기서 하는 건 본인이 매니저의 어떤 인덱스에 있는 데이터인지만 다시 콜백으로 넘겨줌
    private void SwapCharacter()
    {

    }

    private bool isFirstMoveFromSetData;

    public void SetData(Sprite sprite, CharacterData characterData, int characterID)
    {
        this.characterData = characterData;
        spriteRenderer.sprite = sprite;

        if (characterData.IsIsegyeIdol == "O")
        {
            iSEGYEIDOL = ValueCastTo<CharacterManager.ISEGYEIDOL>.From(characterID - ResourceID.Character_ISD_Ine);
        }

        imageTransform = transform.GetChild(0);

        characterSprites = new Sprite[ValueCastTo<long>.From(ResourceType.SpriteCount)];

        characterSprites[ValueCastTo<int>.From(SpriteType.Standing)] = CharacterManager.Instance.GetSpriteFromID(characterData.CharacterID, ResourceType.DefaultSprite);     //standing
        characterSprites[ValueCastTo<int>.From(SpriteType.SitFront)] = CharacterManager.Instance.GetSpriteFromID(characterData.CharacterID, ResourceType.SitFrontSprite);    //sitFront
        characterSprites[ValueCastTo<int>.From(SpriteType.SitBack)] = CharacterManager.Instance.GetSpriteFromID(characterData.CharacterID, ResourceType.SitBackSprite);     //sitback


        aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = aoc;
        
        aoc[ResourceFileName.StandingIdleAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.StandingIdleAni);
        aoc[ResourceFileName.InteractiveAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.InteractiveAni);
        aoc[ResourceFileName.SitAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.SitAni);
        aoc[ResourceFileName.WalkAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.WalkAni);

        aoc[ResourceFileName.BackIdleLookAround.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.BackIdleLookAroundAni);
        aoc[ResourceFileName.BackIdleStretching.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.BackIdleStretchingAni);
        aoc[ResourceFileName.BackWork.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.BackWorkAni);
        aoc[ResourceFileName.FrontIdleLookAround.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.FrontIdleLookAroundAni);
        aoc[ResourceFileName.FrontIdleStretching.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.FrontIdleStretchingAni);
        aoc[ResourceFileName.FrontWork.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.FrontWorkAni);

        AnimationSpeedSet(false);
        animator.writeDefaultValuesOnDisable = false;
        
        isRight = true;
        isFirstMoveFromSetData = true;

        UpdateSeatState(CharacterManager.CharacterInteractiveState.CantInteractive);


        CharacterManager.Instance.testaction.Add(TEST_TurnToMoveState);
        CharacterManager.Instance.RegisterPauseEvent(PauseAction);

        productorInfo = CharacterManager.Instance.GetProductorInfo(characterData.SeatNumber - 1);
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGamePause)
        {
            return;
        }

        switch (currentState)
        {
            case State.Idle:
            Idle();
            break;
            case State.Move:
            Move();
            break;
            case State.Interactive:
            Interactive();
            break;
            case State.Work:
            Work();
            break;
            default:
            return;
        }
    }

    private int targetIndex;
    private Coroutine moveCoroutine;

    private bool isOnMySeat = false;        //내 좌석에 앉아있는지 체크용

    private bool WalkAni { get { return animator.GetBool(AniState.Walk.ToString()); } set { animator.SetBool(AniState.Walk.ToString(), value); } }
    private bool InteractiveAni { get { return animator.GetBool(AniState.Interactive.ToString()); } set { animator.SetBool(AniState.Interactive.ToString(), value); } }
    private bool FrontAni { get { return animator.GetBool(AniState.Front.ToString()); } set { animator.SetBool(AniState.Front.ToString(), value); } }
    private bool BackAni { get { return animator.GetBool(AniState.Back.ToString()); } set { animator.SetBool(AniState.Back.ToString(), value); } }
    private bool WorkAni { get { return animator.GetBool(AniState.Work.ToString()); } set { animator.SetBool(AniState.Work.ToString(), value); } }
    private bool IdleAni { get { return animator.GetBool(AniState.Idle.ToString()); } set { animator.SetBool(AniState.Idle.ToString(), value); } }
    private bool SitAni { get {return animator.GetBool(AniState.Sit.ToString());} set { animator.SetBool(AniState.Sit.ToString(), value); } }
    private int IdleNumAni { get { return animator.GetInteger(AniState.IdleNum.ToString()); } set { animator.SetInteger(AniState.IdleNum.ToString(), value); } }
    private void SetChangeIdleTrigger() { animator.SetTrigger(AniState.ChangeIdleAni.ToString()); }


    [SerializeField] private bool isRight;
    [SerializeField] private bool seatIsFront;

    private void RandomIdleNum() { Invoke("SetRandomIdleNum", UnityEngine.Random.Range(2f, 10f)); }
    private void SetRandomIdleNum() { IdleNumAni = UnityEngine.Random.Range(0, ValueCastTo<int>.From(IdleAniType.Count)); ChangeRandomIdle(); }
    private void CancelRandomIdleNum() { CancelInvoke("SetRandomIdleNum"); CancelChangeRandomIdle(); }
    private void ChangeRandomIdle() { InvokeRepeating("SetChangeIdleTrigger", 2f, 5f); }
    private void CancelChangeRandomIdle() { CancelInvoke("SetChangeIdleTrigger"); }

    private void SetSit() { SitAni = true; currentState = State.Sit; if (!isBroadcastPlanning) RandomIdleNum(); }
    private void RevertSit() { SitAni = false; CancelRandomIdleNum(); }
    
    private void SetWalk() { WalkAni = true; currentState = State.Move; }
    private void RevertWalk() { WalkAni = false; }
    
    private void SetWork() { WorkAni = true; currentState = State.Work; }
    private void RevertWork() { WorkAni = false; }
    
    private void SetIdle() { IdleAni = true; currentState = State.Idle; if (SitAni) RandomIdleNum(); } 
    private void RevertIdle() { IdleAni = false; IdleNumAni = ValueCastTo<int>.From(IdleAniType.None); CancelRandomIdleNum(); } 

    private void CheckSeatDirection()
    {
        if (seatIsFront)
        {
            FrontAni = true;
            BackAni = false;
        }
        else
        {
            FrontAni = false;
            BackAni = true;
        }
    }


    private void Idle()
    {
        //기본 컴퓨터 두드리는 애니메이션 돌림
        //ani.Play(aniNames[ValueCastTo<long>.From(AniName.IdleKeyboard)]);
        //SetIdle();

        //조건으로 제작중이라고 전달 받으면 work로 변경
        // if (isBroadcastPlanning)
        // {
        //     //SetWork();
        // }
    }


    public void TEST_TurnToMoveState()
    {
        currentState = State.Move;
    }

    private void Move()
    {
        //이벤트로 사용할거고
        //매니저한테 본인만 넘겨서 계산해서 넘겨줌
        //캐릭터무브먼트는 어디로 가는지는 모르고 경로만 받고
        //도착하면 interactive 하는거임

        if (moveCoroutine != null)
        {
            return;
        }

        Debug.Log(transform.name);

        moveCoroutine = StartCoroutine(MoveToTarget(path, moveType, moveDelegate));

        path = null;
    }   

    Queue<Vector2> path;
    MoveType moveType;

    Action moveDelegate;

    bool moveToTarget;
    private void CheckMoveToTarget()
    {
        if (moveCoroutine != null || !(bool)OnCharacterCanInteractive?.Invoke(characterData.SeatNumber))
        {
            SetWalkForRandomSec();
            return;
        }

        //targetIndex -> 중복 검사를 위해 넣어둔 매니저의 타겟리스트 내부의 위치
        //이동 끝나고 해당 index는 비워줌
        path = OnGetPath?.Invoke(transform.position, CharacterManager.PathFindMode.Random, 
                                            characterData.SeatNumber - 1, out targetIndex, out lastPos);


        if (path == null)
        {
            if (isBroadcastPlanning) 
            {
                SetWork();
            }
            else
            {
                SetIdle();
            }

            SetWalkForRandomSec();

            return;
        }

        TEST_targetPos = lastPos;
        moveType = MoveType.Target;
        moveDelegate = null;
        moveToTarget = true;
        SetWalk();
    }


    //캐릭터 본인 위치로 이동
    public void ReturnMySeat()
    {
        path = OnGetPath?.Invoke(transform.position, CharacterManager.PathFindMode.MoveToMySeat, 
                                                characterData.SeatNumber - 1, out targetIndex, out lastPos);

        //테스트 메세지
        if (isFirstMoveFromSetData)
        {
            RequestMessage(JsonManager.Instance.GetCharacterComment()[characterData.CharacterID - ValueCastTo<int>.From(ResourceID.Character_ISD_Ine)].GotoWork);
        }

        isFirstMoveFromSetData = true;
        moveType = MoveType.MySeat;
        moveDelegate = Sit;

        SetWalk();
    }

    private Vector2 lastPos;
    //내부에서 방향 y scale 값 설정
    private IEnumerator MoveToTarget(Queue<Vector2> path, MoveType moveType, Action action)
    {
        //isCharacterMove = true;

        if (isOnMySeat)
        {
            yield return StandUp();
        }

        isCharacterMove = true;

        while (path.Count > 0)
        {
            Vector2 targetPos = path.Dequeue();

            FlipToTarget(targetPos);

            yield return StartCoroutine(MoveToTargetPos(targetPos));
        }

        FlipToTarget(lastPos);
        RevertWalk();

        isCharacterMove = false;

        action?.Invoke();
        currentState = State.Interactive;

        path = null;

        //이동 끝나서 코루틴 null로 바꾸고 상태 상호작용으로 변경
        //isCharacterMove = false;
        moveCoroutine = null;
        //WalkAni = false;
    }

    private IEnumerator MoveToTargetPos(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        isCharacterMove = true; 
        while (!(Vector2.Distance(transform.position, targetPos) <= 0.03f))
        {
            yield return new WaitUntil( () => { return GameManager.IsGamePause == false; }); 

            Vector2 newPos = transform.position; 
            newPos += direction * Time.deltaTime * 0.75f;

            transform.position = newPos;
        }

        transform.position = targetPos;
        isCharacterMove = false;

        //seatSpriteOffEvent?.Invoke();
    }

    private void FlipToTarget(Vector2 targetPos)
    {
        bool isRight = targetPos.x < transform.position.x ? false : true;

        Flip(isRight);
    }

    private void Flip(bool isRight)
    {
        Vector2 characterScale = transform.localScale;
        int scaleX = isRight ? -1 : 1;

        characterScale.x = scaleX;
        transform.GetChild(0).localScale = characterScale;
    }

    public void FlipEndInteractive()
    {
        Flip(isRight);
    }

    [SerializeField] private LayerMask interactiveLayer;

    //상호작용 상대의 sprite 및 애니메이션 실행
    //sprite의 경우 애니메이션 내부에서 이벤트 추가해서 해줄 수 있으니깐 그걸로 하고
    //애니메이션 실행 함수만 넘겨서 받는걸로
    private Action interactiveTargetAction;

    public Vector2 TEST_targetPos;

    private void Interactive()
    {
        //마지막 경로로 넘어온 path위치를 overlap하여
        //interactiveevent호출하여 매니저에게 결과 리턴하거나
        //interactiveevent내부에서 매니저에게 호출하는 방식으로 진행할 수도 있음.

        //interactive 시 ineractive 요청한 캐릭터와 요청받은 캐릭터 둘 다 메세지를 띄워야 함.
        //interactive가 끝났다는 것도 리턴 받아야 함.

        Transform target = null;

        if (this.targetIndex != -1)
        {
            LayerMask characterInterLayer = (1 << LayerMask.NameToLayer("Character")) & interactiveLayer.value;

            try
            {
                target = Physics2D.OverlapCircle(lastPos, 0.1f, characterInterLayer.value).transform;

            }catch (Exception e)
            {
                Debug.Log(e.Message + "/" + transform.name);
            }
        }
        else
        {
            LayerMask characterInterLayer = (1 << LayerMask.NameToLayer("Character")) ^ interactiveLayer.value;

            target = Physics2D.OverlapCircle(lastPos, 0.1f, characterInterLayer.value).transform;
        }


        InteractiveEvent interactiveEvent = target.GetComponent<InteractiveEvent>();
        bool interTargetFlipRight = target.position.x < transform.position.x ? false : true;
        
        //캐릭터체크
        var targetCharacter = target.GetComponent<Character>();

        if (targetCharacter != null)
        {
            if (targetCharacter.name == transform.name)
            {
                return;
            }

            int targetID = targetCharacter.GetID();

            targetCharacter.TEST_Message(GetComment(targetID, characterData.CharacterID));
            targetCharacter.FlipToTarget(transform.position);

            TEST_Message(GetComment(characterData.CharacterID, targetID));

            //캐릭터 상호작용
            //return my seat 함수 전달

            interactiveEvent.Interactive(isBroadcastPlanning, targetIndex,            //순서대로 방송중인지, 타겟이 몇번인지(캐릭터용)
                                                                out isRight,                    //target의 방향
                                                                out seatIsFront,                //앉을 좌석이 앞을 보고있는지
                                                                out interactiveTargetAction,    // 타겟의 애니메이션 받을려고 넣음
                                                                ReturnMySeat,                   //상호작용 끝나면 자리로 되돌아갈 수 있도록 콜백
                                                                StatAdd);                       //상호작용 시 스텟추가를 위해 콜백
        }
        else
        {
            //캐릭터가 처음으로 본인 자리에 가서 interactive하는 경우에는 returnmyseat 이벤트 호출 X

            

            if (isFirstMoveFromSetData)
            {
                isFirstMoveFromSetData = false;
                interactiveEvent.Interactive(isBroadcastPlanning, targetIndex,
                                                                    out isRight, 
                                                                    out seatIsFront,
                                                                    out interactiveTargetAction, 
                                                                    null, 
                                                                    StatAdd);
            }
            else
            {
                interactiveEvent.Interactive(isBroadcastPlanning, targetIndex, 
                                                                    out isRight, 
                                                                    out seatIsFront,
                                                                    out interactiveTargetAction, 
                                                                    ReturnMySeat, 
                                                                    StatAdd);
            }
        }

        currentState = State.None;
    }

    private string GetComment(int interID, int targetID)
    {
        //캐릭터군 판단 1000/ 2000/ 3000각각나눠서 0인지 체크
        CharacterManager.ISEGYEIDOL inter = ValueCastTo<CharacterManager.ISEGYEIDOL>.From(interID - ResourceID.Character_ISD_Ine);
        CharacterManager.ISEGYEIDOL target = ValueCastTo<CharacterManager.ISEGYEIDOL>.From(targetID - ResourceID.Character_ISD_Ine);

        string comment = string.Empty;

        switch (inter)
        {
            case CharacterManager.ISEGYEIDOL.Ine:
            case CharacterManager.ISEGYEIDOL.JingBurger:
            case CharacterManager.ISEGYEIDOL.Lilpa:
            case CharacterManager.ISEGYEIDOL.Jururu:
            case CharacterManager.ISEGYEIDOL.Gosegu:
            case CharacterManager.ISEGYEIDOL.Viichan:
            switch (target)
            {
                case CharacterManager.ISEGYEIDOL.Ine:
                comment = JsonManager.Instance.GetCharacterComment()[inter - CharacterManager.ISEGYEIDOL.Ine].ToIne;
                break;
                case CharacterManager.ISEGYEIDOL.JingBurger:
                comment = JsonManager.Instance.GetCharacterComment()[inter - CharacterManager.ISEGYEIDOL.Ine].ToJingBurger;
                break;
                case CharacterManager.ISEGYEIDOL.Lilpa:
                comment = JsonManager.Instance.GetCharacterComment()[inter - CharacterManager.ISEGYEIDOL.Ine].ToLilpa;
                break;
                case CharacterManager.ISEGYEIDOL.Jururu:
                comment = JsonManager.Instance.GetCharacterComment()[inter - CharacterManager.ISEGYEIDOL.Ine].ToJururu;
                break;
                case CharacterManager.ISEGYEIDOL.Gosegu:
                comment = JsonManager.Instance.GetCharacterComment()[inter - CharacterManager.ISEGYEIDOL.Ine].ToGosegu;
                break;
                case CharacterManager.ISEGYEIDOL.Viichan:
                comment = JsonManager.Instance.GetCharacterComment()[inter - CharacterManager.ISEGYEIDOL.Ine].ToViichan;
                break;
            }
            break;
            default:
            comment = "이세돌 아님";
            break;
        }

        return comment;
    }

    public int GetID()
    {
        return characterData.CharacterID;
    }

    //우선 pc 캐싱해서 쓰고
    //추후 environment 매니저 만들어서 ㄱㄱ
    //인덱스만 넘겨줘서 처리할거임 나중에
    Environment pc;

    private void FindPC()
    {
        if (!(characterData.SeatNumber == 1 || characterData.SeatNumber == 4))
            return;

        LayerMask characterInterLayer = (1 << LayerMask.NameToLayer("PC")) & interactiveLayer.value;

        pc = Physics2D.OverlapCircle(lastPos, 1f, characterInterLayer.value).transform.GetComponent<Environment>();

        pc.ChangeImage();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(lastPos, 1f);
    }

    public void ChangeStateInter()
    {
        if (isBroadcastPlanning)
        {
            SetWork();
        }
        else
        {
            SetIdle();
        }
    }

    public void TEST_Message(string msg)
    {
        RequestMessage(msg);
    }

    private void Work()
    {
        //매니저가 전달해 준 이벤트 호출할 거임.
        //내부에서 Productor전달 받아서 가지고 있거나
        //필요한 데이터(작업중이고, 작업이 끝났는지)를 전달받거나 할 거임.
        //받은 데이터대로 하거나
        //Productor쪽에서 함수 실행할 수 있도록 매니저에게 요청하는 방식으로 진행
        //
        //broadcastmanager가 방송 제작 단계로 넘어가면
        //movementmanager통해서 등록된 캐릭터 무브먼트애들한테 받은 이벤트 호출하여
        //전부 work상태로 변경해줄 거
        //work 내부에서는 
        //1. 키보드 두드리는 애니메이션
        //2. 다른 캐릭터로 이동 요청 보내고 받으면 Move로 이동
        //3. 방송 제작중이였다면 매니저 통해서 확인 후 Move하고 Interactive하고
        //  본인자리로 move하고 Work로 돌아옴

        SetWork();

        if (isBroadcastPlanning)
        {
            ProductorWorkProcess();
        }
    }
    private float time = 0.0f;
    private int currentStep = 0;
    private void ProductorWorkProcess()
    {
        if (!isOnMySeat)
        {
            return;
        }

        time += Time.deltaTime;

        if (time >= 1f)
        {
            time = 0.0f;
  
            ProductorManager.Instance.AddStatFieldProcessing(characterData.SeatNumber - 1);

            if (productorInfo.ProcessedPoint >= ProductorManager.Instance.ProcessLevel[currentStep])
            {
                currentStep++;
                BroadCastPlanning.Instance.AddProcessingRate();

                if (currentStep >= 3)
                {
                    currentStep = 0;
                    productorInfo.InitProcessedPoint();
                    RequestPopupStat();
                }

                if (pc != null)
                    pc.ChangeImage();
            }
        }
    }

    private static int WorkCount => 10;

    private bool isBroadcastPlanning;
    private bool isCharacterMove;

    CharacterManager.OnGetPath OnGetPath;
    OnReturnPoolEvent<Character> OnReturnPoolEvent;     //해고하고 다른 작업자로 바뀔 때 호출

    CharacterManager.OnUpdateCharacterState OnUpdateCharacterState; //캐릭터 퇴근, 출근, 해고 상태 업데이트
                                                                    //매니저가 모든 캐릭터의 상태를 가지고 있음.
    CharacterManager.OnUpdateSeatIndex OnUpdateSeatIndex;           //좌석에 앉을 때 0으로 변경, 좌석 일어날 때 1 
    CharacterManager.OnCharacterCanInteractive OnCharacterCanInteractive;

    CharacterManager.OnCharacterInteractiveSenderEvent OnCharacterInteractiveSenderEvent;   //상호작용 스텟팝업 이벤트
    CharacterManager.OnCharacterSFXRequestEvent OnCharacterSFXRequestEvent;                 //효과음 요청 이벤트


    //콜백용 함수 전부 등록
    public void RegisterMovementEventToManager()
    {
        //1. 방송 제작중 전달   
        //2. 방송 제작 끝 전달
        CharacterManager.Instance.RegisterCharacterEvent(() => { this.isBroadcastPlanning = true; SetWork(); callActionAfterSit.Enqueue(FindPC); RevertIdle(); }, 
                                                                () => { this.isBroadcastPlanning = false; SetIdle(); RevertWork(); });
    }


    private void Fever(int count)
    {

        //캐릭터 매니저 내부에서 인덱스 통해서 보내줄거
        //피버 remain 값도 저장을 해서 켰을 때 remain 값이 있을 경우 실행
        //위치 값도 같이 가져옴.
        StartCoroutine(FeverAnimation(count));
    }

    private IEnumerator FeverAnimation(int count)
    {
        //count만큼 숫자 채우면서 (+0, +1 .... +30) 캐릭터 뒤에 피버 효과 sprite animation실행
        int currentCount = 0;

        while (currentCount < count)
        {
            yield return new WaitUntil( () => { return GameManager.IsGamePause == false; }); 

            count++;

            //피버 텍스트에 currentCount 넣음
            //feverText.text = string.Format("+{0}", currentCount);

            float randomYieldTime = UnityEngine.Random.Range(MINFEVERYIELDTIME, MAXFEVERYIELDTIME);

            yield return new WaitForSeconds(randomYieldTime);
        }

        yield return null;
    }

    public void RegisterGetPathEvent(CharacterManager.OnGetPath OnGetPath)
    {
        this.OnGetPath = OnGetPath;
    }

    public void RegisterCharacterStateUpdate(CharacterManager.OnUpdateCharacterState OnUpdateCharacterState)
    {
        this.OnUpdateCharacterState = OnUpdateCharacterState;
    }

    public void Init(OnReturnPoolEvent<Character> OnReturnPoolEvent)
    {
        this.OnReturnPoolEvent = OnReturnPoolEvent;
        
        isCharacterMove = false;

        //RegisterMovementEvent();
        RegisterGetPathEvent(CharacterManager.Instance.GetPath);
        RegisterCharacterStateUpdate(CharacterManager.Instance.SetCharacterStatus);
        RegisterMovementEventToManager();
        RegisterUpdateSeatIndexEvent(CharacterManager.Instance.UpdateSeatInterState); //이벤트 등록 함수 매니저측에서 만들어서 ㄱㄱ
        RegisterCharacterCanInteractiveEvent(CharacterManager.Instance.IsCanMoveForInteractive);
        RegisterCharacterInteractiveSenderEvent(CharacterManager.Instance.ReqPopupStat);
        RegisterCharacterRequestSFXEvent(CharacterManager.Instance.RequestSFX);

        //CharacterManager.Instance.SetCharacterInfo(characterData);
    }

    //interactive 이벤트한테 넘겨서 쓸거임.
    public void RequestPopupStat()
    {
        OnCharacterSFXRequestEvent?.Invoke(CharacterManager.CharacterSFXType.StatPopup);
        OnCharacterInteractiveSenderEvent?.Invoke(characterData.SeatNumber - 1);
    }
    private void RequestMessage(string message)
    {
        CharacterManager.Instance.RequestPopupMessage(message, transform, GetIsCharacterMove);
    }

    private bool GetIsCharacterMove()
    {
        return isCharacterMove;
    }

    public int GetCharacterIndex()
    {
        //if (characterData.SeatNumber == )

        return characterData.SeatNumber;
    }


    private bool StatAddCheck()
    {
        //작업자 작업 할당치 체크

        return false;
    }


    //내부에서 따로 random할거
    //스텟 이벤트는 
    //interactive로 넘겨지고 interactive측에서 실행시킬거.
    //productor스텟치도 알아야함. -> productor 매니저에게 등록된 이벤트로

    private void StatAdd(int amount)
    {
        //할당치 체크 완료 후 작업자 스텟 증가 요청 -> 매니저 통해서 Productor매니저가서 ProductorInfo 증가
        //작업자는 해당 방식으로 추가하면 되는데, 이세돌의 스텟 구조는 아직 결정을 안해서 따로 없을 수도
    }

    //statadd로 받은 결과를 charactermanager에게 받은 이벤트 호출하여 어떤 스텟의 amount값 넘겨서 표시
    private void StatPopupRequest()
    {

    }

    //처음 시작 때 받은 위치로 가서 좌석에 앉는 애니메이션 할거임.
    //targetNode 직전까지 가고 난 뒤에 타겟 좌석의 자식으로 넣고 0, 0위치로 lerp하며 갈 거
    //도착한 뒤 0, 0값으로 설정하고 의자 뒤로 좀 빼면 될 듯

    private Action seatSpriteOffEvent;

    public void Sit()
    {
        StartCoroutine(SitCoroutine());
    }

    //interactiveEvent에 callevent에 추가해서 사용
    public void ResetCharacterInterState()
    {
        UpdateSeatState(CharacterManager.CharacterInteractiveState.CanInteractive);
    }

    //캐릭터에 붙어있는 interactive event에 해당 이벤트 넣어서 call all event에서 실행(interactive 끝날 때)
    private void UpdateSeatState(CharacterManager.CharacterInteractiveState characterInteractiveState)
    {
        OnUpdateSeatIndex?.Invoke(characterData.SeatNumber, characterInteractiveState);
    }

    private IEnumerator SitCoroutine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        //lastpos로 가서 애니메이션 으로 바꿔치기 할 거임
        yield return StartCoroutine(MoveToTargetPos(lastPos));

        AnimationSpeedSet(false);

        isOnMySeat = true;
        UpdateSeatState(CharacterManager.CharacterInteractiveState.CanInteractive);


        SetSit();

        Flip(isRight);
        CheckSeatDirection();

        SetWalkForRandomSec();

        while (callActionAfterSit.Count > 0)
        {
            callActionAfterSit.Dequeue()?.Invoke();
        }

        //관리객체 만들기 전까지 임시 사용
        FindPC();
    }

    private void SetWalkForRandomSec()
    {
        if (moveToTarget)
        {
            Invoke("SetWalkForRandomSec", UnityEngine.Random.Range(10, 15));

            moveToTarget  = false;

            return;
        }

        Invoke("CheckMoveToTarget", UnityEngine.Random.Range(3, 6));
    }

    private void AnimationSpeedSet(bool reverse)
    {
        if (reverse)
        {
            animator.SetFloat("AniSpeed", ANISPEED * -1);
        }
        else
        {
            animator.SetFloat("AniSpeed", ANISPEED);
        }
    }

    public void CallInteractiveAction()
    {
        interactiveTargetAction?.Invoke();
    }

    public IEnumerator StandUp()
    {
        RevertSit();
        UpdateSeatState(CharacterManager.CharacterInteractiveState.CantInteractive);
        AnimationSpeedSet(true);
        isOnMySeat = false;

        yield return new WaitForSeconds(0.5f);

        //interactiveTargetAction?.Invoke();
        AnimationSpeedSet(false);

        yield return new WaitForSeconds(0.5f);
    }


    public void ChangeStateAfterSit()
    {
        if (isBroadcastPlanning)
        {
            SetWork();
        }
        else
        {
            SetIdle();
        }

        SetSprite();
    }

    public void SetSprite()
    {
        if (!SitAni)
        {
            spriteRenderer.sprite = characterSprites[ValueCastTo<int>.From(SpriteType.Standing)];
        }

        if (FrontAni)
        {
            spriteRenderer.sprite = characterSprites[ValueCastTo<int>.From(SpriteType.SitFront)];
        }
        else
        {
            spriteRenderer.sprite = characterSprites[ValueCastTo<int>.From(SpriteType.SitBack)];
        }
    }

    public void RegisterUpdateSeatIndexEvent(CharacterManager.OnUpdateSeatIndex OnUpdateSeatIndex)
    {
        this.OnUpdateSeatIndex = OnUpdateSeatIndex;
    }

    public void RegisterCharacterCanInteractiveEvent(CharacterManager.OnCharacterCanInteractive OnCharacterCanInteractive)
    {
        this.OnCharacterCanInteractive = OnCharacterCanInteractive;
    }

    public void RegisterCharacterInteractiveSenderEvent(CharacterManager.OnCharacterInteractiveSenderEvent OnCharacterInteractiveSenderEvent)
    {
        this.OnCharacterInteractiveSenderEvent = OnCharacterInteractiveSenderEvent;
    }

    public void RegisterCharacterRequestSFXEvent(CharacterManager.OnCharacterSFXRequestEvent OnCharacterSFXRequestEvent)
    {
        this.OnCharacterSFXRequestEvent = OnCharacterSFXRequestEvent;
    }

    public void PauseAction(bool pause)
    {
        animator.speed = pause ? 0.0f : 1.0f; 
    }
}