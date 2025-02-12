using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class LayerData : MonoBehaviour
{
    [Header("상호작용 때 적용할 Layer")]
    [SerializeField] private SpriteRenderer interactiveSpriteLayer;
    [SerializeField] private TilemapRenderer interactiveTileLayer;
    [SerializeField] private SortingGroup interactiveSortingGroupLayer;

    [Header("움직일 때 적용할 Layer")]
    [SerializeField] private SpriteRenderer moveSpriteLayer;
    [SerializeField] private TilemapRenderer moveTileLayer;
    [SerializeField] private SortingGroup moveSortingGroupLayer;

    public int InteractiveLayer 
    { 
        get => interactiveSpriteLayer ? interactiveSpriteLayer.sortingOrder
                 : interactiveTileLayer ? interactiveTileLayer.sortingOrder 
                 : interactiveSortingGroupLayer ? interactiveSortingGroupLayer.sortingOrder
                 : int.MinValue; 
    }

    public int MoveLayer 
    { 
        get => moveSpriteLayer ? moveSpriteLayer.sortingOrder
                 : moveTileLayer ? moveTileLayer.sortingOrder 
                 : moveSortingGroupLayer ? moveSortingGroupLayer.sortingOrder
                 : int.MinValue; 
    }
}
