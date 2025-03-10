using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastKeywordSelection : ObjectPooling<BroadcastKeywordSelection, KeywordSelect>
{
    //broadcastplanning 창에서
    //키워드 선택 시 selection창 띄움
    //selection창 킬 때 
    //카테고리를 켰을 경우 컨텐츠 값이랑 매칭률, 반대로 컨텐츠를 켰을때는 카테고리와의 매칭률

    //데이터 매니저에게 각 멤버의 스킬레벨(키워드 숙련도)등 키워드 관련된 정보를 얻어와야함.
    //해당 함수 작성 후 풀링객체에게 델리게이트로 전달
    //델리게이트 필요사항
    //1. 멤버별 키워드 스킬 정보
    //2. 키워드의 인기정보 및 비용
    //3. 선택된 키워드와 다른 (카테고리 / 컨텐츠)와의 조합률

    private new void Awake()
    {
        base.Awake();
    }

    //public 
}
