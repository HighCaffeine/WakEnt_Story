using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    [SerializeField] private GameObject selected;
    [SerializeField] private bool isFirstButton;

    [SerializeField] private SoundManager.Effect effect;

    void OnEnable()
    {
        selected.SetActive(isFirstButton);
    }

    public void OnPointerEnter()
    {
        selected.SetActive(true);

        SoundManager.Instance.PlaySound(effect.ToString());
    }
}
