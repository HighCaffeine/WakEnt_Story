using UnityEngine;

public class KategorieSibling : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetLastSibling()
    {
        rectTransform.SetAsLastSibling();
    }
}
