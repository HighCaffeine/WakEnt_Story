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
    [Serializable]
    private class Broadcast
    {
        private KeywordManager.Kategorie Kategorie;
        private KeywordManager.Content Content;

        private float matchingRate;

        public float processingRate 
        {
            private set; 
            get; 
        }

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
                    plannerPoint += value;
                    break;
                    case ProductorManager.ProductorType.Designer:
                    designerPoint += value;
                    break;
                    case ProductorManager.ProductorType.Composer:
                    composerPoint += value;
                    break;
                    case ProductorManager.ProductorType.Promotor:
                    promotionPoint += value;
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

        public void SetKategorie(in KeywordManager.Kategorie Kategorie)
        {
            this.Kategorie = Kategorie;
        }

        public void SetContent(in KeywordManager.Content Content)
        {
            this.Content = Content;
        }

        public void SetMatchingRate(in float value)
        {
            matchingRate = value;
        }

        public void SetProcessRate(float value)
        {
            this.processingRate = value;
        }

        public KeywordManager.Kategorie GetKategorie()
        {
            return Kategorie;
        }

        public KeywordManager.Content GetContent()
        {
            return Content;
        }

        public float GetMatchingRate()
        {
            return matchingRate;
        }

        public void Init()
        {
            this.Kategorie = KeywordManager.Kategorie.Game;
            this.Content = KeywordManager.Content.BroadcasterTogether;

            broadCastPoint.Init();
        }
    }

    

    private new void Awake()
    {
        base.Awake();

        

        keywordMatching = new Dictionary<string, float>();
        matchingRateComment = new List<string>();
        
    }

    //방송 기획에 쓰이는 로직 작성 예정
    //컨텐츠(게임, 추가예정)와 종류(유니티, 시참), 장비(풀트, 모캡등)
    //두 가지 조합해서(조합 매칭 방식을 정해야 함) -> 수치로 하는게 좋은데
    //1.비트연산으로 구현하는걸로 곱연산으로 하고 높을수록 조합 좋은걸로
    // 0    0    0    0
    //2. 숫자 4자리로 하고 계산하는 곳에서 10으로 나눠서 값들 판단하기로 하고
    // 비트연산으로 하면 데이터시트에 표시하기게 애매함 
    //  -> 1~5중에 방송종류(행)와 컨텐츠(열)의 매칭률을 값으로 가지고 있는걸로 
    //   값을 Dictionary로  (컨텐츠_방송종류, 매칭률) 구조로 가는걸로

/// <summary>
/// 방송종류와 콘텐츠를 받아와서 결과 값을 반환
/// 두 키워드가 잘 맞는지 판단하는 로직
/// </summary>
/// <param name="broadcast"></param>
/// <param name="Kategorie"></param>
/// 

    [SerializeField] private Dictionary<string, float> keywordMatching;
    [SerializeField] private List<string> matchingRateComment;

    [SerializeField] private Broadcast broadCast;

    void Start()
    {
        DataManager.Instance.SetBroadcastValue(keywordMatching);
        DataManager.Instance.SetBroadcastMatching(matchingRateComment);

        broadCast = new Broadcast();
    }

    public void InitBroadcast()
    {
        broadCast.Init();
    }


    public string CalculateBroadCastMatchingValue(string Kategorie, string Content)
    {
        SetBroadCastValue(Kategorie, Content);

        return GetMatchingRateComment(Mathf.RoundToInt(broadCast.GetMatchingRate()));
    }

    private void SetBroadCastValue(string Kategorie, string Content)
    {
        int KategorieCount = ValueCastTo<int>.From(KeywordManager.Kategorie.Count);

        for (int i = 0; i < KategorieCount; i++)
        {
            if ((KeywordManager.Content.BroadcasterTogether + i).ToString() == Kategorie)
            {
                broadCast.SetContent(KeywordManager.Content.BroadcasterTogether + i);
            }
        }

        int ContentCount = ValueCastTo<int>.From(KeywordManager.Kategorie.Count);

        for (int i = 0; i < ContentCount; i++)
        {
            if ((KeywordManager.Kategorie.Game + i).ToString() == Content)
            {
                broadCast.SetKategorie(KeywordManager.Kategorie.Game + i);
            }
        }
 
        string key = string.Format(Kategorie + "_" + Content);

        float matchingRate = GetMatchingRate(Kategorie, Content);

        broadCast.SetMatchingRate(matchingRate);
    }

    public float GetMatchingRate(string Kategorie, string Content)
    {
        string key = string.Format("{0}_{1}", Kategorie, Content);

        float value = keywordMatching.ContainsKey(key) ? keywordMatching[key] : 0.0f;

        return value;
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
        if (index < 0)
        {
            return matchingRateComment[0];
        }

        return matchingRateComment[index];
    }

    public string GetCurrentKategorie()
    {
        return DataManager.Instance.ParsingBroadCastDataToString(broadCast.GetKategorie());
    }

    public string GetCurrentContent()
    {
        return DataManager.Instance.ParsingBroadCastDataToString(broadCast.GetContent());
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
    }
 

    //화면 하단/상단에 진행도, 스텟 4개 보여지게 
    public void UpdateBroadcastPoint()
    {
        int[] data = new int[(int)ProductorManager.ProductorType.Count];

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
    }

    private bool CheckMaxProcessing()
    {
        if (broadCast.processingRate >= 100)
        {
            //리뷰매니저 리뷰 창 요청
            //BroadcastReviewManager.Instance.OpenReviewData();

            BroadcastPlanningResult.Instance.SetBroadcastResultPoint();
            MenuController.Instance.OpenBroadcastResult();

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
        ProductorManager.Instance.UpdateMoveToFirstProductor();     //작업자 선택 창 설정된 작업자의 제일 앞 작업자로 세팅
        MenuController.Instance.OpenProductorSelection();           //작업자 선택 창 활성화
    }

    public int GetBroadcastPoint(ProductorManager.ProductorType type)
    {
        return broadCast.broadCastPoint[type];
    }
}
