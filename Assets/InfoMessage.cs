using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InfoMessage : MonoBehaviour, OnReturnPool<InfoMessage>, InfoMessageGroup.EndPopupMessage
{
    private OnReturnPoolEvent<InfoMessage> OnReturnPoolEvent;                       //풀 리턴
    private InfoMessageGroup.OnEndPopupMessage OnEndPopupMessage;                   //메세지 출력 종료 후 현재 켜진 메세지 수 감소

    private RectTransform myRect;

    [SerializeField] private float moveSpeed;

    [SerializeField] private TMPro.TextMeshProUGUI message;

    private Vector2 firstPos;

    public void Init(OnReturnPoolEvent<InfoMessage> onReturnPoolEvent)
    {
        OnReturnPoolEvent = onReturnPoolEvent;
        SetOnEndPopupMessage(InfoMessageGroup.Instance.MinusCurrentActiveMessage);

        myRect = GetComponent<RectTransform>();

        firstPos = InfoMessageGroup.Instance.GetFirstPos();

        myRect.position = firstPos;
    }

    public void SetMessage(string message)
    {
        this.message.text = message;
    }

    Coroutine moveToTargetCoroutine;

    public void MoveToTargetPos(Vector2 pos)
    {
        StartCoroutine(MoveToTargetPosCoroutine(pos));
    }

    private IEnumerator MoveToTargetPosCoroutine(Vector2 pos)
    {
        yield return StartCoroutine(CheckMovedCoroutine());


        moveToTargetCoroutine = StartCoroutine(MoveCoroutine(pos, true));
        moveToTargetCoroutine = null;
    }

    private IEnumerator CheckMovedCoroutine()
    {
        while (true)
        {
            if (moveToTargetCoroutine == null)
            {
                break;
            }
        }

        yield return null;
    }

    public void InitPos()
    {
        StartCoroutine(MoveCoroutine(Vector2.zero, false));
    }

    private IEnumerator MoveCoroutine(Vector2 pos, bool isMoveToDown)
    {
        Vector2 newPos = Vector2.zero;
    
        //효과음
        if (isMoveToDown)
        {
            while (pos.y <= myRect.position.y)
            {
                newPos = myRect.position;

                newPos += Vector2.down * Time.deltaTime * moveSpeed * 10f;

                myRect.position = newPos;

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (myRect.position.y <= firstPos.y)
            {
                newPos = myRect.position;

                newPos += Vector2.up * Time.deltaTime * moveSpeed * 10f;

                myRect.position = newPos;

                yield return new WaitForFixedUpdate();
            }

            OnReturnPoolEvent?.Invoke(this);
            OnEndPopupMessage?.Invoke();
            
            myRect.position = firstPos;
        }
    }

    public RectTransform GetRect()
    {
        return myRect;
    }

    public void SetOnEndPopupMessage(InfoMessageGroup.OnEndPopupMessage OnEndPopupMessage)
    {
        this.OnEndPopupMessage = OnEndPopupMessage;
    }
}
