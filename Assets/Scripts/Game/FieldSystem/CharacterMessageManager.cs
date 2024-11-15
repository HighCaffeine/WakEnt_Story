using System;
using UnityEngine;

public class CharacterMessageManager : ObjectPooling<CharacterMessageManager, CharacterMessage>
{
    [Header("메세지 띄우는 시간")] [SerializeField] private float disappearTime;

    public float DisappearTime => disappearTime;

    //풀링으로 객체 관리
    //내부 함수로 PopUpMessage 만들어서 
    //charactermessage내부에서 캐릭터 객체 내부에 만든 message 객체
    //또는 위치 값을 받아와서 띄우는 방식으로 생각중.

    //메세지 자체는 charactermessage내부에서 켜고 끄고 딜레이 걸고 할거임.

    private new void Awake()
    {
        base.Awake();
    }


    //prefab 자체에서 위치 조정해서 캐릭터 위치로 두고
    //캐릭터가 본인이 메세지를 띄울려고 캐릭터매니저에게 요청해서 여기로 오는건데
    //그렇게 되면 메세지가 적용이 될 transform을 알고 있는 상태임.
    //

    //Text는 canvas상에서 존재함.
    //캐릭터는 2d 오브젝트라 별게의 위치를 사용함.
    //screentoworld convert 작업이 필요
    //  -> 이동중에도 메세지를 띄울경우 계속 따라다녀야 함.
    //      ㄴ 캐릭터가 charactermessage를 받아서 이동 시 실시간 위치 업데이트를 시켜줘야 할 듯.
    //      ㄴ 캐릭터가 이동중에 띄우지 않을거면 간단하게 첫 위ㅣㅊ에 띄우고 이동하기전에 빨리 없애는게 맞음.
    //          ㄴ캐릭터가 이동을 했다는건 할 일 다 했다는 거기 때문에 메세지가 띄워져 있는데 이동할 일은 없음.
    //          -> 캐릭터가 이동 중에 상호작용을 할 수도 있기 때문에 나눠둔거 -> 이동 중 이세돌분들끼리 만나면 서로 인사한다거나 그런 이벤트
    //캐릭터 매니저 내부에서 특정 캐릭터들에게 메세지 요청 (상호작용 시 텍스트 효과 등)
    //action

    
    public CharacterMessage SetMessage(string message, Transform target, CharacterManager.OnCharacterIsMove OnCharacterIsMove, params Action[] callback)
    {
        CharacterMessage messageObj = GetPool();

        messageObj.SetMessage(message, target, OnCharacterIsMove, callback);

        return messageObj;
    }

    //scaler값 곱해야할 듯
    public Vector2 MessagePosToScreenPos(Vector2 messagePos)
    {
        float width = Screen.width;
        float height = Screen.height;

        messagePos.x -= width * 0.5f;
        messagePos.y -= height * 0.5f;

        return messagePos;
    }



    //environment / character가 메세지 요청할 때 쓸 함수. 
    //매개변수로 리소스 아이디 넘겨줘서 받는걸로
    //아래에서는 datamanager에게 요청해서 받아오는걸로
    public string GetMessage(long id)
    {
        return null;
    }
}
