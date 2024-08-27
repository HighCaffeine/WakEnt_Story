using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : GenericSingleton<PathFinding>
{
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
        Debug.Log(startNode + "/" + targetNode);

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

    private int GetDistance(Node firstNode, Node secondNode)
    {
        int xDistance = Mathf.Abs(firstNode.xPos - secondNode.xPos);
        int yDistance = Mathf.Abs(firstNode.yPos - secondNode.yPos);

        //isometric 크기로 변경 필요
        if (xDistance < yDistance)
        {
           return xDistance * 14 + (yDistance - xDistance) * 10;
        }
        else
        {
           return yDistance * 14 + (xDistance - yDistance) * 10;
        }


        //return xDistance * 10 + yDistance * 10;
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

                Gizmos.color = Color.red;
                Gizmos.DrawLine(current, target);
            }
        }
    }
}