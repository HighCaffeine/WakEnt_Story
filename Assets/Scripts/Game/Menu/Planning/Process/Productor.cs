using UnityEngine;

public class Productor : MonoBehaviour, OnReturnPool<Productor>, ProductorManager.ProductorStatRequest, ProductorManager.ProductorFieldProcessValue
{
    [SerializeField] private ProductorInfo info;
    ProductorManager.OnProductorStatRequest OnProductorStatRequest;
    ProductorManager.OnProductorFieldProcess OnProductorFieldProcess;

    public void Selected()
    {
        OnProductorStatRequest?.Invoke(info);
    }

    public void FieldProcess()
    {
        OnProductorFieldProcess?.Invoke(info);
    }

    public void Init(OnReturnPoolEvent<Productor> onReturnPoolEvent)
    {
        SetProductorStatRequest(ProductorManager.Instance.CalculateProductorStat);
        SetProductorFieldProcess(ProductorManager.Instance.AddFieldProcessValueToManager);
    }

    public void SetProductorStatRequest(ProductorManager.OnProductorStatRequest OnProductorStatRequest)
    {
        this.OnProductorStatRequest = OnProductorStatRequest;
    }

    public void SetProductorFieldProcess(ProductorManager.OnProductorFieldProcess OnProductorFieldProcess)
    {
        this.OnProductorFieldProcess = OnProductorFieldProcess;
    }

    
    //필드 작업자 추가 후 Astar추가 예정 -> Isometric예정이라 Layer로 검색/grid검색 둘 중 하나로
    //필드 작업로직 추가
    //애니메이션 컨트롤
}
