using System;
using System.Collections.Generic;
using UnityEngine;



public class BroadCastPlanning : GenericSingleton<BroadCastPlanning>
{
    //MenuController가 알고있는 컨텐츠와 방송타입 종류를 선택하고 여기로 넘겨줌.
    //menu는 컨텐츠와 방송종류는 모르고 몇 번째껄 선택했는지만 넘겨주고
    //메뉴를 만들 때 broadcastplaning에 list로 되어있는 걸
    //data매니저가 먼저 세팅을 해 주고, menucontroller가 메뉴를 생성할 때(start)
    //hash/dictionary로 넘겨줄 듯(menucontroller는 이름만 알면 됨)
    [Serializable]
    private class Broadcast
    {
        private Contents contents;
        private BroadcastType broadcastType;

        private float matchingRate;

        public void SetContents(in Contents contents)
        {
            this.contents = contents;
        }

        public void SetBroadcastType(in BroadcastType broadcastType)
        {
            this.broadcastType = broadcastType;
        }

        public void SetMatchingRate(in float value)
        {
            matchingRate = value;
        }

        public Contents GetContents()
        {
            return contents;
        }

        public BroadcastType GetBroadcastType()
        {
            return broadcastType;
        }

        public float GetMatchingRate()
        {
            return matchingRate;
        }
    }

    public enum KategorieType
    {
        Gear,
        Content,
        Type,
    }

    private enum Contents
    {
        BroadcasterTogether,
        ViewerParticipation,

        Count,
    }

    private enum BroadcastType
    {
        Game,
        VRTalk,
        Dance,
        SingASong,
        Talk,
        Radio,

        Count,
    }

    private new void Awake()
    {
        base.Awake();

        

        keywordMatching = new Dictionary<string, float>();
        matchingRateComment = new List<string>();
        
    }

    //방송 기획에 쓰이는 로직 작성 예정
    //컨텐츠(게임, 추가예정)와 종류(유니티, 시참), 장비(풀트, 모캡등)
    //두 가지 조합해서(조합 매칭 방식을 정해야 함) -> 수치로 하는게 좋은데
    //1.비트연산으로 구현하는걸로 곱연산으로 하고 높을수록 조합 좋은걸로
    // 0    0    0    0
    //2. 숫자 4자리로 하고 계산하는 곳에서 10으로 나눠서 값들 판단하기로 하고
    // 비트연산으로 하면 데이터시트에 표시하기게 애매함 
    //  -> 1~5중에 방송종류(행)와 컨텐츠(열)의 매칭률을 값으로 가지고 있는걸로 
    //   값을 Dictionary로  (컨텐츠_방송종류, 매칭률) 구조로 가는걸로

/// <summary>
/// 방송종류와 콘텐츠를 받아와서 결과 값을 반환
/// 두 키워드가 잘 맞는지 판단하는 로직
/// </summary>
/// <param name="broadcast"></param>
/// <param name="contents"></param>
/// 

    [SerializeField] private Dictionary<string, float> keywordMatching;
    [SerializeField] private List<string> matchingRateComment;

    //필요없을수도
    [SerializeField] private Broadcast broadCast;

    void Start()
    {
        DataManager.Instance.SetBroadcastValue(keywordMatching);
        DataManager.Instance.SetBroadcastMatching(matchingRateComment);

        broadCast = new Broadcast();
    }


    public string CalculateBroadCastMatchingValue(string contents, string broadcastType)
    {
        SetBroadCastValue(contents, broadcastType);

        return GetMatchingRateComment(Mathf.RoundToInt(broadCast.GetMatchingRate()));
    }

    private void SetBroadCastValue(string contents, string broadcastType)
    {
        int contentsCount = (int)Contents.Count;

        for (int i = 0; i < contentsCount; i++)
        {
            if ((Contents.BroadcasterTogether + i).ToString() == contents)
            {
                broadCast.SetContents(Contents.BroadcasterTogether + i);
            }
        }

        int broadcastTypeCount = (int)BroadcastType.Count;

        for (int i = 0; i < broadcastTypeCount; i++)
        {
            if ((BroadcastType.Game + i).ToString() == broadcastType)
            {
                broadCast.SetContents(Contents.BroadcasterTogether + i);
            }
        }
 
        string key = string.Format(contents + "_" + broadcastType);

        float matchingRate = GetMatchingRate(contents, broadcastType);

        broadCast.SetMatchingRate(matchingRate);
    }

    public float GetMatchingRate(string contents, string broadcastType)
    {
        string key = string.Format("{0}_{1}", contents, broadcastType);

        Debug.Log(key);

        float value = keywordMatching.ContainsKey(key) ? keywordMatching[key] : 0.0f;

        return value;
    }

    /// <summary>
    /// rate가 1부터 5까지 존재하고 제일낮은 0(눕) 부터 계륵, 프로, 국밥, 해커(4)순으로 되고
    /// MenuController에게 매칭률 전달해줌
    /// </summary>
    /// <param name="rate">매칭률</param>
    /// <returns></returns>
    public string GetMatchingRateComment(int index)
    {
        if (index < 0)
        {
            return matchingRateComment[0];
        }

        return matchingRateComment[index];
    }

    //matchingrate 값을 broadcastplanning에서 관리하고, 모든 곳에서 수치로 사용
    //카테고리에 넘기는 값만 해당하는 string값으로

    /// <summary>
    /// 
    /// create팝업 패널을 우선적으로 만들어야 하고, 해당 패널에서 작업자 선택을 함 이후 나올 작업자 선택도 해당 패널로 진행
    /// broadcastplanning패널에서 비용을 명시해 줘야 함 => 동시에 플레이어의 돈 관리 시스템도 추가(돈은 GameManager 통해서 datamanager로 반영하는걸로)
    /// 
    /// 
    /// 1. 키워드로 받은 결과 값(matchingRate)을 저장
    /// 2. broadcastcreate창에서 작업자고르기(추후 수정될 수 있음) 
    ///     => 왁타버스 작업자들 등장시킬 예정 (무리라고 생각되면 다른 방안으로)
    /// 3. 작업자 값 + 매칭값 + 장비로 기획 비용 및 결과값 계산 로직
    /// 4. 방송 제작 단계별로 작업자를 정해서 방송을 만드는 걸로
    ///    => 기획, 맵 제작 ... 등 각 단계별로 집중적으로 오르는 분야 점수가 다름
    ///       기획 => 완성도
    ///       맵 제작 => 시청자 만족도 증가
    ///       2개정도 더 추가 예정
    /// 5. 방송을 하고 결과값으로 방송당일의 값과 유튜브 업로드 후의 반응 댓글 및 조회수, 좋아요 수에 따른 결과 반영 로직
    /// 
    /// Textwindow에서 json에서 이벤트 값으로 튜토리얼 설명 및 정보 안내
    /// 
    /// </summary>

    public void GetPlan()
    {

    }
}
