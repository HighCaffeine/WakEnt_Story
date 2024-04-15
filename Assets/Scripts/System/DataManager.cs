using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class DataManager : GenericSingleton<DataManager>
{

    private new void Awake()
    {
        base.Awake();

        //SetBroadcastValue(null);
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
}
