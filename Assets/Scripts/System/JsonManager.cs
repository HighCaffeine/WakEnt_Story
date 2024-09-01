using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public enum Extension
{
    txt,
    xml,
    json,
    mp3,
    wave,
    mp4,
    avi,
    mov,
}

public class JsonManager : GenericSingleton<JsonManager>
{
    [SerializeField] private DataTable data;
    [SerializeField] private string path;
    public Extension extension = Extension.json;
    public string fileName;


    private new void Awake()
    {
        base.Awake();

#if UNITY_EDITOR || UNITY_EDITOR || UNITY_IOS
        path = Path.Combine(Application.streamingAssetsPath, fileName + "." + extension.ToString());
#else 
        path = Path.Combine(Application.persistentDataPath, fileName + "." + extension.ToString());
#endif

        GetData();
    }

    private void GetData()
    {
        string jsonString = string.Empty;
#if UNITY_EDITOR || UNITY_IOS || UNITY_STANDALONE_WIN

        jsonString = File.ReadAllText(path);

#elif UNITY_ANDROID

        WWW www = new WWW(path);

        if (www.isDone)
        {
            Debug.Log("Downloaded");
        }

        yield return www;

        if (www.error != null)
        {
            throw new Exception("www downloaded : " + www.error);
        }

        jsonString = www.text;
#endif

        
        var textData = Resources.Load("ISDGameData") as TextAsset;

        data = JsonUtility.FromJson<DataTable>(textData.ToString());
    }

    public List<MatchingData> GetMatchingData()
    {
        return data.MatchingData;
    }

    public List<ReviewCommentData> GetReviewCommentData()
    {
        List<ReviewCommentData> returnList = new List<ReviewCommentData>();

        foreach (var value in data.ReviewComment)
        {
            returnList.Add(value);
        }

        return returnList;
    }

    public List<EventData> GetEventData()
    {
        return data.Event;
    }

    public PlayerData GetPlayerData()
    {
        return data.PlayerData[0];
    }

    public List<BroadcastRecord> GetBroadcastRecord()
    {
        return data.BroadcastRecord;
    }

    public List<Keyword> GetKeyword()
    {
        return data.Keyword;
    }
}
