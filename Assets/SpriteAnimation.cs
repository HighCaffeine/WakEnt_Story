using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class SpriteAnimation : GenericSingleton<SpriteAnimation>
{
    [SerializeField] private SpriteAnimationData aniData;   //enum으로 관리 나중에
    [SerializeField] private UnityEngine.UI.Image target;   //타겟

    [Header("Frame")] [SerializeField] private float frame;                  //fps

    [SerializeField] private List<Sprite> sprites;

    int aniCount;           //이미지 수
    private float timer;    //시간 체크
    private int index;
    private int beforeFrameCount;
    
    private int frameCount;
    private bool loop;

    [Header("애니메이션 딜레이")][SerializeField] private float aniDelay;

    [SerializeField] private float aniSpeed;

    private new void Awake()
    {
        base.Awake();

        index = -1;
        timer = 0.0f;

        sprites = new List<Sprite>();
    }
    
    //애니메이션 필요 패널 켰을 때 실행
    public void SetData(ResourceType resourceType, UnityEngine.UI.Image target)
    {
        aniCount = aniData.aniCount;
        this.target = target;
    }

    //패널 내부에서 전환 시 사용
    public void PlayAnimation(string characterName, bool loop)
    {
        aniCount = aniData.aniCount;

        this.loop = loop;
        string aniKey = string.Format($"{aniData.animationName}_{characterName}");

        if (sprites.Count > 1)
        {
            sprites.Clear();
        }

        for (int i = 0; i < aniData.aniCount; i++)
        {
            Sprite sprite = aniData.atlas.GetSprite($"{aniKey}_{i}");
            sprites.Add(sprite);
        }

        //코루틴 실행
        if (aniCoroutine != null)
        {
            StopCoroutine(aniCoroutine);
        }

        aniCoroutine = StartCoroutine(SpriteAniCoroutine());
    }

    Coroutine aniCoroutine;

    public IEnumerator SpriteAniCoroutine()
    {
        bool isPlay = true;

        index = 0;

        while (isPlay)
        {
            timer += Time.deltaTime * aniSpeed;

            if (timer >= 1f / frame)
            {
                if (aniData.frames[index] == (frameCount++ + beforeFrameCount))
                {
                    beforeFrameCount = aniData.frames[index];
                    frameCount = 0;
                    timer = 0.0f;
                    index++;

                    if (index >= aniData.spriteOrder.Length - 1)
                    {
                        if (loop)
                        {
                            beforeFrameCount = 0;
                            index = 0;

                            yield return new WaitForSeconds(aniDelay);
                        }
                        else
                        {
                            index = aniData.spriteOrder.Length - 1;
                            isPlay = false;
                        }
                    }

                    target.sprite = sprites[aniData.spriteOrder[index]];
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
