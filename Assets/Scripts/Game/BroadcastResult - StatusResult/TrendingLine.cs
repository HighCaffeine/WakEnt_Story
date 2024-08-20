using UnityEngine;

public class TrendingLine : MonoBehaviour, OnReturnPool<TrendingLine>
{
    OnReturnPoolEvent<TrendingLine> onReturnPoolEvent;

    public void Init(OnReturnPoolEvent<TrendingLine> onReturnPoolEvent)
    {
        this.onReturnPoolEvent = onReturnPoolEvent;
    }

    public void OnReturnPool()
    {
        gameObject.SetActive(false);
        onReturnPoolEvent?.Invoke(this);
    }
}
