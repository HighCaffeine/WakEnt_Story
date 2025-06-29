using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationManager : GenericSingleton<SpriteAnimationManager>
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

    public AtlasAniType atlasType;
    [Header("0 : Idle, 1 : Work")][SerializeField] private SpriteAnimationData[] aniData;   //enum으로 관리 나중에
    

    private bool isSpriteRenderer;

    [Header("Frame")][SerializeField] private float frame;                  //fps

    [Header("애니메이션 딜레이")][SerializeField] private float aniDelay;

    [SerializeField] private float aniSpeed;

    private new void Awake()
    {
        base.Awake();

        sprites = new List<Sprite>();
    }

    List<Sprite> sprites;

    public float GetAniSpeed() { return aniSpeed; }
    public float GetFrame() { return frame; }

    public Sprite[] GetSprite(AtlasAniType atlasAniType, string characterName)
    {
        string aniKey = $"{aniData[(int)atlasAniType].animationName}_{characterName}";

        sprites.Clear();

        for (int i = 0; i < aniData[(int)atlasAniType].aniCount; i++)
        {
            string spriteName = $"{aniKey}_{i}";
            Sprite sprite = aniData[(int)atlasAniType].atlas.GetSprite(spriteName);
            if (sprite == null)
            {
                Debug.LogWarning($"Sprite not found: {spriteName}");
            }
            sprites.Add(sprite);
        }

        return sprites.ToArray();
    }

    public int[] GetFrameTiming(AtlasAniType atlasAniType)
    {
        return aniData[(int)atlasAniType].frames;
    }

    public int[] GetSpriteOrder(AtlasAniType atlasAniType)
    {
        return aniData[(int)atlasAniType].spriteOrder;
    }
}
