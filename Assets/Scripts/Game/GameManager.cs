using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GameManager : GenericSingleton<GameManager>
{
    public static int OneWeekMaxValue => 10;

    public float GameTime => gameTime;

    private float gameTime;

    public static bool IsGamePause => isGamePause;

    private static bool isGamePause;

    private float beforeTimeScale;
    private int currenttime;

    private List<Action<bool>> pauseEventList = new List<Action<bool>>();

    [SerializeField] private const float oneTickTime = 1f;
    [SerializeField] private const float oneWeekTime = 10f;

    //뷰어 값 한 주 값 따라서 변동되게 (기존 설정한 1주 10tick / targetTick)
    public static float GetGameValueMultiple() =>  10f / oneWeekTime;

    [SerializeField] private TMPro.TextMeshProUGUI frameText;


    [Header("Cursor")]
    [SerializeField] private Texture2D cursorImage;

    public Action Save => OnSave;
    public Action Load => OnLoad;

    private void OnSave()
    {
        Debug.Log("save");
    }

    private void OnLoad()
    {
        Debug.Log("load");
    }

    private float time;
    private short countForCheckWeek = 0;

    private void FixedUpdate()
    {
        if (MenuController.IsOpenTab)
        {
            return;
        }

        //에디터 외에는 메인부터 시작해서 제한 둠
        if (!SceneController.IsLoadGameScene)
        {
            return;
        }
        

        time += Time.deltaTime;

        if (time >= oneTickTime)
        {
            time = 0f;

            currenttime++;
            countForCheckWeek++;

            UpdateTime();

            if (ViewerTabManager.Instance.GetIsStartBroadcast())
            {
                ViewerTabManager.Instance.AddViewer();

                if (countForCheckWeek >= oneWeekTime)
                {
                    countForCheckWeek = 0;

                    ViewerTabManager.Instance.UpdateGraph();
                }
            }
        }
    }

    //==============================빌드 시 삭제========================================
    //===============================FrameCheck========================================
    private float frameTime = 0.0f;

    private float deltaTime = 0.0f;

    private float frameCheckTime = 1f;

    private void OnGUI()
    {
        if (frameText == null)
        {
            return;
        }
        float fps = 1.0f / deltaTime;
        float ms = deltaTime * 1000.0f;
        frameTime += Time.deltaTime;


        if (frameCheckTime <= frameTime)
        {
            frameTime = 0.0f;

            frameText.text = string.Format("FPS : {0:N0} ({1:N1}ms)", fps, ms);
        }
    }

    //===============================FrameCheck========================================

    public IEnumerator CheckCanStartBroadcast()
    {
        if (currenttime % 10 == 0)
        {
            yield return null;
        }
        else
        {
            while (currenttime % 10 != 0)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;
    }

    private new void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(this);

        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.Auto);



        //프레임 값은 추후 설정으로 ㄱㄱ
        Application.targetFrameRate = 60;
        beforeTimeScale = Time.timeScale;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }


    private void Start()
    {
        DataManager.Instance.SetDate(ref currenttime);

        UpdateTime();

    }

    private void Test()
    {
        Time.timeScale = 0.0f;

    }

    public void GameExit()
    {
        Save();

        Application.Quit();
    }


    public void AddTime()
    {
        currenttime ++;

        UpdateTime();
    }

    private void UpdateTime()
    {
        int year = 1 + currenttime / 10 / 48;
        int month = 1 + (currenttime / 10 % 48) / 4;
        int week = 1 + (currenttime / 10 % 48) % 4; 
        int time = 1 + currenttime % 10;

        MenuController.Instance.UpdateDate(year, month, week, time);
    }

    public void PauseGame()
    {
        isGamePause = true;
        
        CallbackPauseEvent(isGamePause);
    }

    public void ResumeGame()
    {
        isGamePause = false;

        CallbackPauseEvent(isGamePause);
    }

    public void RegisterPauseEvent(List<Action<bool>> pauseEventInManager)
    {
        foreach (var pauseEvent in pauseEventInManager)
        {
            pauseEventList.Add(pauseEvent);
        }
    }

    public void RegisterPauseEvent(Action<bool> pauseEvent)
    {
        pauseEventList.Add(pauseEvent);
    }

    /// <summary>
    /// 게임 일시정지 시 멈춰야 할 객체들을 관리하는 측에서
    /// 각 객체들에게 멈추게할 수 있는 함수들을 정의 후 
    /// 관리 객체가 해당 함수들을 콜백함수로써 호출하는 함수를 pauseEventList에 등록하여 사용
    /// </summary>
    /// <param name="gamePause"></param>
    private void CallbackPauseEvent(bool gamePause)
    {
            foreach (var pauseEvent in pauseEventList)
            {
                pauseEvent?.Invoke(gamePause);
            }
    }
}