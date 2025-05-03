using BroadcastKeyword;
using TMPro;
using UnityEngine;
using static BroadcastKeywordSelection;

public class KeywordSelect : MonoBehaviour, OnReturnPool<KeywordSelect>
{
    OnReturnPoolEvent<KeywordSelect> OnReturnPool;
    BroadcastKeywordSelection.SetCurrentKeywordEvent OnSelectKeyword;
    BroadcastKeywordSelection.ConfirmSelectKeyword OnConfirmSelect;

    BroadcastKeywordSelection.CancelButtonEvent OnCancelButtonEvent;

    [Header("키워드 이름")] [SerializeField] private TextMeshProUGUI keywordNameTMP;            //키워드 이름
    [Header("인기도")] [SerializeField] private TextMeshProUGUI popularityTMP;                  //인기도
    [Header("키워드 비용")] [SerializeField] private TextMeshProUGUI priceTMP;                  //제작비용

    //선택된 멤버의 각 키워드 숙련도
    [Header("멤버 키워드 숙련도")]      
    [SerializeField] private TextMeshProUGUI[] memberKeywordLevels;


    private bool currentSelected;

    public void Init(OnReturnPoolEvent<KeywordSelect> onReturnPoolEvent)
    {
        this.OnReturnPool = onReturnPoolEvent;
        OnSelectKeyword = BroadcastKeywordSelection.Instance.SetSelectedKeyword;
        OnConfirmSelect = BroadcastKeywordSelection.Instance.ConfirmKeyword;
        OnCancelButtonEvent = BroadcastKeywordSelection.Instance.buttonSelectionController.RegisterCancelEvent;

    }

    //멤버들 및 각 키워드의 데이터는 데이터 매니저가 가지고 있기 때문에
    //키워드매니저 통해서 델리게이트 호출방식으로 초기화
    // 
    public void SetData(string keywordName, IsedolMemberSkillInfo[] memberSkills, string popularity, int price, int keywordIndex)
    {
        //return pool event 등록
        BroadcastKeywordSelection.Instance.AddPoolEvent( () => { OnReturnPool?.Invoke(this); });

        keywordNameTMP.text = keywordName;

        for (int i = 0; i < memberSkills.Length; i++)
        {
            memberKeywordLevels[i].text = memberSkills[i].level.ToString();

            Color color = memberKeywordLevels[i].color;
            color.a = (memberSkills[i].isSelected ? 255f : 65f) / 255f;

            memberKeywordLevels[i].color = color;
        }

        popularityTMP.text = popularity;
        priceTMP.text = price.ToString();

        currentSelected = false;
        this.keywordIndex = keywordIndex;
    }

    private int keywordIndex;

    private void CancelSelect()
    {
        currentSelected = false;
    }

    public void SelectedKeyword()
    {
        if (currentSelected)
        {
            OnConfirmSelect?.Invoke(keywordIndex);

            return;
        }

        currentSelected = true;
        OnCancelButtonEvent?.Invoke(CancelSelect);

        OnSelectKeyword?.Invoke(keywordIndex);
    }
}
