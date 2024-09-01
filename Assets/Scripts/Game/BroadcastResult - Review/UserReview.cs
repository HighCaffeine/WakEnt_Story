using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UserReview : MonoBehaviour, BroadcastReviewManager.OnGetComment
{
    [SerializeField] private Image userImage;
    [SerializeField] private TextMeshProUGUI comment;
    [SerializeField] private TextMeshProUGUI point;

    [SerializeField] private BroadcastReviewManager.CafeRank cafeRank;

    [SerializeField] private CafeRankInfo cafeRankInfo;

    BroadcastReviewManager.OnGetCommentEvent OnGetCommentEvent;
    BroadcastReviewManager.OnGetPointEvnet OnGetPointEvnet;
    BroadcastReviewManager.OnGetDefaultCommentEvent OnGetDefaultCommentEvent;
    BroadcastReviewManager.UserReviewSetUp userReviewSetUp;

    void Start()
    {
        userReviewSetUp = new BroadcastReviewManager.UserReviewSetUp();

        SetGetCommentEvent(BroadcastReviewManager.Instance.GetComment);
        SetGetPointEvent(BroadcastReviewManager.Instance.GetBroadcastReviewPoint);
        SetGetDefaultCommentEvent(BroadcastReviewManager.Instance.GetDefaultCommentMessage);

        userReviewSetUp.InitSetUpEvnet(cafeRank, SetData);


        BroadcastReviewManager.Instance.CommentEventAddToList(userReviewSetUp);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>멘트, 점수 세팅 완료 시 True 반환</returns>
    private void SetData()
    {
        StartCoroutine(SetDataRollCorotuine());
    }

    private IEnumerator SetDataRollCorotuine()
    {
        float time = 0.0f;

        comment.text = OnGetDefaultCommentEvent?.Invoke(cafeRank);

        while (time <= BroadcastReviewManager.PointRollTime)
        {
            time += 0.1f;

            yield return new WaitForSeconds(0.1f);

            this.point.text = Random.Range(0, BroadcastReviewManager.ReviewMaxPoint).ToString();
        }

        int point = (int)OnGetPointEvnet?.Invoke(cafeRank, cafeRankInfo);

        this.point.text = point.ToString();
        comment.text = OnGetCommentEvent?.Invoke(cafeRank, cafeRankInfo); 

        if (point < 5)
        {
            SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_ReviewSetUnder5.ToString(), false);
        }
        else if (point < 10)
        {
            SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_ReviewSetOver5.ToString(), false);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Effect.Effect_ReviewSet10.ToString(), false);
        }

        yield return null;
    }

    public void SetGetCommentEvent(BroadcastReviewManager.OnGetCommentEvent OnGetCommentEvent)
    {
        this.OnGetCommentEvent = OnGetCommentEvent;
    }

    public void SetGetPointEvent(BroadcastReviewManager.OnGetPointEvnet OnGetPointEvnet)
    {
        this.OnGetPointEvnet = OnGetPointEvnet;
    }

    public void SetGetDefaultCommentEvent(BroadcastReviewManager.OnGetDefaultCommentEvent OnGetDefaultCommentEvent)
    {
        this.OnGetDefaultCommentEvent = OnGetDefaultCommentEvent;
    }
}