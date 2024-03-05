using UnityEngine;
using UnityEngine.UI;

public class LoadingImageController : MonoBehaviour
{
    private void Awake()
    {
        Image image = transform.GetComponent<Image>();

        SceneController.Instance.SetLoadImage(image);
    }
}
