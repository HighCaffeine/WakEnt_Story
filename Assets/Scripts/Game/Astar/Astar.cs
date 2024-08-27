using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Astar : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the isometric tilemap
    public Grid gridBase;
    public LayerMask walkableLayerMask; // For obstacle detection

    Node[,] grid;

    [SerializeField]private int worldSizeX; // Number of tiles in the X axis
    [SerializeField]private int worldSizeY; // Number of tiles in the Y axis

    void Start()
    {
        worldSizeX = tilemap.cellBounds.size.x;
        worldSizeY = tilemap.cellBounds.size.y;

        Debug.Log("Number of tiles on the x-axis = " + worldSizeX.ToString() + ", on the y-axis = " + worldSizeY.ToString());
        //Prints 40,38 which are the number of tiles in scene view.

        GenerateNodes();
    }

    void GenerateNodes()
    {
        grid = new Node[worldSizeX, worldSizeY];

        Vector3 tilemapOrigin = tilemap.origin; //Origin of the tilemap
       
        Vector3 actualOrigin = tilemap.transform.position; //Actual position of the tilemap

        Vector3 originOffset = actualOrigin - tilemapOrigin; //Calculation to offset the node to the correct tile

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                cellPosition -= Vector3Int.FloorToInt(originOffset); //Places the nodes on the correct tile

                Vector3 cellCenterWorldPos = tilemap.GetCellCenterWorld(cellPosition);

                // Check if a tile exists in the current cell
                bool hasTile = tilemap.HasTile(cellPosition);

                if (!hasTile)
                {
                    grid[x, y] = null; // Skip this cell
                    continue;
                }

                bool walkable = !(Physics2D.OverlapCircle(cellCenterWorldPos, 0.01f, walkableLayerMask)); //Check to see if tile is able to walked on
                Node node = new Node(x, y, cellCenterWorldPos, walkable); //For the Node, gives information for if the tile is walkable
                grid[x, y] = node;

                Debug.Log("(x, y) : " + cellCenterWorldPos);
            }
        }
    }

    public Node GetNode(Vector2 pos)
    {
        float xPercent;
        float yPercent;

        int nodeXPos = 0;
        int nodeYPos = 0;

        xPercent = (pos.x + worldSizeX * 0.5f) / worldSizeX;
        yPercent = pos.y / worldSizeY;

        Debug.Log(pos);

        Debug.Log("xPercent : " + xPercent);
        Debug.Log("ypercent : " + yPercent);

        xPercent = Mathf.Clamp01(xPercent);
        yPercent = Mathf.Clamp01(yPercent);

        nodeXPos = Mathf.RoundToInt((worldSizeX - 1) * xPercent);
        nodeYPos = Mathf.RoundToInt((worldSizeY - 1) * yPercent);

        Debug.Log("Max : " + worldSizeX + "/ nodePos : " + nodeXPos);
        Debug.Log("Max : " + worldSizeY + "/ nodePos : " + nodeYPos);

        return grid[nodeXPos, nodeYPos];
    }

    public List<Node> GetAroundNode(Node middleNode)
    {
        List<Node> aroundNodeList = new List<Node>();

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
                    if (grid[aroundNodeX, aroundNodeY].IsWalkalbe)
                    {
                        aroundNodeList.Add(grid[aroundNodeX, aroundNodeY]);
                    }
                }
            }
        }

        return aroundNodeList;
    }

    // Draw Gizmos in the Scene view
    void OnDrawGizmos()
    {
        if (grid != null)
        {
            // Visualize nodes using Gizmos
            foreach (Node node in grid)
            {
                // Check for null node
                if (node == null)
                    continue;

                Gizmos.color = node.IsWalkalbe ? Color.green : Color.red;
                Gizmos.DrawWireSphere(node.Pos, 0.05f);
            }
        }
    }

}