using TMPro;
using UnityEngine;

public class KeywordSelect : MonoBehaviour, OnReturnPool<KeywordSelect>
{
    OnReturnPoolEvent<KeywordSelect> OnReturnPool;

    [SerializeField] private TextMeshProUGUI memberSkillTMP;    //선택된 멤버의 각 키워드 숙련도
    //

    public void Init(OnReturnPoolEvent<KeywordSelect> onReturnPoolEvent)
    {
        this.OnReturnPool = onReturnPoolEvent;
    }

    //멤버들 및 각 키워드의 데이터는 데이터 매니저가 가지고 있기 때문에
    //키워드매니저 통해서 델리게이트 호출방식으로 초기화
    // 
    public void SetData()
    {

    }
}
