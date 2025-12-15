using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BroadcastDirectionSelection;

public class BroadcastDirection : MonoBehaviour
{
    //세팅은 미리 해두고
    //클릭 이벤트만 전달할 듯
    //버튼으로만 구성 및 내부에서 메뉴 컨트롤러에 요청하는걸로
    //키워드 셀렉션이랑 비슷하니깐
    

    [Header("비용 배율")] [SerializeField] private float priceMulti;
    [TextArea][Header("방향성 설명")][SerializeField] private string directionDescription;

    private bool currentSelected;

    MenuController.CancelButtonEvent OnCancelButtonEvent;
    OnDirectionRatioValueUpdate OnDirectionRatioValueUpdate;
    OnMenuBack OnMenuBack;
    OnSetDirectionText OnSetDirectionText;

    //broadcast planning 쪽에 request하는 거 (선택 결과)

    private void Awake()
    {
        OnDirectionRatioValueUpdate = BroadcastDirectionSelection.Instance.RequestDirectionRatioUpdate;
        OnMenuBack = BroadcastDirectionSelection.Instance.RequestMenuBack;
        OnSetDirectionText = BroadcastDirectionSelection.Instance.SetDirectionText;
        OnCancelButtonEvent = BroadcastDirectionSelection.Instance.buttonSelection.RegisterCancelEvent;

    }

    private void OnEnable()
    {
        currentSelected = false;
    }

    private void CancelSelect()
    {
        currentSelected = false;
    }


    public void OnSelectEvent()
    {
        if (currentSelected)
        {
            OnDirectionRatioValueUpdate?.Invoke(priceMulti * 0.01f);    // pricemulti 값으로 배율 변경
            OnMenuBack?.Invoke();                               // 탭 종료

            return;
        }

        currentSelected = true;
        OnSetDirectionText?.Invoke(directionDescription);   //설명 업데이트
        OnCancelButtonEvent?.Invoke(CancelSelect);
    }
}
