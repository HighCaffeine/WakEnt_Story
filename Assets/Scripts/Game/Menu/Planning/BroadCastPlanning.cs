using System;
using System.Collections;
using System.Collections.Generic;
using Devcat;
using UnityEngine;



public class BroadCastPlanning : GenericSingleton<BroadCastPlanning>
{
    //MenuController가 알고있는 컨텐츠와 방송타입 종류를 선택하고 여기로 넘겨줌.
    //menu는 컨텐츠와 방송종류는 모르고 몇 번째껄 선택했는지만 넘겨주고
    //메뉴를 만들 때 broadcastplaning에 list로 되어있는 걸
    //data매니저가 먼저 세팅을 해 주고, menucontroller가 메뉴를 생성할 때(start)
    //hash/dictionary로 넘겨줄 듯(menucontroller는 이름만 알면 됨)
    [Header("방송 제작 키워드")]
    [SerializeField] private TMPro.TextMeshProUGUI gearText;
    [SerializeField] private TMPro.TextMeshProUGUI kategorieText;
    [SerializeField] private TMPro.TextMeshProUGUI contentText;
    [SerializeField] private TMPro.TextMeshProUGUI directionText;

    [SerializeField] private TMPro.TextMeshProUGUI priceText;           //방송제작 비용
    //[SerializeField] private TMPro.TextMeshProUGUI memberSelectCount;   //선택할 멤버 수 (특정 키워드는 2명 이상 선택 가능)

    [Space(10f)]
    [Header("이세돌 선택")]
    [SerializeField] private GameObject isedolSelectParent;
    private List<ImagePointerEvent> isedolPointerEvents = new List<ImagePointerEvent>();
    private int isedolSelected = 0;
    private int isedolSelectCount = 0;

    private float priceRatio;   // 제작 비용 배율 

    private const float DefaultPriceRatio = 1.00f;

    public bool IsActiveMember(CharacterManager.ISEGYEIDOL isegyeidol)
    {
        int value = 1 << (ValueCastTo<int>.From(isegyeidol));

        return ((isedolSelected & value) != 0);
    }

    public void SelectIsedol(int index)
    {
        if (index < ValueCastTo<int>.From(CharacterManager.ISEGYEIDOL.Ine)
            || index > ValueCastTo<int>.From(CharacterManager.ISEGYEIDOL.Viichan))
        {
            //out of range
            return;
        }

        int isedolIndex = ValueCastTo<int>.From(CharacterManager.ISEGYEIDOL.Ine) + index;
        int isedolBinary = 1 << isedolIndex;

        if (isedolSelectCount < BroadcastKeywordSelection.Instance.GetSelectCharacterLimit())
        {
            //선택 안햇으면 무조건 다 하고 return
            if ((isedolSelected & isedolBinary) == 0)
            {
                isedolSelectCount++;
                isedolSelected |= isedolBinary;

                isedolPointerEvents[index].SetNormalColor();
                isedolPointerEvents[index].SetAllowMouseEvent(false);

                return;
            }
        }

        if ((isedolSelected & isedolBinary) != 0)
        {
            isedolSelectCount--;
            isedolSelected &= ~isedolBinary;

            isedolPointerEvents[index].SetTransparencyColor();
            isedolPointerEvents[index].SetAllowMouseEvent(true);

            return;
        }
    }


    [Serializable]
    private class Broadcast
    {
        private BroadcastKeyword.Kategorie Kategorie;
        private BroadcastKeyword.Content Content;


        private int matchingRate;

        public float processingRate { private set; get; }

        public void InitProcessingRate() { this.processingRate = 0; }

        public BroadCastPoint broadCastPoint = new BroadCastPoint();

        public class BroadCastPoint
        {
            private int plannerPoint;
            private int designerPoint;
            private int composerPoint;
            private int promotionPoint;

            public int this[ProductorManager.ProductorType type]
            {
                get { return GetValue(type); }
                set { SetValue(type, value); }
            }

            private int GetValue(ProductorManager.ProductorType type)
            {
                switch (type)
                {
                    case ProductorManager.ProductorType.Planner:
                        return plannerPoint;
                    case ProductorManager.ProductorType.Designer:
                        return designerPoint;
                    case ProductorManager.ProductorType.Composer:
                        return composerPoint;
                    case ProductorManager.ProductorType.Promotor:
                        return promotionPoint;
                }

                return 0;
            }

            private void SetValue(ProductorManager.ProductorType type, int value)
            {
                switch (type)
                {
                    case ProductorManager.ProductorType.Planner:
                        plannerPoint = value;
                        break;
                    case ProductorManager.ProductorType.Designer:
                        designerPoint = value;
                        break;
                    case ProductorManager.ProductorType.Composer:
                        composerPoint = value;
                        break;
                    case ProductorManager.ProductorType.Promotor:
                        promotionPoint = value;
                        break;
                }
            }

