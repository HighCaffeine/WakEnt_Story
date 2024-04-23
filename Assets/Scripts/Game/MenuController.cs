using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : GenericSingleton<MenuController>
{
    //게임씬에 있는 모든 메뉴를 관리하고
    //기획창은 BroadcastPlaning 파일 만들어서 따로 거기서 작동소스 작성하기로

    

    private enum Menu
    {
        Planning,
        Arbeit,
        Info,
        Settings,

        Count
    }

    private new void Awake()
    {
        base.Awake();
    }

    public void OpenArbeit()
    {

    }

    public void OpenInfo()
    {

    }
    
    public void OpenSettings()
    {

    }

    public void OpenBroadCastPlan()
    {
        broadcastPlanningPanel.SetActive(true);
    }

    [SerializeField] private GameObject broadcastPlanningPanel;

    [SerializeField] private TextMeshProUGUI broadCastMatchingComment;

    public void RequestGetBroadcastMatchingComment()
    {
        broadCastMatchingComment.text = BroadCastPlanning.Instance.GetBroadCastMatchingRateComment();
    }
}
