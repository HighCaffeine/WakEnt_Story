using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Devcat;

public class ProductorManager : GenericSingleton<ProductorManager>
{
    public const int PRODUCTORMAXSTAT = 999;
    public const int MINFEVERCOUNT = 3;
    public const int MAXFEVERCOUNT = 30;

    public enum ProductorType
    {
        Planner,    //기획자  (기획자는 왁굳님이지만 세부 기획으로 들어가는 작업자를 뜻 함)
        GraphicDesigner,  //맵 제작자 (컨텐츠에 해당하는 맵을 제작하는 작업자)
        SoundDesigner,   //작곡가 (방송에 쓰일 곡을 작곡하는 작업자) -> 해당 작곡가의 곡을 사용하면 좋겠지만 어려울 듯
        Marketer,   //공지 및 홍보 담당 (방송 컨텐츠를 왁물원에 정리 및 홍보하는 역할)
        Count,
    }

    public enum ProcessingType
    {
        Planning,       //제작 진행 0%일 경우
        Quality,        //제작 진행 40%일 경우
        Compose,        //제작 진행 80%일 경우
        Count,          
    }

    private float productorScore;

    //작업 프로세스는 단계는 3단계만 사용 예정임
    //현재는 작업자 종류만큼 4단계 사용으로 되어있어서 작업자 타입으로 타입을 해놨는데
    //새로 만든 프로세스타입 으로 타입변경 후 변환 사용해야 함.
    //-> 기존 현재 진행 타입과, 선택 작업자의   작업 타입 비교부분에서 문제가 생김.
    [SerializeField] [Tooltip("현재 방송 제작 단계")] private ProcessingType currentProcessProductorType;

    //Devcat namespace안에 있는 Dictionary로 Enum을 int로 가지고 있음으로써, 명시적 형변환으로 인한 Boxing이 일어나지 않도록 함.
    private ProductorType CurrentProductorType => ValueCastTo<ProductorType>.From(ValueCastTo<int>.From(currentProcessProductorType));

    //작업자 상태창
    [Space(10f)]
    [Header("Productor Status")]
    [SerializeField] private Image productorImage;
    [SerializeField] private TextMeshProUGUI productorName;
    [Tooltip("작업자 스텟값 부모")][SerializeField] private GameObject productorStatObj;
    
    /// <summary>
    /// Index
    /// 0   기획
    /// 1   맵
    /// 2   작곡
    /// 3   홍보
    /// </summary>
    [SerializeField] private List<TextMeshProUGUI> productorStats;   
    [SerializeField] private TextMeshProUGUI productorLevel;
    [SerializeField] private TextMeshProUGUI productorPrice;
    [SerializeField] private Image productorStemina;

    /// <summary>
    /// 작업자가 직전 작업에 참여했을 경우에 스텟치 감소적용
    /// </summary>
    [Tooltip("작업자 이전 작업 유무")][SerializeField] private TextMeshProUGUI previousBroadcastProduction;
    [SerializeField] private GameObject previousBroadcastObj;

    [SerializeField] private TextMeshProUGUI companyMember;
    [SerializeField] private TextMeshProUGUI infoText;
    
    [SerializeField] private List<ProductorInfo> productorInfos;        //datamanager 통해서 읽어올 거임.
    private List<ProductorInfoStatusData> productorStatusList;

    [SerializeField] private ProductorInfoStatusData currentStatusData;



    [Space(10f)][Header("작업")]
    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private Image processingProductorImage;
    [SerializeField] private TextMeshProUGUI productorMessage;

    

    //작업자 이미지 ㅇ
    //작업자 이름 ㅇ
    //작업자 스텟 ㅇ
    //작업자 레벨   직군 : 기획자                                    LV.5
    //              string format 직군 : {0}                                    LV.{1}, productorType, level
    //작업자 비용 - 자사일 경우 무료
    //작업자 작업 중복여부
    //자사인지 아닌지
    //선택, 직전/다음 작업자 버튼
    // 정보

