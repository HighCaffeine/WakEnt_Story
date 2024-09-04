
/// <summary>
/// Json파일의 데이터를 읽기위해 각 데이터테이블 구조를 작성.
/// </summary>
[System.Serializable]
public class DataTable
{
    public System.Collections.Generic.List<PlayerData> PlayerData;
    public System.Collections.Generic.List<MatchingData> MatchingData;
    public System.Collections.Generic.List<ReviewCommentData> ReviewComment;
    public System.Collections.Generic.List<EventData> Event;
    public System.Collections.Generic.List<Keyword> Keyword;
    public System.Collections.Generic.List<BroadcastRecord> BroadcastRecord;
}

//플레이어의 데이터
[System.Serializable]
public class PlayerData
{
    public string UserName;
    public string UserID;
    public int Money;
    public int TimeElapsed;


    public void DebugLog()
    {
        UnityEngine.Debug.Log("UserName : " + UserName + "/ UserId : " + UserID + "/ Money : " + Money + "/ Time : " + TimeElapsed);
    }
}

//키워드의 매칭률
[System.Serializable]
public class MatchingData
{
    public string Kategorie;         //컨텐츠 키워드
    public string Type;             //타입 키워드
    public int MatchingPoint;       //매칭률 1~5(눕, 계륵, 프로, 국밥, 해커)
    public string Unlocked;         //해금된 조합인지(O, X)

    public void DebugLog()
    {
        UnityEngine.Debug.Log("Kategorie : " + Kategorie + "/ Type : " + Type + "/ MatchingPoint : " + MatchingPoint + "/ Unlocked : " + Unlocked);
    }
}

//왁물원의 각 등급들의 리뷰 멘트들
[System.Serializable]
public class ReviewCommentData
{
    public string Comment;          //리뷰 멘트
    /// <summary>
    /// 1: 1~3점
    /// 2: 4~6점
    /// 3: 7~9점
    /// 4: 10점
    /// </summary>
    public int Point;               //리뷰 점수
    public string CafeRank;         //왁물원 카페랭크 (진드기, 닭둘기, 왁무새, 침팬치, 느그자)

    public void DebugLog()
    {
        UnityEngine.Debug.Log("Comment : " + Comment + "/ Point : " + Point + "/ CafeRank : " + CafeRank);
    }
}

//제작한 방송의 기록들 최대 20개
[System.Serializable]
public class BroadcastRecord
{
    public string BroadcastName;    //방송이름
    public int Viewer;              //조회 수
    public int Likes;               //좋아요 수
    public int PlannerStat;         //기획 수치
    public int MapStat;             //맵 제작 수치
    public int ComposStat;          //작곡 수치
    public int PromotionStat;       //홍보 수치
}

[System.Serializable]
public class EventData
{
    public string EventKey;         //이벤트 이름, 접근 시 사용
    public string EventComment;     //이벤트 멘트
    public string Achivement;       //도전과제
    public string Progressed;       //진행유무(O, X)
    public string Note;             //언락시기 ( ex) 튜토리얼 -> 초회한정)
    public string IsRepeat;         //반복유무(X -> 1회성 이벤트(튜토리얼))

    public void DebugLog()
    {
        UnityEngine.Debug.Log("EventKey : " + EventKey + "/ EventComment : " + EventComment + "/ Achivement : " + Achivement + "/ Progressed : " + Progressed + "/ Note : " + Note + "/ IsRepeat : " + IsRepeat);
    }
}    

[System.Serializable]
public class Keyword
{
    public string KeywordKey;       //enum타입으로 사용할 키워드 영어버전
    public string KoreanName;       //해당 영어 한국어 버전
    public string Type;             //컨텐츠인지 종류인지 

    public void DebugLog()
    {
        UnityEngine.Debug.Log("KeywordKey : " + KeywordKey + "/ KoreanName : " + KoreanName + "/ Type : " + Type);
    }
}