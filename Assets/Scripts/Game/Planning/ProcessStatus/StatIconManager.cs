using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public class StatIconManager : ObjectPooling<StatIconManager, StatIcon>
{
    [SerializeField] private Transform dropStartYPos;

    [SerializeField] private RectTransform avatar;
    [SerializeField] private float reverseDelay;

    [Header("기획")]
    [SerializeField] private Transform plannerStartXPos; 
    [SerializeField] private TextMeshProUGUI plannerStat;
    [Header("맵")]
    [SerializeField] private Transform mapEditorStartXPos;
    [SerializeField] private TextMeshProUGUI mapEditorStat;
    [Header("작곡")]
    [SerializeField] private Transform composerStartXPos;
    [SerializeField] private TextMeshProUGUI composerStat;
    [Header("홍보")]
    [SerializeField] private Transform marketerStartXPos;
    [SerializeField] private TextMeshProUGUI marketerStat;

    private RectTransform rect;

    private new void Awake()
    {
        rect = plannerStartXPos.GetComponent<RectTransform>();

        base.Awake();
    }
    
    [Tooltip("1 : 기획, 2 : 맵, 3 : 사운드, 4 : 홍보")][SerializeField] private Sprite[] icons;
    
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
            case ProductorManager.ProductorType.MapEditor:
            startPos.x = mapEditorStartXPos.position.x;
            sprite = icons[1];
            break;
            case ProductorManager.ProductorType.Composer:
            startPos.x = composerStartXPos.position.x;
            sprite = icons[2];
            break;
            case ProductorManager.ProductorType.Marketer:
            startPos.x = marketerStartXPos.position.x;    
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
        mapEditorStat.text = string.Format("0");
        composerStat.text = string.Format("0");
        marketerStat.text = string.Format("0");
    }

    public void SetStat(ProductorManager.ProductorType type)
    {
        switch (type)
        {
            case ProductorManager.ProductorType.Planner:
            plannerStat.text = string.Format("{0}", int.Parse(plannerStat.text) + 1);
            break;
            case ProductorManager.ProductorType.MapEditor:
            mapEditorStat.text = string.Format("{0}", int.Parse(mapEditorStat.text) + 1);
            break;
            case ProductorManager.ProductorType.Composer:
            composerStat.text = string.Format("{0}", int.Parse(composerStat.text) + 1);
            break;
            case ProductorManager.ProductorType.Marketer:  
            marketerStat.text = string.Format("{0}", int.Parse(marketerStat.text) + 1);
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
        value[1] = int.Parse(mapEditorStat.text);
        value[2] = int.Parse(composerStat.text);
        value[3] = int.Parse(marketerStat.text);

        return value;
    }
}
