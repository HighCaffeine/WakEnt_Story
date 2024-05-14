using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private ScrollRect scrollRect;
    [SerializeField] private MonoBehaviour passToMono;  //viewport

    void Awake()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(eventData);

        scrollRect.OnBeginDrag(eventData);
        ((IBeginDragHandler)passToMono).OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(eventData);

        scrollRect.OnDrag(eventData);
        ((IDragHandler)passToMono).OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(eventData);

        scrollRect.OnEndDrag(eventData);
        ((IEndDragHandler)passToMono).OnEndDrag(eventData);
    }
}
