using System;
using System.Collections;
using UnityEngine;

public class CharacterMessage : MonoBehaviour, OnReturnPool<CharacterMessage>
{
    [SerializeField] private TMPro.TextMeshProUGUI message;                 //Text 객체
    
    OnReturnPoolEvent<CharacterMessage> OnReturnPoolEvent;                  //pool 이벤트

    private Transform targetTransform;

    private RectTransform rect;

    private Coroutine fixedPosCoroutine;
    private Coroutine messageDisappearCoroutine;

    private CharacterManager.OnCharacterIsMove OnCharacterIsMove;       //캐릭터 움직이는지 체크
    private Action[] callback;                                          //메세지 출력 종료 후 이벤트들 콜백용 스텟 추가 요청 등, 중간에서 매니저 측에서 amount

    public void Init(OnReturnPoolEvent<CharacterMessage> OnReturnPoolEvent)
    {
        this.OnReturnPoolEvent = OnReturnPoolEvent;

        rect = GetComponent<RectTransform>();
    }

    public string targetName;

    public void SetMessage(string text, Transform targetTransform, CharacterManager.OnCharacterIsMove OnCharacterIsMove, params Action[] callback)
    {
        //test
        targetName = targetTransform.name;

        this.targetTransform = targetTransform;
        message.text = text;

        this.OnCharacterIsMove = OnCharacterIsMove;
        this.callback = callback;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
        rect.anchoredPosition = CharacterMessageManager.Instance.MessagePosToScreenPos(screenPos); 

        fixedPosCoroutine = StartCoroutine(FixedPos());
        messageDisappearCoroutine = StartCoroutine(MessageDisappear());
    }

    private IEnumerator FixedPos()
    {
        while (true)
        {
            if (!(bool)OnCharacterIsMove?.Invoke())
            {
                yield return new WaitForFixedUpdate();

                continue;
            }

            Vector2 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);

            rect.anchoredPosition = CharacterMessageManager.Instance.MessagePosToScreenPos(screenPos); 

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator MessageDisappear()
    {
        float time = 0.0f;

        while (true)
        {
            time += Time.deltaTime;

            if (CharacterMessageManager.Instance.DisappearTime <= time)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        Init();

        yield return null;
    }

    private void Init()
    {
        foreach (var eventData in callback)
        {
            eventData?.Invoke();
        }

        //callback 정리
        for (int i = 0; i < callback.Length; i++)
        {
            callback[i] = null;
        }

        OnReturnPoolEvent?.Invoke(this);

        StopCoroutine(fixedPosCoroutine);
        StopCoroutine(messageDisappearCoroutine);

        messageDisappearCoroutine = null;
        fixedPosCoroutine = null;
    }
}