    private new void Awake()
    {
        base.Awake();

        productorStats = new List<TextMeshProUGUI>();

        productorInfos = new List<ProductorInfo>();
        productorStatusList = new List<ProductorInfoStatusData>();

        for (int i = 0; i < productorStatObj.transform.childCount; i++)
        {
            TextMeshProUGUI text = productorStatObj.transform.GetChild(i).Find("Value").GetComponent<TextMeshProUGUI>();

            productorStats.Add(text);
        }
        DataManager.Instance.SetProductorInfo(productorInfos);

        SetProductorStatusList();       //각 데이터의 NextInfo값 넣고 list에 추가
        productorStatusList.Reverse();  //앞 뒤 바뀐 상태라 reverse
        SetProductorPreviousData();     //Previous값 넣어주기

        OpenProductorSelection();

        isRun = true;
    }

    bool isRun = false;

    public int TEST_MoveToNextProcessing()
    {
        if (currentProcessProductorType >= ProcessingType.Count)
        {
            return ValueCastTo<int>.From(ProcessingType.Count);
        }

        currentProcessProductorType++;

        return ValueCastTo<int>.From(currentProcessProductorType);
    }

    public interface ProductorFieldProcessValue
    {
        public void SetProductorFieldProcess(OnProductorFieldProcess OnProductorFieldProcess);
    }

    public delegate int OnProductorFieldProcess(ProductorInfo info);

    public interface ProductorStatRequest
    {
        public void SetProductorStatRequest(OnProductorStatRequest OnProductorStatRequest);
    }

    private UnityEngine.Events.UnityEvent checkObjEvent;        //화면에 확인용 체크표시 띄우기 위한 이벤트
    public void RegisterCheckObjEvent(UnityEngine.Events.UnityAction unityEvent)
    {
        checkObjEvent.AddListener (() => unityEvent.Invoke());
    }

    //작업자 선택 후 제작 시작하면 각 작업자 스크립트에서 매니저로 값 넘겨주는걸로
    //델리게이트 호출 시 작업자가 본인 값을 그냥 넘겨줘서 여기
    public delegate void OnProductorStatRequest(ProductorInfo info);

