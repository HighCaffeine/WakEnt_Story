using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KategorieSelect : MonoBehaviour
{
    [SerializeField] private GameObject kategorie;
    [SerializeField] private KategorieManager.KategorieType type;

    public void Selected()
    {
        kategorie.SetActive(!kategorie.activeSelf);

        KategorieManager.Instance.RequestInActiveOther(type);
    }
}
