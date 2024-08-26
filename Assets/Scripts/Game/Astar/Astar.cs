using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Grid : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the isometric tilemap
    public Grid gridBase;
    public LayerMask walkableLayerMask; // For obstacle detection

    Node[,] grid;

    private int numTilesX; // Number of tiles in the X axis
    private int numTilesY; // Number of tiles in the Y axis

    void Start()
    {
        numTilesX = CalculateNumTilesX(tilemap.cellBounds);
        numTilesY = CalculateNumTilesY(tilemap.cellBounds);

        Debug.Log("Number of tiles on the x-axis = " + numTilesX.ToString() + ", on the y-axis = " + numTilesY.ToString());
        //Prints 40,38 which are the number of tiles in scene view.

        GenerateNodes();
    }

    int CalculateNumTilesX(BoundsInt bounds)
    {
        return bounds.size.x;
    }

    int CalculateNumTilesY(BoundsInt bounds)
    {
        return bounds.size.y;
    }

    void GenerateNodes()
    {
        grid = new Node[numTilesX, numTilesY];

        Vector3 tilemapOrigin = tilemap.origin; //Origin of the tilemap
       
        Vector3 actualOrigin = tilemap.transform.position; //Actual position of the tilemap

        Vector3 originOffset = actualOrigin - tilemapOrigin; //Calculation to offset the node to the correct tile

        for (int x = 0; x < numTilesX; x++)
        {
            for (int y = 0; y < numTilesY; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                cellPosition -= Vector3Int.FloorToInt(originOffset); //Places the nodes on the correct tile

                Vector3 cellCentreWorld = tilemap.GetCellCenterWorld(cellPosition);

                // Check if a tile exists in the current cell
                bool hasTile = tilemap.HasTile(cellPosition);

                if (!hasTile)
                {
                    grid[x, y] = null; // Skip this cell
                    continue;
                }

                bool walkable = !(Physics2D.OverlapCircle(cellCentreWorld, 0.01f, walkableLayerMask)); //Check to see if tile is able to walked on
                Node node = new Node(x, y, cellCentreWorld, walkable); //For the Node, gives information for if the tile is walkable
                grid[x, y] = node;


            }
        }
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