using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;



public class BroadCastPlaning : GenericSingleton<BroadCastPlaning>
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


    public float CalculateBroadCastMatchingValue(string contents, string broadcastType)
    {
        SetBroadCastValue(contents, broadcastType);

        return broadCast.GetMatchingRate();
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

        float matchingRate = keywordMatching.ContainsKey(key) ? keywordMatching[key] : 0.0f;

        broadCast.SetMatchingRate(matchingRate);
    }

    /// <summary>
    /// rate가 1부터 5까지 존재하고 제일낮은 0(눕) 부터 계륵, 프로, 국밥, 해커(4)순으로 되고
    /// MenuController에게 매칭률 전달해줌
    /// </summary>
    /// <param name="rate">매칭률</param>
    /// <returns></returns>
    public string GetBroadCastMatchingRateComment()
    {
        int value = (int)Math.Round(broadCast.GetMatchingRate());

        if (value < 0)
        {
            return matchingRateComment[0];
        }

        return matchingRateComment[value - 1];
    }
}
