using TMPro;
using UnityEngine;

public class ProcessStatus : GenericSingleton<ProcessStatus>
{
    [Header("작업 총 스텟")]
    [Space(5f)]
    [Header("0 : 기획, 1 : 맵, 2 : 작곡, 3 : 홍보")]
    [SerializeField] private TextMeshProUGUI[] broadcastStat;
    [SerializeField] private TextMeshProUGUI progress;

    [Header("하단 정보창")] [SerializeField] private GameObject statusParent;


    [Header("방송 조회수")]
    [Space(5f)]
    [SerializeField] private TextMeshProUGUI broadcastName;         //기존 result에 설정한 값 그대로 받아옴
    [SerializeField] private TextMeshProUGUI viewerCount;           //조회수 ㅇㅇㅇ,ㅇㅇㅇ,ㅇㅇㅇ,ㅇㅇㅇ회
    [SerializeField] private TextMeshProUGUI tredingRank;           //인기 급상승 동영상 #n

    

    private new void Awake()
    {
        base.Awake();
    }

    public void UpdateViewerTap()
    {
        broadcastName.text = BroadcastPlanningResult.Instance.GetBroadcastTitle();
        UpdateViewerCount(0);
    }

    public void UpdateViewerCount(int value)
    {
        viewerCount.text = string.Format("조회수 : {0}회", value);
    }

    public void UpdateStatus(int[] data, float progress)
    {
        for (int i = 0; i < data.Length; i++)
        {
            broadcastStat[i].text = string.Format(data[i].ToString());        
        }

        this.progress.text = string.Format("{0:N0}%", progress);
    }

    //PlanningPoint는 Planning 제작 버튼을 누르면 활성화
    //아래 관련 함수 작성

    private int currentStatusCount = 1;

    private GridScaler statusPanelContentGridScaler;

    public void OpenPlanningPoint()
    {
        statusParent.transform.GetChild(currentStatusCount).gameObject.SetActive(true);

        //statusPanelContentGridScaler.DynamicScaler();

        BroadCastPlanning.Instance.UpdateBroadcastPoint();
    }
}
