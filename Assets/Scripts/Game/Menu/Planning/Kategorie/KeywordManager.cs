using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KeywordManager : GenericSingleton<KeywordManager>
{
    //kategorieitem들의 요청을 받고 정보를 broadcastplanning에 넘겨줘서
    //broadcastplanning이 나머지 정보들 업데이트 해서 결과값 반영되게


    public enum BroadcastElement
    {
        Gear,
        Content,
        Type,
    }

    public enum Content
    {
        BroadcasterTogether,
        ViewerParticipation,

        Count,
    }

    public enum Kategorie
    {
        Game,
        VRTalk,
        Dance,
        SingASong,
        Talk,
        Radio,

        Count,
    }

    

    [Serializable]
    public class KategorieData
    {
        [SerializeField] private string kategorieData;
        [SerializeField] private BroadcastElement BroadcastElement;

        public string GetName()
        {
            return kategorieData;
        }

        public BroadcastElement GetBroadcastElement()
        {
            return BroadcastElement;
        }

        public void Init(BroadcastElement BroadcastElement, string kategorieData)
        {
            this.BroadcastElement = BroadcastElement;
            this.kategorieData = kategorieData;
        }
    }

    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private GameObject newAttempt;

    [Header("Kategorie")]
    [SerializeField] private GameObject gear;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject type;

    private GameObject gearObj;
    private GameObject kategorieObj;
    private GameObject contentObj;

    [SerializeField] private TextMeshProUGUI gearTMP;
    private TextMeshProUGUI kategorieTMP;
    private TextMeshProUGUI contentTMP;

    private TextMeshProUGUI matchingResult;

    private new void Awake()
    {
        base.Awake();

        gearObj = transform.Find("Panel/Gear/KeywordSelect").gameObject;
        kategorieObj = transform.Find("Panel/Kategorie/KeywordSelect").gameObject;
        contentObj = transform.Find("Panel/Content/KeywordSelect").gameObject;

        gearTMP = transform.Find("Panel/Gear/SelectedItem").GetComponent<TextMeshProUGUI>();
        kategorieTMP = transform.Find("Panel/Kategorie/SelectedItem").GetComponent<TextMeshProUGUI>();
        contentTMP = transform.Find("Panel/Content/SelectedItem").GetComponent<TextMeshProUGUI>();

        matchingResult = transform.Find("Panel/Result/Matching").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        InitKategorieItem(BroadcastElement.Gear);
        InitKategorieItem(BroadcastElement.Content);
        InitKategorieItem(BroadcastElement.Type);

        UpdateKategorieSelect();
    }

    public void InitKategorieItem(BroadcastElement BroadcastElement)
    {
        InstantiateKategorieObject(BroadcastElement, DataManager.Instance.GetKategorieData(BroadcastElement));
    }

    public void UpdateKategorieSelect(KategorieData kategorieData)
    {
        switch (kategorieData.GetBroadcastElement())
        {
            case BroadcastElement.Gear:
            gearTMP.text = kategorieData.GetName();

            gearObj.SetActive(false);
            break;
            case BroadcastElement.Content:
            kategorieTMP.text = kategorieData.GetName();

            kategorieObj.SetActive(false);
            break;
            case BroadcastElement.Type:
            contentTMP.text = kategorieData.GetName();

            contentObj.SetActive(false);
            break;
        }

        UpdateKategorieSelect();
    }

    public void RequestInActiveOther(BroadcastElement type)
    {
        switch (type)
        {
            case BroadcastElement.Gear:
            if (kategorieObj.activeSelf) kategorieObj.SetActive(false);
            if (contentObj.activeSelf) contentObj.SetActive(false);
            break;
            case BroadcastElement.Content:
            if (gearObj.activeSelf) gearObj.SetActive(false);
            if (contentObj.activeSelf) contentObj.SetActive(false);
            break;
            case BroadcastElement.Type:
            if (kategorieObj.activeSelf) kategorieObj.SetActive(false);
            if (gearObj.activeSelf) gearObj.SetActive(false);
            break;
        }
    }

    private void UpdateKategorieSelect()
    {
        string matchingValue = BroadCastPlanning.Instance.CalculateBroadCastMatchingValue(kategorieTMP.text, contentTMP.text);

        newAttempt.SetActive(false);

        Debug.Log(matchingValue);

        if (matchingValue == BroadCastPlanning.Instance.GetMatchingRateComment(0))
        {
            newAttempt.SetActive(true);
        }

        matchingResult.text = string.Format("{0} + {1} -> {2}", kategorieTMP.text, contentTMP.text, matchingValue);
    }

    private void InstantiateKategorieObject(BroadcastElement BroadcastElement, string[] itemNames)
    {
        GameObject parent = BroadcastElement == BroadcastElement.Gear ? gear
                            : BroadcastElement == BroadcastElement.Content ? content
                            : type;

        for (int i = 0; i < itemNames.Length; i++)
        {
            GameObject obj = Instantiate(itemPrefab, parent.transform);

            KeywordItem keywordItem = obj.GetComponent<KeywordItem>();
            TextMeshProUGUI text = obj.transform.GetComponentInChildren<TextMeshProUGUI>();

            keywordItem.Init(BroadcastElement, itemNames[i]);

            text.text = itemNames[i];
            
        }
    }
}
