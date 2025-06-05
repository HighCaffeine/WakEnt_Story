using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public class StatIconManager : ObjectPooling<StatIconManager, StatIcon>
{
    [SerializeField] private Transform dropStartYPos;

    [SerializeField] private UnityEngine.UI.Image avatar;
    [SerializeField] private float reverseDelay;

    [Header("재미")]
    [SerializeField] private Transform plannerStartXPos; 
    [SerializeField] private TextMeshProUGUI plannerStat;
    [Header("퀄리티")]
    [SerializeField] private Transform GraphicDesignerStartXPos;
    [SerializeField] private TextMeshProUGUI GraphicDesignerStat;
    [Header("음향")]
    [SerializeField] private Transform SoundDesignerStartXPos;
    [SerializeField] private TextMeshProUGUI SoundDesignerStat;
    [Header("홍보")]
    [SerializeField] private Transform MarketerStartXPos;
    [SerializeField] private TextMeshProUGUI MarketerStat;

    private RectTransform rect;

    private new void Awake()
    {
        rect = plannerStartXPos.GetComponent<RectTransform>();

        base.Awake();
    }
    
    [Tooltip("1 : 기획, 2 : 맵, 3 : 사운드, 4 : 홍보")][SerializeField] private Sprite[] icons;

    public UnityEngine.UI.Image GetImageComponenet()
    {
        return avatar;
    }
    
    public void AddStatIcon(ProductorManager.ProductorType type)
    {
        StatIcon statIcon = GetPool();
        Vector2 startPos = Vector2.zero + new Vector2(dropStartYPos.position.x, dropStartYPos.position.y);
        
        statIcon.SetEndPos(new Vector2(0f, rect.position.y));

        Sprite sprite = null;

        switch (type)
        {
            case ProductorManager.ProductorType.Planner:
            startPos.x = plannerStartXPos.position.x;
            sprite = icons[0];
            break;
            case ProductorManager.ProductorType.GraphicDesigner:
            startPos.x = GraphicDesignerStartXPos.position.x;
            sprite = icons[1];
            break;
            case ProductorManager.ProductorType.SoundDesigner:
            startPos.x = SoundDesignerStartXPos.position.x;
            sprite = icons[2];
            break;
            case ProductorManager.ProductorType.Marketer:
            startPos.x = MarketerStartXPos.position.x;    
            sprite = icons[3];        
            break;
        }

        SetStat(type);

        statIcon.SetImage(sprite);

        statIcon.transform.position = startPos;
    }

    public void SetInitStat()
    {
        plannerStat.text = string.Format("0");
        GraphicDesignerStat.text = string.Format("0");
        SoundDesignerStat.text = string.Format("0");
        MarketerStat.text = string.Format("0");
    }

    public void SetStat(ProductorManager.ProductorType type)
    {
        switch (type)
        {
            case ProductorManager.ProductorType.Planner:
            plannerStat.text = string.Format("{0}", int.Parse(plannerStat.text) + 1);
            break;
            case ProductorManager.ProductorType.GraphicDesigner:
            GraphicDesignerStat.text = string.Format("{0}", int.Parse(GraphicDesignerStat.text) + 1);
            break;
            case ProductorManager.ProductorType.SoundDesigner:
            SoundDesignerStat.text = string.Format("{0}", int.Parse(SoundDesignerStat.text) + 1);
            break;
            case ProductorManager.ProductorType.Marketer:  
            MarketerStat.text = string.Format("{0}", int.Parse(MarketerStat.text) + 1);
            break;
        }
    }

    public Vector2 GetSize()
    {
        if (rect == null)
        {
            return Vector2.zero; 
        }

        return new Vector2(rect.rect.width, rect.rect.height);
    }

    public IEnumerator MovingBurger()
    {
        while (true)
        {
            Quaternion avatarRect = rect.rotation;

            avatarRect.y = avatarRect.y == 0 ? 180 : 0;

            yield return new WaitForSeconds(reverseDelay);
        }
    }

    public int[] GetProcessingPoint()
    {
        int[] value = new int[4];

        value[0] = int.Parse(plannerStat.text);
        value[1] = int.Parse(GraphicDesignerStat.text);
        value[2] = int.Parse(SoundDesignerStat.text);
        value[3] = int.Parse(MarketerStat.text);

        return value;
    }
}
