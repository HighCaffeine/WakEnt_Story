using UnityEngine;

public class TrendingRankLineManager : ObjectPooling<TrendingRankLineManager, TrendingLine>
{
    private new void Awake()
    {
        base.Awake();
    }
    
    public void TestDrawLine(ViewerBar[] barGroup)
    {
        ViewerBar beforeBar = barGroup[0];

        Debug.Log("line");

        foreach (var bar in barGroup)
        {
            if (bar == beforeBar)
            {
                continue;
            }

            if (!bar.gameObject.activeSelf)
            {
                continue;
            }

            TrendingLine item = GetPool();

            ViewerBar currentBar = bar;

            DrawLine(item, beforeBar.GetTrendingRankPointPos(), currentBar.GetTrendingRankPointPos());

            beforeBar = currentBar;
        }
    }


    //line을 보관해두고 있다가 꺼내서 쓸건데.
    //주마다 그래프가 왼쪽으로 1칸씩 움직이게 되는데, 때마다 다시 그리기 / 옮긴만큼 선도 같이 옮김
    //
    public void DrawLine(TrendingLine lineClass, Vector2 pointA, Vector2 pointB)
    {
        RectTransform line = lineClass.GetComponent<RectTransform>();
        Vector2 dir = pointA - pointB;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        line.sizeDelta = new Vector2(dir.magnitude, 1f);
        line.pivot = new Vector2(0, 0.5f);
        line.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}