using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Devcat;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Analytics;

public class LayerCalculator : MonoBehaviour
{
    [Header("Layer검사할 layer와 tag선택")]
    [SerializeField] private LayerMask checkLayer;
    [TagField][SerializeField] private string checkTag;
    private enum ComparePosValue { Left, Bottom, Equal, Right, Top }
    private enum CheckDirection { NONE = -1, Right, Left, Top, Bottom }
    private enum CheckRightSideSequence { Upper, RightUp, RightBottom, Bottom, Count }
    private enum CheckLeftSideSequence { Upper, LeftUp, LeftBottom, Bottom, Count }
    private enum CheckTopSideSequence { LeftUp, Upper, RightUpper, Count }
    private enum CheckBottomSideSequence { LeftBottom, Bottom, RightBottom, Count }

    private int[] checkRightSideXSequence = { 1, 1, 0, -1 };
    private int[] checkRightSideYSequence = { 1, 0, -1, -1 };

    private int[] checkLeftSideXSequence = { 1, 0, -1, -1 };
    private int[] checkLeftSideYSequence = { 1, 1, 0, -1 };

    private int[] checkTopSideXSequence = { 0, 1, 1 };
    private int[] checkTopSideYSequence = { 1, 1, 0 };

    private int[] checkBottomSideXSequence = { -1, -1, 0 };
    private int[] checkBottomSideYSequence = { 0, -1, -1 };

    private Vector2 beforePos;
    private int beforeNodeX;
    private int beforeNodeY;
    private bool side = true;

    private float beforeDistance;

    private void UpdateBeforeNodeValue(ref Stack<Vector2> noneCurvedPath)
    {
        if (noneCurvedPath.Count > 0)
            beforePos = noneCurvedPath.Pop();

        Node newNode = PathFinding.Instance.GetNode(beforePos);

        beforeNodeX = newNode.xPos;
        beforeNodeY = newNode.yPos;
    }
    public int GetOrderInLayer(Vector2 characterPos, Vector2 targetPos, bool isFirst, ref Stack<Vector2> noneCurvedPath)
    {
        //경로 받은 후 첫 이동 시 값 세팅
        if (isFirst)
        {
            beforeDistance = float.MaxValue;
            UpdateBeforeNodeValue(ref noneCurvedPath);
        }

        if (IsAllowedCheckLayer(characterPos))
        {
            UpdateBeforeNodeValue(ref noneCurvedPath);
        }
        else
        {
            return int.MaxValue;
        }

        return LayerCalculate(PathFinding.Instance.GetNode(characterPos),
                                PathFinding.Instance.GetNode(beforePos),
                                false,
                                ref noneCurvedPath);
    }

    private bool checkReachCenter = false;

    private bool IsAllowedCheckLayer(Vector2 characterPos)
    {
        //사이드 이동 시시
        // if (checkReachCenter)
        // {
        //     float distance = Vector2.Distance(characterPos, beforePos);

        //     if (distance > beforeDistance)
        //     {
        //         side = !side;
        //         checkReachCenter = false;
        //         beforeDistance = float.MaxValue;

        //         return true;
        //     }

        //     beforeDistance = distance;
        // }
        // else
        // {
        // }

        Node characterNode = PathFinding.Instance.GetNode(characterPos);

        if ((characterNode.xPos == beforeNodeX) && (characterNode.yPos == beforeNodeY))
        {
            return true;
        }
        return false;
    }

    public int GetOrderInLayerInteractive(Vector2 characterPos, Vector2 targetPos)
    {
        Stack<Vector2> temp = new Stack<Vector2>();

        return LayerCalculate(PathFinding.Instance.GetNode(characterPos),
                                PathFinding.Instance.GetNode(targetPos),
                                true,
                                ref temp);
    }

