using System;
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
        testCheckError = new List<Vector2>();
    } 

    public Queue<Vector2> CurvedPathFind(Vector2 productorPos, Vector2 targetPos)
    {
        Stack<Vector2> vec = PathFind(productorPos, targetPos);

        return GetBezierCurve(vec);
    }

    private Stack<Vector2> PathFind(Vector2 productorPos, Vector2 targetPos)
    {
        Node productorNode = astar.GetNode(productorPos);
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
            testCheckError.Add(currentNode.Pos);

            //interactive노드일 경우의 체크.
            if (currentNode == targetNode)
            {
                return GetPath(productorNode, targetNode);
            }

            List<Node> aroundNode = astar.GetAroundNode(currentNode, targetNode);

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


    private Stack<Vector2> GetPath(Node startNode, Node targetNode)
    {
        if (testCheckBezier == null)
            testCheckPath = new List<Vector2>();

        Stack<Vector2> s = new Stack<Vector2>();

        //Node currentNode = targetNode;
        Node currentNode = targetNode.parentNode;       //타겟 제외

        bool isSameY = false;
        bool isSameX = false;

        testCheckPath.Add(currentNode.Pos);
        s.Push(currentNode.Pos);

        while (currentNode != startNode)
        {
            //이전 노드까지 넣어서 맵 뚫는 구간 아예 없도록 했는데
            //childnode체크 버그 있음.
            if (currentNode != targetNode)
            {
                currentNode.parentNode.childNode = currentNode;
            }

            if (currentNode.parentNode == startNode)
            {
                if (!s.Contains(currentNode.Pos))
                {
                    testCheckPath.Add(currentNode.Pos);
                    s.Push(currentNode.Pos);
                }
   
                break;
            }

            if (currentNode.xPos == currentNode.parentNode.xPos)
            {
                if (isSameY)
                {
                    isSameY = false;

                    if (currentNode.childNode != null && !s.Contains(currentNode.childNode.Pos))
                    {
                        testCheckPath.Add(currentNode.childNode.Pos);
                        s.Push(currentNode.childNode.Pos);
                    }

                    if (!s.Contains(currentNode.Pos))
                    {
                        testCheckPath.Add(currentNode.Pos);
                        s.Push(currentNode.Pos);
                    }

                    if (!s.Contains(currentNode.parentNode.Pos))
                    {
                        testCheckPath.Add(currentNode.parentNode.Pos);
                        s.Push(currentNode.parentNode.Pos);
                    }
                }

                isSameX = true;
                
            }
            else if (currentNode.yPos == currentNode.parentNode.yPos)
            {
                if (isSameX)
                {
                    isSameX = false;

                    if (currentNode.childNode != null &&!s.Contains(currentNode.childNode.Pos))
                    {
                        testCheckPath.Add(currentNode.childNode.Pos);
                        s.Push(currentNode.childNode.Pos);
                    }

                    if (!s.Contains(currentNode.Pos))
                    {
                        testCheckPath.Add(currentNode.Pos);
                        s.Push(currentNode.Pos);
                    }

                    if (!s.Contains(currentNode.parentNode.Pos))
                    {
                        testCheckPath.Add(currentNode.parentNode.Pos);
                        s.Push(currentNode.parentNode.Pos);
                    }
                }

                isSameY = true;
            }
            else if ((currentNode.xPos != currentNode.parentNode.xPos)
                    &&(currentNode.yPos != currentNode.parentNode.yPos))
            {
                if (!isSameX || !isSameY)
                {
                    if (currentNode.childNode != null &&!s.Contains(currentNode.childNode.Pos))
                    {
                        testCheckPath.Add(currentNode.childNode.Pos);
                        s.Push(currentNode.childNode.Pos);
                    }

                    if (!s.Contains(currentNode.Pos))
                    {
                        testCheckPath.Add(currentNode.Pos);
                        s.Push(currentNode.Pos);
                    }

                    if (!s.Contains(currentNode.parentNode.Pos))
                    {
                        testCheckPath.Add(currentNode.parentNode.Pos);
                        s.Push(currentNode.parentNode.Pos);
                    }
                }

                isSameX = true;
                isSameY = true;

                currentNode = currentNode.parentNode;

                continue;
            }
            else
            {
                if (!s.Contains(currentNode.Pos))
                {
                    testCheckPath.Add(currentNode.Pos);
                    s.Push(currentNode.Pos);
                }
            }
            
            currentNode = currentNode.parentNode;
        }

        return s;
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

    //카스텔조 알고리즘

    [Space(5f)] [Header("베지어 곡선 곡률")] [Range(0.0f, 2.0f)] [SerializeField] private float bezierCurvature;
    private Queue<Vector2> GetBezierCurve(Stack<Vector2> path)
    {
        Queue<Vector2> calPos = new Queue<Vector2>();
        Queue<Vector2> calStorage = new Queue<Vector2>();
        
        if (testCheckBezier == null)
        {
            testCheckBezier = new List<Vector2>();
            testCheckBezierPoint = new List<Vector2>();
        }

        //Path는 노드 상 대각선 이동 허용한 루트로 받아올거임.
        //각 지점들을 pop해서 vecterQueue에 계속 enqueue 하다가 3개 되면 곡선 계산해서 VectorStack에 push 

        //베지어 곡선 공식
        //t = [0, 1]
        //3점 P = (1−t)^2 * P1 + 2(1−t)t * P2 + t^2 * P3
        //x = (1−t)^2 * x1 + 2(1−t)t * x2 + t^2 * x3    
        //y = (1−t)^2 * y1 + 2(1−t)t * y2 + t^2 * y3
        //lerp로 v1 -> v2, v2 -> v3 계산

        int remain = (3 - path.Count % 3) % 3;

        Vector2 remainPos = path.Pop();

        //밑에 계산식 안 꼬이게 3개 단위로 나눌려고 임의로 넣어서 맞춤
        for (int i = 0; i < remain; i++)
        {
            path.Push(remainPos);
        }

        path.Push(remainPos);

        calPos.Enqueue(remainPos);
        testCheckBezier.Add(remainPos);

        Vector2 newV3Pos = Vector2.zero;

        while (path.Count > 0)
        {
            Vector2 pos = path.Pop();
            float t = 0.0f;

            calStorage.Enqueue(pos);

            if (calStorage.Count >= 3)
            {
                Vector2 v1 = calStorage.Dequeue();
                Vector2 v2 = calStorage.Dequeue();          //중간이기 때문에 해당 pos조절로 곡률 변경 가능.
                Vector2 v3 = calStorage.Dequeue();

                if (path.Count > 0)
                {
                    Node v1Node = astar.GetNode(v1);
                    Node v3Node = astar.GetNode(v3);

                    while (((v1Node.xPos == v3Node.xPos) || (v1Node.yPos == v3Node.yPos)))
                    {
                        Vector3 newV3 = path.Pop();
                        v1 = v2;
                        v2 = v3;
                        v3 = newV3;

                        v1Node = astar.GetNode(v1);
                        v3Node = astar.GetNode(v3);

                        newV3Pos = newV3;
                    }
                }

                v2 = CalculateCurvatue(v1, v2, v3);         //베지어 곡선 곡률 계산
                
                testCheckBezierPoint.Add(v1);
                testCheckBezierPoint.Add(v2);
                testCheckBezierPoint.Add(v3);

                while (t <= 1)
                {
                    t += 0.1f;

                    Vector2 v4 = Vector2.Lerp(v1, v2, t);
                    Vector2 v5 = Vector2.Lerp(v2, v3, t);
                    Vector2 targetPos = Vector2.Lerp(v4, v5, t);

                    if (!calPos.Contains(targetPos))
                    {
                        calPos.Enqueue(targetPos);
                        testCheckBezier.Add(targetPos);
                    }
                }

                if (path.Count > 0)
                {
                    Vector2 nextNodePos = path.Pop();

                    Node nextNode = astar.GetNode(nextNodePos);
                    Node currentNode = astar.GetNode(v3);

                    int dis = GetDistance(nextNode, currentNode);
                    
                    if (dis <= 100 && dis != 0)
                    {
                        calStorage.Enqueue(v3);
                    }

                    path.Push(nextNodePos);
                }
            }
        }

        if (newV3Pos != Vector2.zero)
        {
            calStorage.Enqueue(newV3Pos);
        }

        while (calStorage.Count > 0)
        {
            Vector2 pos = calStorage.Dequeue();

            if (!calPos.Contains(pos))
            {
                calPos.Enqueue(pos);
                testCheckBezier.Add(pos);
                testCheckBezierPoint.Add(pos);
            }
        }

        return calPos;
    }

    private Vector2 CalculateCurvatue(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        //v2에서 계산해줄거
        float x = 0.5f * (1 - bezierCurvature);
        float y = 0.25f * (1 - bezierCurvature);

        //v2가 어느곳에 위치하는지 체크

        Vector2 newPos = v2;

        if (v1.y == v3.y)
        {
            if (v1.y > v2.y)        //아래
            {
                newPos.y += y;
            }
            else if (v1.y < v2.y)   //위
            {
                newPos.y -= y;
            }
        }
        else if (v1.x == v3.x)
        {
            if (v1.x > v2.x)        //왼쪽
            {
                newPos.x += x;
            }
            else if (v1.x < v2.x)   //오른쪽
            {
                newPos.x -= x;
            }
        }

        return newPos;
    }

    public Vector2 GetNodePos(Vector2 targetPos)
    {
        Node node = astar.GetNode(targetPos);

        if (node == null)
        {
            return Vector2.zero;
        }

        return node.Pos;
    }

    List<Vector2> testCheckBezier;
    List<Vector2> testCheckPath;

    List<Vector2> testCheckBezierPoint;

    List<Vector2> testCheckError;

    public void TestResetList()
    {
        testCheckBezier.Clear();
        testCheckBezier.Clear();
        testCheckBezierPoint.Clear();
        testCheckError.Clear();
    }   

    private Vector2[] checkPos = {new Vector2(-33f, -5.75f), new Vector2(-32.5f, -6f)};

    //경로 체크
    void OnDrawGizmos()
    {
        if (testCheckBezier != null)
        {
            for (int i = 0; i < testCheckBezier.Count - 1; i++)
            {
                Vector2 current = testCheckBezier[i];
                Vector2 target = testCheckBezier[i + 1];

                if (target == checkPos[0] || target == checkPos[1])
                {
                    continue;
                }

                //Gizmos.color = Color.red;
                //Gizmos.DrawLine(current, target);

                var p1 = current;
                var p2 = target;
                var thickness = 10;
                UnityEditor.Handles.DrawBezier(p1, p2, p1, p2, Color.blue, null, thickness);
            }
        }

        if (testCheckPath != null)
        {
            for (int i = 0; i < testCheckPath.Count - 1; i++)
            {
                Vector2 current = testCheckPath[i];
                Vector2 target = testCheckPath[i + 1];


                if (current == checkPos[0] || current == checkPos[1])
                {
                    continue;
                }

                var p1 = current;
                var p2 = target;
                var thickness = 5;
                UnityEditor.Handles.DrawBezier(p1, p2, p1, p2, Color.red, null, thickness);
            }
        }

        if (testCheckBezierPoint != null)
        {
            foreach (var point in testCheckBezierPoint)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(point, 0.05f);
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


        // if (testCheckError != null)
        // {
        //     foreach (var node in testCheckError)
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawSphere(node, 0.05f);
        //     }
        // }
    }
}