using System.Collections;
using System.Collections.Generic;
using Devcat;
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

        timeRectMask = timeRect.transform.GetComponent<RectMask2D>();
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

    public void OpenCreateBroadcast()
    {
        broadcastCreatePanel.SetActive(true);
    }

    public void OpenProductorSelection()
    {
        productorSelectionPanel.SetActive(true);
    }

    public void OpenCafeUserReview()
    {
        broadcastResultPanel.SetActive(false);
        cafeUserReviewPanel.SetActive(true);
    }

    public void OpenBroadcastResult()
    {
        broadcastResultPanel.SetActive(true);
    }

    public void CloseCafeUserReview()
    {
        cafeUserReviewPanel.SetActive(false);
    }

    public void ClosePanelOnEndProcess()
    {
        broadcastPlanningPanel.SetActive(false);
        broadcastCreatePanel.SetActive(false);
        productorSelectionPanel.SetActive(false);
    }

    public void UpdateDate(int year, int month, int week, int time)
    {
        if (dateText != null)
        {
            dateText.text = string.Format("{0}년 {1}월 {2}주", year, month, week);
        }

        if (timeRectMask != null)
        {
            float eachSpace = timeRect.rect.height / 10;
            float bottom = Mathf.Clamp(eachSpace * (time - 1), 0f, timeRect.rect.height);
            float top = timeRect.rect.height - time * eachSpace;
            
            timeRectMask.padding = new Vector4(0f, bottom, 0f, top);
        }
    }

    public void UpdateMoney(long money)
    {
        if (moneyText != null)
        {
            moneyText.text = string.Format("{0:#,###}", money);
        }
    }

    [SerializeField] private TextMeshProUGUI dateText;
    private RectMask2D timeRectMask;
    [SerializeField] private RectTransform timeRect;
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] private GameObject broadcastPlanningPanel;     //방송기획창
    [SerializeField] private GameObject broadcastCreatePanel;       //카테고리 전부 선택 후 
    [SerializeField] private GameObject productorSelectionPanel;    //작업자 선택 창
    [SerializeField] private GameObject cafeUserReviewPanel;        //리뷰창
    [SerializeField] private GameObject broadcastResultPanel;       //방송 제작 후 수치 확인 및 방제 변경
}
