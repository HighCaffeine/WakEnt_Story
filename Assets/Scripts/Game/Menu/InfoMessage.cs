using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMessage : MonoBehaviour, 
                            OnReturnPool<InfoMessage>, 
                            InfoMessageGroup.EndPopupMessage, 
                            InfoMessageGroup.GetTargetPosition
{
    private OnReturnPoolEvent<InfoMessage> OnReturnPoolEvent;                       //풀 리턴
    private InfoMessageGroup.OnEndPopupMessage OnEndPopupMessage;                   //메세지 출력 종료 후 현재 켜진 메세지 수 감소
    private InfoMessageGroup.OnGetTargetPos OnGetTargetPos;

    private RectTransform myRect;

    [SerializeField] private float moveSpeed;

    [SerializeField] private TMPro.TextMeshProUGUI message;

    private Vector2 firstPos;

    private bool isWhileMoving = false;

    public void Init(OnReturnPoolEvent<InfoMessage> onReturnPoolEvent)
    {
        OnReturnPoolEvent = onReturnPoolEvent;
        SetOnEndPopupMessage(InfoMessageGroup.Instance.MinusCurrentActiveMessage);
        SetOnGetTargetPos(InfoMessageGroup.Instance.GetTargetPos);

        myRect = GetComponent<RectTransform>();

        firstPos = InfoMessageGroup.Instance.GetFirstPos();

        myRect.position = firstPos;
    }

    public void SetMessage(string message)
    {
        this.message.text = message;
    }

    public void MoveToTargetPos()
    {
        StartCoroutine(MoveToTargetPosCoroutine());
    }

    private IEnumerator MoveToTargetPosCoroutine()
    {
        yield return StartCoroutine(CheckMovedCoroutine());

        yield return StartCoroutine(MoveCoroutine(true));
    }

    private IEnumerator CheckMovedCoroutine()
    {
        while (true)
        {
            if (!isWhileMoving)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    public void InitPos()
    {
        StartCoroutine(MoveCoroutine(false));
    }


    public void SetFirstPos()
    {
        myRect.anchoredPosition = firstPos;
    }

    private IEnumerator MoveCoroutine(bool isMoveToDown)
    {
        isWhileMoving = true;
        Vector2 newPos = Vector2.zero;

        //yield return StartCoroutine(BREAKTIME());

        //효과음
        SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_UIPopupMessage.ToString(), false);
        if (isMoveToDown)
        {
            Vector2 pos = (Vector2)OnGetTargetPos?.Invoke(myRect);

            while (pos.y <= myRect.anchoredPosition.y)
            {
                yield return new WaitUntil( () => { return GameManager.IsGamePause == false; }); 

                newPos = myRect.anchoredPosition;

                newPos += Vector2.down * Time.deltaTime * moveSpeed * 10f;

                myRect.anchoredPosition = newPos;

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (myRect.anchoredPosition.y <= firstPos.y)
            {
                yield return new WaitUntil( () => { return GameManager.IsGamePause == false; }); 

                newPos = myRect.anchoredPosition;

                newPos += Vector2.up * Time.deltaTime * moveSpeed * 10f;

                myRect.anchoredPosition = newPos;

                yield return new WaitForFixedUpdate();
            }

            OnReturnPoolEvent?.Invoke(this);
            OnEndPopupMessage?.Invoke();
            
            myRect.anchoredPosition = firstPos;
        }

        isWhileMoving = false;
    }

    public RectTransform GetRect()
    {
        return myRect;
    }

    public void SetOnEndPopupMessage(InfoMessageGroup.OnEndPopupMessage OnEndPopupMessage)
    {
        this.OnEndPopupMessage = OnEndPopupMessage;
    }

    public void SetOnGetTargetPos(InfoMessageGroup.OnGetTargetPos OnGetTargetPos)
    {
        this.OnGetTargetPos = OnGetTargetPos;
    }
}
