using System;
using UnityEngine;

[SerializeField]
public class Node
{
    private Vector3 nodePos;
    private float xPos;
    private float yPos;

    private int gCost; // 시작 노드부터 현재 노드까지 거리
    private int hCost; // 현재 노드부터 목표 노드까지의 거리
    public int FCost { get=> gCost + hCost; } // 현재 노드의 총 거리 값

    private bool isWalkable;
    private bool isBreakable;

    public Node(float _xPos, float _yPos, Vector3 _nodePos, bool _isWalkable)
    {
        xPos = _xPos;
        yPos = _yPos;
        nodePos = _nodePos;
        isWalkable = _isWalkable;
    }
}