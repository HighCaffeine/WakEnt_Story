using System.Collections.Generic;
using Devcat;
using UnityEngine;

public class Environment : MonoBehaviour
{
    //추후 environment 매니저 만들어서 프리팹 받아서 내부 이미지만 변경하여 사용하는 방식으로 변경

    enum DefaultSpriteDirection { LeftImage, RightImage, Count, };
    [Header("상호작용 시 전환 이미지")] [SerializeField] private List<Sprite> processImageList; 
    [Header("상호작용 이전 기본 이미지 0 : left, 1 : right")][SerializeField] private Sprite[] idleImage;               // 0 leftimage, 1 right Image

    [Header("사물 방향")]
    [SerializeField] private bool isRight;
    [SerializeField] private bool isFront;
    [SerializeField] private bool useIdleImage = false;

    [Header("변경 대상 오브젝트")]
    [SerializeField] private SpriteRenderer objImage;
    private Animator animator;
    [SerializeField] private AnimatorOverrideController aoc;

    [SerializeField] private AnimationClip environmentAni;
    private int imageIndex;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (processImageList == null)
        {
            return;
        }

        if (useIdleImage)
        {
            if (!objImage) objImage = transform.GetChild(0).GetComponent<SpriteRenderer>(); 
            
            objImage.sprite = GetSprite();
        }

        animator.runtimeAnimatorController = aoc;
        aoc["EnvironmentAni"] = environmentAni;

        if (animator != null)
        {
            GameManager.Instance.RegisterPauseEvent(PauseEvent);
        }
    }

    public Sprite GetSprite()
    {
        return isRight ? idleImage[ValueCastTo<int>.From(DefaultSpriteDirection.RightImage)] : idleImage[ValueCastTo<int>.From(DefaultSpriteDirection.RightImage)];
    }

    public bool GetIsRight() { return isRight; }
    public bool GetIsFront() { return isFront; }

    //진행도를 받아와서 전환해야 함.
    //진행도는 요청측에서 계산하며 changeimage함수 호출만 함.


    public void ChangeImage()
    {
        if (processImageList == null)
        {
            return;
        }

        objImage.sprite = processImageList[imageIndex];
        
        imageIndex = (imageIndex + 1) % processImageList.Count;

    }

    private void PauseEvent(bool pause)
    {
        animator.speed = pause ? 0.0f : 1.0f;
    }

    public int GetCurrentStep()
    {
        return imageIndex;
    }
}