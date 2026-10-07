using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using UnityEngine.Events;

[DefaultExecutionOrder(-50)]
public class AuthManager : Singleton<AuthManager>
{
    public static bool userAuthenticated = false;
    // userAuthenticated includes anonymous guests; only accounts may submit scores
    public static bool hasAccount = false;
    public static string username;
    public UnityEvent<RequestFailedException> errorEvent;
    public UnityEvent onLogIn;
    public UnityEvent onLogOut;

    private async void Start()
    {
        // AuthenticationService.Instance throws until services finish initializing (ServicesManager starts it)
        await UnityServices.InitializeAsync();
        SetupEvents();
        await SignInCachedUserAsync();
        // guests get an anonymous session so they can still read the leaderboard
        if (!AuthenticationService.Instance.IsSignedIn) await SignInGuestAsync();
    }

    async Task SignInGuestAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    public void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += OnSignIn;
        AuthenticationService.Instance.SignInFailed += OnSignInFailed;
        AuthenticationService.Instance.SignedOut += OnSignOut;
        AuthenticationService.Instance.Expired += OnSessionExpired;
    }

    public async Task SignInCachedUserAsync()
    {
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerInfo info = await AuthenticationService.Instance.GetPlayerInfoAsync();
            username = info.Username;
            hasAccount = !string.IsNullOrEmpty(username);
            if (hasAccount) onLogIn?.Invoke();
            Debug.Log("Sign in anonymously succeeded!");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    public void OnSignIn()
    {
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        userAuthenticated = true;
    }
    public void OnSignInFailed(RequestFailedException err)
    {
        Debug.LogError(err);
        userAuthenticated = false;
    }
    public void OnSignOut()
    {
        Debug.Log("Player signed out.");
        userAuthenticated = false;
        hasAccount = false;
        username = null;
        onLogOut?.Invoke();
    }
    public void OnSessionExpired()
    {
        Debug.Log("Player session expired.");
        userAuthenticated = false;
        hasAccount = false;
        username = null;
        onLogOut?.Invoke();
    }

    [ContextMenu("Test Sign Up")]
    private void TestSignUp()
    {
        SignUp("admin", "Admin@123");
    }

    [ContextMenu("Test Sign In")]
    private void TestSignIn()
    {
        SignIn("admin", "Admin@123");
    }

    [ContextMenu("Test Sign Out")]
    private void TestSignOut()
    {
        SignOut();
    }
    public async void SignUp(string username, string password)
    {
        await SignUpWithUsernamePasswordAsync(username, password);
    }


    public async void SignIn(string username, string password)
    {
        await SignInWithUsernamePasswordAsync(username, password);
    }

    public async void SignOut()
    {
        AuthenticationService.Instance.SignOut(true);
        // back to a fresh guest session so the leaderboard stays readable
        await SignInGuestAsync();
    }

    public async void UpdatePassword(string currentPassword, string newPassword)
    {
        await UpdatePasswordAsync(currentPassword, newPassword);
    }
    public async void UpdatePlayerName(string newName)
    {
        await UpdatePlayerNameAsync(newName);
    }
    public async Task UpdatePlayerNameAsync(string newName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            Debug.Log("Player name updated.");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            errorEvent.Invoke(ex);
        }
    }
    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            // a guest links credentials to their anonymous player; UGS rejects sign-up while signed in
            if (AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
            else
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            AuthManager.username = username;
            hasAccount = true;
            onLogIn?.Invoke();
            Debug.Log("SignUp is successful.");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            errorEvent.Invoke(ex);
        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            // UGS rejects sign-in while the guest session is active
            if (AuthenticationService.Instance.IsSignedIn) AuthenticationService.Instance.SignOut(true);
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username.Trim(), password.Trim());
            AuthManager.username = username.Trim();
            hasAccount = true;
            onLogIn?.Invoke();
            Debug.Log("SignIn is successful.");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            errorEvent.Invoke(ex);
            if (!AuthenticationService.Instance.IsSignedIn) await SignInGuestAsync();
        }
    }
    async Task UpdatePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
            Debug.Log("Password updated.");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

}
