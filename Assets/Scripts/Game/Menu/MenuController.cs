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

    [Header("팝업들")]
    [SerializeField] private GameObject broadcastPlanningPanel;     //방송기획창
    [SerializeField] private GameObject broadcastCreatePanel;       //카테고리 전부 선택 후 
    [SerializeField] private GameObject productorSelectionPanel;    //작업자 선택 창
    [SerializeField] private GameObject productorProcessPanel;      //작업자 프로세싱 창
    [SerializeField] private GameObject cafeUserReviewPanel;        //리뷰창
    [SerializeField] private GameObject broadcastResultPanel;       //방송 제작 후 수치 확인 및 방제 변경
    [SerializeField] private GameObject popupStorePanel;            //팝업스토어 패널


    //해당 메뉴들이 켜졌을 경우 화면 좌측 하단에 Temp버튼이 뒤로가기로 변경
    [Header("메뉴들")]
    [SerializeField] private GameObject menuList;                   //메뉴창
    [SerializeField] private GameObject broadcastPlanningMenu;      //방송버튼 누르면 나오는 메뉴창
    [SerializeField] private GameObject productorMenu;              //작업자버튼 누르면 나오는 메뉴
    [SerializeField] private GameObject infoMenu;                   //정보버튼 누르면 나오는 메뉴
    [SerializeField] private GameObject systemMenu;                 //시스템 메뉴

    [SerializeField] private GameObject raycastDisabledPanel;       //


    [SerializeField] private CanvasScaler canvasScaler;             //rect position 사용을 위해 비율 계산

    private new void Awake()
    {
        base.Awake();

        timeRectMask = timeRect.transform.GetComponent<RectMask2D>();
        tempButtonText.text = string.Format("Save");
    }

    public float CanvasScalerRatio
    {
        get
        {
            if (canvasScaler == null)
            {
                canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
            }

            return Screen.width / canvasScaler.referenceResolution.x;
        }
    }


    //==============================팝업창==============================
    //방송-방송제작
    public void OpenBroadcastBroadCastPlan()
    {
        broadcastPlanningPanel.SetActive(true);
        TimeNotElapseWhenOpenTab();
    }
    //방송-숙제방송
    public void OpenBroadcastHomeworkBroadcast()
    {

    }

    //방송-고정컨텐츠
    public void OpenBroadcastFixedContent()
    {

    }

    //작업자-고용
    public void OpenProductorEmployment()
    {

    }

    //작업자-레벨업
    public void OpenProductorLevelUp()
    {

    }

    //작업자-교육
    public void OpenProductorEducation()
    {

    }

    //작업자-해고
    public void OpenProductorFire()
    {


    }

    //정보-작업자정보
    public void OpenInfoProductorInfo()
    {

    }

    //정보-방송이력
    public void OpenInfoBroadcastRecord()
    {

    }

    //정보-팬카페정보
    public void OpenInfoCafeInfo()
    {

    }

    //시스템-게임정보
    public void OpenSystemGameInfo()
    {

    }

    //시스템-저장
    public void OpenSystemSave()
    {

    }

    //시스템-종료
    public void OpenSystemGameExit()
    {

    }

    //시스템-설정
    public void OpenSystemSettings()
    {

    }


    /////////////////////방송제작 관련/////////////////////////
    //방송-방송제작-키워드선택
    public void OpenCreateBroadcast()
    {
        broadcastCreatePanel.SetActive(true);
        TimeNotElapseWhenOpenTab();
    }

    //방송-방송제작-키워드선택-제작
    public void OpenProductorSelection()
    {
        productorSelectionPanel.SetActive(true);
        TimeNotElapseWhenOpenTab();
    }


    public void OpenProductorWorkProcess()
    {
        productorProcessPanel.SetActive(true);
    }

    public void CloseProductorWorkProcess()
    {
        productorProcessPanel.SetActive(false);
        ClosePanelOnEndProcess();
    }
    
    //방송제작완료-결과창
    public void OpenBroadcastResult()
    {
        broadcastResultPanel.SetActive(true);

        //ProcessStatus.Instance.OffCurrentStatusPanel();         //진행정보 탭 끔
        ProcessStatus.Instance.DynamicScaler();                 //스크롤 바 재설정
        
        TimeNotElapseWhenOpenTab();
    }

    //방송제작완료-결과창-리뷰
    public void OpenCafeUserReview()
    {
        broadcastResultPanel.SetActive(false);
        cafeUserReviewPanel.SetActive(true);
        TimeNotElapseWhenOpenTab();
    }
    public void CloseCafeUserReview()
    {
        ProcessStatus.Instance.OffCurrentStatusPanel(); 
        cafeUserReviewPanel.SetActive(false);
        CloseTabElapseTime();
    }

    //팝업스토어-팝업스토어창
    public void OpenPopupStorePanel()
    {

    }
    
    public void ClosePanelOnEndProcess()
    {
        broadcastPlanningPanel.SetActive(false);
        broadcastCreatePanel.SetActive(false);
        productorSelectionPanel.SetActive(false);
        CloseTabElapseTime();
        OpenMenu();
        CloseOtherMenu();

    }
    /////////////////////방송제작 관련/////////////////////////
    //==============================팝업창==============================


    


    //==============================메뉴==============================
    public void OpenMenu()
    {
        if (menuList.activeSelf)
        {
            menuList.SetActive(false);
            tempButtonText.text = string.Format("Save");

            return;
        }

        menuList.SetActive(true);
        tempButtonText.text = string.Format("Back");
    }
    public void OpenBroadcastPlanningMenu()
    {
        OpenDetailMenu();

        broadcastPlanningMenu.SetActive(true);
    }

    public void OpenProductorMenu()
    {
        OpenDetailMenu();

        productorMenu.SetActive(true);
    }

    public void OpenInfoMenu()
    {
        OpenDetailMenu();

        infoMenu.SetActive(true);
    }

    public void OpenSystemMenu()
    {
        OpenDetailMenu();

        systemMenu.SetActive(true);
    }

    //메뉴오픈 시 메뉴 끔
    //평소에는 세이브 버튼
    [SerializeField] private TMPro.TextMeshProUGUI tempButtonText;
    private bool isOpenDetailMenu = false;

    public void TempButtonMethod()
    {
        if (menuList.activeSelf)
        {
            if (isOpenDetailMenu)
            {
                CloseOtherMenu();

                isOpenDetailMenu = false;
            }
            else
            {
                menuList.SetActive(false);

                tempButtonText.text = string.Format("Save");
            }
        }
        else
        {
            GameManager.Instance.Save();
        }
    }

    private void OpenDetailMenu()
    {
        isOpenDetailMenu = true;

        CloseOtherMenu();
    }

    private void CloseOtherMenu()
    {
        broadcastPlanningMenu.SetActive(false);
        productorMenu.SetActive(false);
        infoMenu.SetActive(false);
        systemMenu.SetActive(false);
    }
    //==============================메뉴==============================



    [Header("상단 정보 탭 - 돈, 시간")]
    [SerializeField] private TextMeshProUGUI dateText;
    private RectMask2D timeRectMask;
    [SerializeField] private RectTransform timeRect;
    [SerializeField] private TextMeshProUGUI moneyText;

    //=========================상단 탭(돈, 시간) 업데이트==============================
    public static bool IsOpenTab => isOpenTab;
    private static bool isOpenTab;

    public void TimeNotElapseWhenOpenTab()
    {
        isOpenTab = true;
    }

    public void CloseTabElapseTime()
    {
        isOpenTab = false;
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

    //=========================상단 탭(돈, 시간) 업데이트==============================
}
