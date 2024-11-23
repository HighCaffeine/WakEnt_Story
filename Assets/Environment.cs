using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    //추후 environment 매니저 만들어서 프리팹 받아서 내부 이미지만 변경하여 사용하는 방식으로 변경


    [Header("상호작용 시 전환 이미지")] [SerializeField] private List<Sprite> processImageList; 
    [Header("상호작용 이전 기본 이미지")][SerializeField] private Sprite idleImage;

    [Header("사물 방향")]
    [SerializeField] private bool isRight;
    [SerializeField] private bool useIdleImage = false;

    private SpriteRenderer objImage;
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
            objImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
            objImage.sprite = idleImage;
        }

        animator.runtimeAnimatorController = aoc;
        aoc["EnvironmentAni"] = environmentAni;

        if (animator != null)
        {
            GameManager.Instance.RegisterPauseEvent(PauseEvent);
        }
    }

    public bool GetIsRight() { return isRight; }

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