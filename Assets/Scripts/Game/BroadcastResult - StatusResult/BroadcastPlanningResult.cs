using System.Collections;
using System.Collections.Generic;
using Devcat;
using TMPro;
using UnityEngine;

public class BroadcastPlanningResult : GenericSingleton<BroadcastPlanningResult>
{
    [SerializeField] private UnityEngine.UI.Image gearImage;
    [SerializeField] private TextMeshProUGUI kategorie;
    [SerializeField] private TextMeshProUGUI content;

    [SerializeField] private TMP_InputField broadcastTitle;

    [Header("방송 스텟들")]
    [SerializeField] private TextMeshProUGUI planningPoint;
    [SerializeField] private TextMeshProUGUI designerPoint;
    [SerializeField] private TextMeshProUGUI composerPoint;
    [SerializeField] private TextMeshProUGUI promotionPoint;

    private new void Awake()
    {
        base.Awake();
    }
    

    public void SetBroadcastResultPoint()
    {
        int count = ValueCastTo<int>.From(ProductorManager.ProductorType.Count);

        for (int i = 0; i < count; i++)
        {
            int value = BroadCastPlanning.Instance.GetBroadcastPoint(ProductorManager.ProductorType.Planner + i);

            switch (ValueCastTo<ProductorManager.ProductorType>.From(i))
            {
                case ProductorManager.ProductorType.Planner:
                planningPoint.text = value.ToString();
                break;
                case ProductorManager.ProductorType.Designer:
                designerPoint.text = value.ToString();
                break;
                case ProductorManager.ProductorType.Composer:
                composerPoint.text = value.ToString();
                break;
                case ProductorManager.ProductorType.Promotor:
                promotionPoint.text = value.ToString();
                break;
            }
        }
    }

    //json파일로 keywordname -> 한국어 이름 으로 하나 만들어서 사용하는걸로

    public void SetBroadcastResultKeyword()
    {
        
    }

    //기어들 파일 만들어서 이미지 로드로 불러오거나 미리 캐싱하거나
    //몇 개 없으면 로드로하고 아니면 캐싱

    public void SetBroadcastResultGear()
    {

    }


    //얘 받아서 추가되는 탭에다가 넘겨줌
    public string GetBroadcastTitle()
    {
        if (broadcastTitle != null)
        {
            return broadcastTitle.text;
        }

        return null;
    }
}
