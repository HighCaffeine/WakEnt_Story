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


    private new void Awake()
    {
        base.Awake();
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

    [SerializeField] private GridScaler statusPanelContentGridScaler;

    public void OpenPlanningPoint()
    {
        statusParent.transform.GetChild(currentStatusCount).gameObject.SetActive(true);

        DynamicScaler();

        BroadCastPlanning.Instance.UpdateBroadcastPoint();
    }

    public void DynamicScaler()
    {
        statusPanelContentGridScaler.DynamicScaler();
    }

    public void InActiveProcessTab()
    {
        statusParent.transform.GetChild(currentStatusCount).gameObject.SetActive(true);
    }

    
}