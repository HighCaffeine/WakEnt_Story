using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AstarTest : MonoBehaviour
{
    [SerializeField] private GameObject startTransform;
    [SerializeField] private GameObject targetTransform;

    public static int SPEED => 5;

    public void SetPath()
    {
        GoToTargetPos();
    }

    Stack<Vector2> temp = new Stack<Vector2>();

    private void GoToTargetPos()
    {
        Queue<Vector2> path = PathFinding.Instance.CurvedPathFind(startTransform.transform.position, targetTransform.transform.position, ref temp, false, PathFinding.StartNodeDirectionLimit.NONE);

        //PathFinding.Instance.LineRender(startTransform.transform.position);
        StartCoroutine(MoveToTarget(path, startTransform));
    }


    //ProductorMovementController 추가 후 옮길거임 델리게이트로 사용예정

    private IEnumerator MoveToTarget(Queue<Vector2> path, GameObject npc)
    {
        while (path.Count > 0)
        {
            Vector2 targetPos = path.Dequeue();
            Vector2 direction = (targetPos - (Vector2)npc.transform.position).normalized;

            while (!(Vector2.Distance(npc.transform.position, targetPos) <= 0.01f))
            {
                Vector2 newPos = npc.transform.position;
                newPos += direction * Time.deltaTime;

                npc.transform.position = newPos;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
