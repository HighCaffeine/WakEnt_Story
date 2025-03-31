using System;
using System.Collections;
using System.Collections.Generic;
using Devcat;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    [Serializable]
    struct PanelObject
    {
        public GameObject panel;
        public UnityEvent offEvent;
        public UnityEvent onEvent;

        public void OpenPanel()
        {
            panel.SetActive(true);
            onEvent?.Invoke();
        }

        public void ClosePanel()
        {
            offEvent?.Invoke();
            panel.SetActive(false);
        }
    }

    [Header("팝업들")]
    [SerializeField] private PanelObject broadcastPlanningPanelObject;  //방송기획창

    [SerializeField] private PanelObject gearSelectionPanelObject;      //장비 선택창
    [SerializeField] private PanelObject keywordSelectionPanelObject;   //키워드 선택창
    [SerializeField] private PanelObject directionSelectionPanelObject; //기획 방향성 선택창
    [SerializeField] private PanelObject broadcastCreatePanelObject;    //카테고리 전부 선택 후 

    [SerializeField] private PanelObject productorSelectionPanelObject; //작업자 선택 창

    [SerializeField] private PanelObject productorProcessPanelObject;   //작업자 프로세싱 창

    [SerializeField] private PanelObject cafeUserReviewPanelObject;     //리뷰창

    [SerializeField] private PanelObject broadcastResultPanelObject;    //방송 제작 후 수치 확인 및 방제 변경

    [SerializeField] private PanelObject popupStorePanelObject;         //팝업스토어 패널


    //해당 메뉴들이 켜졌을 경우 화면 좌측 하단에 Temp버튼이 뒤로가기로 변경
    [Header("메뉴들")]
    [SerializeField] private PanelObject menuList;                   //메뉴창
    [SerializeField] private PanelObject broadcastPlanningMenu;      //방송버튼 누르면 나오는 메뉴창
    [SerializeField] private PanelObject productorMenu;              //작업자버튼 누르면 나오는 메뉴
    [SerializeField] private PanelObject infoMenu;                   //정보버튼 누르면 나오는 메뉴
    [SerializeField] private PanelObject systemMenu;                 //시스템 메뉴

    [SerializeField] private GameObject raycastDisabledPanel;       //


    [SerializeField] private CanvasScaler canvasScaler;             //rect position 사용을 위해 비율 계산

    private Stack<PanelObject> menuStorage;

    private new void Awake()
    {
        base.Awake();

        menuStorage = new Stack<PanelObject>();

        timeRectMask = timeRect.transform.GetComponent<RectMask2D>();
        interactiveButton.text = string.Format("Save");
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

    public void MenuBack()
    {
        PanelObject menu = menuStorage.Pop();
        menu.ClosePanel();

        CheckActiveBackButton(true);

        if (menuStorage.Count == 0)
        {
            TimeResume();
        }
    }

    public void CloseAllMenu()
    {
        while (menuStorage.Count > 0)
        {
            MenuBack();
        }
    }

    private void MenuOpen(PanelObject menu)
    {
        if (menuStorage.Count == 0)
        {
            TimePause();
        }

        menuStorage.Push(menu);
        menu.OpenPanel();
        CheckActiveBackButton(false);
    }

    private void TimePause()
    {
        isOpenTab = true;

        GameManager.Instance.PauseGame();
    }

    private void TimeResume()
    {
        isOpenTab = false;

        GameManager.Instance.ResumeGame();
    }

    private void CheckActiveBackButton(bool back)
    {
        if (menuStorage.Count == 0 && back)
        {
            //back버튼 비활성화 및 시간 다시 흐르게
            interactiveButton.text = string.Format("Save");
            GameManager.Instance.ResumeGame();
        }
        else if (menuStorage.Count == 1 && !back)
        {
            //back버튼 활성화 및 시간 정지
            interactiveButton.text = string.Format("Back");
            GameManager.Instance.PauseGame();
        }
    }

    public void LeftButtonInteractive()
    {
        if (menuStorage.Count == 0)
        {
            GameManager.Instance.Save();
        }
        else
        {
            MenuBack();
        }
    }

    public void OpenMenuList()
    {
        if (menuList.panel.activeSelf)
        {
            MenuBack();
            return;
        }

        MenuOpen(menuList);
    }

    //==============================팝업창==============================
    //방송-방송제작
    //버튼 누를 때 나오는 창에만 closealldetailmenu 실행
    public void OpenBroadcastBroadCastPlan()
    {
        CloseAllDetailMenu();
        MenuOpen(broadcastPlanningPanelObject);
    }
    public void OpenBroadcastGearSelection()
    {
        MenuOpen(gearSelectionPanelObject);
    }

    public void OpenBroadcastPlanningDirection()
    {
        MenuOpen(directionSelectionPanelObject);
    }

    public void OpenBroadcastKeywordSelection()
    {
        MenuOpen(keywordSelectionPanelObject);
    }
    //방송-숙제방송
    public void OpenBroadcastHomeworkBroadcast()
    {
        CloseAllDetailMenu();
        //
    }

    //방송-고정컨텐츠
    public void OpenBroadcastFixedContent()
    {
        CloseAllDetailMenu();
        //
    }

    //작업자-고용
    public void OpenProductorEmployment()
    {
        CloseAllDetailMenu();
        //
    }

    //작업자-레벨업
    public void OpenProductorLevelUp()
    {
        CloseAllDetailMenu();
        //
    }

    //작업자-교육
    public void OpenProductorEducation()
    {
        CloseAllDetailMenu();
        //
    }

    //작업자-해고
    public void OpenProductorFire()
    {
        CloseAllDetailMenu();
        //
    }

    //정보-작업자정보
    public void OpenInfoProductorInfo()
    {
        CloseAllDetailMenu();
        //
    }

    //정보-방송이력
    public void OpenInfoBroadcastRecord()
    {
        CloseAllDetailMenu();
        //
    }

    //정보-팬카페정보
    public void OpenInfoCafeInfo()
    {
        CloseAllDetailMenu();
        //
    }

    //시스템-게임정보
    public void OpenSystemGameInfo()
    {
        CloseAllDetailMenu();
        //
    }

    //시스템-저장
    public void OpenSystemSave()
    {
        CloseAllDetailMenu();
        //
    }

    //시스템-종료
    public void OpenSystemGameExit()
    {
        CloseAllDetailMenu();
        //
    }

    //시스템-설정
    public void OpenSystemSettings()
    {
        CloseAllDetailMenu();
        //
    }


    /////////////////////방송제작 관련/////////////////////////
    //방송-방송제작-키워드선택
    public void OpenCreateBroadcast()
    {
        CloseAllDetailMenu();
        MenuOpen(broadcastCreatePanelObject);
    }

    //방송-방송제작-키워드선택-제작
    public void OpenProductorSelection()
    {
        MenuOpen(productorSelectionPanelObject);
    }


    public void OpenProductorWorkProcess()
    {
        MenuOpen(productorProcessPanelObject);
    }

    public void CloseProductorWorkProcess()
    {
        MenuOpen(productorProcessPanelObject);
        ClosePanelOnEndProcess();
    }
    
    //방송제작완료-결과창
    public void OpenBroadcastResult()
    {
        MenuOpen(broadcastResultPanelObject);

        //ProcessStatus.Instance.OffCurrentStatusPanel();         //진행정보 탭 끔
        ProcessStatus.Instance.DynamicScaler();                 //스크롤 바 재설정
    }

    //방송제작완료-결과창-리뷰
    public void OpenCafeUserReview()
    {
        MenuBack();
        MenuOpen(cafeUserReviewPanelObject);
    }
    public void CloseCafeUserReview()
    {
        ProcessStatus.Instance.OffCurrentStatusPanel(); 
        MenuBack();
    }

    //팝업스토어-팝업스토어창
    public void OpenPopupStorePanel()
    {
        CloseAllDetailMenu();
        //
    }
    
    public void ClosePanelOnEndProcess()
    {
        CloseAllMenu();
    }
    /////////////////////방송제작 관련/////////////////////////
    //==============================팝업창==============================


    


    //==============================메뉴==============================
    // public void OpenMenu()
    // {
    //     if (menuList.activeSelf)
    //     {
    //         menuList.SetActive(false);
    //         interactiveButton.text = string.Format("Save");

    //         CloseTabElapseTime();

    //         return;
    //     }

    //     TimeNotElapseWhenOpenTab();

    //     menuList.SetActive(true);
    //     interactiveButton.text = string.Format("Back");
    // }
    public void OpenBroadcastPlanningMenu()
    {
        OpenDetailMenu();

        MenuOpen(broadcastPlanningMenu);
        //broadcastPlanningMenu.SetActive(true);
    }

    public void OpenProductorMenu()
    {
        OpenDetailMenu();
        MenuOpen(productorMenu);
        //productorMenu.SetActive(true);
    }

    public void OpenInfoMenu()
    {
        OpenDetailMenu();
        MenuOpen(infoMenu);
        //infoMenu.SetActive(true);
    }

    public void OpenSystemMenu()
    {
        OpenDetailMenu();
        MenuOpen(systemMenu);
        //systemMenu.SetActive(true);
    }

    private void CloseAllDetailMenu()
    {
        if (!isOpenDetailMenu) return;

        CloseAllMenu();
        isOpenDetailMenu = false;
    }

    //메뉴오픈 시 메뉴 끔
    //평소에는 세이브 버튼
    [SerializeField] private TMPro.TextMeshProUGUI interactiveButton;

    private bool isOpenDetailMenu = false;

    private void OpenDetailMenu()
    {
        if (isOpenDetailMenu)
        {
            MenuBack();
        }

        isOpenDetailMenu = true;
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
