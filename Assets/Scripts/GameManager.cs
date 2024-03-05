using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : GenericSingleton<GameManager>
{
    public float GameTime => gameTime;

    private float gameTime;

    public Action Save => OnSave;
    public Action Load => OnLoad;

    private void OnSave()
    {
        Debug.Log("save");
    }

    private void OnLoad()
    {
        Debug.Log("load");
    }
}
