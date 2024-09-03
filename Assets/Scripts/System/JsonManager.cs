using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;

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
        Debug.Log("===================================================");
        Debug.Log("data : " + data);

        Test_LoadJson();

        Test_DebugCount();

        Debug.Log("end TestLoadJson Method");
        Debug.Log("===================================================");
        Test_JsonDebug();

        Debug.Log("data : " + data);

        Test_DebugCount();
        
        //GetData();
    }

    void Start()
    {
        Debug.Log("===================================================");
        Debug.Log("JsonManager start");
        Debug.Log("data : " + data);

        Test_DebugCount();


        Invoke("Test_DebugCount", 10f);
    }




    /////////////////////////////////테스트///////////////////////////////////////////
    
    //StreamWriter sw;

    //얘 왜 빌드 때 안됨 내일 체크 ㄱㄱ
//     public void WriteBuildLog(string str)
//     {
// #if UNITY_EDITOR || UNITY_EDITOR || UNITY_IOS
//         string testLogPath = Path.Combine(Application.streamingAssetsPath, "BuildLog" + "." + Extension.txt.ToString());
// #else 
//         string testLogPath = Path.Combine(Application.persistentDataPath, "BuildLog" + "." + Extension.txt.ToString());
// #endif

//         if (str == null)
//         {
//             str = "asd";
//         }

//         FileStream fs;

//         if (!File.Exists(testLogPath))
//         {
//             fs = new FileStream(testLogPath, FileMode.CreateNew);
//         }
//         else
//         {
//             fs = new FileStream(testLogPath, FileMode.Append);
//         }

//         sw = new StreamWriter(fs);
//         sw.WriteLine(str);
//         sw.Close();
//     }


    public void Test_LoadJson()
    {
        path = Path.Combine(Application.persistentDataPath, fileName + "." + extension.ToString());
        string beforePath = Path.Combine(Application.streamingAssetsPath, fileName + "." + extension.ToString());

        if (!File.Exists(path))
        {
            System.IO.File.Copy(beforePath, path);
        }


        var jsonData = File.ReadAllText(path);
        data = JsonConvert.DeserializeObject<DataTable>(jsonData);
        //data = JsonUtility.FromJson<DataTable>(jsonData.ToString());
        Debug.Log(jsonData);

        Test_JsonDebug();
    }

    public void Test_JsonDebug()
    {
        Debug.Log("json out");

        Debug.Log("PlayerData");
        foreach (var item in data.PlayerData)
        {
            item.DebugLog();
        }

        Debug.Log("MatchingData");
        foreach (var item in data.MatchingData)
        {
            item.DebugLog();
        }

        Debug.Log("ReviewComment");
        foreach (var item in data.ReviewComment)
        {
            item.DebugLog();
        }

        Debug.Log("Event");
        foreach (var item in data.Event)
        {
            item.DebugLog();
        }

        Debug.Log("Keyword");
        foreach (var item in data.Keyword)
        {
            item.DebugLog();
        }
        Debug.Log("json test end");
    }

     /////////////////////////////////테스트///////////////////////////////////////////


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

        Debug.Log("add to review List");
        foreach (var value in data.ReviewComment)
        {
            returnList.Add(value);
        }

        Debug.Log("end of add to review list");

        return returnList;
    }

    public List<EventData> GetEventData()
    {
        return data.Event;
    }

    public PlayerData GetPlayerData()
    {
        Test_DebugCount();

        return data.PlayerData[0];
    }

    private void Test_DebugCount()
    {
        Debug.Log(Instance);

        Debug.Log("data : " + data);
        Debug.Log("Playerdata count : " + data.PlayerData.Count);
        Debug.Log("MatchingData count : " + data.MatchingData.Count);
        Debug.Log("ReviewComment count : " + data.ReviewComment.Count);
        Debug.Log("Event count : " + data.Event.Count);
        Debug.Log("Keyword count : " + data.Keyword.Count);
        Debug.Log("BroadcastRecord count : " + data.BroadcastRecord.Count);
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
