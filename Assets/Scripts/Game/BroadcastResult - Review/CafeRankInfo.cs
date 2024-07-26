using UnityEngine;


[CreateAssetMenu(fileName = "NewCafeRank", menuName = "CafeReview/CafeRankInfo")]
public class CafeRankInfo : ScriptableObject
{
    [System.Serializable]
    public class ReviewRatio
    {
        public int MatchingReviewRatio => matchingReviewRatio;
        public int ExpectationsRatio => expectationsRatio;
        public int StatPointRatio => statPointRatio;

        [Header("전부 합해서 10으로 맞춰야 함.")]
        [Range(1, 10)] [SerializeField] private int matchingReviewRatio;    //매칭률
        [Range(1, 10)] [SerializeField] private int expectationsRatio;      //기대도
        [Range(1, 10)] [SerializeField] private int statPointRatio;         //스텟값
    }

    public ReviewRatio reviewRatio;
}
