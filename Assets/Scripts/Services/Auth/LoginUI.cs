using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Collections;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

[DefaultExecutionOrder(2)]
public class LoginUI : MonoBehaviour
{
    public Button loginButton, signUpButton;
    public GameObject openButton; // global sign-in button, hidden once signed in
    public GameObject signOutButton; // shown only while signed in
    public GameObject authPanel; // book holding the login/sign-up pages
    public GameObject loginFirstPopup;
    public TMP_Text playerInfoText; // best score + account, bottom right
    public TMP_InputField userName_login, password_login;
    public TMP_InputField userName, password, playerName;
    public TMP_Text errorText_Login, errorText_SignUp;
    public RequestFailedException error = null;
    public bool recievedError = false;

    void Start()
    {
        if (AuthManager.instance != null)
        {
            AuthManager.instance.errorEvent.AddListener(HandleErrors);
            AuthManager.instance.onLogIn.AddListener(LoggedIn);
            AuthManager.instance.onLogOut.AddListener(LoggedOut);
        }

        if (AuthManager.hasAccount)
        {
            LoggedIn();
        }
        RefreshPlayerInfo();
    }

    void OnDestroy()
    {
        if (AuthManager.instance != null)
        {
            AuthManager.instance.errorEvent.RemoveListener(HandleErrors);
            AuthManager.instance.onLogIn.RemoveListener(LoggedIn);
            AuthManager.instance.onLogOut.RemoveListener(LoggedOut);
        }
    }

    void OnEnable()
    {
        if (loginButton != null) loginButton.onClick.AddListener(LoginWrapper);
        if (signUpButton != null) signUpButton.onClick.AddListener(SignUpWrapper);
    }

    void OnDisable()
    {
        if (loginButton != null) loginButton.onClick.RemoveListener(LoginWrapper);
        if (signUpButton != null) signUpButton.onClick.RemoveListener(SignUpWrapper);
    }

    void LoginWrapper()
    {
        StartCoroutine(Login());
    }

    void SignUpWrapper()
    {
        StartCoroutine(SignUp());
    }

    IEnumerator SignUp()
    {
        string passwordError = ValidatePasswordDetailed(password.text);

        if (passwordError != null)
        {
            errorText_SignUp.text = passwordError;
        }
        else
        {
            AuthManager.instance.SignUp(userName.text, password.text);
            yield return new WaitUntil(() =>
            {
                return AuthManager.hasAccount || recievedError;
            });

            if (recievedError)
            {
                Debug.Log("some error has occured unfortunately");
                errorText_SignUp.text = error.Message;
                recievedError = false;
            }
            else
            {
                // empty = keep the name UGS auto-generates
                if (!string.IsNullOrWhiteSpace(playerName.text)) AuthManager.instance.UpdatePlayerName(playerName.text.Trim());
                Debug.Log("User account connected");
            }
        }
    }

    void LoggedIn()
    {
        Debug.Log("User logged in");
        if (authPanel != null) authPanel.SetActive(false);
        if (openButton != null) openButton.SetActive(false);
        if (signOutButton != null) signOutButton.SetActive(true);
        RefreshPlayerInfo();
    }

    void LoggedOut()
    {
        if (openButton != null) openButton.SetActive(true);
        if (signOutButton != null) signOutButton.SetActive(false);
        RefreshPlayerInfo();
    }

    // button target: the scene's own AuthManager copy is destroyed as a duplicate on menu reloads
    public void SignOut()
    {
        AuthManager.instance.SignOut();
    }

    void RefreshPlayerInfo()
    {
        if (playerInfoText == null) return;
        string who = AuthManager.hasAccount
            ? $"{AuthManager.username}\nID: {AuthenticationService.Instance.PlayerId}"
            : "Guest";
        playerInfoText.text = $"Best: {SaveData.HighScore}\n{who}";
    }

    public void ShowLoginFirst()
    {
        loginFirstPopup.SetActive(true);
    }

    IEnumerator Login()
    {
        AuthManager.instance.SignIn(userName_login.text, password_login.text);

        yield return new WaitUntil(() =>
        {
            return AuthManager.hasAccount || recievedError;
        });

        if (recievedError)
        {
            Debug.Log("some error has occured unfortunately");
            errorText_Login.text = error.Message;
            recievedError = false;
        }
        else
        {
            Debug.Log("User account connected");
        }
    }

    void HandleErrors(RequestFailedException ex)
    {
        error = ex;
        recievedError = true;
    }

    string ValidatePasswordDetailed(string password)
    {
        if (password.Length < 8)
            return "Password must be at least 8 characters long.";

        if (!Regex.IsMatch(password, @"[a-z]"))
            return "Password must contain at least one lowercase letter.";

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return "Password must contain at least one uppercase letter.";

        if (!Regex.IsMatch(password, @"\d"))
            return "Password must contain at least one number.";

        if (!Regex.IsMatch(password, @"[\W_]"))
            return "Password must contain at least one special character.";

        return null; 
    }
}
