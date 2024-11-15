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

    public bool IsNotWalkable { get => isNotWalkable; private set {} }
    public bool IsInteractable { get => isInteractiveArea; private set {}}
    public Vector2 Pos { get => nodePos; private set{} }

    public Node parentNode;
    public Node childNode;

    private bool isNotWalkable;
    private bool isInteractiveArea;

    public Node(int xPos, int yPos, Vector2 nodePos, bool isNotWalkable, bool isInteractiveArea)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.nodePos = nodePos;
        this.isNotWalkable = isNotWalkable;

        this.isInteractiveArea = isInteractiveArea;
    }
}