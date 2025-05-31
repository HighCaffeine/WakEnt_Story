using Devcat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastGearSelection : GenericSingleton<BroadcastGearSelection>
{
    [SerializeField] private TMPro.TextMeshProUGUI gearName;            //장비 이름
    [SerializeField] private TMPro.TextMeshProUGUI realeaseData;        //출시일
    [SerializeField] private TMPro.TextMeshProUGUI company;             //업체
    [SerializeField] private TMPro.TextMeshProUGUI preferenceRatio;     //선호비율
    [SerializeField] private TMPro.TextMeshProUGUI price;
    [SerializeField] private TMPro.TextMeshProUGUI createCount;         //방송제작횟수

    [SerializeField] private TMPro.TextMeshProUGUI processText;         //선택, 구입버튼

    [SerializeField] private UnityEngine.UI.Image gearImage;


    private List<GearData> gearList;
    [SerializeField] private List<GearInfo> gearInfoList;

    public float gearPriceMultiRatio { get; private set; }

    private GearData currentGearData;

    private int totalPreferenceValue;

    public class GearData
    {
        public GearInfo gearInfo;
        public GearData previousData;
        public GearData nextData;

        public GearData(GearInfo gearInfo, GearData previousData, GearData nextData)
        {
            this.gearInfo = gearInfo;
            this.previousData = previousData;
            this.nextData = nextData;
        }
    }

    private new void Awake()
    {
        base.Awake();

        gearList = new List<GearData>();

        //gearInfoList = new List<GearInfo>();
        // gearInfoList 세팅 -> dataManager

        SetGearList();          //각 데이터의 NextInfo값 넣고 list에 추가
        gearList.Reverse();     //앞 뒤 바뀐 상태라 reverse
        SetPreviousData();      //Previous값 넣어주기

        gearPriceMultiRatio = 1.0f;
    }

    private GearData SetGearList(int index = 0)
    {
        if (index >= gearInfoList.Count)
        {
            return null;
        }

        GearData newData = new GearData(gearInfoList[index++], null, SetGearList(index));

        totalPreferenceValue += newData.gearInfo.preferenceValue;

        gearList.Add(newData);

        return newData;
    }

    private void SetPreviousData()
    {
        foreach (var data in gearList)
        {
            if (data.nextData == null)
            {
                break;
            }

            data.nextData.previousData = data;
        }
    }

    public void OpenGearSelection()
    {
        //첫 기어로 세팅
        UpdateGearData(gearInfoList[0]);
        currentGearData = gearList[0];
    }

    private void UpdateGearData(GearInfo info)
    {
        gearName.text = info.gearName;              //장비 이름

        string[] datas = info.releaseData.Split(':');

        gearImage.sprite = info.gearImage;      //이미지
        realeaseData.text = info.releaseData;   //출시일
        company.text = info.company;            //업체
        preferenceRatio.text = string.Format("{0}%", ValueCastTo<int>.From((100.0f * info.preferenceValue / totalPreferenceValue)));           //선호비율
        price.text = (info.isUnlocked ? info.usePrice : info.buyPrice).ToString();                          //가격
        createCount.text = info.count.ToString();                    //방송제작횟수

        SetProcessButtonText(info.isBought);
    }

    private void SetProcessButtonText(bool isBought)
    {
        processText.text = isBought ? "선택" : "구매";
        //processText.color = isBought ? Color.black : Color.red;
    }

    public void ChangeGearData(bool isPrevious)
    {
        GearData newData;

        newData = isPrevious ? currentGearData.previousData : currentGearData.nextData;

        currentGearData = newData;
        if (newData == null)
        {
            return;
        }
        
        UpdateGearData(newData.gearInfo);
    }

    public void Select()
    {
        if (currentGearData.gearInfo.isBought)
        {
            // 선택 broadcastplanning으로 전송

            string gearName = currentGearData.gearInfo.usePrice == 158 ? "그 긴거" : currentGearData.gearInfo.gearName;

            BroadCastPlanning.Instance.SetGearText(gearName);
            gearPriceMultiRatio = currentGearData.gearInfo.priceRatio;
            MenuController.Instance.MenuBack(); //메뉴 종료
        }
        else
        {
            // 구매 탭 킴 (info window에 출력)
            currentGearData.gearInfo.isBought = true;

            //window 매니저 만들어서 메세지 출력
        }
    }

}
