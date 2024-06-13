using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class StatIcon : MonoBehaviour, OnReturnPool<StatIcon>
{
    OnReturnPoolEvent<StatIcon> OnReturnPoolEvent;
    [SerializeField] private float speed;

    private Image image;
    
    private Vector2 endPos;

    private RectTransform myRect;

    public void Init(OnReturnPoolEvent<StatIcon> onReturnPoolEvent)
    {
        OnReturnPoolEvent = onReturnPoolEvent;

        image = GetComponent<Image>();
        myRect = GetComponent<RectTransform>();

        myRect.sizeDelta = StatIconManager.Instance.GetSize();
    }

    Vector2 newPos;

    void OnEnable()
    {
        if (OnReturnPoolEvent == null)
        {
            return;
        }
    }

    private void FixedUpdate()
    {
        newPos = transform.position;
        newPos += Vector2.down * Time.deltaTime * speed * 100f;

        transform.position = newPos;

        if (endPos.y >= myRect.position.y)
        {
            OnReturnPoolEvent?.Invoke(this);
        }
    }

    public void SetImage(Sprite image)
    {
        if (image == null)
        {
            return;
        }

        this.image.sprite = image;
    }

    public void SetEndPos(Vector2 pos)
    {
        endPos = pos;
    }
}
