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

    private RectTransform rect;

    public void Init(OnReturnPoolEvent<CharacterStatPopup> onReturnPoolEvent)
    {
        OnReturnPool = onReturnPoolEvent;

        rect = GetComponent<RectTransform>();

        RegisterPlaySFXEvent(CharacterPopupStatManager.Instance.ReqPlaySFX);
    }

    public void SetStatPopup(int amount, Sprite statSprite, Vector2 position)
    {
        statImage.sprite = statSprite;
        statAmount.text = string.Format("+{0}", amount);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
        rect.anchoredPosition = CharacterMessageManager.Instance.MessagePosToScreenPos(screenPos); 

        popupAni.Play();
    }

    public void PlaySFX()
    {
        OnPlaySFXEvent?.Invoke();
    }

    public void ReturnPool()
    {
        OnReturnPool?.Invoke(this);
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
