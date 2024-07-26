using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : GenericSingleton<GameManager>
{

    public static int OneWeekMaxValue => 10;

    public float GameTime => gameTime;

    private float gameTime;

    private int currenttime;
    [SerializeField] private float oneTickTime = 1f;
    [SerializeField] private float oneWeekTime = 10f;

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
        time += Time.deltaTime;

        if (time >= oneTickTime)
        {
            time = 0f;

            currenttime++;
            countForCheckWeek++;

            UpdateTime();

            if (ViewerCalculate.Instance.GetIsStartBroadcast())
            {
                ViewerCalculate.Instance.AddViewer();

                if (countForCheckWeek >= 10)
                {
                    countForCheckWeek = 0;

                    ViewerCalculate.Instance.UpdateGraph();
                }
            }
        }
    }

    private new void Awake()
    {
        base.Awake();
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