    [SerializeField] [System.Serializable]
    public class ProductorInfoStatusData
    {
        public ProductorInfo productorInfo;
        public ProductorInfoStatusData previousInfo;
        public ProductorInfoStatusData nextInfo;
        public ProductorInfoStatusData(ProductorInfoStatusData previousInfo, ProductorInfo productorInfo, ProductorInfoStatusData nextInfo)
        {
            this.productorInfo = productorInfo;
            this.previousInfo = previousInfo;
            this.nextInfo = nextInfo;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isPrevious">true = 이전 작업자, false = 다음 작업자</param>
    public void ChangeProductorMethod(bool isPrevious)
    {
        ProductorInfoStatusData newData;
        ProductorInfoStatusData currentNewData = currentStatusData;

        while (true)
        {
            newData = isPrevious ? currentNewData.previousInfo : currentNewData.nextInfo;

            if (newData == null)
            {
                return;
            }

            currentNewData = newData;

            if (newData.productorInfo.GetProductorType() != CurrentProductorType)
            {
                continue;
            }
            else
            {
                currentStatusData = newData;
            }

            break;
        }

        UpdateProductorStatus(currentStatusData.productorInfo);
    }

    private void ChangeProductorMethodMoveToFirst(bool isPrevious)
    {
        ProductorInfoStatusData newData = currentStatusData;

        while (isPrevious ? newData.previousInfo != null : newData.nextInfo != null)
        {
            newData = isPrevious ? currentStatusData.previousInfo : currentStatusData.nextInfo;

            currentStatusData = newData;

            if (newData.productorInfo.GetProductorType() != CurrentProductorType)
            {
                continue;
            }

            break;
        }

        if (newData.productorInfo.GetProductorType() != CurrentProductorType)
        {
            ChangeProductorMethodMoveToFirst(false);
        }
        else
        {
            UpdateProductorStatus(newData.productorInfo);
        }
    }

    //작업자 선택 시 첫 작업자로 세팅
    public void UpdateMoveToFirstProductor()
    {
        ChangeProductorMethodMoveToFirst(true);
    }

    public void OpenProductorSelection()
    {
        foreach (var data in productorStatusList)
        {
            if (CurrentProductorType == data.productorInfo.GetProductorType())
            {
                UpdateProductorStatus(data.productorInfo);

                currentStatusData = data;

                break;
            }
        }
    }



    private ProductorInfoStatusData SetProductorStatusList(int index = 0)
    {
        if (index == productorInfos.Count)
        {
            return null;
        }

        ProductorInfoStatusData newData = new ProductorInfoStatusData(null, productorInfos[index++], SetProductorStatusList(index));

        productorStatusList.Add(newData);

        return newData;
    }

    //작업자의 이전 작업자 세팅
    private void SetProductorPreviousData()
    {
        foreach (var data in productorStatusList)
        {
            if (data.nextInfo == null)
            {
                break;
            }

            data.nextInfo.previousInfo = data;
        }
    }

    //작업자 화면 값 업데이트
    private void UpdateProductorStatus(ProductorInfo info)
    {
        CalculateProductorStat(info);                                   //작업자 스텟값 반영
        //productorImage.sprite = info.productorImage;                    //작업자 이미지
        productorName.text = info.GetName();                        //작업자 이름
        UpdateProductorStat(info);                                      //작업자 스텟값 4개
        UpdateProductorLevel(info);                                     //작업자 레벨
        productorPrice.text = info.GetPrice().ToString();                    //작업자 비용
        ProductorPreviousPlanningCheck(info);                           //작업자 이전 작업 유무
        companyMember.text = info.isCompanyMember ? "작업계" : "외주";   //회사맴
        infoText.text = info.GetInfo();                                      //정보
        productorStemina.fillAmount = info.currentStemina / 100;

        if (isRun)
        {
            string[] resourceName = ValueCastTo<ResourceID>.From(info.GetID()).ToString().Split('_');
            SpriteAnimation.Instance.PlayAnimation(resourceName[2], true);
        }
    }

    private void UpdateProductorLevel(ProductorInfo info)
    {
        string job = null;

        switch (CurrentProductorType)
        {
            case ProductorType.Planner:
            job = "기획자";
            break;
            case ProductorType.GraphicDesigner:
            job = "맵 제작자";
            break;
            case ProductorType.SoundDesigner:
            job = "작곡가";
            break;
            case ProductorType.Marketer:
            job = "홍보 담당";
            break;
        }
        
        productorLevel.text = string.Format("직군 : {0}                                     LV.{1}", 
                                                    job, info.productorLevel[CurrentProductorType]);
    }

    private void UpdateProductorStat(ProductorInfo info)
    {
        for (int i = 0; i < productorStats.Count; i++)
        {
            productorStats[i].text = info.productorStat[ProductorType.Planner + i].ToString();
        }
    }

    private void ProductorPreviousPlanningCheck(ProductorInfo info)
    {
        if (info.previousBroadcastProduction)
        {
            previousBroadcastObj.SetActive(true);

            previousBroadcastProduction.text = "또작업";

            return;
        }

        previousBroadcastObj.SetActive(false);
    }

    //임시로 0.3씩 값 
    public void CalculateProductorStat(ProductorInfo info)
    {
        productorScore = 0.0f;

        float multiplier = 0.7f;

        if (CurrentProductorType == info.GetProductorType())
        {
            multiplier = 1.3f;
        }

        productorScore += info.productorStat[CurrentProductorType] * multiplier;
    }

    //작업자 화면 값 업데이트

    //작업자 선택 버튼에 연동
    public void SelectedProductor()
    {
        //float matchingRate = BroadCastPlanning.Instance.GetCurrentMatchingRate();
        MenuController.Instance.OpenProductorWorkProcess();

        ProductorGetWorkProcess();
    }

    //작업자 선택 후 작업자 추가 스텟 진행창
    private void ProductorGetWorkProcess()
    {
        //matchingRate는 0(첫 시도), 1(눕), 2(계륵), 3(프로), 4(국밥), 5(해커)로 되어있음

        StartCoroutine(WorkProcess());
    }

    [Space(10f)][Header("단계 진행률, 초회는 100퍼")][Tooltip("초회는 100퍼")][SerializeField] private float[] processRate; //각 단계별로 진행할 기본 확률 

    //작업 진행은 작업자 스텟 및 여러가지 계산으로 됨
    //Stemina  -> 스테미너에 따라 단게별 진행 확률 변동
    //현재 작업단계에 해당하는 작업자의 스텟
    //현재 작업단계 외에 해당하는 작업자의 스텟 (현재는 같은 타입의 작업자만 선택 가능함) -> 이런식으로 할 경우 기획 + 맵에디터식의 직업을 만들어야 함
    //작업자가 이전 작업을 참여했는지 -> 단순하게 감소할지 지속적으로 참여할 경우 계속 감소
    //매칭률의 경우 작업자의 스텟에 추가가 들어감 
    //         -> 현재 매칭률은 언락된 거 기준으로만 등록하게 되어있는데, 모든 데이터를 가지고 있되, 언락 유무로 첫 시도로 표시할지 안할지로 변경
    //1(눕), 2(계륵), 3(프로), 4(국밥), 5(해커)  
    //50    70        100      110      130*
    //얘가 스텟 추가 이벤트
    //받은 배율 값으로 결과값 및 다음단계 진행 확률 계산
    //아이콘들 따로 코드 만들고 풀링

    //     ProductorManager productor선택하면 broadcastmanager가 매칭 값에 대한 보너스 배율넘겨줘서
    // ProductorInfo값으로 작업 프로세스 돌린 결과 값을 broadcastmanager에게 다시 넘겨줘서 제작으로 들어감
    // 이후 productor선택후 프로세스 결과 값은 broadcastmanger에게 넘겨서 진행중인 제작에 추가점수로 들어감

    private bool isProcessed = false;
    CheckTime checkTime;
    private IEnumerator WorkProcess()
    {
        BroadCastPlanning.Instance.UpdateBroadcastPoint();

        ProductorInfo info = currentStatusData.productorInfo;
        //processingProductorImage.sprite = info.productorImage;
        //sprite animation 실행

        isProcessed = false;

        int processStep = 0;
        
        float[] newProcessRate = CalculateStemina(info);    //스테미나 패널티

        float processMulti = info.previousBroadcastProduction == true ? 0.7f : 1.0f; //이전 작업 유무 패널티
        //float matchingRateMulti = GetMatchingRateMulti(BroadCastPlanning.Instance.GetCurrentMatchingRate());
        float[] productorStats = new float[4];
        //float processingMulti = processMulti * matchingRateMulti;

        float processingMulti = 1.0f;

        float processingDelay = 0.0f;
        float processingTime = 0.0f;
        float randomLength = info.AllStat;

        for (int i = 0; i < productorStats.Length; i++)
        {
            productorStats[i] = info.productorStat[ProductorType.Planner + i];
        }

        checkTime = new CheckTime();

        StatIconManager.Instance.SetInitStat();
        SetTitle(1);
        

        //ProcessingMessage(BroadCastPlanning.Instance.GetCurrentKategorie() + "방송 제작 알잘딱하게");
        
        //GameScene에서 Processing종료 후 플레이어가 확인 후 SOundmanager에게 replay요청

        bool isFirst = true;

        
        while (true)
        {
            //각 단계별로 확률 체크
            if (processStep >= newProcessRate.Length)
            {
                break;
            }
            else if (CalculatePercentage(newProcessRate[processStep]))
            {   
                processStep++;

                checkTime.isOverTime = false;

                processingTime = GetProcessingTime(info, processingMulti);
                processingDelay = GetProcessingDelay(info, processingMulti);
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(2f);

            if (SoundManager.Instance != null && isFirst) 
            {
                SoundManager.Instance.PlaySound(SoundManager.BGM.BGM_Processing_1.ToString(), false, true);

                isFirst = false;
            }

            StartCoroutine(TimeCheck(processingTime, checkTime));

            ProcessingMessage("ㄷㄱㅈ");
            SetTitle(processStep);
            
            while (true)
            {
                if (checkTime.isOverTime)
                {
                    checkTime.isOverTime = false;
                    break;
                }

                //작업자 최종 스텟으로 나온 값들을 수치화
                //기획 10, 맵 40, 곡 50, 홍보 10을 기준으로
                //기획 1~10, 맵 11~50, 곡 51~90, 홍보 91~100
                //Random.Range(1, 4개 스텟 합)에서 구간 값을 해당되는 스텟들로 하고 
                //랜덤값을 뽑아서 최종 작업 스텟으로 추가
                //시간 조정, 딜레이 조정으로 하기로 하고
                //시간값 영향 -> 스테미나만 영향 받는걸로
                //딜레이 영향 -> 스테미나, 해당 작업 단게의 스텟(전체 스텟치 / 작업자 스텟치)

                StatIconManager.Instance.AddStatIcon(ProductorType.Planner + CalculateWorkProcess(info));

                yield return new WaitForSeconds(processingDelay);
            }
        }

        //현재 기획중인 방송에 스텟 추가 및 
        //게임 화면 하단에 현재 기획중이 방송 스텟 정보 창 추가
        //맵에 작업자들 작업
        productorMessage.text = string.Format("{0}", info.GetProcessCompleteComment());
        if (SoundManager.Instance != null) SoundManager.Instance.EndMultiAudio();

        isProcessed = true;
        checkObjEvent?.Invoke();
    }

    private void GetBackPrevious(ProductorInfoStatusData productorInfoStatusData)
    {
        if (productorInfoStatusData.previousInfo != null)
        {
            productorInfoStatusData.previousInfo.productorInfo.previousBroadcastProduction = false;

            GetBackPrevious(productorInfoStatusData.previousInfo);

            return;
        }
        else
        {
            if (productorInfoStatusData != currentStatusData)
            {
                productorInfoStatusData.nextInfo.productorInfo.previousBroadcastProduction = false;
            }

            GetBackPrevious(productorInfoStatusData.nextInfo);

            return;
        }
    }

    public void ConfirmProcessing()
    {
        if (!isProcessed)
        {
            return;
        }

        //작업자 고르고 프로세스 끝난 뒤 broadcastplanning한테 방송 시작 알림.  이벤트로 넘길거 나중에
        //하단 함수들 전부 BroadcastPlanningEvent로 추가해서 foreach 
        CharacterManager.Instance.CallBackEvent(CharacterManager.CharacterEventType.IsBroadcastPlanning);

        BroadCastPlanning.Instance.SetBroadcastPlanning();

        ProcessStatus.Instance.OpenPlanningPoint();
        MenuController.Instance.CloseProductorWorkProcess();

        if (SoundManager.Instance != null) SoundManager.Instance.ReplayAudio();
        ProcessedPointAddToBroadcast();

        BroadCastPlanning.Instance.UpdateBroadcastPoint();

        MenuController.Instance.CloseAllMenu();         //확인해야함 정상작동하는지 모든 켜진 메뉴를 다 꺼야해서 
        //MenuController.Instance.CloseOtherMenu();
        if (SoundManager.Instance) SoundManager.Instance.ReplayAudio();
    }

    private void ProcessedPointAddToBroadcast()
    {
        //broadcastmanager에게 결과로 나온 값 받아서 진행중인 방송 기획에 추가
        int[] processedPoint = StatIconManager.Instance.GetProcessingPoint();

        BroadCastPlanning.Instance.CalculateProcessingData(processedPoint);
    }

    private void SetTitle(int step)
    {
        string value = null;

        switch (CurrentProductorType)
        {
            case ProductorType.Planner:
            value = "기획";
            break;
            case ProductorType.GraphicDesigner:
            value = "방송 맵 제작";
            break;
            case ProductorType.SoundDesigner:
            value = "곡";
            break;
            case ProductorType.Marketer:
            value = "홍보";
            break;
        }

        title.text = string.Format("{0}작업 {1}회", value, step);
    }

    private void ProcessingMessage(string value)
    {
        productorMessage.text = value;
    }

    private class CheckTime
    {
        public bool isOverTime;
    }

    private IEnumerator TimeCheck(float time, CheckTime checkTime)
    {
        yield return new WaitForSeconds(time);

        checkTime.isOverTime = true;
    }

    // 선택된 작업자의 현재스테미나 / 최대 스테미나 비율만큼 패널티 부여 로직
    private float[] CalculateStemina(ProductorInfo info)
    {
        float[] newRateArray = processRate;
        float currentSteminaRate = info.currentStemina * 0.01f;

        for (int i = 1; i < newRateArray.Length; i++)
        {
            float rate = newRateArray[i];

            rate = Mathf.Clamp(rate * currentSteminaRate, 0, 100);

            newRateArray[i] = rate;
        }

        return newRateArray;
    }

    //StatmaxValue = 3996
    //딜레이 값 최소 0.001, 최대 0.2
    //시간 값 최소 3 최대 14
    private float GetProcessingDelay(ProductorInfo info, float multi)
    {
        int firstValue = (PRODUCTORMAXSTAT * 4 + 1) / info.AllStat;
        float secondValue = (5 + Mathf.Sqrt(PRODUCTORMAXSTAT) - Mathf.Sqrt(info.AllStat)) / PRODUCTORMAXSTAT;

        float value = Mathf.Clamp(firstValue * secondValue * multi * 0.1f, 0.001f, 2f);

        //Debug.Log(value);

        return value;
    }
    private float GetProcessingTime(ProductorInfo info, float multi)
    {
        float firstValue = Mathf.Log10(100 - (5 * info.fatigueLevel)) - Mathf.Log10(1 + info.fatigueLevel);
        float secondValue = Mathf.Sqrt(100 - (5 * info.fatigueLevel));

        float value = Mathf.Clamp(firstValue * secondValue * multi * 0.1f, 1f, 14f);

        //Debug.Log(value);

        return value;
    }
    
    private int CalculateWorkProcess(ProductorInfo info)
    {
        int maxValue = info.AllStat;
        int randomValue = UnityEngine.Random.Range(1, Mathf.RoundToInt(maxValue));

        int statAdd = 0;
        int index = 0;

        for (var i = ProductorType.Planner; i < ProductorType.Count; i++)
        {
            statAdd += Mathf.RoundToInt(info.productorStat[i]);

            if (randomValue <= statAdd)
            {
                return index;
            }

            index++;
        }

        return 0;
    }

    private float GetMatchingRateMulti(float matchingRate)
    {
        float matchingRateMulti = 1.0f;
        
        switch (matchingRate)
        {
            case 1:     //눕
            matchingRateMulti = 0.5f;
            break;
            case 2:     //계륵
            matchingRateMulti = 0.7f;
            break;
            case 3:     //프로
            matchingRateMulti = 1.0f;
            break;
            case 4:     //국밥
            matchingRateMulti = 1.1f;
            break;
            case 5:     //해커
            matchingRateMulti = 1.3f;
            break;
        }

        return matchingRateMulti;
    }

    /// <summary>
    /// 확률 계산 함수
    /// </summary>
    /// <param name="value">확률</param>
    /// <returns></returns>
    private bool CalculatePercentage(float value)
    {
        if (value == 0f)
        {
            return false;
        }

        int multi = 100;

        while (value <= 0f)
        {
            multi /= 10;
            value *= 10f;
        }

        int randomValue = UnityEngine.Random.Range(0, multi + 1);

        if (randomValue <= value)
        {
            return true;
        }

        return false;
    }

    //4가지 스텟 중 하나를 피버 효과로 
    //계산식 넣어서 수치 결정할 거
    private int ProductorFever(ProductorInfo info, int statIndex)
    {
        //특정 캐릭터 info 내부에 자리 번호 추가 예정임.
        //자리 번호를 CharacterManager에게 넘겨서 해당 캐릭터에게 fever효과 애니메이션 실행


        //최대 30까지 넘겨줄 거.
        //스텟치가 999인데 최대치 퍼센트 계산해서 하는걸로 최소치 3


        int feverCount = MINFEVERCOUNT;
        float percent = info.productorStat[ProductorType.Planner + statIndex] / PRODUCTORMAXSTAT * 100;

        float calPercent = Mathf.Clamp(percent, 5, 100);

        while (true)
        {
            if (!CalculatePercentage(calPercent))
            {
                break;
            }

            feverCount++;

            if (feverCount >= MAXFEVERCOUNT)
            {
                break;
            }
        }

        return feverCount;
    }

    /*
    총 작업량 값에 각 작업자들이 필드 작업을 할 경우 productormanager에게 값 전달 및 업데이트(delegate)
    시간 값으로 하면 계속 while 돌리다가 한명이라도 작업을 할 경우 return 예외처리 해야함
    */
    
    // public float GetFieldWorkMaxValue()
    // {
        
    // }


    public float[] ProcessLevel = {0.0f, 0.4f, 0.7f, 1.0f};

    // 계산 후 작업자의 총 작업량이 1이 넘으면 스텟 추가 이벤트
    // ProductorType.planner + 리턴값으로 ㅈㅁ
    // 아니면 0리턴 
    public int AddFieldProcessValueToManager(ProductorInfo info, int index)
    {
        if (info.ProcessedPoint >= 1.0f)
        {
            //스텟 추가

            info.InitProcessedPoint();

            CharacterManager.Instance.ReqPopupStat(index);

            return 0;
        }

        info.ProcessedPoint += 0.1f;      

        return 0;
    }

    public void AddStatFieldProcessing(int index)
    {
        AddFieldProcessValueToManager(productorStatusList[index].productorInfo, index);
    }

    public ProductorInfo GetProductorInfo(int index)
    {
        return productorStatusList[index].productorInfo;
    }

    //callback으로 스탯 몇 개 올리는지 넘겨줌

    public int AddStatFieldInteractive(int targetIndex)
    {
        if (targetIndex < 0 && productorInfos.Count <= targetIndex)
        {
            return -1;
        }


        if (targetIndex < 0)
        {
            return 0;
        }

        ProductorInfo targetCharacter = productorInfos[targetIndex];

        //추가할 스텟 랜덤으로 가져옴
        int addIndex = CalculateWorkProcess(targetCharacter);       //추가할 Index
        int addValue = ProductorFever(targetCharacter, addIndex);     //추가할 값

        //end
        return addValue;
    } 

    public void UpdateToBroadcast(int index, int amount)
    {
        int[] calValues = new int[ValueCastTo<int>.From(ProductorType.Count)];

        calValues[index] += amount;
        BroadCastPlanning.Instance.CalculateProcessingData(calValues);  //방송 스텟에 추가, 캐릭터 애니메이션이 끝나야 스텟 추가를 해야 함.
    }

    public ProductorType GetStatType(int targetIndex)
    {
        ProductorInfo targetCharacter = productorInfos[targetIndex];

        int statIndex = CalculateWorkProcess(targetCharacter);


        return ProductorType.Planner + statIndex;
    }
}
 