using System.Collections;
using System.Collections.Generic;
using Devcat;
using TMPro;
using UnityEngine;

public class BroadcastReviewManager : GenericSingleton<BroadcastReviewManager>
{
    //각 Review스크립트에 UserInfo로 점수를 여기서 계산해서 줌
    //점수와 점수에 해당하는 등급 맨트를 넘겨줘서 넘겨받은 멘트를 각자 스크립트에서 본인 메세지 업데이트 함

    public enum CafeRank
    {
        JinDeuGi, 
        DalgDulGi, 
        WakMuSae, 
        Chimpaenchi, 
        NeuGeuJa,
        Count,
    }

    public enum ReviewPointSections
    {
        FIRST = 3,
        SECOND = 6,
        THIRD = 9,
        FOURTH = 10,

        Count = 4
    }

    
    [System.Serializable]
    public class UserReviewSetUp : QuickSort.GetIntValue
    {
        [SerializeField] private CafeRank cafeRank;
        [SerializeField] public CafeRank CafeRank => ValueCastTo<CafeRank>.From(ValueCastTo<int>.From(cafeRank));
        [SerializeField] public int CafeRankInt => ValueCastTo<int>.From(cafeRank);
        [SerializeField] private OnSetUpEvent onSetUpEvent;

        [SerializeField] public delegate void OnSetUpEvent();


        //초기화
        public void InitSetUpEvnet(CafeRank cafeRank, OnSetUpEvent OnSetUpEvent)
        {
            this.cafeRank = cafeRank;
            onSetUpEvent = OnSetUpEvent;
        }

        //이벤트 실행
        public void EventAction()
        {
            onSetUpEvent?.Invoke();
        }

        public int GetIntForSort()
        {
            return CafeRankInt;
        }
    }

    public static float PointRollTime => 2f;

    public static int ReviewMaxPoint => 10;

    // int : caferank, int : point, string : comment
    private Dictionary<int, Dictionary<int, string[]>> commentDictionary;

    [SerializeField]private List<UserReviewSetUp> setUpEvents;


    [SerializeField] private Animator totalPointDownAnimator;

    [SerializeField] private TextMeshProUGUI broadcastTitle;

    [SerializeField] private TextMeshProUGUI totalPoint;
    [SerializeField] private GameObject fixedContent;

    private int totalPointValue;

    public delegate void OnSetUp();

    public interface OnGetComment
    {
        public void SetGetCommentEvent(OnGetCommentEvent OnGetCommentEvent);
        public void SetGetPointEvent(OnGetPointEvnet OnGetPointEvnet);
        public void SetGetDefaultCommentEvent(OnGetDefaultCommentEvent OnGetDefaultCommentEvent);
    }

    public delegate string OnGetCommentEvent(CafeRank rank, CafeRankInfo cafeRankInfo);
    public delegate int OnGetPointEvnet(CafeRank rank, CafeRankInfo cafeRankInfo);
    public delegate string OnGetDefaultCommentEvent(CafeRank rank);

    private new void Awake()
    {
        base.Awake();

        setUpEvents = new List<UserReviewSetUp>();
        commentDictionary = new Dictionary<int, Dictionary<int, string[]>>();

        MenuController.Instance.OpenCafeUserReview();

        InitPanelset();
    }

    void Start()
    {
        DataManager.Instance.SetReviewComment(commentDictionary);
    }

    /// <summary>
    /// index 0 : point, 1 : comment
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public string GetComment(CafeRank rank, CafeRankInfo cafeRankInfo)
    {
        int key = ConvertPointToDictionaryKey(GetBroadcastReviewPoint(rank, cafeRankInfo));

        int rand = Random.Range(0, 2);

        string value = commentDictionary[ValueCastTo<int>.From(rank)][key][rand];

        return value;
    }

    [System.Serializable] private class DefaultReviewCommentEachCafeRank
    {
        [SerializeField] private string[] comment;

        public string this[int index]
        {
            get
            {
                return comment[index];
            }

            private set
            {

            }
        }

        DefaultReviewCommentEachCafeRank()
        {
            comment = new string[(int)CafeRank.Count];
        }
    }

    [SerializeField] DefaultReviewCommentEachCafeRank defaultReviewCommentEachCafeRank;


    public void OpenReviewData()
    {
        MenuController.Instance.OpenCafeUserReview();

        InitPanelset();

        StartCoroutine(SetUpComment());
    }

