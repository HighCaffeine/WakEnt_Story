using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GenericSingleton<PlayerController>
{
    private long money;

    private new void Awake()
    {
        base.Awake();

        money = 0;
    }

    private void Start()
    {
        Debug.Log("playerController start");

        DataManager.Instance.SetMoney(ref money);

        MenuController.Instance.UpdateMoney(money);
    }

    public void AddMoney(long amount)
    {
        money += amount;

        MenuController.Instance.UpdateMoney(money);
    }
}
