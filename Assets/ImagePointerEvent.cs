using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImagePointerEvent : MonoBehaviour
{
    [Header("Target Image")][SerializeField] private UnityEngine.UI.Image target;
    [Header("Normal Color")][SerializeField] private Color normalColor;
    [Header("Pointer Enter Color")][SerializeField] private Color pointerEnterColor;

    [Header("Click Event")][SerializeField] private UnityEvent clickEvent;

    public void OnPointerEnter()
    {
        target.color = pointerEnterColor;
    }

    public void OnPointerExit()
    {
        target.color = normalColor; 
    }

    public void OnPointerClick()
    {
        clickEvent?.Invoke();
    }
}
