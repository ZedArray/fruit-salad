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
    public GameObject loginPage, signUpPage;
    public TMP_InputField userName_login, password_login;
    public TMP_InputField userName, password, playerName;
    public TMP_Text errorText_Login, errorText_SignUp;
    public RequestFailedException error = null;
    public bool recievedError = false;

    void Start()
    {
        if (loginButton != null) loginButton.onClick.AddListener(LoginWrapper);
        if (signUpButton != null) signUpButton.onClick.AddListener(SignUpWrapper);
        if (AuthManager.instance != null)
        {
            AuthManager.instance.errorEvent.AddListener((x) => { HandleErrors(x); });
            AuthManager.instance.onLogIn.AddListener(LoggedIn);
        }

        if (AuthenticationService.Instance.IsSignedIn)
        {
            LoggedIn();
        }
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
                return AuthManager.userAuthenticated || recievedError;
            });

            if (recievedError)
            {
                Debug.Log("some error has occured unfortunately");
                errorText_SignUp.text = error.Message;
                recievedError = false;
            }
            else
            {
                AuthManager.instance.UpdatePlayerName(playerName.text);
                Debug.Log("User account connected");
                loginPage.SetActive(false);
                signUpPage.SetActive(false);
            }
        }
    }

    void LoggedIn()
    {
        Debug.Log("User logged in");
        loginPage.SetActive(false);
        signUpPage.SetActive(false);
    }

    IEnumerator Login()
    {
        AuthManager.instance.SignIn(userName_login.text, password_login.text);

        yield return new WaitUntil(() =>
        {
            return AuthManager.userAuthenticated || recievedError;
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
            loginPage.SetActive(false);
            signUpPage.SetActive(false);
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
