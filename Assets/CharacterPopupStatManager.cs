using UnityEngine;

public class CharacterPopupStatManager : ObjectPooling<CharacterPopupStatManager, CharacterStatPopup>
{
    public interface PopupEvent
    {
        void RegisterPlaySFXEvent(OnPlaySFXEvent OnPlaySFXEvent);
    }

    public delegate void OnPlaySFXEvent();


    [Header("이미지 띄우는 시간")] [SerializeField] private float disappearTime;

    public float DisappearTime => disappearTime;


    private new void Awake()
    {
        base.Awake();
    }
    public void SetStatPopup(int statAmount, ResourceID resourceID)
    {
        CharacterStatPopup popupObj = GetPool();

        popupObj.SetStatPopup(statAmount, DataManager.Instance.GetSpriteFromID(resourceID, ResourceType.Stat));
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
        //SoundManager.Instance.PlaySound();
    }
}
