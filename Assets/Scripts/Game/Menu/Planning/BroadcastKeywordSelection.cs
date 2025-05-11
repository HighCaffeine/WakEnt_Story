using System;
using System.Collections;
using System.Collections.Generic;
using BroadcastKeyword;
using Devcat;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace BroadcastKeyword
{
    //keyword의 각 카테고리 값이 음수일 경우 첫 시도로 취급.
    public enum MatchingValue
    {
        Noob = 1,
        Gyerueok,
        Pro,
        Gookbab,
        Hacker,
    }

    public enum KeywordType
    {
        Kategorie, Content, Count,
    }

    public enum Kategorie
    {
        Game, 
        Sports,
        Music,
        Event,
        VRChat,
        Life,
        Creative,

        Count,
    }

    public enum Content
    {
        LOL, Valorant, Battleground, Minecraft, FIFA, IntegratedGame, HorrorGame,

        Baseball, Badminton, Rugby, Analyze, Golf, Billiards, Bowling, ETCSports,

        Karaoke, Cover, NewAlbum, MusicLive, Streaming, VirtualConcert, Concert,

        Compotition, CompetitionRelay, Audition,

        Map, Avatar, SituationalComedy,

        Talk, ASMR, Mukbang, Fashion, Dance,Cookbang, TaravelVlog, Camping, Fishing, Dubbing, Radio,
        Animal, Car, Comedy, Exercise, Movie, Anime, Food, Beauty,

        Style, Drawing, Science, Tech, Education, Ent, KnowHow,

        Count,
    }
}

public class BroadcastKeywordSelection : ObjectPooling<BroadcastKeywordSelection, KeywordSelect> 
{
    public delegate void SetCurrentKeywordEvent(int index);
    public delegate void ConfirmSelectKeyword(int index);

    //broadcastplanning 창에서
    //키워드 선택 시 selection창 띄움
    //selection창 킬 때 
    //카테고리를 켰을 경우 컨텐츠 값이랑 매칭률, 반대로 컨텐츠를 켰을때는 카테고리와의 매칭률

    //데이터 매니저에게 각 멤버의 스킬레벨(키워드 숙련도)등 키워드 관련된 정보를 얻어와야함.
    //해당 함수 작성 후 풀링객체에게 델리게이트로 전달
    //델리게이트 필요사항
    //1. 멤버별 키워드 스킬 정보
    //2. 키워드의 인기정보 및 비용
    //3. 선택된 키워드와 다른 (카테고리 / 컨텐츠)와의 조합률

    [Header("키워드 이미지 0 : 카테고리, 1 : 컨텐츠")] [SerializeField] private Sprite[] keywordImages;
    [SerializeField] private UnityEngine.UI.Image keywordIcon;

    [SerializeField] private GameObject keywordParent; 
    private Stack<Action> exitPanelPoolEvents;
    private int[,] matchingValues;

    [SerializeField] private Kategorie currentKategorie;
    [SerializeField] private Content currentContent;

    [Header("매칭률 출력부")][SerializeField] private TextMeshProUGUI matchingTMP;
                            [SerializeField] private TextMeshProUGUI keywordMatchingTMP;

    [Space(10f)]
    [Header("이세돌 스킬 레벨 하트")]
    [SerializeField] private TextMeshProUGUI[] memberSkillLevels;

    //언락된 애들만 담거나 해야할듯
    private List<Keyword> kategories;
    private List<Keyword> contents;

    private string[] matchingValueWords = { "늅", "계륵", "프로", "국밥", "해커" };
    private string[] popularity = { "혐", "애매쓰", "중간", "높음", "GOAT" };

    //얘로 패널 끌 때, 리셋
    //이벤트 등록 함수 넘겨주기
    public MenuController.UIButtonSelectionControll buttonSelectionController = new MenuController.UIButtonSelectionControll();

    private new void Awake()
    {
        base.Awake();

        exitPanelPoolEvents = new Stack<Action>();
        kategories = new List<Keyword>();
        contents = new List<Keyword>();

        matchingValues = new int[ValueCastTo<int>.From(Kategorie.Count), ValueCastTo<int>.From(contents.Count)];

        DataManager.Instance.SetMatchingValue(ref matchingValues);
        kategories = DataManager.Instance.GetKeyword(true);
        contents = DataManager.Instance.GetKeyword(false);
    }

    public int GetSelectCharacterLimit()
    {
        //각 키워드별 멤버 선택 수 제한

        return 2;
    }

    //패널 열 때에
    //키워드 두종류 언락된 애들만 받아오고
    //언락하는건 추후에 추가해야하는데
    //걔는 따로 데이터쪽에 수정 요청 보내는 걸로 하는거고 어차피 여기서 하는거 아님

    private bool isKategoriPanelActive;

    public struct IsedolMemberSkillInfo
    {
        public int level;
        public bool isSelected;

        public IsedolMemberSkillInfo(int level, bool isSelected)
        {
            this.level = level;
            this.isSelected = isSelected;
        }
    }

