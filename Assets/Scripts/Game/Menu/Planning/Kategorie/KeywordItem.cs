using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordItem : MonoBehaviour
{
    //아이템 선택을 할 경우 창에 표시되어 있는 것을 교체 해야함.
    //BroadcastPlanning에게 값을 주고 어떤 애를 바꿔야 하는지도 같이 넘겨주고
    //broadcastplanning에게 넘겨줘야 result 반영 가능

    [SerializeField] private KeywordManager.KategorieData kategorieData;

    public void Init(KeywordManager.BroadcastElement BroadcastElement, string itemName)
    {
        kategorieData = new KeywordManager.KategorieData();

        kategorieData.Init(BroadcastElement, itemName);
    }

    public void SelectItem()
    {
        //KeywordManager.Instance.UpdateKategorieSelect(kategorieData);
    }
}
