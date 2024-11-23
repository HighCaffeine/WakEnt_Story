using System.Collections;
using UnityEngine;
using TMPro;

public class ViewerTab : MonoBehaviour, OnReturnPool<ViewerTab>
{
    [SerializeField] private ViewerBar[] viewerBars;

    [Header("방송 조회수")]
    [Space(5f)]
    [SerializeField] private TextMeshProUGUI broadcastName;         //기존 result에 설정한 값 그대로 받아옴
    [SerializeField] private TextMeshProUGUI viewerCount;           //조회수 ㅇㅇㅇ,ㅇㅇㅇ,ㅇㅇㅇ,ㅇㅇㅇ회
    [SerializeField] private TextMeshProUGUI trendingRank;           //인기 급상승 동영상 #n

    OnReturnPoolEvent<ViewerTab> OnReturnPoolEvent;

    [SerializeField] private long viewerEachTime;
    [SerializeField]   private long totalViewer = 0;
    [SerializeField]   private long currentWeekViewer = 0;
    [SerializeField]   private int week = 1;

    [SerializeField]   private bool isBroadcastStarted = false; 

    void OnEnable()
    {
        Transform graph = transform.Find("ViewerGraph/Graph").transform;

        viewerBars = new ViewerBar[ViewerTabManager.MaxBarCount];

        for (int i = 0; i < graph.childCount; i++)
        {
            viewerBars[i] = graph.GetChild(i).GetComponent<ViewerBar>();
        }
    }

    public void Init()
    {
        broadcastName.text = BroadcastPlanningResult.Instance.GetBroadcastTitle();

        UpdateViewerCount(0);
    }

    public void SetViewerEachTime(int value)
    {
        StartCoroutine(SetViewerEachTimeCoroutine(value));
    }


    private IEnumerator SetViewerEachTimeCoroutine(int value)
    {
        yield return StartCoroutine(GameManager.Instance.CheckCanStartBroadcast());

        InfoMessageGroup.Instance.RequestMessage(BroadcastPlanningResult.Instance.GetBroadcastTitle() + " 업로드");

        isBroadcastStarted = true;
        isFirstWeek = true;

        totalViewer = 0;
        week = 1;
        viewerEachTime = value;

        viewerBars[viewerBars.Length - 1].gameObject.SetActive(true);

        StartCoroutine(Calculate());
    }

    public IEnumerator Calculate()
    {
        int maxViewer = ViewerTabManager.MaxViewerLimit;
        float fillAmount = 0;

        while (isBroadcastStarted)
        {
            yield return new WaitUntil( () => { return GameManager.IsGamePause == false; }); 

            if (currentWeekViewer >= maxViewer)
            {
                maxViewer *= 10;

                UpdateLimit();
            }

            fillAmount = 1.0f * currentWeekViewer / maxViewer * GameManager.GetGameValueMultiple();

            viewerBars[viewerBars.Length - 1].SetUpData(fillAmount);

            yield return new WaitForFixedUpdate();
        }

        EndCalculate();
    }

    private void EndCalculate()
    {
        OnReturnPoolEvent?.Invoke(this);

        ViewerTabManager.Instance.TabRemoveFromList(this);

        ViewerTabManager.Instance.SetPoolParent(transform);
    }

    public int CalculateTrendVideoRanking()
    {
        //주간 조회수 랭킹 집계
        int rank = GetRank(totalViewer);      //11위부터는 순위권 밖

        UpdateTrendingRank(rank);  

        return rank;          
    }

    public void UpdateLimit()
    {
        foreach (var bar in viewerBars)
        {
            bar.MultiBarFill(0.1f);
        }
    }

    private bool isFirstWeek;
    public void UpdateGraph()
    {
        if (!isBroadcastStarted || isFirstWeek)
        {
            isFirstWeek = false;

            return;
        }

        DecreaseViewerEachTime();

        week++;
        currentWeekViewer = 0;

                       

        //FirstLineReturnPool();                      //전부 다 켜져있다면 막대들 왼쪽으로 밀기전에 제일 왼쪽에 있는 line pool로 돌려보냄
                                                        //각자 받은 line을 계속 쓰다가 탭 끝나면 다 pool return

        viewerBars[viewerBars.Length - 1].SetEachRank(CalculateTrendVideoRanking()); //주간 조회수 집계 및 랭킹 업데이트, 막대기 랭크포인트 위치 등록 
        viewerBars[viewerBars.Length - 1].OffPointObject();

        ChageFillAmount(Mathf.Clamp(viewerBars.Length - week + 1, 1, 9), Mathf.Clamp(viewerBars.Length - week, 0, 9));

        //TrendingRankLineManager.Instance.TestDrawLine(viewerBars);
        StartCoroutine(TrendingRankLineManager.Instance.TestDrawCoroutine(viewerBars));
    }

    int testRank = 1;

    private int GetRank(long viewer)
    {
        //return ViewerTabManager.Instance.GetRank(viewer);

        return testRank++;
    }

    public void DecreaseViewerEachTime()
    {
        viewerEachTime = Mathf.Clamp((int)(viewerEachTime * (1.0f - 0.1 * week)), 0, int.MaxValue);
    }

    public void IncreaseViewerEachTime(int value)
    {

    }

    private void ChageFillAmount(int before, int after)
    {
        if (before >= viewerBars.Length)
        {
            return;
        }

        viewerBars[after].SetUpData(viewerBars[before].GetFillAmount());

        SetRankPoint(viewerBars[before].GetRank(), after);

        ChageFillAmount(before + 1, after + 1);
    }

    public void AddViewer()
    {
        totalViewer += viewerEachTime;
        currentWeekViewer += viewerEachTime;

        long addMoneyValue = (long)(viewerEachTime * 0.18f);

        PlayerController.Instance.AddMoney(addMoneyValue);

        ProcessStatus.Instance.UpdateDataEachYear(viewerEachTime, addMoneyValue);

        UpdateViewerCount(totalViewer);
    }

    public void UpdateTrendingRank(int rank)
    {
        if (1 <= rank && rank <= ViewerTabManager.MaxRank)
        {
            trendingRank.text = string.Format("인기 급상승 동영상 #{0}", rank);

            return;
        }

        trendingRank.text = string.Format("인기 급상승 동영상 순위 밖");
    }

    public void UpdateViewerTap()
    {
        broadcastName.text = BroadcastPlanningResult.Instance.GetBroadcastTitle();
        UpdateViewerCount(0);
    }

    public void UpdateViewerCount(long value)
    {
        viewerCount.text = string.Format("조회수 : {0:#,###}회", value);
    }

    public bool GetIsStartBroadcast()
    {
        return isBroadcastStarted;
    } 


    public void SetBarFill(float fillAmount, int index)
    {
        viewerBars[index].SetUpData(fillAmount);
    }

    public void SetRankPoint(int rank, int index)
    {
        viewerBars[index].SetEachRank(rank);
    }

    public void Init(OnReturnPoolEvent<ViewerTab> onReturnPoolEvent)
    {
        this.OnReturnPoolEvent = onReturnPoolEvent;
    }
}
