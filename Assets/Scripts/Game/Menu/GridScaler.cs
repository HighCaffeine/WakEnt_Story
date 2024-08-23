using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(GridLayoutGroup))]
public class GridScaler : MonoBehaviour
{
    [Header("배경 Transform")][SerializeField] protected GameObject backObj;
    [Header("Grid Parent")][SerializeField] protected GameObject itemsParent; 
    [Space(5f)]

    protected float beforeWidth, beforeHeight;
    
    protected GridLayoutGroup grid;

    [SerializeField] protected int count;     //칼럼 갯수
    [SerializeField] protected int minCol;    //한 row에 칼럼 수
    [SerializeField] protected int maxRow;    //최대 row수

    [Range(0f, 1f)]
    [SerializeField] protected float multiplier;

    [Tooltip("true : 배경 고정, false : 아이템 고정")]
    [SerializeField] protected bool fixedItemSize;

    protected const int MaxMenuItemCount = 6;

    private void Awake()
    {
        DynamicScaler();
    }

    public virtual void DynamicScaler()
    {
        RectTransform rect = backObj.gameObject.GetComponent<RectTransform>();
        GridLayoutGroup grid = itemsParent.gameObject.GetComponent<GridLayoutGroup>();

        beforeWidth = rect.rect.width;
        beforeHeight = rect.rect.height;

        if (fixedItemSize)
        {
            float cellSizeY = grid.cellSize.y * GetVisibleChild();

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, cellSizeY);
        }
        else
        {
            int row = Mathf.Clamp(Mathf.CeilToInt((float) count / minCol), 1, maxRow + 1);
            int col = Mathf.CeilToInt((float) count / row);

            float spaceWidth = (grid.padding.left + grid.padding.right) + (grid.spacing.x * (col - 1));
            float spaceHeight = (grid.padding.top + grid.padding.bottom) + (grid.spacing.y * (row - 1));

            float maxWidth = beforeWidth - spaceWidth;
            float maxHeight = beforeHeight - spaceHeight;

            float width = Mathf.Min(rect.rect.width - (grid.padding.left + grid.padding.right) - (grid.spacing.x * (col - 1)), maxWidth);
            float height = Mathf.Min(rect.rect.height - (grid.padding.top + grid.padding.bottom) - (grid.spacing.y * (row - 1)), maxHeight);

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

    protected int GetVisibleChild()
    {
        int value = 1;

        for (int i = 1; i < itemsParent.transform.childCount; i++)
        {
            if (itemsParent.transform.GetChild(i).gameObject.activeSelf)
            {
                value++;
            }
        }

        return value;
    }
}
