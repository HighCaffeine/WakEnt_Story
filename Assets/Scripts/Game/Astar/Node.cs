using System;
using UnityEngine;

[SerializeField]
[Serializable]
public class Node
{
    private Vector3 nodePos;
    public int xPos { get; private set;}
    public int yPos { get; private set;}

    public int gCost { get; set;} // 시작 노드부터 현재 노드까지 거리
    public int hCost { get; set;} // 현재 노드부터 목표 노드까지의 거리
    public int FCost { get=> gCost + hCost; } // 현재 노드의 총 거리 값

    public bool IsWalkalbe { get => isWalkable; private set {} }
    public Vector3 Pos { get => nodePos; private set{} }

    public Node parentNode;

    private bool isWalkable;

    public Node(int _xPos, int _yPos, Vector3 _nodePos, bool _isWalkable)
    {
        xPos = _xPos;
        yPos = _yPos;
        nodePos = _nodePos;
        isWalkable = _isWalkable;
    }
}