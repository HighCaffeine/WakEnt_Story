using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Astar : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the isometric tilemap
    public Grid gridBase;
    public LayerMask walkableLayerMask; // For obstacle detection

    [SerializeField] Node[,] grid;

    [SerializeField]private int worldSizeX; // Number of tiles in the X axis
    [SerializeField]private int worldSizeY; // Number of tiles in the Y axis

    void Start()
    {
        tilemap.CompressBounds();                   //맵 cellbound 재설정

        worldSizeX = tilemap.cellBounds.size.x;
        worldSizeY = tilemap.cellBounds.size.y;

        GenerateNodes();
    }

    private void GenerateNodes()
    {
        grid = new Node[worldSizeX, worldSizeY];

        Vector3 tilemapOrigin = tilemap.origin; //Origin of the tilemap
        Vector3 actualOrigin = tilemap.transform.position; //Actual position of the tilemap
        Vector3 originOffset = actualOrigin - tilemapOrigin; //Calculation to offset the node to the correct tile

        testGridPos = new List<Vector2>();//grid size 체크

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                cellPosition -= Vector3Int.FloorToInt(originOffset); //Places the nodes on the correct tile

                Vector3 cellCenterWorldPos = tilemap.GetCellCenterWorld(cellPosition);

                // Check if a tile exists in the current cell
                bool hasTile = tilemap.HasTile(cellPosition);

                testGridPos.Add(cellCenterWorldPos);

                if (!hasTile)
                {
                    grid[x, y] = null; // Skip this cell
                    continue;
                }

                UpdateMaxMinPos(cellCenterWorldPos);

                bool walkable = !(Physics2D.OverlapCircle(cellCenterWorldPos, 0.01f, walkableLayerMask)); //Check to see if tile is able to walked on
                Node node = new Node(x, y, cellCenterWorldPos, walkable); //For the Node, gives information for if the tile is walkable
                grid[x, y] = node;
            }
        }
    }

    private Vector2 higher;
    private Vector2 lower;

    private List<Vector2> testGridPos;

    private void UpdateMaxMinPos(Vector2 pos)
    {
        if (higher.x < pos.x)
        {
            higher.x = pos.x;
        }

        if (higher.y < pos.y)
        {
            higher.y = pos.y;
        }

        if (lower.x > pos.x)
        {
            lower.x = pos.x;
        }

        if (lower.y > pos.y)
        {
            lower.y = pos.y;
        }
    }

    public Node GetNode(Vector2 pos)
    {
        int nodeXPos = 0;
        int nodeYPos = 0;

        //x or y 한 쪽(xPosition이 음수일 경우 y먼저) 탐색으로 node의 x값과 pos의 x값이 동일한 노드를 찾음
        //찾은 노드의 위쪽 노드들을 탐색
        //isometric은 위쪽 노드로 가기 위해서 x + 1, y + 1해줌
        //같은 방법으로 y값도 동일한 노드를 찾으면 해당 노드가 최종으로 찾아야 할 노드


        //설계 당시에는 [0, y]가 맞는데 지금은 테스트 용으로 다른 구역도 만들어서 해당 부분 생각해서 수정 필요
        if (pos.x < grid[0, 0].Pos.x)
        {  
            for (int y = 0; y < worldSizeY; y++)
            {
                //null이면 continue
                if (grid[0, y] == null)
                {
                    continue;
                }

                if (Mathf.Abs(grid[0, y].Pos.x - pos.x) <= 0.1f)
                {
                    nodeYPos = y;

                    break;
                }
            }

            for (int x = 0; x < worldSizeX; x++)
            {
                if (grid[x, nodeYPos] == null)
                {
                    continue;
                }

                if (Mathf.Abs(grid[x, nodeYPos].Pos.y - pos.y) <= 0.1f)
                {
                    nodeXPos = x;

                    break;
                }

                nodeYPos++;
            }
        }
        else
        {
            for (int x = 0; x < worldSizeY; x++)
            {
                if (grid[x, 0] == null)
                {
                    continue;
                }

                if (Mathf.Abs(grid[x, 0].Pos.x - pos.x) <= 0.1f)
                {
                    nodeXPos = x;

                    break;
                }
            }

            for (int y = 0; y < worldSizeX; y++)
            {
                if (grid[nodeXPos, y] == null)
                {
                    continue;
                }

                if (Mathf.Abs(grid[nodeXPos, y].Pos.y - pos.y) <= 0.1f)
                {
                    nodeYPos = y;

                    break;
                }

                nodeXPos++;
            }
        }

        return grid[nodeXPos, nodeYPos];
    }

    public List<Node> GetAroundNode(Node middleNode)
    {
        List<Node> aroundNodeList = new List<Node>();
        List<int> xNotWalkable = new List<int>();
        List<int> yNotWalkable = new List<int>();

        //iswalkable 노드 위치 미리 캐싱
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0)
                    || (x == -1 && y == -1)
                    || (x == -1 && y == 1)
                    || (x == 1 && y == -1)
                    || (x == 1 && y == 1))
                {
                    continue;
                }

                int aroundNodeX = middleNode.xPos + x;
                int aroundNodeY = middleNode.yPos + y;

                if (aroundNodeX >= 0 && aroundNodeX < worldSizeX && aroundNodeY >= 0 && aroundNodeY < worldSizeY)
                {
                    Node aroundNode = grid[aroundNodeX, aroundNodeY];

                    if (!aroundNode.IsWalkalbe)
                    {
                        xNotWalkable.Add(aroundNode.xPos);
                        yNotWalkable.Add(aroundNode.yPos);
                    }
                }
            }
        }

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int aroundNodeX = middleNode.xPos + x;
                int aroundNodeY = middleNode.yPos + y;

                if ((x == -1 && y == -1)
                    || (x == -1 && y == 1)
                    || (x == 1 && y == -1)
                    || (x == 1 && y == 1))
                {
                    if (xNotWalkable.Contains(aroundNodeX) || yNotWalkable.Contains(aroundNodeY))
                    {
                        continue;
                    }
                }

                if (aroundNodeX >= 0 && aroundNodeX < worldSizeX && aroundNodeY >= 0 && aroundNodeY < worldSizeY)
                {
                    if (grid[aroundNodeX, aroundNodeY] == null)
                    {
                        continue;
                    }

                    if (grid[aroundNodeX, aroundNodeY].IsWalkalbe)
                    {
                        aroundNodeList.Add(grid[aroundNodeX, aroundNodeY]);
                    }
                }
            }
        }

        return aroundNodeList;
    }
}