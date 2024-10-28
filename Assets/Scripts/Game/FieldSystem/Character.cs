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

    private Sprite[] characterSprites;

    private CharacterData characterData;

    private Transform imageTransform;

    enum State
    {
        None,
        Idle,
        Move,
        Interactive,
        Work,
        Sit,
    }

    enum AniState
    {
        IdleAni,            //Idle 애니메이션
        WorkAni,            //작업 애니메이션
        WalkAni,            //걷는 애니메이션

        SitAni,             //스탠딩 애니메이션

        AniSpeed,           //앉는 애니메이션 반대로 재생할려고 만듦

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

    public void SetData(Sprite sprite, CharacterData characterData, int characterID)
    {
        this.characterData = characterData;
        spriteRenderer.sprite = sprite;

        if (characterData.IsIsegyeIdol == "O")
        {
            iSEGYEIDOL = ValueCastTo<CharacterManager.ISEGYEIDOL>.From(characterID - ResourceID.Character_ISD_Ine);
        }

        imageTransform = transform.GetChild(0);

        //
        characterSprites = new Sprite[ValueCastTo<long>.From(ResourceType.SpriteCount)];

        //standing
        characterSprites[0] = CharacterManager.Instance.GetSpriteFromID(characterData.CharacterID, ResourceType.DefaultSprite);   
        //sit
        characterSprites[1] = CharacterManager.Instance.GetSpriteFromID(characterData.CharacterID, ResourceType.SitSprite);   


        aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = aoc;
        
        aoc[ResourceType.IdleAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.IdleAni);
        aoc[ResourceType.WalkAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.WalkAni);
        aoc[ResourceType.WorkAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.WorkAni);
        aoc[ResourceType.SitAni.ToString()] = CharacterManager.Instance.GetAnimationClip(characterID, ResourceType.SitAni);

        animator.speed = ANISPEED;


        animator.writeDefaultValuesOnDisable = false;
    }

    private void FixedUpdate()
    {
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


    private Animator ani;

    private Coroutine moveCoroutine;

    private bool isOnMySeat = false;        //내 좌석에 앉아있는지 체크용

    private bool IdleAni {get { return animator.GetBool(AniState.IdleAni.ToString()); } set { animator.SetBool(AniState.IdleAni.ToString(), value); }}
    private bool WorkAni {get { return animator.GetBool(AniState.WorkAni.ToString()); } set { animator.SetBool(AniState.WorkAni.ToString(), value); }}
    private bool WalkAni {get { return animator.GetBool(AniState.WalkAni.ToString()); } set { animator.SetBool(AniState.WalkAni.ToString(), value); }}
 
    private bool SitAni {get {return animator.GetBool(AniState.SitAni.ToString());} set {animator.SetBool(AniState.SitAni.ToString(), value);}}

    private int targetIndex;

    private void SetSit()
    {
        IdleAni = false;
        WorkAni = false;
        WalkAni = false;
        SitAni = true;

        currentState = State.Sit;
    }
    
    private void SetWalk()
    {
        IdleAni = false;
        WorkAni = false;
        WalkAni = true;
        SitAni = false;

        currentState = State.Move;
    }

    private void SetWork()
    {
        IdleAni = false;
        WorkAni = true;
        WalkAni = false;
        SitAni = false;

        currentState = State.Work;
    }

    private void SetIdle()
    {
        IdleAni = true;
        WorkAni = false;
        WalkAni = false;
        SitAni = false;

        currentState = State.Idle;
    } 

    private void Idle()
    {
        //기본 컴퓨터 두드리는 애니메이션 돌림
        //ani.Play(aniNames[ValueCastTo<long>.From(AniName.IdleKeyboard)]);
        SetIdle();

        //조건으로 제작중이라고 전달 받으면 work로 변경
        if (isBroadcastPlanning)
        {
            currentState = State.Work;
        }
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


        //targetIndex -> 중복 검사를 위해 넣어둔 매니저의 타겟리스트 내부의 위치
        //이동 끝나고 해당 index는 비워줌
        Queue<Vector2> path = OnGetPath?.Invoke(transform.position, CharacterManager.PathFindMode.Random, 
                                                characterData.SeatNumber - 1, out targetIndex, out lastPos);

        TEST_targetPos = lastPos;

        if (path == null)
        {
            SetIdle();

            Debug.Log(transform.name + " Path Not Found");

            return;
        }

        moveCoroutine = StartCoroutine(MoveToTarget(path, MoveType.Target, null));
    }   


    //캐릭터 본인 위치로 이동
    public void ReturnMySeat()
    {
        Queue<Vector2> path = OnGetPath?.Invoke(transform.position, CharacterManager.PathFindMode.MoveToMySeat, 
                                                characterData.SeatNumber - 1, out targetIndex, out lastPos);

                                                

        //테스트 메세지
        RequestMessage("하이네");

        moveCoroutine = StartCoroutine(MoveToTarget(path, MoveType.MySeat, Sit));
    }

    private Vector2 lastPos;
    //내부에서 방향 y scale 값 설정
    private IEnumerator MoveToTarget(Queue<Vector2> path, MoveType moveType, Action action)
    {
        //isCharacterMove = true;

        Debug.Log(transform.name + " - Move");

        if (isOnMySeat)
        {
            yield return StandUp();

            Debug.Log(transform.name + " - Stand Up");
        }

        Debug.Log(transform.name + " PathCount : " + path.Count);

        SetWalk();
        isCharacterMove = true;

        while (path.Count > ValueCastTo<int>.From(moveType))
        {
            Vector2 targetPos = path.Dequeue();

            FlipToTarget(targetPos);

            yield return StartCoroutine(MoveToTargetPos(targetPos));
        }

        Debug.Log(transform.name + " - end move");

        FlipToTarget(lastPos);

        isCharacterMove = false;
        currentState = State.Interactive;
        action?.Invoke();

        //이동 끝나서 코루틴 null로 바꾸고 상태 상호작용으로 변경
        //isCharacterMove = false;
        moveCoroutine = null;
        //WalkAni = false;
    }

    private IEnumerator MoveToTargetPos(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        while (!(Vector2.Distance(transform.position, targetPos) <= 0.02f))
        {
            Vector2 newPos = transform.position; 
            newPos += direction * Time.deltaTime * 0.75f;

            transform.position = newPos;

            yield return new WaitForFixedUpdate();
        }

        transform.position = targetPos;

        //seatSpriteOffEvent?.Invoke();
    }

    private void FlipToTarget(Vector2 targetPos)
    {
        Vector2 characterScale = transform.localScale;

        int multi = targetPos.x <= transform.position.x ? 1 : -1;

        characterScale.x = multi;

        transform.localScale = characterScale;
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

        var target = Physics2D.OverlapCircle(lastPos, 0.5f, interactiveLayer).transform;

        InteractiveEvent interactiveEvent = target.GetComponent<InteractiveEvent>();
        
        //캐릭터체크
        var targetCharacter = target.GetComponent<Character>();
        int targetIndex = -1;

        //Debug.Log(transform.name + "interactive start");
        //Debug.Log("target : " + target);



        if (targetCharacter != null && !isOnMySeat)
        {
            if (targetCharacter.name == transform.name)
            {
                return;
            }

            targetIndex = targetCharacter.GetCharacterIndex();

            targetCharacter.TEST_Message("안녕 언니");
            targetCharacter.FlipToTarget(transform.position);

            TEST_Message("안녕 버거야");

            currentState = State.None;

            //캐릭터 상호작용
            //return my seat 함수 전달
            interactiveEvent.Interactive(isBroadcastPlanning, targetIndex, out interactiveTargetAction,this.StatAdd);

            return;
        }

        //Debug.Log(transform.name + " -> " + target);
        //Debug.Log("inetactive : " + interactiveEvent);

        //사물 상호작용
        interactiveEvent.Interactive(isBroadcastPlanning, targetIndex, out interactiveTargetAction,this.StatAdd);

        currentState = State.None;
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

        }
        else
        {

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


    //콜백용 함수 전부 등록
    public void RegisterMovementEventToManager()
    {
        //1. 방송 제작중 전달   
        //2. 방송 제작 끝 전달
        CharacterManager.Instance.RegisterCharacterEvent(() => { this.isBroadcastPlanning = true; SetWork(); }, 
                                                                () => { this.isBroadcastPlanning = false; SetIdle(); });
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

        //CharacterManager.Instance.SetCharacterInfo(characterData);
    }
    

    //interactive 이벤트한테 넘겨서 쓸거임.
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

    private void StatAdd(int amount)
    {
        //할당치 체크 완료 후 작업자 스텟 증가 요청 -> 매니저 통해서 Productor매니저가서 ProductorInfo 증가
        //작업자는 해당 방식으로 추가하면 되는데, 이세돌의 스텟 구조는 아직 결정을 안해서 따로 없을 수도
    }

    //처음 시작 때 받은 위치로 가서 좌석에 앉는 애니메이션 할거임.
    //targetNode 직전까지 가고 난 뒤에 타겟 좌석의 자식으로 넣고 0, 0위치로 lerp하며 갈 거
    //도착한 뒤 0, 0값으로 설정하고 의자 뒤로 좀 빼면 될 듯

    private Action seatSpriteOffEvent;

    public void Sit()
    {
        StartCoroutine(SitCoroutine());
    }

    //캐릭터에 붙어있는 interactive event에 해당 이벤트 넣어서 call all event에서 실행(interactive 끝날 때)
    private void UpdateSeatState(CharacterManager.CharacterInteractiveState characterInteractiveState)
    {
        OnUpdateSeatIndex?.Invoke(characterData.SeatNumber, characterInteractiveState);
    }

    private IEnumerator SitCoroutine()
    {
        //lastpos로 가서 애니메이션 으로 바꿔치기 할 거임
        yield return StartCoroutine(MoveToTargetPos(lastPos));

        AnimationSpeedSet(false);

        isOnMySeat = true;
        UpdateSeatState(CharacterManager.CharacterInteractiveState.CanInteractive);
        SetSit();

        Invoke("SetWalk", UnityEngine.Random.Range(5, 10));
    }

    private void AnimationSpeedSet(bool reverse)
    {
        if (reverse)
        {
            animator.speed = ANISPEED * -1; 
            //animator.StartPlayback();
        }
        else
        {
            animator.speed = ANISPEED; 
        }
    }

    public void CallInteractiveAction()
    {
        interactiveTargetAction?.Invoke();
    }

    public IEnumerator StandUp()
    {
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
            currentState = State.Work;
            SetWork();
        }
        else
        {
            currentState = State.Idle;
            SetIdle();
        }

        SetSitSprite();
    }


    //애니메이션 내부에 함수 추가
    //
    public void SetSitSprite()
    {

        spriteRenderer.sprite = characterSprites[0];

        //상대 스트라이트끄는 이벤트 받아와서 여기서 실행
        //해당 이벤트 받아와야 플레이어가 왔다갔따할 때 키고 끌 수 있음
    }
    //
    public void SetStandingSprite()
    {
        spriteRenderer.sprite = characterSprites[1];
    }

    public void RegisterUpdateSeatIndexEvent(CharacterManager.OnUpdateSeatIndex OnUpdateSeatIndex)
    {
        this.OnUpdateSeatIndex = OnUpdateSeatIndex;
    }
}