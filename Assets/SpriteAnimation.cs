using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : GenericSingleton<SpriteAnimation>
{
    //현재는 여기서 UI쪽 컨트롤로하고, 
    //2d오브젝트 캐릭터들도 컨트롤 해야할 경우 classgroup에 있는 resourcetype을 사용할듯
    //모든 캐릭터들에 해당 소스를 붙여도 써도 괜찮을지와
    //그러면 결국 지금 매니저 방식으로 만들고 있지만
    //필요한 곳에서 각자 캐싱해서 사용하는 방식으로 해야함.
    public enum AtlasAniType
    {
        Idle,
        FrontWork,
        BackWork,
        Walk,
        FrontIdleStretching,
        FrontIdleLookAround,
        BackIdleStretching,
        BackIdleLookAround,
    }

    public AtlasAniType TEST_atlasType;
    [Header("0 : Idle, 1 : Work")][SerializeField] private SpriteAnimationData[] aniData;   //enum으로 관리 나중에
    private UnityEngine.UI.Image imageTarget;               //타겟
    private UnityEngine.SpriteRenderer spriteRenderTarget;

    private bool isSpriteRenderer;

    [Header("Frame")][SerializeField] private float frame;                  //fps

    private List<Sprite> sprites;

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

    private void Init()
    {
        imageTarget = null;
        spriteRenderTarget = null;
        aniCount = 0;
    }

    //애니메이션 필요 패널 켰을 때 실행
    public void SetDataImage(AtlasAniType TEST_atlasType, UnityEngine.UI.Image target)
    {
        aniCount = aniData[(int)TEST_atlasType].aniCount;
        this.imageTarget = target;
        isSpriteRenderer = false;
    }

    public void SetDataSpriteRender(AtlasAniType TEST_atlasType, UnityEngine.SpriteRenderer target)
    {
        aniCount = aniData[(int)TEST_atlasType].aniCount;
        this.spriteRenderTarget = target;
        isSpriteRenderer = true;
    }

    //패널 내부에서 전환 시 사용
    public void PlayAnimation(string characterName, bool loop, float playDealy)
    {
        aniCount = aniData[(int)TEST_atlasType].aniCount;

        this.loop = loop;
        string aniKey = string.Format($"{aniData[(int)TEST_atlasType].animationName}_{characterName}");

        if (sprites.Count > 1)
        {
            sprites.Clear();
        }

        for (int i = 0; i < aniData[(int)TEST_atlasType].aniCount; i++)
        {
            Sprite sprite = aniData[(int)TEST_atlasType].atlas.GetSprite($"{aniKey}_{i}");
            sprites.Add(sprite);
        }

        //코루틴 실행
        if (aniCoroutine != null)
        {
            StopCoroutine(aniCoroutine);
        }

        Debug.Log($"Play {characterName}");
        aniCoroutine = StartCoroutine(SpriteAniCoroutine(playDealy));
    }

    Coroutine aniCoroutine;

    public IEnumerator SpriteAniCoroutine(float playDelay)
    {
        bool isPlay = true;

        index = 0;

        if (isSpriteRenderer)
        {
            spriteRenderTarget.sprite = sprites[0];
        }
        else
        {
            imageTarget.sprite = sprites[0];   
        }

        //if (playDelay > 0.0f) yield return new WaitForSeconds(playDelay);

        if (sprites[0] == null) isPlay = false;

        Debug.Log(isPlay ? "Play" : "Not Play");

        while (isPlay)
        {
            timer += Time.deltaTime * aniSpeed;

            if (timer >= 1f / frame)
            {
                if (aniData[(int)TEST_atlasType].frames[index] == (frameCount++ + beforeFrameCount))
                {
                    beforeFrameCount = aniData[(int)TEST_atlasType].frames[index];
                    frameCount = 0;
                    timer = 0.0f;
                    index++;

                    if (index >= aniData[(int)TEST_atlasType].spriteOrder.Length - 1)
                    {
                        if (loop)
                        {
                            beforeFrameCount = 0;
                            index = 0;

                            yield return new WaitForSeconds(aniDelay);
                        }
                        else
                        {
                            index = aniData[(int)TEST_atlasType].spriteOrder.Length - 1;
                            isPlay = false;
                        }
                    }

                    if (isSpriteRenderer)
                    {
                        spriteRenderTarget.sprite = sprites[aniData[(int)TEST_atlasType].spriteOrder[index]];
                    }
                    else
                    {
                        imageTarget.sprite = sprites[aniData[(int)TEST_atlasType].spriteOrder[index]];
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
