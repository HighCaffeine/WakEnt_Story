using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(GridLayoutGroup))]
public class GridScaler : MonoBehaviour
{
    private float beforeWidth, beforeHeight;
    private RectTransform rect;
    private GridLayoutGroup grid;

    [SerializeField] private int count;     //칼럼 갯수
    [SerializeField] private int minCol;    //한 row에 칼럼 수
    [SerializeField] private int maxRow;    //최대 row수

    [Range(0f, 1f)]
    [SerializeField] private float multiplier;

    private void Awake()
    {
        rect = gameObject.GetComponent<RectTransform>();
        grid = gameObject.GetComponent<GridLayoutGroup>();

        beforeWidth = rect.rect.width;
        beforeHeight = rect.rect.height;

        DynamicScaler();
    }

    private void DynamicScaler()
    {
        

        int row = Mathf.Clamp(Mathf.CeilToInt((float) count / minCol), 1, maxRow + 1);
        int col = Mathf.CeilToInt((float) count / row);

        float spaceWidth = (grid.padding.left + grid.padding.right) + (grid.spacing.x * (col - 1));
        float spaceHeight = (grid.padding.top + grid.padding.bottom) + (grid.spacing.y * (row - 1));

        float maxWidth = beforeWidth - spaceWidth;
        float maxHeight = beforeHeight - spaceHeight;

        float width = Mathf.Min(rect.rect.width - (grid.padding.left + grid.padding.right) - (grid.spacing.x * (col - 1)), maxWidth);
        float height = Mathf.Min(rect.rect.height - (grid.padding.top + grid.padding.bottom) - (grid.spacing.y * (row - 1)), maxHeight);

        Debug.Log(width);

        switch (grid.startAxis.ToString())
        {
            case "Vertical":
                grid.cellSize = new Vector2(width / col * multiplier, height / row);
            break;
            case "Horizontal":
                grid.cellSize = new Vector2(width / row, height / col * multiplier);
            break;
        }
    }
}
