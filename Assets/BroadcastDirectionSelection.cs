using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastDirectionSelection : GenericSingleton<BroadcastDirectionSelection>
{
    public MenuController.UIButtonSelectionControll buttonSelection = new MenuController.UIButtonSelectionControll();

    public delegate void OnDirectionRatioValueUpdate(float ratio);
    public delegate void OnMenuBack();
    public delegate void OnSetDirectionText(string str);

    [SerializeField] private TMPro.TextMeshProUGUI directionInfo;

    public float ratio { get; private set; }

  
    //menuController에 openMenu에 등록해서 사용할 거
    public void Init()
    {
        ratio = 1.00f;
    }

    public void RequestDirectionRatioUpdate(float ratio)
    {
        this.ratio = ratio;
    }

    public void RequestMenuBack()
    {
        MenuController.Instance.MenuBack();
    }

    public void SetDirectionText(string str)
    {
        directionInfo.text = str;
    }
}
