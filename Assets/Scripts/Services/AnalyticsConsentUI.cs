using TMPro;
using UnityEngine;
using UnityEngine.UI;

// asks each signed-in account once whether they're 18+ and want to share gameplay analytics
public class AnalyticsConsentUI : MonoBehaviour
{
    public GameObject popup;
    public Toggle adultToggle, consentToggle;
    public Button saveButton;
    public Button deleteButton;
    public TMP_Text deleteLabel;

    private bool confirmingDelete;

    void Start()
    {
        saveButton.onClick.AddListener(Save);
        deleteButton.onClick.AddListener(Delete);
        if (AuthManager.instance != null) AuthManager.instance.onLogIn.AddListener(ShowIfNeeded);
        ShowIfNeeded();
    }

    void OnDestroy()
    {
        if (AuthManager.instance != null) AuthManager.instance.onLogIn.RemoveListener(ShowIfNeeded);
    }

    void ShowIfNeeded()
    {
        if (AnalyticsManager.choiceNeeded) Open();
    }

    // button target too, so players can change their answer later
    public void Open()
    {
        if (!AuthManager.hasAccount) return; // guests are never collected, nothing to choose
        adultToggle.isOn = consentToggle.isOn = AnalyticsManager.optedIn;
        confirmingDelete = false;
        deleteLabel.text = "DELETE MY DATA";
        deleteButton.interactable = true;
        popup.SetActive(true);
    }

    void Save()
    {
        AnalyticsManager.SetChoice(adultToggle.isOn, consentToggle.isOn);
        popup.SetActive(false);
    }

    // two taps so a stray one can't wipe anything
    void Delete()
    {
        if (!confirmingDelete)
        {
            confirmingDelete = true;
            deleteLabel.text = "TAP AGAIN TO DELETE";
            return;
        }
        AnalyticsManager.DeleteData();
        adultToggle.isOn = consentToggle.isOn = false;
        deleteLabel.text = "DELETION REQUESTED";
        deleteButton.interactable = false;
    }
}