    private void InitPanelset()
    {
        fixedContent.SetActive(false);
        totalPointValue = 0;

        ProcessStatus.Instance.DynamicScaler(); 
    }

    
    //테스트용으로 버튼만들어서 작업자 단계로 바로 스킵 가능하게 만들예정
    //0%(기획), 40%(맵 제작), 80%(작곡)
    public int GetBroadcastReviewPoint(CafeRank rank, CafeRankInfo cafeRankInfo)
    {
        //         BroadcastReviewManager
        // ->방송매니저가 넘겨준 값으로 여기서 점수 매기는 걸로 ()
        // ->점수 매기는 기준 총 10점까지 줄 수 있고, 
        // 	4점 -> 매칭률
        // 	2점 -> 기대도
        // 	4점 -> 스텟치
        //  각 랭크마다 3요소를 다르게 체크할 예정
        // 테스트 위해서 임의로 지정 예정
        //

        int matchingReview = GetMatchingReview(cafeRankInfo.reviewRatio.MatchingReviewRatio);
        int expectations = GetExpectations(cafeRankInfo.reviewRatio.ExpectationsRatio);
        int statPoint = GetStatPoint(cafeRankInfo.reviewRatio.StatPointRatio);

        switch (rank)
        {
            case CafeRank.JinDeuGi:
            break;
            case CafeRank.DalgDulGi:
            break;
            case CafeRank.WakMuSae:
            break;
            case CafeRank.Chimpaenchi:
            break;
            case CafeRank.NeuGeuJa:
            break;
        }

        return 10;
    }

    private int ConvertPointToDictionaryKey(int point)
    {
        int index = ValueCastTo<int>.From(ReviewPointSections.Count);

        for (int i = 0; i < index; i++)
        {
            int enumNum = Mathf.Clamp(3 * (i + 1), 0, 10);

            ReviewPointSections reviewPointSections = ValueCastTo<ReviewPointSections>.From(enumNum);

            if (point <= ValueCastTo<int>.From(reviewPointSections))
            {
                totalPointValue += point;

                return i + 1;
            }
        }

        return 0;
    }

    //아래 함수들은 카페 랭크들의 점수 반영 비율에 따라 계산해서 넘겨줌
    private int GetMatchingReview(int ratio)
    {
        return 1;
    }

    private int GetExpectations(int ratio)
    {
        return 1;
    }

    private int GetStatPoint(int ratio)
    {
        return 1;
    }

    //방송 제작 끝난 후 broadcastmanager가 해당 함수로 요청
    public IEnumerator SetUpComment()
    {
        broadcastTitle.text = BroadcastPlanningResult.Instance.GetBroadcastTitle();

        foreach (var eventData in setUpEvents)
        {
            eventData.EventAction();

            yield return new WaitForSeconds(PointRollTime + 0.2f);
        }

        yield return new WaitForSeconds(0.3f);

        totalPointDownAnimator.Play("TotalPointMoveToDown");
        totalPoint.text = totalPointValue.ToString();

        if (totalPointValue >= 40)
        {
            fixedContent.SetActive(true);

            SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_ReviewTotalPointFixedContent.ToString(), false);
        }
        else
        {
            //sfx 점수별로 다르게 1~4, 5~9, 10
            //고정 컨텐츠 점수 미만일 경우 (40점 미만) 일반 sfx
            SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_ReviewTotalPoint.ToString(), false);

        }
    
    }

    public string GetDefaultCommentMessage(CafeRank cafeRank)
    {
        int index = (int)cafeRank;

        if (index >= (int)CafeRank.Count)
        {
            return "흠르르";
        }

        return defaultReviewCommentEachCafeRank[index];
    }

    public void CommentEventAddToList(UserReviewSetUp SetUP)
    {
        setUpEvents.Add(SetUP);

        if (setUpEvents.Count == ValueCastTo<int>.From(CafeRank.Count))
        {
            UserReviewSetUp[] sortedArray = QuickSort.GetSorting(setUpEvents.ToArray());
            setUpEvents.Clear();

            foreach (var data in sortedArray)
            {
                setUpEvents.Add(data);
            }

            MenuController.Instance.CloseCafeUserReview();
        }
    }

    public void ConfirmReviewPanel()
    {
        //리뷰점수를 broadcastplanning한테 넘겨서
        //broadcastplanning에서 스텟 + 기대도 + 리뷰점수 총합해서 viewercalculate로 첫 주 뷰어수 넘겨줌

        BroadCastPlanning.Instance.StartBroadcast(totalPointValue);
        //SoundManager.Instance.PlaySound(SoundManager.BGM.BGM_WakEnt_1.ToString());
    }
}
