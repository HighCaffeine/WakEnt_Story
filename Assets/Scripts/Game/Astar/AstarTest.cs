using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class AstarTest : MonoBehaviour
{
    [SerializeField] private GameObject startTransform;
    [SerializeField] private GameObject targetTransform;


    [Header("위치 값 테스트")]
    [SerializeField] private Transform newPos;
    [SerializeField] private Transform messagePrefab;

    public static int SPEED => 5;

    public void TestPosMessage(Node[,] nodes)
    {
        Vector2 pivotPos = nodes[0,0].Pos * -1f;

        foreach (var node in nodes)
        {
            if (node == null)
            {
                continue;
            }

            Transform newItem = Instantiate(messagePrefab);
            TMPro.TextMeshPro textMeshPro = newItem.GetChild(0).GetComponent<TMPro.TextMeshPro>();


            newItem.position = node.Pos + newPos.position;
            string newString = string.Format("({0}, {1})", node.xPos, node.yPos);
            textMeshPro.text = newString + string.Format("\n({0}, {1})", node.Pos.x + pivotPos.x, node.Pos.y + pivotPos.y);
        }
    }

    void Start()
    {
        //StartCoroutine(DelayedTest());
    }

    public IEnumerator DelayedTest()
    {
        yield return new WaitForSeconds(0.5f);

        TestPosMessage(PathFinding.Instance.TEST_GetGrid());
    }


    public Tilemap tilemap;
    public void TestCellBound()
    {

    }

    public void SetPath()
    {
        GoToTargetPos();
    }

    private void GoToTargetPos()
    {
        Stack<Vector2> path = PathFinding.Instance.PathFind(startTransform.transform.position, targetTransform.transform.position);

        //PathFinding.Instance.LineRender(startTransform.transform.position);
        StartCoroutine(MoveToTarget(path, startTransform));
    }


    //ProductorMovementController 추가 후 옮길거임 델리게이트로 사용예정

    private IEnumerator MoveToTarget(Stack<Vector2> path, GameObject npc)
    {
        while (path.Count > 0)
        {
            Vector2 targetPos = path.Pop();
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
