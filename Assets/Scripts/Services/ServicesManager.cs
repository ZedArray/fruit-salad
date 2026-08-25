using System;
using Unity.Services.Core;
using UnityEngine;

public class ServicesManager : Singleton<ServicesManager>
{

    public static bool UserAuthenticated
    {
        get { return AuthManager.userAuthenticated; }
        private set { AuthManager.userAuthenticated = value; }
    } 
    

new async void Awake()
    {
        base.Awake();
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
