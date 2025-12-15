using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "NewSpriteAnimation", menuName = "SpriteAni/CreateNewAnimation")]
public class SpriteAnimationData : ScriptableObject
{
    public string animationName;
    public int aniCount;

    [Header("Sprite Change Frame")] public int[] frames;            //각 이미지들 바꿀 때까지 프레임 수
    [Header("Sprite Animation Order")] public int[] spriteOrder;    //애니메이션 실행 순서
    [SerializeField] public SpriteAtlas atlas;
}
