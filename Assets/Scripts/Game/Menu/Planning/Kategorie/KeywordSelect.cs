using TMPro;
using UnityEngine;

public class KeywordSelect : MonoBehaviour, OnReturnPool<KeywordSelect>
{
    OnReturnPoolEvent<KeywordSelect> OnReturnPool;
    BroadcastKeywordSelection.SetCurrentKeywordEvent OnSelectKeyword;

    [Header("키워드 이름")] [SerializeField] private TextMeshProUGUI keywordNameTMP;            //키워드 이름
    [Header("멤버 키워드 숙련도")] [SerializeField] private TextMeshProUGUI memberSkillTMP;      //선택된 멤버의 각 키워드 숙련도
    [Header("인기도")] [SerializeField] private TextMeshProUGUI popularityTMP;                  //인기도
    [Header("키워드 비용")] [SerializeField] private TextMeshProUGUI priceTMP;                  //제작비용

    public void Init(OnReturnPoolEvent<KeywordSelect> onReturnPoolEvent)
    {
        this.OnReturnPool = onReturnPoolEvent;
        OnSelectKeyword = BroadcastKeywordSelection.Instance.SetSelectedKeyword;
    }

    //멤버들 및 각 키워드의 데이터는 데이터 매니저가 가지고 있기 때문에
    //키워드매니저 통해서 델리게이트 호출방식으로 초기화
    // 
    public void SetData(string keywordName, int memberSkill, string popularity, int price, int keywordIndex)
    {
        //return pool event 등록
        BroadcastKeywordSelection.Instance.AddPoolEvent( () => { OnReturnPool?.Invoke(this); });

        keywordNameTMP.text = keywordName;
        memberSkillTMP.text = memberSkill.ToString();
        popularityTMP.text = popularity;
        priceTMP.text = price.ToString();

        this.keywordIndex = keywordIndex;
    }

    private int keywordIndex;

    public void SelectedKeyword()
    {
        OnSelectKeyword?.Invoke(keywordIndex);
    }
}
