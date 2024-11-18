using UnityEngine;
using System;
using System.Collections;

public class CharacterStatPopup : MonoBehaviour, OnReturnPool<CharacterStatPopup>, CharacterPopupStatManager.PopupEvent
{
    OnReturnPoolEvent<CharacterStatPopup> OnReturnPool;
    CharacterPopupStatManager.OnPlaySFXEvent OnPlaySFXEvent;

    [Header("스텟 이미지")] [SerializeField] private UnityEngine.UI.Image statImage;
    [SerializeField] private TMPro.TextMeshProUGUI statAmount;
    [SerializeField] private Animation popupAni;

    public void Init(OnReturnPoolEvent<CharacterStatPopup> onReturnPoolEvent)
    {
        OnReturnPool = onReturnPoolEvent;

        RegisterPlaySFXEvent(CharacterPopupStatManager.Instance.ReqPlaySFX);
    }

    public void SetStatPopup(int amount, Sprite statSprite)
    {
        statImage.sprite = statSprite;
        statAmount.text = string.Format("+{0}", statAmount);

        popupAni.Play();
    }

    public void PopupDisappear()
    {
        gameObject.SetActive(false);
    }

    public void RegisterPlaySFXEvent(CharacterPopupStatManager.OnPlaySFXEvent OnPlaySFXEvent)
    {
        this.OnPlaySFXEvent = OnPlaySFXEvent;
    }
}
