using System;
using Unity.Services.Core;
using UnityEngine;
public class ServicesManager : Singleton<ServicesManager>
{
    async void Awake()
    {
        InitSingleton();
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
