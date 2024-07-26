using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PanelCheckEvent : MonoBehaviour
{
    private enum WindowType
    {
        ProductorProcessing,
        InfoWindow,
    }

    [SerializeField] GameObject checkObj;
    [SerializeField] private WindowType currentWindowType;
    [SerializeField] private UnityEvent registerEvent;
    
    private void OnEnable()
    {
        switch (currentWindowType)
        {
            case WindowType.ProductorProcessing:
            registerEvent?.Invoke();
            break;
            case WindowType.InfoWindow:
            break;
        }
        
    }


    private void OnEnableCheckObj()
    {
        checkObj.SetActive(true);
    }

}
