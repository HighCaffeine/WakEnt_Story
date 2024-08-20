using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordSelect : MonoBehaviour
{
    [SerializeField] private GameObject kategorie;
    [SerializeField] private KeywordManager.BroadcastElement type;

    public void Selected()
    {
        kategorie.SetActive(!kategorie.activeSelf);

        KeywordManager.Instance.RequestInActiveOther(type);
    }
}
