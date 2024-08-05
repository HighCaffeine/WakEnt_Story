using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerTabManager : ObjectPooling<ViewerTabManager, ViewerTab>
{
    [SerializeField] private List<ViewerTab> viewerTabs;
    [SerializeField] private Transform processTabsParent;           //정보 탭 상위 오브젝트


    //viewerTab으로 옮겨서 각자 탭에서 계산하는 걸로
    //long currentWeekViewer = 0;
    //private long totalViewer = 0;
    //[SerializeField] private long viewerEachTime;

    [SerializeField] private int maximunMulti = 1;                  //조회수 그래프 최대치 배율(첫 주차 10만 넘을경우 * 10)

    

    public static int MaxRank => 10;

    public static int MaxBarCount => 10;

    public static int MaxViewerLimit => 100000;

    private new void Awake()
    {
        base.Awake();
    }

    public void SetViewerTab(int value)
    {
        //pool에서 하나 받아와서 탭 추가
        //넘겨줄 값들 -> title

        ViewerTab viewerTab = GetPool();

        viewerTabs.Add(viewerTab);

        viewerTab.Init();

        viewerTab.transform.SetParent(processTabsParent);
        viewerTab.SetViewerEachTime(value);
    }

    public void TabRemoveFromList(ViewerTab tab)
    {
        viewerTabs.Remove(tab);
    }

    public int GetRank(long viewer)
    {
        return CalculateRank(viewer);
    }

    private int CalculateRank(long viewer)
    {
        //시간에 따라 랭크 값 변경.

        //임시로 1~10까지 랜덤 값
        return Random.Range(1, 11);
    }

    public bool GetIsStartBroadcast()
    {
        foreach (var tab in viewerTabs)
        {
            if (tab.GetIsStartBroadcast())
            {
                return true;
            }
        }

        return false;
    }

    public void AddViewer()
    {
        foreach (var tab in viewerTabs)
        {
            if (tab.GetIsStartBroadcast())
            {
                tab.AddViewer();
            }
        }
    }

    public void UpdateGraph()
    {
        foreach (var tab in viewerTabs)
        {
            if (tab.GetIsStartBroadcast())
            {
                tab.UpdateGraph();
            }
        }
    }

    public void SetPoolParent(Transform tab)
    {
        PooledObjectSetParent(tab);
    }
}
