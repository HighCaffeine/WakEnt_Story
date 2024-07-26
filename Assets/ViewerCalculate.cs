using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerCalculate : GenericSingleton<ViewerCalculate>
{
    [SerializeField] private GameObject graphParent;                //그래프 grid 달려있는 오브젝트

    [SerializeField] private List<UnityEngine.UI.Image> graphElements;

    private int totalViewer = 0;

    [SerializeField] private int viewerEachTime;

    [SerializeField] private int maximunMulti = 1;                  //조회수 그래프 최대치 배율(첫 주차 10만 넘을경우 * 10)

    private bool isBroadcastStarted = false; 

    private new void Awake()
    {
        base.Awake();

        for (int i = 0; i < graphParent.transform.childCount; i++)
        {
            graphElements.Add(graphParent.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>());
        }
    }

    public void SetViewerEachTime(int value)
    {
        isBroadcastStarted = true;

        totalViewer = 0;
        week = 1;
        viewerEachTime = value;

        graphElements[graphElements.Count - 1].gameObject.SetActive(true);

        StartCoroutine(Calculate());
    }

    int currentWeekViewer = 0;

    public IEnumerator Calculate()
    {
        int maxViewer = 100000;
        float fillAmount = 0;

        while (isBroadcastStarted)
        {
            if (totalViewer >= maxViewer)
            {
                maxViewer *= 10;

                UpdateLimit();
            }

            fillAmount = 1.0f * currentWeekViewer / maxViewer;

            graphElements[graphElements.Count - 1].fillAmount = fillAmount;

            yield return new WaitForFixedUpdate();
        }
    }

    public void UpdateLimit()
    {
        foreach (var image in graphElements)
        {
            if (image.gameObject.activeSelf)
            {
                image.fillAmount *= 0.1f;
            }
        }
    }

    private int week = 1;

    public void UpdateGraph()
    {
        DecreaseViewerEachTime();

        week++;
        currentWeekViewer = 0;

        graphElements[graphElements.Count - week].gameObject.SetActive(true);

        ChageFillAmount(Mathf.Clamp(graphElements.Count - week, 1, 9), Mathf.Clamp(graphElements.Count - week - 1, 0, 9));
    }

    private void ChageFillAmount(int before, int after)
    {
        // if (before >= graphElements.Count || graphElements[after].gameObject.activeSelf == false)
        // {
        //     return;
        // }

        Debug.Log("왼쪽애 : " + graphElements[after].fillAmount );
        Debug.Log("오른족 : " + graphElements[before].fillAmount);

        graphElements[after].fillAmount = graphElements[before].fillAmount;

        ChageFillAmount(before + 1, after + 1);
    }

    public void DecreaseViewerEachTime()
    {
        viewerEachTime = (int)(viewerEachTime * (1.0f - 0.1 * week));
    }

    public void IncreaseViewerEachTime(int value)
    {

    }

    public bool GetIsStartBroadcast()
    {
        return isBroadcastStarted;
    }

    public void AddViewer()
    {
        totalViewer += viewerEachTime;
        currentWeekViewer += viewerEachTime;

        PlayerController.Instance.AddMoney((long)(viewerEachTime * 0.18f));


        ProcessStatus.Instance.UpdateViewerCount(totalViewer);
    }
}
