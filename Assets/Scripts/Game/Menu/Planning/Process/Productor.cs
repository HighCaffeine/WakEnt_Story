using System.Collections;
using UnityEngine;

public class Productor : MonoBehaviour, 
                        OnReturnPool<Productor>, 
                        ProductorManager.ProductorStatRequest, 
                        ProductorManager.ProductorFieldProcessValue
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
        //SetProductorFieldProcess(ProductorManager.Instance.AddFieldProcessValueToManager);
    }

    public void SetProductorStatRequest(ProductorManager.OnProductorStatRequest OnProductorStatRequest)
    {
        this.OnProductorStatRequest = OnProductorStatRequest;
    }

    public void SetProductorFieldProcess(ProductorManager.OnProductorFieldProcess OnProductorFieldProcess)
    {
        this.OnProductorFieldProcess = OnProductorFieldProcess;
    }

    public void BroadcastPointReq()
    {
        //ProductorManager.Instance.AddStatFieldProcessing(info);
    }
}
