using UnityEngine;

[CreateAssetMenu(fileName = "NewProductor", menuName = "Productor/CreateNewProductor")]
public class ProductorInfo : ScriptableObject
{
    public string productorName;    //작업계 이름
    public Sprite productorImage;   //특정 작업자 이미지 (기본은 팬덤 이미지로 대체)
    public ProductorManager.ProductorType productorType;      //어떤 작업자인지(작업자 타입별로 해당 분야 보너스 존재)\

    public string productorProcessedMessage = "킹아";       //임시 값


    //====
    public float ProcessedPoint { get { return processedPoint; } 
                                    private set { AddToPoint(value); } }
    private float processedPoint;
    private void AddToPoint(float value)
    {
        processedPoint += value;
    }

    public void InitProcessedPoint()
    {
        processedPoint = 0.0f;
    }

    //====

    public bool isCompanyMember;
    public bool previousBroadcastProduction;

    public int price;

    public ProductorStat productorStat;
    public ProductorLevel productorLevel;

    
    [Header("피로도")] [Range(0, 10)] public int fatigueLevel;
    [Range(1, 100)] public int maxStemina;

    public int currentStemina  { get { return Mathf.Clamp(maxStemina - fatigueLevel * 10, 0, 100); } private set {} }

    [Space(10f)][Multiline(3)] public string info;

    public int AllStat { get { return productorStat[ProductorManager.ProductorType.Planner]
                                        + productorStat[ProductorManager.ProductorType.MapEditor]
                                        + productorStat[ProductorManager.ProductorType.Composer]
                                        + productorStat[ProductorManager.ProductorType.Marketer]; } }

    [System.Serializable]
    public class ProductorLevel
    {
        [Range(0, 5)] [SerializeField] private int plannerLevel;
        [Range(0, 5)] [SerializeField] private int mapEditorLevel;
        [Range(0, 5)] [SerializeField] private int composerLevel;
        [Range(0, 5)] [SerializeField] private int marketerLevel;

        public int this [ProductorManager.ProductorType type]
        {
            get { return GetValue(type); }
        }

        private int GetValue(ProductorManager.ProductorType type)
        {
            switch (type)
            {
                case ProductorManager.ProductorType.Planner:
                return plannerLevel;
                case ProductorManager.ProductorType.MapEditor:
                return mapEditorLevel;
                case ProductorManager.ProductorType.Composer:
                return composerLevel;
                case ProductorManager.ProductorType.Marketer:
                return marketerLevel;
            }

            return 0;
        }
    }

    [System.Serializable]
    public class ProductorStat
    {
        [Range(0, ProductorManager.PRODUCTORMAXSTAT)] [SerializeField] private short plannerStat;
        [Range(0, ProductorManager.PRODUCTORMAXSTAT)] [SerializeField] private short mapEditorStat;
        [Range(0, ProductorManager.PRODUCTORMAXSTAT)] [SerializeField] private short composerStat;
        [Range(0, ProductorManager.PRODUCTORMAXSTAT)] [SerializeField] private short marketerStat;

        public short this [ProductorManager.ProductorType type]
        {
            get { return GetValue(type); }
        }

        private short GetValue(ProductorManager.ProductorType type)
        {
            switch (type)
            {
                case ProductorManager.ProductorType.Planner:
                return plannerStat;
                case ProductorManager.ProductorType.MapEditor:
                return mapEditorStat;
                case ProductorManager.ProductorType.Composer:
                return composerStat;
                case ProductorManager.ProductorType.Marketer:
                return marketerStat;
            }

            return 0;
        }
    }
}
