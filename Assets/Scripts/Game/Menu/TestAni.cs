using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAni : MonoBehaviour
{
    [SerializeField] private bool isOpened = false;

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

            TimeElapse();
        }
        else
        {
            menuButtonAnimator.Play("MenuButtonOpen");
            menuListAnimator.Play("MenuOpen");

            TimeStop();
        }

        isOpened = !isOpened;
    }

    public void TimeStop()
    {
        MenuController.Instance.TimeNotElapseWhenOpenTab();
    }

    public void TimeElapse()
    {
        MenuController.Instance.CloseTabElapseTime();
    }


    //특정 버튼들은 애니메이션 끝나기 전에 그냥 바로 close 애니메이션 실행
    public void MenuCloseWhenButtonClick()
    {
        menuButtonAnimator.Play("MenuButtonClose");
        menuListAnimator.Play("MenuClose");

        isOpened = false;
    }
}
