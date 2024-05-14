using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KategorieManager : GenericSingleton<KategorieManager>
{
    //kategorieitem들의 요청을 받고 정보를 broadcastplanning에 넘겨줘서
    //broadcastplanning이 나머지 정보들 업데이트 해서 결과값 반영되게

    [Serializable]
    public class KategorieData
    {
        [SerializeField] private string kategorieData;
        [SerializeField] private BroadCastPlanning.KategorieType kategorieType;

        public string GetName()
        {
            return kategorieData;
        }

        public BroadCastPlanning.KategorieType GetKategorieType()
        {
            return kategorieType;
        }

        public void Init(BroadCastPlanning.KategorieType kategorieType, string kategorieData)
        {
            this.kategorieType = kategorieType;
            this.kategorieData = kategorieData;
        }
    }

    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private GameObject newAttempt;

    [Header("Kategorie")]
    [SerializeField] private GameObject gear;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject type;

    private GameObject gearKategorie;
    private GameObject contentKategorie;
    private GameObject typeKategorie;

    [SerializeField] private TextMeshProUGUI gearTMP;
    private TextMeshProUGUI contentTMP;
    private TextMeshProUGUI typeTMP;

    private TextMeshProUGUI matchingResult;

    private new void Awake()
    {
        base.Awake();

        gearKategorie = transform.Find("Gear/KategorieSelect").gameObject;
        contentKategorie = transform.Find("Content/KategorieSelect").gameObject;
        typeKategorie = transform.Find("Type/KategorieSelect").gameObject;

        gearTMP = transform.Find("Gear/SelectedItem").GetComponent<TextMeshProUGUI>();
        contentTMP = transform.Find("Content/SelectedItem").GetComponent<TextMeshProUGUI>();
        typeTMP = transform.Find("Type/SelectedItem").GetComponent<TextMeshProUGUI>();

        matchingResult = transform.Find("Result/Matching").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        InitKategorieItem(BroadCastPlanning.KategorieType.Gear);
        InitKategorieItem(BroadCastPlanning.KategorieType.Content);
        InitKategorieItem(BroadCastPlanning.KategorieType.Type);

        UpdateKategorieSelect();
    }

    public void InitKategorieItem(BroadCastPlanning.KategorieType kategorieType)
    {
        InstantiateKategorieObject(kategorieType, DataManager.Instance.GetKategorieData(kategorieType));
    }

    public void UpdateKategorieSelect(KategorieData kategorieData)
    {
        switch (kategorieData.GetKategorieType())
        {
            case BroadCastPlanning.KategorieType.Gear:
            gearTMP.text = kategorieData.GetName();

            gearKategorie.SetActive(false);
            break;
            case BroadCastPlanning.KategorieType.Content:
            contentTMP.text = kategorieData.GetName();

            contentKategorie.SetActive(false);
            break;
            case BroadCastPlanning.KategorieType.Type:
            typeTMP.text = kategorieData.GetName();

            typeKategorie.SetActive(false);
            break;
        }

        UpdateKategorieSelect();
    }

    public void RequestInActiveOther(BroadCastPlanning.KategorieType type)
    {
        switch (type)
        {
            case BroadCastPlanning.KategorieType.Gear:
            if (contentKategorie.activeSelf) contentKategorie.SetActive(false);
            if (typeKategorie.activeSelf) typeKategorie.SetActive(false);
            break;
            case BroadCastPlanning.KategorieType.Content:
            if (gearKategorie.activeSelf) gearKategorie.SetActive(false);
            if (typeKategorie.activeSelf) typeKategorie.SetActive(false);
            break;
            case BroadCastPlanning.KategorieType.Type:
            if (contentKategorie.activeSelf) contentKategorie.SetActive(false);
            if (gearKategorie.activeSelf) gearKategorie.SetActive(false);
            break;
        }
    }

    private void UpdateKategorieSelect()
    {
        string matchingValue = BroadCastPlanning.Instance.CalculateBroadCastMatchingValue(contentTMP.text, typeTMP.text);

        newAttempt.SetActive(false);

        Debug.Log(matchingValue);

        if (matchingValue == null)
        {
            newAttempt.SetActive(true);

            matchingValue = "첫 시도";
        }

        matchingResult.text = string.Format("{0} + {1} -> {2}", contentTMP.text, typeTMP.text, matchingValue);
    }

    private void InstantiateKategorieObject(BroadCastPlanning.KategorieType kategorieType, string[] itemNames)
    {
        GameObject parent = kategorieType == BroadCastPlanning.KategorieType.Gear ? gear
                            : kategorieType == BroadCastPlanning.KategorieType.Content ? content
                            : type;

        for (int i = 0; i < itemNames.Length; i++)
        {
            GameObject obj = Instantiate(itemPrefab, parent.transform);

            KategorieItem kategorieItem = obj.GetComponent<KategorieItem>();
            TextMeshProUGUI text = obj.transform.GetComponentInChildren<TextMeshProUGUI>();

            kategorieItem.Init(kategorieType, itemNames[i]);

            text.text = itemNames[i];
            
        }
    }
}
