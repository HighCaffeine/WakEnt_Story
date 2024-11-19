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

    DefaultSprite                       = 10000,         //현재 Sprite는 아이네님 스탠딩 프로토타입으로 해둠.
    SitSprite                           = 20000,
    SpriteCount                         = 2,

    WalkAni                             = 100000,        //디롬님께서 주신 기본 캐릭터 Walk애니로 대체
    WorkAni                             = 200000,
    IdleAni                             = 300000,
    SitAni                              = 400000,
    AniCount                            = 4,
}

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
    public string CharacterType;    //ISD, WAK, Productor 타입
    public string Comment;          //타입별 멘트
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
    public string Key;          //리소스 키 값
    //public string Info;         //정보
    public int ID;              //리소스 ID 저장할 때 쓰는 값
    public long SpriteID;          //Sprite ID
    public long SitSpriteID;        //앉은 이미지
    public long WalkAniID;         //걷는 애니메이션 ID
    public long WorkAniID;         //작업 애니메이션 ID
    public long IdleAniID;          //Idle 애니메이션 ID
    public long SitAniID;
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