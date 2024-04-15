using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    [SerializeField] private GameObject selected;
    [SerializeField] private bool isFirstButton;

    void OnEnable()
    {
        selected.SetActive(isFirstButton);
    }

    public void OnPointerEnter()
    {
        selected.SetActive(true);

        SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_GoSeGu_Muyo.ToString());
    }
}
