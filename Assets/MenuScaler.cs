using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScaler : GridScaler
{
    //grid 내부 값으로 배경 길이 조정.
    public override void DynamicScaler()
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
    }
}
