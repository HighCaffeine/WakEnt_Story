using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class AstarTest : MonoBehaviour
{
    [SerializeField] private GameObject testTransformA;
    [SerializeField] private GameObject testTransformB;


    public Tilemap tilemap;
    public void TestCellBound()
    {

    }

    public void SetPath()
    {
        PathFinding.Instance.PathFind(testTransformA.transform.position, testTransformB.transform.position);
    }
}
