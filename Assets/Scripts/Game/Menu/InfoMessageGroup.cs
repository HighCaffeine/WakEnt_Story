using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMessageGroup : ObjectPooling<InfoMessageGroup, InfoMessage>
{
    //메세지 프리팹으로 풀링진행
    //텍스트랑 같이 요청 받으면 해당 텍스트 적용 후 풀에서 가져와서 애니메이션 실행

    private int currentActiveMessage = 0;
    private List<InfoMessage> currentActiveMessages;

    [Tooltip("메세지 다시 올려보내기 까지 시간")]
    [SerializeField] private float moveDelayTime;

    [SerializeField] private RectTransform firstPos;
    [SerializeField] private RectTransform secondPos;

    public interface EndPopupMessage
    {
        public void SetOnEndPopupMessage(OnEndPopupMessage OnEndPopupMessage);
    }

    public delegate void OnEndPopupMessage();
    public interface GetTargetPosition
    {
        public void SetOnGetTargetPos(OnGetTargetPos OnGetTargetPos);
    }

    public delegate Vector2 OnGetTargetPos(RectTransform myRect);

    [SerializeField] private float messageTerm;

    private new void Awake()
    {
        base.Awake();

        currentActiveMessages = new List<InfoMessage>();

        float height = firstPos.anchoredPosition.y - secondPos.anchoredPosition.y;

        messageTerm = height + (height / 10);
    }


    //Test//
    public TMPro.TMP_InputField inputField;

    public void TestMessageMethod()
    {
        RequestMessage(inputField.text);
    }

    //Test//

    public void RequestMessage(string message)
    {
        foreach (var activeMessage in currentActiveMessages)
        {
            SetTargetPos(activeMessage);
        }

        currentActiveMessage++;                         //현재 활성화 메세지 갯수 추가

        InfoMessage infoMessage = GetPool();            //풀에서 메세지에 쓸 객체 가져옴

        infoMessage.SetFirstPos();
        currentActiveMessages.Add(infoMessage);         //리스트에 추가

        SetMessage(infoMessage, message);               //메세지 업데이트

        if (checkTimeToUpdateMessagePos != null)        
        {
            StopCoroutine(checkTimeToUpdateMessagePos); //코루틴 돌고있는지 확인 및 종료

            checkTimeToUpdateMessagePos = null;
        }

        checkTimeToUpdateMessagePos = StartCoroutine(CheckTimeToUpdateMessagePos());    //메세지 올리는 시간 체크
    }

    Coroutine checkTimeToUpdateMessagePos;

    private IEnumerator CheckTimeToUpdateMessagePos()
    {
        float time = 0.0f;

        while (true)
        {
            time += Time.deltaTime;

            if (time >= moveDelayTime)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        InitMessage();
    }

    private void InitMessage()
    {
        foreach (var message in currentActiveMessages)
        {
            message.InitPos();
        }

        currentActiveMessages.Clear();
        currentActiveMessage = 0;
    }

    private void SetMessage(InfoMessage infoMessage, string message)
    {
        infoMessage.SetMessage(message);

        SetTargetPos(infoMessage);
    }

    private void SetTargetPos(InfoMessage infoMessage)
    {
        RectTransform rect = infoMessage.GetRect();
        

        infoMessage.MoveToTargetPos();
    }

    public Vector2 GetTargetPos(RectTransform myRect)
    {
        float targetYPos = myRect.anchoredPosition.y - messageTerm;

        Vector2 targetPos = new Vector2(myRect.anchoredPosition.x, targetYPos);

        return targetPos;
    }

    public void MinusCurrentActiveMessage()
    {
        currentActiveMessage--;
    }

    public Vector2 GetFirstPos()
    {
        return firstPos.anchoredPosition;
    }
}
