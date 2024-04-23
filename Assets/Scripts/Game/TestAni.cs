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
        if ((menuListAnimator.GetCurrentAnimatorStateInfo(0).IsName("MenuClose")
            || menuListAnimator.GetCurrentAnimatorStateInfo(0).IsName("MenuOpen"))
            && menuListAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return;
        }

 
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
