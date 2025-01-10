using System;

public enum ResourceID
{
    Item_Broadcast_Fever                = 101,
    Item_Broadcast_FatigueReliever      = 102,
    Item_Broadcast_PlanPointBoost       = 103,
    Item_Broadcast_DesignPointBoost     = 104,
    Item_Broadcast_SoundPointBoost      = 105,
    Item_Broadcast_PromotionPointBoost  = 106,
    Stat_Broadcast_InterestPoint        = 201,
    Stat_Broadcast_QualityPoint         = 202,
    Stat_Broadcast_SoundPoint           = 203,
    Stat_Broadcast_EditingPoint         = 204,
    Character_ISD_Ine                   = 1001,
    Character_ISD_Jingburger            = 1002,
    Character_ISD_Lilpa                 = 1003,
    Character_ISD_Jururu                = 1004,
    Character_ISD_Gosegu                = 1005,
    Character_ISD_Viichan               = 1006,
    Character_WAK_Wakgood               = 2001,
    Character_WAK_Roentgenium           = 2002,
    Character_Productor_Temp1           = 3001,
    Character_Productor_Temp2           = 3002,
    Character_Productor_Temp3           = 3003,
    Character_Productor_Temp4           = 3004,
    Character_Productor_Temp5           = 3005,
    Character_Productor_Temp6           = 3006,
} 

//위의 ResourceID + Type값 -> 리소스 데이터
//값이 너무 커져서 비트 플래그 연산으로 변경 필요
//bitarray 아니면 자체적으로 만들어서 쓰거나 할 듯
public enum ResourceType : long
{
    Item                                = 100,
    Stat                                = 200,
    ISD                                 = 1000,
    WAK                                 = 2000,
    Productor                           = 3000,
    TypeCount                           = 5,

    DefaultSprite                       = 10000,   
    StandingSprite                      = 10000,      
    SitFrontSprite                      = 20000,
    SitBackSprite                       = 30000,
    SpriteCount                         = 3,

    WalkAni                             = 100000,
    FrontWorkAni                        = 200000,
    BackWorkAni                         = 210000,
    StandingIdleAni                     = 300000,
    FrontIdleStretchingAni              = 400000,
    FrontIdleLookAroundAni              = 410000,
    BackIdleStretchingAni               = 420000,
    BackIdleLookAroundAni               = 430000,
    SitAni                              = 500000,
    InteractiveAni                      = 600000,         
    AniCount                            = 10,
}

public enum ResourceFileName 
{   
    DefaultSprite, SitBack, SitFront, Standing, //Sprite 
    BackIdleLookAround, BackIdleStretching, BackWork, FrontIdleLookAround, FrontIdleStretching, FrontWork, //Work Ani File
    WalkAni, StandingIdleAni, WorkAni, SitAni, InteractiveAni, //Ani Root File
};

public enum CommentType {ToIne, ToJingBurger, ToLilpa, ToJururu, ToGosegu, ToViichan, Count,};

// [Flags]
// public enum ResourceTypeBit
// {
//     Item                                = 1 << 2,
//     ISD                                 = 1 << 3,
//     WAK                                 = 1 << 4,
//     Productor                           = 1 << 5,
//     DefaultSprite                       = 1 << 6,         
//     SitSprite                           = 1 << 7,
//     WalkAni                             = 1 << 8,       
//     WorkAni                             = 1 << 9,
//     IdleAni                             = 1 << 10,
//     SitAni                              = 1 << 11,
// }


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
    public System.Collections.Generic.List<CharacterData> CharacterData;
    public System.Collections.Generic.List<CharacterComment> CharacterComment;
    public System.Collections.Generic.List<GameHelpInfo> GameHelpInfo;
    public System.Collections.Generic.List<ResourcesTable> ResourcesTable;
}

//플레이어의 데이터
[System.Serializable]
public class PlayerData
{
    public string UserName;
    public string UserID;
    public int Money;
    public int TimeElapsed;
}

[System.Serializable]
public class CharacterData
{
    public string Name;                 //이름
    public string IsIsegyeIdol;         //이세돌인지
    public string isFieldCharacter;     //필드에 나와있는 캐릭터인지
    public int SeatNumber;              //좌석 번호
    public int CharacterID;             //캐릭터 고유번호(저장 및 리소스 임포트에 사용)
    public int RemainFeverCount;        //저장 시 피버 상태였던 캐릭터의 남은 피버 수(남은 수 * 100 + 최대치로 저장)
                                        //(ex. 25번 피버 중 12번 피버를 했다 -> 1325)
}

[System.Serializable]
public class CharacterComment
{
    public long ID;
    public string GotoWork;
    public string LeaveWork;
    public string Signature;
    public string ToIne;
    public string ToJingBurger;
    public string ToLilpa;
    public string ToJururu;
    public string ToGosegu;
    public string ToViichan;
}

[System.Serializable]
public class GameHelpInfo
{
    public string Title;        //텝 이름
    public string Info;         //정보
}

[System.Serializable]
public class ResourcesTable
{
    public string Key;                          //리소스 키 값
    //public string Info;                       //정보
    public int ID;                              //리소스 ID 저장할 때 쓰는 값
    public long SpriteID;                       //Sprite ID
    public long SitFrontID;                     //정면 앉은 도트
    public long SitBackID;                      //뒷면 앉은 도트
    public long WalkAniID;                      //걷는 ani
    public long FrontWorkAniID;                 //정면 일하는 ani
    public long BackWorkAniID;                  //뒷면 일하는 ani
    public long StandingIdleAniID;              //서있는 idle ani
    public long SitFrontIdleStretchingAniID;    //앉은 정면 기지개 ani
    public long SitFrontIdleLookAroundAniID;    //앉은 정면 두리번 ani
    public long SitBackIdleStretchingAniID;     //앉은 뒷면 기지개 ani
    public long SitBackIdleLookAroundAniID;     //앉은 뒷면 두리번 ani
    public long SittingAniID;                   //앉는 ani
    public long InteractiveAniID;               //상호작용 ani
}

//키워드의 매칭률
[System.Serializable]
public class MatchingData
{
    public string Kategorie;         //컨텐츠 키워드
    public string Type;             //타입 키워드
    public int MatchingPoint;       //매칭률 1~5(눕, 계륵, 프로, 국밥, 해커)
    public string Unlocked;         //해금된 조합인지(O, X)
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
}    

[System.Serializable]
public class Keyword
{
    public string KeywordKey;       //enum타입으로 사용할 키워드 영어버전
    public string KoreanName;       //해당 영어 한국어 버전
    public string Type;             //컨텐츠인지 종류인지 
    public int Popularity;          //해당 키워드의 인기도
}