            public void Init()
            {
                plannerPoint = 0;
                designerPoint = 0;
                composerPoint = 0;
                promotionPoint = 0;
            }
        }

        public void SetProcessRate(float value)
        {
            this.processingRate = value;
        }

        public float GetMatchingRate()
        {
            return matchingRate;
        }

        public void Init()
        {
            this.Kategorie = BroadcastKeyword.Kategorie.Game;
            this.Content = BroadcastKeyword.Content.LOL;

            broadCastPoint.Init();
        }
    }
    private new void Awake()
    {
        base.Awake();

        SetIsedolImageComponenet();
    }

    [SerializeField] private Broadcast broadCast;

    public static bool IsBroadcastPlanning => isBroadcastPlanning;

    private static bool isBroadcastPlanning;

    void Start()
    {
        broadCast = new Broadcast();
    }

    public void InitBroadcast()
    {
        broadCast.Init();
        isedolSelected = 0;
        isedolSelectCount = 0;

        priceRatio = DefaultPriceRatio;

        foreach (var isedolPointerEvent in isedolPointerEvents)
        {
            isedolPointerEvent.SetAllowMouseEvent(true);
            isedolPointerEvent.SetTransparencyColor();
        }
    }

    private void SetIsedolImageComponenet()
    {
        for (int i = 0; i < isedolSelectParent.transform.childCount; i++)
        {
            isedolPointerEvents.Add(isedolSelectParent.transform.GetChild(i).GetChild(0).GetComponent<ImagePointerEvent>());
        }
    }

    public void SetBroadcastGearText(string str)
    {
        gearText.text = str;
    }

    public void SetKategorieText(string str)
    {
        kategorieText.text = str;
    }

    public void SetContentText(string str)
    {
        contentText.text = str;
    }

    public void SetDirectionText(string str)
    {
        directionText.text = str;
    }

    public void SetPriceRatio(float ratio)
    {
        this.priceRatio = ratio;
    }

    //키워드쪽에서 수치 계산해서 줘야함.
    //패널 끌 때 여기서 가지고 있거나 selection쪽에서 해야하는데.
    //여기서 받은 수치 * 기어배율 * 기획방향 배율로 비용 결정정
    public void SetPriceText(int price)
    {
        priceText.text = string.Format("제작비 : {0}", price.ToString());
    }

    //키워드쪽에서 판단해서 limit값 전달.
    //limit보다 현재 선택된 캐릭터수가 많을 경우
    //글자수를 빨간색으로 변경만하고 다른행동 X
    public void SetMemberCount(int limit)
    {
        
    }


    public float GetCurrentMatchingRate()
    {
        return broadCast.GetMatchingRate();
    }

    /// <summary>
    /// rate가 1부터 5까지 존재하고 제일낮은 0(첫 시도), 1(눕), 2(계륵), 3(프로), 4(국밥), 5(해커)순으로 되고
    /// MenuController에게 매칭률 전달해줌
    /// </summary>
    /// <param name="rate">매칭률</param>
    /// <returns></returns>
    public string GetMatchingRateComment(int index)
    {
        // if (index < 0)
        // {
        //     return matchingRateComment[0];
        // }

        // return matchingRateComment[index];

        return "";
    }

    //matchingrate 값을 broadcastplanning에서 관리하고, 모든 곳에서 수치로 사용
    //카테고리에 넘기는 값만 해당하는 string값으로

    /// <summary>
    /// 
    /// create팝업 패널을 우선적으로 만들어야 하고, 해당 패널에서 작업자 선택을 함 이후 나올 작업자 선택도 해당 패널로 진행
    /// broadcastplanning패널에서 비용을 명시해 줘야 함 => 동시에 플레이어의 돈 관리 시스템도 추가(돈은 GameManager 통해서 datamanager로 반영하는걸로)
    /// 
    /// 
    /// 1. 키워드로 받은 결과 값(matchingRate)을 저장
    /// 2. broadcastcreate창에서 작업자고르기(추후 수정될 수 있음) 
    ///     => 왁타버스 작업자들 등장시킬 예정 (무리라고 생각되면 다른 방안으로)
    /// 3. 작업자 값 + 매칭값 + 장비로 기획 비용 및 결과값 계산 로직
    /// 4. 방송 제작 단계별로 작업자를 정해서 방송을 만드는 걸로
    ///    => 기획, 맵 제작 ... 등 각 단계별로 집중적으로 오르는 분야 점수가 다름
    ///       기획 => 완성도
    ///       맵 제작 => 시청자 만족도 증가
    ///       2개정도 더 추가 예정
    /// 5. 방송을 하고 결과값으로 방송당일의 값과 유튜브 업로드 후의 반응 댓글 및 조회수, 좋아요 수에 따른 결과 반영 로직
    /// 
    /// Textwindow에서 json에서 이벤트 값으로 튜토리얼 설명 및 정보 안내
    /// 
    /// </summary>
    public void CalculateProcessingData(int[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            broadCast.broadCastPoint[ProductorManager.ProductorType.Planner + i] += data[i];
        }

        UpdateBroadcastPoint();
    }


