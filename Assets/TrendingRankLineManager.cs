using System.Collections.Generic;
using UnityEngine;

public class TrendingRankLineManager : ObjectPooling<TrendingRankLineManager, TrendingLine>
{
    private new void Awake()
    {
        base.Awake();
    }

    public List<TrendingLine> testLineList = new List<TrendingLine>();

    public System.Collections.IEnumerator TestDrawCoroutine(ViewerBar[] barGroup)
    {
        foreach (var line in testLineList)
        {
            line.OnReturnPool();
        }

        testLineList.Clear();

        yield return new WaitForSeconds(Time.deltaTime);

        TestDrawLine(barGroup);
    }
    
    public void TestDrawLine(ViewerBar[] barGroup)
    {
        ViewerBar beforeBar = barGroup[0];

        foreach (var bar in barGroup)
        {
            if (!(bar.gameObject.activeSelf && beforeBar.gameObject.activeSelf))
            {
                beforeBar = bar;

                continue;
            }

            if (bar == barGroup[barGroup.Length - 1])
            {
                break;
            }

            TrendingLine line = GetPool();
            testLineList.Add(line);

            DrawLine(line, beforeBar.GetTrendingRankPointPos(), bar.GetTrendingRankPointPos());

            beforeBar = bar;
        }
    }

    public void DrawLine(TrendingLine lineClass, Vector2 pointA, Vector2 pointB)
    {
        RectTransform line = lineClass.GetComponent<RectTransform>();
        line.transform.localScale = Vector3.one;

        Vector2 dir = pointB- pointA;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        line.sizeDelta = new Vector2(dir.magnitude, 2f);
        line.pivot = new Vector2(0, 0.5f);
        line.position = pointA;
        line.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}