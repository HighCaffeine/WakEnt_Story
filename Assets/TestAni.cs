using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAni : MonoBehaviour
{
    private bool isOpened = false;

    public Animator menuButtonAnimator;
    public Animator menuListAnimator;

    public void TEST()
    {
        if (isOpened)
        {
            menuButtonAnimator.Play("MenuButtonClose");
            menuListAnimator.Play("MenuClose");
        }
        else
        {
            menuButtonAnimator.Play("MenuButtonOpen");
            menuListAnimator.Play("MenuOpen");
        }

        isOpened = !isOpened;
    }
}
