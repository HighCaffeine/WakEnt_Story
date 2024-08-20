using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class GameManager : GenericSingleton<GameManager>
{

    public static int OneWeekMaxValue => 10;

    public float GameTime => gameTime;

    private float gameTime;

    public static bool IsGamePause => isGamePause;

    private static bool isGamePause;

    private int currenttime;
    [SerializeField] private const float oneTickTime = 1f;
    [SerializeField] private const float oneWeekTime = 10f;

    public static float GetGameValueMultiple() =>  10f / oneWeekTime;

    [SerializeField] private TMPro.TextMeshProUGUI frameText;


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


    //===============================FrameCheck========================================
    private float frameTime = 0.0f;

    private float deltaTime = 0.0f;

    private float frameCheckTime = 1f;

    private void OnGUI()
    {
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

        //프레임 값은 추후 설정으로 ㄱㄱ
        Application.targetFrameRate = 60;
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
}
