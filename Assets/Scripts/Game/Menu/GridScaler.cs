using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(GridLayoutGroup))]
public class GridScaler : MonoBehaviour
{
    [Header("배경 Transform")][SerializeField] private GameObject backObj;
    [Header("Grid Parent")][SerializeField] private GameObject itemsParent; 
    [Space(5f)]

    private float beforeWidth, beforeHeight;
    
    private GridLayoutGroup grid;

    [SerializeField] private int count;     //칼럼 갯수
    [SerializeField] private int minCol;    //한 row에 칼럼 수
    [SerializeField] private int maxRow;    //최대 row수

    [Range(0f, 1f)]
    [SerializeField] private float multiplier;

    [Tooltip("true : 배경 고정, false : 아이템 고정")]
    [SerializeField] private bool fixedItemSize;

    private const int MaxMenuItemCount = 6;

    private void Awake()
    {
        DynamicScaler();
    }

    public void DynamicScaler()
    {
        RectTransform rect = backObj.gameObject.GetComponent<RectTransform>();
        GridLayoutGroup grid = itemsParent.gameObject.GetComponent<GridLayoutGroup>();

        beforeWidth = rect.rect.width;
        beforeHeight = rect.rect.height;

        if (fixedItemSize)
        {
            RectMask2D rectMask2D = rect.gameObject.GetComponent<RectMask2D>();

            //Vector2 newSize = new Vector2(grid.cellSize.x, grid.cellSize.y);
            //newSize.y *= GetVisibleChild();

            //rect.sizeDelta = newSize;

            //메뉴 창 최대 grid 수 6개 - 메뉴 갯수 = 빈 공간
            int blankCount = MaxMenuItemCount - GetVisibleChild();


            float cellSizeY = grid.cellSize.y / minCol;
            float valueAmount = (cellSizeY + grid.spacing.y) * (blankCount / minCol); 

            Vector2 newVector = rectMask2D.padding;

            newVector.y = valueAmount;

            rectMask2D.padding = newVector;
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

    private int GetVisibleChild()
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
