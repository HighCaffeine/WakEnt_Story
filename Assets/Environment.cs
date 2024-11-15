using UnityEngine;

public class Environment : MonoBehaviour
{
    [Header("사물 방향")]
    [SerializeField] private bool isRight;

    public bool GetIsRight() { return isRight; }
}