    public void SetPanel(bool isKategorie)
    {
        isKategoriPanelActive = isKategorie;
        buttonSelectionController.ResetEvent();

        for (int i = 0; i < memberSkillLevels.Length; i++)
        {
            Color color = memberSkillLevels[i].color;
            color.a = (BroadCastPlanning.Instance.IsActiveMember(CharacterManager.ISEGYEIDOL.Ine + i) ? 255f : 65f) / 255f;

            memberSkillLevels[i].color = color;
        }

        for (int i = 0; i < (isKategorie ? kategories.Count : contents.Count); i++)
        {
            KeywordSelect obj = GetPool();
            Keyword keyword = GetKeyword(isKategorie, i);

            obj.transform.SetParent(keywordParent.transform);

            int keywordIndex = i + (isKategorie ? 0 : ValueCastTo<int>.From(Kategorie.Count));
            ISDKeywordLevel isdKeywordLevel = DataManager.Instance.GetISDKeyworldLevel(keywordIndex);

            obj.SetData(keyword.KoreanName, 
                new IsedolMemberSkillInfo[]
                { new IsedolMemberSkillInfo(isdKeywordLevel.Ine, BroadCastPlanning.Instance.IsActiveMember(CharacterManager.ISEGYEIDOL.Ine)),
                  new IsedolMemberSkillInfo(isdKeywordLevel.JingBurger, BroadCastPlanning.Instance.IsActiveMember(CharacterManager.ISEGYEIDOL.JingBurger)),
                  new IsedolMemberSkillInfo(isdKeywordLevel.Lilpa, BroadCastPlanning.Instance.IsActiveMember(CharacterManager.ISEGYEIDOL.Lilpa)),
                  new IsedolMemberSkillInfo(isdKeywordLevel.Jururu, BroadCastPlanning.Instance.IsActiveMember(CharacterManager.ISEGYEIDOL.Jururu)),
                  new IsedolMemberSkillInfo(isdKeywordLevel.Gosegu, BroadCastPlanning.Instance.IsActiveMember(CharacterManager.ISEGYEIDOL.Gosegu)),
                  new IsedolMemberSkillInfo(isdKeywordLevel.Viichan, BroadCastPlanning.Instance.IsActiveMember(CharacterManager.ISEGYEIDOL.Viichan)) },
                  popularity[keyword.Popularity - 1], 
                  999, 
                  i);
        }

        SetKeywordIcon();
        SetMatchingValue(0);
    }

    private void SetKeywordIcon()
    {
        keywordIcon.sprite = isKategoriPanelActive ? keywordImages[ValueCastTo<int>.From(KeywordType.Kategorie)]
                                                    : keywordImages[ValueCastTo<int>.From(KeywordType.Content)];
    }

    private void SetMatchingValue(int index)
    {
        int matchingValue = GetMatchingValue(index);

        keywordMatchingTMP.text = string.Format("{0}(와)과 조합=",
                                                        isKategoriPanelActive ? GetKeyword(false, ValueCastTo<int>.From(currentContent)).KoreanName
                                                                                : GetKeyword(true, ValueCastTo<int>.From(currentKategorie)).KoreanName);

        //매칭값이 음수일 경우 첫 시도하는 조합
        if (matchingValue > 0)
        {
            matchingTMP.text = matchingValueWords[matchingValue - 1];
        }
        else
        {
            matchingTMP.text = "첫 시도";
        }
    }

    public void SetSelectedKeyword(int index)
    {
        if (isKategoriPanelActive)
        {
            SetMatchingValue(index);
        }
        else
        {
            SetMatchingValue(index);
        }
    }

    public void ConfirmKeyword(int index)
    {
        if (isKategoriPanelActive)
        {
            currentKategorie = Kategorie.Game + index;
        }
        else
        {
            currentContent = Content.LOL + index;
        }

        ConfirmToBroadcastPanel();
        MenuController.Instance.MenuBack();
    }

    private void ConfirmToBroadcastPanel()
    {
        if (isKategoriPanelActive)
        {
            BroadCastPlanning.Instance.SetKategorieText(GetKeyword(isKategoriPanelActive, ValueCastTo<int>.From(currentKategorie)).KoreanName);
        }
        else
        {
            BroadCastPlanning.Instance.SetContentText(GetKeyword(isKategoriPanelActive, ValueCastTo<int>.From(currentContent)).KoreanName);
        }
    }

    //broadcastpanel에서 결정 누를 때 해당 함수도 같이 호출
    public void SetMatchingValuePositive()
    {
        if (GetMatchingValue() < 0)
        {
            matchingValues[ValueCastTo<int>.From(currentContent), ValueCastTo<int>.From(currentKategorie)] *= -1;
        }
    }

    private Keyword GetKeyword(bool isKategorie, int index)
    {
        return isKategorie ? kategories[index] : contents[index];
    }

    //선택된 키워드를 기준으로
    //아래에 매칭률 출력
    private int GetMatchingValue()
    {
        return matchingValues[ValueCastTo<int>.From(currentContent), ValueCastTo<int>.From(currentKategorie)];
    }

    private int GetMatchingValue(int index)
    {
        Kategorie kategorie = isKategoriPanelActive ?  Kategorie.Game + index : currentKategorie;
        Content content = isKategoriPanelActive ? currentContent : Content.LOL + index ;

        return matchingValues[ValueCastTo<int>.From(content), ValueCastTo<int>.From(kategorie)];
    }


    //매니저 측에서 창 껏을 때 등록된 애들 호출해서 return pool
    //setdata 시 등록
    public void AddPoolEvent(Action returnPool)
    {
        exitPanelPoolEvents.Push(returnPool);
    }

    public void ClosePanel()
    {
        while (exitPanelPoolEvents.Count > 0)
        {
            exitPanelPoolEvents.Pop()?.Invoke();
        }
    }

    private void UpdateSelectedKeyword()
    {

    }

    //public 
}
