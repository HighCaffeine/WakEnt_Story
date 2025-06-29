using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    Coroutine aniCoroutine;         //코루틴

    //애니메이션 데이터
    private float timer;            //시간 체크
    private int index;
    private int beforeFrameCount;
    private int frameCount;
    private bool loop;

    //애니메이션 Sprite데이터
    private Sprite[] sprites;
    private int[] frames;
    private int[] spriteOrders;

    [Header("Sprite / Image 중 하나 캐싱")]
    [SerializeField] private UnityEngine.UI.Image imageTarget;               //타겟
    [SerializeField] private SpriteRenderer spriteRenderTarget;

    private void Init()
    {
        timer = 0.0f;
        index = 0;
        beforeFrameCount = 0;
        frameCount = 0;
    }

    //캐싱한 클래스쪽에서 사용
    public void PlayAnimation(SpriteAnimationManager.AtlasAniType atlasType, string characterName, bool loop, float playDealy)
    {
        Init();
        this.loop = loop;

        sprites = SpriteAnimationManager.Instance.GetSprite(atlasType, characterName);
        frames = SpriteAnimationManager.Instance.GetFrameTiming(atlasType);
        spriteOrders = SpriteAnimationManager.Instance.GetSpriteOrder(atlasType);
        //코루틴 실행
        if (aniCoroutine != null)
        {
            StopCoroutine(aniCoroutine);
        }

        Debug.Log($"Play {characterName} {atlasType.ToString()}");
        aniCoroutine = StartCoroutine(SpriteAniCoroutine(playDealy));
    }

    public IEnumerator SpriteAniCoroutine(float playDelay)
    {
        bool isPlay = true;

        index = 0;

        if (spriteRenderTarget != null)
        {
            spriteRenderTarget.sprite = sprites[0];
        }
        else
        {
            imageTarget.sprite = sprites[0];
        }

        if (playDelay > 0.0f) yield return new WaitForSeconds(playDelay);

        if (sprites[0] == null) isPlay = false;

        Debug.Log(isPlay ? "Play" : "Not Play");

        float aniSpeed = SpriteAnimationManager.Instance.GetAniSpeed();
        float frame = SpriteAnimationManager.Instance.GetFrame();

        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        while (isPlay)
        {
            timer += Time.deltaTime * aniSpeed;

            if (timer >= 1f / frame)
            {
                if (frames[index] == (frameCount++ + beforeFrameCount))
                {
                    beforeFrameCount = frames[index];
                    frameCount = 0;
                    timer = 0.0f;
                    index++;

                    if (index > spriteOrders.Length - 1)
                    {
                        if (loop)
                        {
                            beforeFrameCount = 0;
                            index = 0;
                        }
                        else
                        {
                            index = spriteOrders.Length - 1;
                            isPlay = false;
                        }
                    }

                    if (spriteRenderTarget != null)
                    {
                        spriteRenderTarget.sprite = sprites[spriteOrders[index]];
                    }
                    else
                    {
                        imageTarget.sprite = sprites[spriteOrders[index]];
                    }
                }
            }

            yield return wait;
        }
    }

    public void StopAni()
    {
        if (aniCoroutine != null)
        {
            StopCoroutine(aniCoroutine);
        }

        if (spriteRenderTarget != null)
        {
            spriteRenderTarget.sprite = sprites[0];
        }
        else
        {
            imageTarget.sprite = sprites[0];
        }
    }
}
