using System;
using UnityEngine;

public class CharacterPopupStatManager : ObjectPooling<CharacterPopupStatManager, CharacterStatPopup>
{
    public interface PopupEvent
    {
        void RegisterPlaySFXEvent(OnPlaySFXEvent OnPlaySFXEvent);
    }

    public delegate void OnPlaySFXEvent();


    [Header("이미지 띄우는 시간")] [SerializeField] private float disappearTime;

    private System.Collections.Generic.List<Action<bool>> pauseEventList;

    private RectTransform rect;

    private new void Awake()
    {
        pauseEventList = new System.Collections.Generic.List<Action<bool>>();

        base.Awake();

        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="statAmount"></param>
    /// <param name="productorType">증가시킬 스텟 종류</param>
    public void SetStatPopup(int statAmount, Vector2 position, ProductorManager.ProductorType productorType)
    {
        CharacterStatPopup popupObj = GetPool();

        ResourceID resourceID = ResourceID.Stat_Broadcast_InterestPoint;
        int statIndex = -1;

        switch (productorType)
        {
            case ProductorManager.ProductorType.Planner:
            resourceID = ResourceID.Stat_Broadcast_InterestPoint;
            statIndex = 0;
            break;
            case ProductorManager.ProductorType.GraphicDesigner:
            resourceID = ResourceID.Stat_Broadcast_QualityPoint;
            statIndex = 1;
            break;
            case ProductorManager.ProductorType.SoundDesigner:
            resourceID = ResourceID.Stat_Broadcast_SoundPoint;
            statIndex = 2;
            break;
            case ProductorManager.ProductorType.Marketer:
            resourceID = ResourceID.Stat_Broadcast_EditingPoint;
            statIndex = 3;
            break;
        }

        popupObj.SetStatPopup(statAmount, 
                                DataManager.Instance.GetSpriteFromID(resourceID, ResourceType.DefaultSprite), 
                                position,
                                () => { ProductorManager.Instance.UpdateToBroadcast(statIndex, statAmount); });
    }

    //scaler값 곱해야할 듯
    public Vector2 MessagePosToScreenPos(Vector2 messagePos)
    {
        float width = Screen.width;
        float height = Screen.height;

        messagePos.x -= width * 0.5f;
        messagePos.y -= height * 0.5f;

        return messagePos;
    }

    public void ReqPlaySFX()
    {
        if (!SoundManager.Instance) return;

        SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_FieldStatPopup.ToString(), false);
    }

    public void RegisterPauseEvent(Action<bool> pauseEvent)
    {
        pauseEventList.Add(pauseEvent);
    }
}
