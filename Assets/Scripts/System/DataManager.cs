using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : GenericSingleton<DataManager>
{

    private new void Awake()
    {
        base.Awake();

        //SetBroadcastValue(null);
    }

    private void OnEnable()
    {
        SetKategorieData();

    }
    
    //테스트 함수들 (예시임)
    public void SetBroadcastValue(Dictionary<string, float> broadCast)
    {
        string key = string.Format("BroadcasterTogether_" + "Game");
        float value = 5.0f;

        Debug.Log(broadCast);

        broadCast.Add(key, value);
    } 

    public void SetBroadcastMatching(List<string> matchingRateComment)
    {
        string[] matchings = { "해커", "국밥", "프로", "계륵", "눕" }; 

        foreach (var value  in matchings)
        {
            matchingRateComment.Add(value);
        }
    }



    private Dictionary<BroadCastPlanning.KategorieType, string[]> kategorieDatas = new Dictionary<BroadCastPlanning.KategorieType, string[]>();

    private void SetKategorieData()
    {
        //json데이터 읽는 거 추가 후 변경
        //초기 프로토타입으로 4개씩 임의로 설정하는 걸로
        //Gear
        string[] gears = { "기본", "VR", "트래커", "모션캡쳐" };
        
        //Content
        string[] contents = { "게임", "노래", "댄스", "토크" };

        //Type
        string[] types = { "개인", "합방", "시참", "대결" };

        kategorieDatas.Add(BroadCastPlanning.KategorieType.Gear, gears);
        kategorieDatas.Add(BroadCastPlanning.KategorieType.Content, contents);
        kategorieDatas.Add(BroadCastPlanning.KategorieType.Type, types);
    } 

    public string[] GetKategorieData(BroadCastPlanning.KategorieType kategorieType)
    {
        if (!kategorieDatas.ContainsKey(kategorieType))
        {
            return new string[] { "" };
        }

        return kategorieDatas[kategorieType];
    }
}
