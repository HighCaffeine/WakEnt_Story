using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KategorieItem : MonoBehaviour
{
    //아이템 선택을 할 경우 창에 표시되어 있는 것을 교체 해야함.
    //BroadcastPlanning에게 값을 주고 어떤 애를 바꿔야 하는지도 같이 넘겨주고
    //broadcastplanning에게 넘겨줘야 result 반영 가능

    [SerializeField] private KategorieManager.KategorieData kategorieData;

    public void Init(KategorieManager.KategorieType kategorieType, string itemName)
    {
        kategorieData = new KategorieManager.KategorieData();

        kategorieData.Init(kategorieType, itemName);
    }

    public void SelectItem()
    {
        KategorieManager.Instance.UpdateKategorieSelect(kategorieData);
    }
}
