using Devcat;
using System;
using UnityEngine;

public class ProductorInfo : ScriptableObject
{
    ProductorData productorData;

    public void TEST_SetID()
    {
        productorData.ID = (long)ResourceID.Character_Productor_Victory;
    }


    public void SetProductorData(ProductorData productorData)
    {
        this.productorData = productorData;

        //MAX -> ProductorManager.PRODUCTORMAXSTAT
        productorStat = new ProductorStat(productorData.PlannerStat, productorData.DesignStat, productorData.SoundStat, productorData.MarketerStat);
        productorLevel = new ProductorLevel(productorData.PlannerLevel, productorData.DesignLevel, productorData.SoundLevel, productorData.MarketerLevel);
    }

    public void InitStat(params int[] stat)
    {
        productorStat.Init(stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)],
                            stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)],
                            stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)],
                            stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)]);
    }
    public void InitLevel(params int[] stat)
    {
        productorLevel.Init(stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)],
                            stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)],
                            stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)],
                            stat[ValueCastTo<int>.From(ProductorManager.ProductorType.Planner)]);
    }

    public void InitSeat(int seatNum)
    {
        productorData.SeatNum = seatNum;

        isCompanyMember = seatNum != 0;
    }

    public string GetIDName()
    {
        return ((ResourceID)GetID()).ToString().Split('_')[2];
    }

    public string GetName() { return productorData.Name; }
    public long GetID() { return productorData.ID; }
    public ProductorManager.ProductorType GetProductorType()
    {
        ProductorManager.ProductorType type = ProductorManager.ProductorType.Count;

        //Planner, GraphicDesigner, SoundGraphicDesigner, Marketer
        switch (productorData.CharacterType)
        {
            case string str when str == ProductorManager.ProductorType.Planner.ToString():
                type = ProductorManager.ProductorType.Planner;
                break;
            case string str when str == ProductorManager.ProductorType.GraphicDesigner.ToString():
                type = ProductorManager.ProductorType.GraphicDesigner;
                break;
            case string str when str == ProductorManager.ProductorType.SoundDesigner.ToString():
                type = ProductorManager.ProductorType.SoundDesigner;
                break;
            case string str when str == ProductorManager.ProductorType.Marketer.ToString():
                type = ProductorManager.ProductorType.Marketer;
                break;
        }

        return type;
    }

    public string GetProcessCompleteComment() { return productorData.ProcessCompleteComment; }
    public string GetInfo() { return productorData.Info; }
    public int GetPrice() { return /*isCompanyMember ?*/ productorData.WorkPrice /*: productorData.OutsourcePrice*/; }

    //====
    public float ProcessedPoint
    {
        get { return processedPoint; }
        set { AddToPoint(value); }
    }
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

    public bool isCompanyMember;                        //회사 멤버인지
    public bool previousBroadcastProduction;            //이전 작업을 했는지

    public ProductorStat productorStat;
    public ProductorLevel productorLevel;


    [Header("피로도")][Range(0, 10)] public int fatigueLevel;

    public int currentStemina { get { return Mathf.Clamp(productorData.MaxStemina - fatigueLevel * 10, 0, 100); } private set { } }

    [Space(10f)][Multiline(3)] public string info;

    public int AllStat
    {
        get
        {
            return productorStat[ProductorManager.ProductorType.Planner]
                                        + productorStat[ProductorManager.ProductorType.GraphicDesigner]
                                        + productorStat[ProductorManager.ProductorType.SoundDesigner]
                                        + productorStat[ProductorManager.ProductorType.Marketer];
        }
    }

    [System.Serializable]
    public class ProductorLevel
    {
        private int plannerLevel;
        private int GraphicDesignerLevel;
        private int SoundDesignerLevel;
        private int MarketerLevel;

        public ProductorLevel(int plannerLevel, int GraphicDesignerLevel, int SoundDesignerLevel, int MarketerLevel)
        {
            Init(plannerLevel, GraphicDesignerLevel, SoundDesignerLevel, MarketerLevel);
        }

        public void Init(int plannerLevel, int GraphicDesignerLevel, int SoundDesignerLevel, int MarketerLevel)
        {
            this.plannerLevel = plannerLevel;
            this.GraphicDesignerLevel = GraphicDesignerLevel;
            this.SoundDesignerLevel = SoundDesignerLevel;
            this.MarketerLevel = MarketerLevel;
        }

        public int this[ProductorManager.ProductorType type]
        {
            get { return GetValue(type); }
        }

        private int GetValue(ProductorManager.ProductorType type)
        {
            switch (type)
            {
                case ProductorManager.ProductorType.Planner:
                    return plannerLevel;
                case ProductorManager.ProductorType.GraphicDesigner:
                    return GraphicDesignerLevel;
                case ProductorManager.ProductorType.SoundDesigner:
                    return SoundDesignerLevel;
                case ProductorManager.ProductorType.Marketer:
                    return MarketerLevel;
            }

            return 0;
        }
    }

    [System.Serializable]
    public class ProductorStat
    {
        private int plannerStat;
        private int GraphicDesignerStat;
        private int SoundDesignerStat;
        private int MarketerStat;

        public ProductorStat(int plannerStat, int GraphicDesignerStat, int SoundDesignerStat, int MarketerStat)
        {
            Init(plannerStat, GraphicDesignerStat, SoundDesignerStat, MarketerStat);
        }

        public void Init(int plannerStat, int GraphicDesignerStat, int SoundDesignerStat, int MarketerStat)
        {
            this.plannerStat = plannerStat;
            this.GraphicDesignerStat = GraphicDesignerStat;
            this.SoundDesignerStat = SoundDesignerStat;
            this.MarketerStat = MarketerStat;
        }

        public int this[ProductorManager.ProductorType type]
        {
            get { return GetValue(type); }
        }

        private int GetValue(ProductorManager.ProductorType type)
        {
            switch (type)
            {
                case ProductorManager.ProductorType.Planner:
                    return plannerStat;
                case ProductorManager.ProductorType.GraphicDesigner:
                    return GraphicDesignerStat;
                case ProductorManager.ProductorType.SoundDesigner:
                    return SoundDesignerStat;
                case ProductorManager.ProductorType.Marketer:
                    return MarketerStat;
            }

            return 0;
        }
    }
}