    private CheckDirection GetCheckDirection(ComparePosValue xComp, ComparePosValue yComp, out int indexSize)
    {
        CheckDirection retval = CheckDirection.NONE; 
        indexSize = -1;

        if (xComp == ComparePosValue.Left && yComp == ComparePosValue.Bottom)
        {
            retval = CheckDirection.Bottom;
        }
        else if (xComp == ComparePosValue.Right && yComp == ComparePosValue.Top)
        {
            retval = CheckDirection.Top;
        }
        else if ((xComp == ComparePosValue.Left && yComp == ComparePosValue.Top)
                || (xComp == ComparePosValue.Equal && yComp == ComparePosValue.Top)
                || (xComp == ComparePosValue.Left && yComp == ComparePosValue.Equal))
        {
            retval = CheckDirection.Left;
        }
        else if ((xComp == ComparePosValue.Right && yComp == ComparePosValue.Bottom)
                || (xComp == ComparePosValue.Equal && yComp == ComparePosValue.Bottom)
                || (xComp == ComparePosValue.Right && yComp == ComparePosValue.Equal))
        {
            retval = CheckDirection.Right;
        }

        if (retval == CheckDirection.NONE)
        {
            return CheckDirection.NONE;
        }

        indexSize = ((retval == CheckDirection.Right) || (retval == CheckDirection.Left))
                        ? ValueCastTo<int>.From(CheckRightSideSequence.Count)
                        : ValueCastTo<int>.From(CheckTopSideSequence.Count);

        return retval;
    }

    private void SetPosArray(CheckDirection checkDirection, ref int[] array, bool xPos)
    {
        switch (checkDirection)
        {
            case CheckDirection.Right:
                array = xPos ? checkRightSideXSequence : checkRightSideYSequence;
                break;
            case CheckDirection.Left:
                array = xPos ? checkLeftSideXSequence : checkLeftSideYSequence;
                break;
            case CheckDirection.Top:
                array = xPos ? checkTopSideXSequence : checkTopSideYSequence;
                break;
            case CheckDirection.Bottom:
                array = xPos ? checkBottomSideXSequence : checkBottomSideYSequence;
                break;
        }
    }

    int beforeLayerValue = 0;

    private int LayerCalculate(Node characterNode, Node centerNode, bool isInteractive, ref Stack<Vector2> noneCurvedPath)
    {
        int calLayer = int.MaxValue;
        GameObject[] targetObjs;
        Vector2 checkNodePos = Vector2.zero;
        int layer = int.MinValue;
        test_CheckCollision.Clear();

        //Debug.Log(string.Format("{0} : ({1}, {2}) -> ({3}, {4})", transform.name, characterNode.xPos, characterNode.yPos, centerNode.xPos, centerNode.yPos));

        if (isInteractive)
        {
            checkNodePos = centerNode.Pos;

            if (CheckTileCollision(checkNodePos, out targetObjs))
            {
                foreach (var targetObj in targetObjs)
                {
                    int newLayer = GetLayer(targetObj, isInteractive);

                    if (newLayer != int.MinValue)
                    {
                        layer = centerNode.Pos.y > checkNodePos.y ? newLayer - 1 : newLayer + 1;
                        if (layer < calLayer) calLayer = layer;
                    }
                }
            }
        }
        else
        {
            bool isSide = (characterNode.xPos == centerNode.xPos || characterNode.yPos == centerNode.yPos) ? false : true;
            ComparePosValue xComp = (characterNode.xPos > centerNode.xPos) ? ComparePosValue.Left 
                                    : (characterNode.xPos < centerNode.xPos) ? ComparePosValue.Right : ComparePosValue.Equal;
            ComparePosValue yComp = (characterNode.yPos > centerNode.yPos) ? ComparePosValue.Bottom 
                                    : (characterNode.yPos < centerNode.yPos) ? ComparePosValue.Top : ComparePosValue.Equal;

            // if (noneCurvedPath.Count > 0)
            // {
            //     //nextPosNode = PathFinding.Instance.GetNode(noneCurvedPath.Peek());
            //     checkReachCenter = (centerNode.xPos == characterNode.xPos || centerNode.yPos == characterNode.yPos) ? false : true;
            //     //isSide = (centerNode.xPos == nextPosNode.xPos || centerNode.yPos == nextPosNode.yPos) ? false : true;
            // }

            //if (isSide) checkReachCenter = true;

            int indexSize = 0;

            CheckDirection checkDirection = GetCheckDirection(xComp, yComp, out indexSize);

            if (checkDirection == CheckDirection.NONE) return beforeLayerValue;

            int[] xPosArray = new int[indexSize];
            int[] yPosArray = new int[indexSize];

            SetPosArray(checkDirection, ref xPosArray, true);
            SetPosArray(checkDirection, ref yPosArray, false);

            for (int i = 0; i < indexSize; i++)
            {
                int checkXPos = centerNode.xPos + xPosArray[i];
                int checkYPos = centerNode.yPos + yPosArray[i];

                checkNodePos = PathFinding.Instance.GetNodePos(checkXPos, checkYPos);

                test_CheckCollision.Add(checkNodePos);

                if (CheckTileCollision(checkNodePos, out targetObjs))
                {
                    foreach (var targetObj in targetObjs)
                    {
                        int newLayer = GetLayer(targetObj, isInteractive);

                        if (newLayer != int.MinValue)
                        {
                            layer = centerNode.Pos.y > checkNodePos.y ? newLayer - 1 : newLayer + 1;
                            if (layer < calLayer) calLayer = layer;
                        }
                    }
                }
            }
        }

        beforeLayerValue = calLayer;

        return calLayer;

        // for (int i = 0; i < ValueCastTo<int>.From(CheckSequence.Count); i++)
        // {
        //     if (i == ValueCastTo<int>.From(CheckSequence.Center)) { continue; }

        //     int checkXPos = centerNode.xPos + checkXSequence[i];
        //     int checkYPos = centerNode.yPos + checkYSequence[i];

        //     checkNodePos = PathFinding.Instance.GetNodePos(checkXPos, checkYPos);
        //     int layer = int.MinValue;

        //     test_CheckCollision.Add(checkNodePos);

        //     if (CheckTileCollision(checkNodePos, out targetObj))
        //     {
        //         layer = GetLayer(targetObj, isInteractive);

        //         if (layer != int.MinValue) calLayer = layer;
        //     }
        // }
        // return calLayer;
    }

