using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    private float nodeDiameter; 

    [SerializeField] private float nodeRadius;
    [SerializeField] private Vector2 worldSize;

    private int xSize;
    private int ySize;

    private Node[,] worldNode;

    void Awake()
    {
        CreateWorldNode();
    }

    private void CreateWorldNode()
    {
        nodeDiameter = nodeRadius * 2.0f;

        xSize = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        ySize = Mathf.RoundToInt(worldSize.y / nodeDiameter);

        worldNode = new Node[xSize, ySize];

        Vector3 pivotPos = transform.position - new Vector3(xSize * 0.5f, 0f, 0f);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {

                Vector3 nodePos = pivotPos + new Vector3(nodeDiameter * x, 0.1f, nodeDiameter * y);
                bool isWalkable = false;
                //저장된 건물 정보를 얻어서 넣어주는 부분 추가.

                
            }
        }
    }
}
