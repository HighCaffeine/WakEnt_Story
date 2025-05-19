using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//임의로 몇 개 세팅하고 나머지는 datatable 연결해서 진행
[CreateAssetMenu(fileName = "NewGear", menuName = "Gear/CreateNewGear")]
public class GearInfo : ScriptableObject
{
    public Sprite gearImage;        // 기어 이미지

    public string gearName;         // 기어 이름
    public string releaseData;      // 출시일 (##년:##월:##주)
    public string company;          // 업체명

    [Range(1.00f, 5.00f)]public float priceRatio;        // 키워드 비용 배율 (1.00f)

    public int preferenceValue;     // 선호 수치
    public bool isUnlocked;         // 언락됐는지?
    public bool isBought;           // 구매했는지?

    public int usePrice;            // 사용 가격
    public int buyPrice;            // 구매 가격 

    public int count;               // 제작 횟수
}