    // private bool IsAllowedCheckLayer(Vector2 centerPos)
    // {
    //     float x = centerPos.x;
    //     float y = centerPos.y;
    //     bool allow = true;

    //     x *= 100.0f;
    //     y *= 100.0f;

    //     Debug.Log(string.Format("postest : ({0}, {1})", x, y));
    //     allow = x % 25 == 0 ? true : false;
    //     allow = y % 25 == 0 ? true : false;

    //     return allow;
    // }

    private bool CheckTileCollision(Vector2 pos, out GameObject[] objs)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.1f);

        objs = new GameObject[hits.Length];

        for (int i = 0; i < hits.Length; i++)
        {
            objs[i] = hits[i].gameObject;
        }

        return objs.Length > 0;
    }

    private int GetLayer(GameObject target, bool isInteractive)
    {
        int layer = int.MinValue;

        if (target)
        {
            LayerData layerData = target.GetComponent<LayerData>();

            if (layerData)
            {
                layer = isInteractive ? layerData.InteractiveLayer : layerData.MoveLayer;
            }
        }

        return layer;
    }

    private List<Vector2> test_CheckCollision = new List<Vector2>();

    public Color gizmosColor;
    private Color ineColor = new Color(138f / 255f, 43f / 255f, 226f / 255f);
    private Color jingBurgerColor = new Color(240f / 255f, 169f / 255f, 87f / 255f);
    private Color lilpaColor = new Color(68f / 255f, 57f / 255f, 101f / 255f);
    private Color jururuColor = new Color(255f / 255f, 0f / 255f, 140f / 255f);
    private Color goseguColor = new Color(70f / 255f, 126f / 255f, 198f / 255f);
    private Color viichanColor = new Color(149f / 255f, 193f / 255f, 0f / 255f);

    void Start()
    {
        string name = gameObject.name;
        gizmosColor = name == "아이네" ? ineColor
                        : name == "징버거" ? jingBurgerColor
                        : name == "릴파" ? lilpaColor
                        : name == "주르르" ? jururuColor
                        : name == "고세구" ? goseguColor
                        : viichanColor;
    }

    void OnDrawGizmos()
    {
        if (test_CheckCollision.Count > 0)
        {
            foreach (var pos in test_CheckCollision)
            {
                Gizmos.color = gizmosColor;
                Gizmos.DrawWireSphere(pos, 0.1f);
            }
        }
    }
}