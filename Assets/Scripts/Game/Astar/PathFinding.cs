using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinding : GenericSingleton<PathFinding>
{

    
    ////////////////////////////테스트///////////////////////////////
    public Node[,] TEST_GetGrid()
    {
        return astar.TEST_GetGrid();
    }
    [SerializeField] private Astar astar;

    new void Awake()
    {
        base.Awake();

        astar = GetComponent<Astar>();
    } 

    public Stack<Vector2> PathFind(Vector2 productorPos, Vector2 targetPos)
    {
        Debug.Log("productor");
        Node productorNode = astar.GetNode(productorPos);
        Debug.Log("target");
        Node targetNode = astar.GetNode(targetPos);

        List<Node> openNode = new List<Node>();
        HashSet<Node> closeNode = new HashSet<Node>();

        openNode.Add(productorNode);

        while (openNode.Count != 0)
        {
            Node currentNode = openNode[0];

            for (int i = 1; i < openNode.Count; i++)
            {
                if (currentNode.FCost > openNode[i].FCost || (currentNode.FCost == openNode[i].FCost && currentNode.hCost > openNode[i].hCost))
                {
                    currentNode = openNode[i];
                }
            }

            openNode.Remove(currentNode);
            closeNode.Add(currentNode);

            if (currentNode == targetNode)
            {
                return GetPath(productorNode, targetNode);
            }

            List<Node> aroundNode = astar.GetAroundNode(currentNode);

            foreach (var node in aroundNode)
            {
                if (closeNode.Contains(node))
                {
                    continue;
                }

                int newGcost = currentNode.gCost + GetDistance(currentNode, node);

                if (openNode.Contains(node))
                {
                    if (node.gCost > newGcost)
                    {
                        node.gCost = newGcost;
                        node.parentNode = currentNode;
                    }
                }
                else
                {
                    node.gCost = newGcost;
                    node.hCost = GetDistance(currentNode, targetNode);
                    node.parentNode = currentNode;

                    openNode.Add(node);
                }
            }
        }

        return null;
    }

    List<Vector2> testCheckPath;

    private Stack<Vector2> GetPath(Node startNode, Node targetNode)
    {
        string startNodePos = string.Format("({0}, {1})", startNode.xPos, startNode.yPos);
        string targetNodePos = string.Format("({0}, {1})", targetNode.xPos, targetNode.yPos);

        Debug.Log(startNodePos + "/" + targetNodePos);

        testCheckPath = new List<Vector2>();

        Stack<Vector2> returnValue = new Stack<Vector2>();

        testCheckPath.Add(targetNode.Pos);

        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            returnValue.Push(currentNode.Pos);
            testCheckPath.Add(currentNode.Pos);

            currentNode = currentNode.parentNode;
        }

        return returnValue;
    }

    private static int SIDE => 100;
    private static int TOPSIDE => 50;
    private static int DIAGONALSIDE => 56;

    private int GetDistance(Node firstNode, Node secondNode)
    {
        int xSign = firstNode.xPos - secondNode.xPos;
        int ySign = firstNode.yPos - secondNode.yPos;

        int xDistance = Mathf.Abs(firstNode.xPos - secondNode.xPos);
        int yDistance = Mathf.Abs(firstNode.yPos - secondNode.yPos);

        if (xDistance < yDistance)
        {
           return xDistance * TOPSIDE + (yDistance - xDistance) * DIAGONALSIDE;
        }
        else if (xDistance > yDistance)
        {
           return yDistance * TOPSIDE + (xDistance - yDistance) * DIAGONALSIDE;
        }
        else
        {
            if (xSign * ySign > 0)
            {
                return yDistance * SIDE;
            }
            else
            {
                return yDistance * TOPSIDE;
            }
        }
    }

    //경로 체크
    void OnDrawGizmosSelected()
    {
        if (testCheckPath != null)
        {
            for (int i = 0; i < testCheckPath.Count - 1; i++)
            {
                Vector2 current = testCheckPath[i];
                Vector2 target = testCheckPath[i + 1];

                //Gizmos.color = Color.red;
                //Gizmos.DrawLine(current, target);

                var p1 = current;
                var p2 = target;
                var thickness = 3;
                UnityEditor.Handles.DrawBezier(p1, p2, p1, p2, Color.red, null, thickness);
            }
        }

        // if (testCheckPath != null)
        // {
        //     foreach (var pos in testCheckPath)
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawSphere(pos, 0.1f);
        //     }
        // }
    }
}