using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImagePointerEvent : MonoBehaviour
{
    [Header("Target Image")][SerializeField] private UnityEngine.UI.Image target;
    [Header("Trancparency Color")][SerializeField] private Color trancparencyColor;
    [Header("Pointer Enter Color")][SerializeField] private Color pointerEnterColor;

    [Header("Click Event")][SerializeField] private UnityEvent clickEvent;


    private bool isAllowMouseEvent = true;

    public void OnPointerEnter()
    {
        if (!isAllowMouseEvent) return;

        SetNormalColor();
    }

    public void OnPointerExit()
    {
        if (!isAllowMouseEvent) return;

        SetTransparencyColor();
    }

    public void OnPointerClick()
    {
        clickEvent?.Invoke();
    }

    public void SetNormalColor()
    {
        target.color = pointerEnterColor;
    }

    public void SetTransparencyColor()
    {
        target.color = trancparencyColor;
    }

    public void SetAllowMouseEvent(bool value)
    {
        isAllowMouseEvent = value;
    }
}
