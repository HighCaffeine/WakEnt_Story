using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEvent : MonoBehaviour
{
    private enum InteractiveType
    {
        Environment,
        Character,
    }

    private enum InteractiveAniName
    {
        SeatMove,
        CharacterInteractive,
    }

    [SerializeField] private InteractiveType interactiveType;

    [Header("자체 사용 이벤트")]
    //사물의 경우 자체 애니메이션 함수 등록등 상호작용 시 본인이 할 것들
    [SerializeField] private List<UnityEngine.Events.UnityEvent> interactiveEventList;

    [SerializeField] private Animation interactiveAni;
    [SerializeField] private SpriteRenderer interactiveSpriteRender;

    void Awake()
    {
        interactiveAni = GetComponent<Animation>();
    }

    //callback -> 상호작용 이후 증가하는 요소들 넣어줌
    //targetAni -> 상호작용 시 애니메이션 동작하는 이벤트 넘겨줌, 자리가 바뀌게 될 수도 있으니 상호작용 때 마다 계속 넘겨주는걸로
    public void Interactive(bool isBroadcastPlanning, int targetIndex, out Action targetAni, params Action<int>[] callback)
    {
        targetAni= null;

        switch (interactiveType)
        {
            case InteractiveType.Environment:
            targetAni = PlayInteractiveAnimation;       //임시 등록
            EnvironmentInteractive(isBroadcastPlanning);
            break;
            case InteractiveType.Character:
            CharacterInteractive(isBroadcastPlanning, targetIndex, callback);
            break;
        }
    }

    private void CharacterInteractive(bool isBroadcastPlanning, int targetIndex, params Action<int>[] callback)
    {
        if (isBroadcastPlanning)
        {
            //방송제작중일 경우 방송 스텟 증가
            CharacterManager.Instance.AddBroadcastStat(targetIndex, callback);
            //메세지 이벤트랑은 별게라서 다시 callback넣어서 하는 게 나을수도 있을 듯.
        }
        else
        {

            //테스트
            CharacterManager.Instance.RequestPopupMessage("흠..", transform, null, null);
            //방송제작중이 아닐 경우 특수 재화 증가
            //캐릭터 매니저 통해서 broadcastplanning에게 추가 요청
        }
    }
    
    //캐릭터는 콜백받아서 

    private void EnvironmentInteractive(bool isBroadcastPlanning, params Action<int>[] callback)
    {
        //캐릭터의 체력을 회복하는 등 캐릭터 자체에 효과가 들어가게 됨

        if (isBroadcastPlanning)
        {  
            //제작중인 경우는 없는걸로 하는게 맞는 듯.
        }
        else
        {
            //캐릭터 상호작용 콜백
            //피로도 회복 또는 본인 자리 착석등 해줌
            //자리 넘버가 등록이 되어있을건데 해당 정보 넘겨줘야됨    
            //이세돌이 사용할경우 
            //콜백 내부에 정보 업데이트까지 넣어서 줌
        }
    }

    private void PlayInteractiveAnimation()
    {
        interactiveAni.Play();
    }

    //사물 ani, sprite

    //자체적으로 들고있는 이벤트들 호출 -> 특정 이벤트 발생 시 추가 스텟 발생 등
    //캐릭터가 넘겨주는 이벤트는 본인이 사용할 거-> 피버 효과 때 
    private void CallAllEvent()
    {
        foreach (var eventData in interactiveEventList)
        {
            eventData?.Invoke();
        }
    }

    /*
        이벤트 액션을 받고
        interactive발생 시 해당 액션 invoke


        Interactive세부 사항
        1. 캐릭터 - 사물
            시설 종류에 따른 능력치 증가
	            ㄴ 사물 코드 내부에서 매니저 요청으로 들어가는 이벤트 처리
				        (사물 -> 사물 매니저 -> 캐릭터 매니저 / 방송 매니저)

        2. 캐릭터 - 캐릭터
            방송제작중 O - 방송 스텟 +(현재 제작중인 단계 / 랜덤)
            방송제작중 X - 특수재화 +
	            ㄴ 캐릭터 코드 내부에서 매니저 요청을 들어가는 이벤트 처리
				        (캐릭터 -> 캐릭터 매니저 -> 방송 매니저)


        관련하여 추가할 것들
        방송 매니저가 특수 재화 관리필요 (타 매니저가 특수재화 + 요청도 할 수 있게)
    */
}