    //화면 하단/상단에 진행도, 스텟 4개 보여지게 
    public void UpdateBroadcastPoint()
    {
        int[] data = new int[ValueCastTo<int>.From(ProductorManager.ProductorType.Count)];

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = broadCast.broadCastPoint[ProductorManager.ProductorType.Planner + i];
        }

        ProcessStatus.Instance.UpdateStatus(data, GetProgress());
    }

    private float GetProgress()
    {
        return broadCast.processingRate;
    }

    public void AddProcessingRate()
    {
        if (CheckMaxProcessing())
        {
            return;
        }


        broadCast.SetProcessRate(broadCast.processingRate + 1);

        UpdateBroadcastPoint();
        CheckNextProcessingStep();
    }

    private bool CheckMaxProcessing()
    {
        if (broadCast.processingRate >= 100)
        {
            //리뷰매니저 리뷰 창 요청
            //BroadcastReviewManager.Instance.OpenReviewData();

            BroadcastPlanningResult.Instance.SetBroadcastResultPoint();
            MenuController.Instance.OpenBroadcastResult();

            broadCast.InitProcessingRate();
            isBroadcastPlanning = false;

            return true;
        }

        return false;
    }

    public void StartBroadcast(int reviewPoint)
    {
        //리뷰값에 따른 추가 점수
        //스텟값 비율
        //팬 수
        //해당 기간 이벤트

        //우선 임의로 3만으로 넣음
        ViewerTabManager.Instance.SetViewerTab(30000);
        //ProcessStatus.Instance.UpdateViewerTap();       //tab에서 받아서 업데이트
    }

    private int[] stepValue = { 40, 80, 100 };

    private int currentProcessStep = 0;

    private void CheckNextProcessingStep()
    {
        if (broadCast.processingRate >= stepValue[currentProcessStep])
        {
            currentProcessStep++;

            NextStepProcess();
        }

        broadCast.SetProcessRate(broadCast.processingRate);             //새로 업데이트 된 진행률 값
        UpdateBroadcastPoint();                                         //새로 받은 값 업데이트(하단 패널 표시)
    }

    private void NextStepProcess()
    {
        if (currentProcessStep >= ValueCastTo<int>.From(ProductorManager.ProcessingType.Count))
        {
            //맥스치 체크 후 return
            CheckMaxProcessing();

            return;
        }
        else
        {
            //현재 단계에 맞는 작업자 선택창
            SetNextProductorSelection();
        }
    }



    public void TestProcessingMethod()
    {
        int processStep = ProductorManager.Instance.TEST_MoveToNextProcessing();
        int value = 0;

        processStep = 3;

        switch (processStep)
        {
            case 0:     //기획
                value = 0;
                break;
            case 1:     //맵 제작
                value = 40;
                break;
            case 2:     //작곡
                value = 80;
                break;
            case 3:
                value = 100;
                break;
        }

        broadCast.SetProcessRate(value);                                //새로 업데이트 된 진행률 값
        UpdateBroadcastPoint();                                         //새로 받은 값 업데이트(하단 패널 표시)

        //이전 작업자가 작곡가였으면
        if (processStep >= ValueCastTo<int>.From(ProductorManager.ProcessingType.Count))
        {
            //맥스치 체크 후 return
            CheckMaxProcessing();

            return;
        }
        else
        {
            //현재 단계에 맞는 작업자 선택창
            SetNextProductorSelection();
        }
    }

    private void SetNextProductorSelection()
    {
        ProductorManager.Instance.TEST_MoveToNextProcessing();
        ProductorManager.Instance.UpdateMoveToFirstProductor();     //작업자 선택 창 설정된 작업자의 제일 앞 작업자로 세팅
        MenuController.Instance.OpenProductorSelection();           //작업자 선택 창 활성화
    }

    public int GetBroadcastPoint(ProductorManager.ProductorType type)
    {
        return broadCast.broadCastPoint[type];
    }

    public void SetBroadcastPlanning()
    {
        isBroadcastPlanning = true;

        //MenuController.Instance.CloseOtherMenu();
    }



    //캐릭터 이벤트 관련
    public void AddSpecialCurrency(int amount)
    {
        //특수 재화 추가
    }
}
