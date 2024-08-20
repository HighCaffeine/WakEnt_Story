using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerBar : MonoBehaviour
{
    [SerializeField] private RectTransform trendingRankPoint;

    [SerializeField] private UnityEngine.UI.Image bar;

    private Vector2 firstPoint;

    private int myRankPoint;

    void Awake()
    {
        firstPoint = trendingRankPoint.anchoredPosition;
    }


    public void SetUpData(float fillAmount)
    {
        gameObject.SetActive(true);
          
        bar.fillAmount = fillAmount;
    }

    public float GetFillAmount()
    {
        return bar.fillAmount;
    }

    public void MultiBarFill(float value)
    {
        bar.fillAmount *= value;
    }

    private void SetTrendingRankPoint(int rank)
    {
        Vector2 newPos = firstPoint;

        trendingRankPoint.gameObject.SetActive(true);

        newPos.y -= rank * (200 / ViewerTabManager.MaxRank);

        trendingRankPoint.anchoredPosition = newPos;
    }

    public void SetEachRank(int rank)
    {
        myRankPoint = rank;

        SetTrendingRankPoint(rank);
    }

    public void OffPointObject()
    {
        trendingRankPoint.gameObject.SetActive(false);
    }

    public int GetRank()
    {
        return myRankPoint;
    }

    public Vector2 GetTrendingRankPointPos()
    {
        return trendingRankPoint.position;
    }
}
