using Unity.Services.Analytics;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UnityConsent;

// collects only for signed-in accounts that said they're 18+ and opted in; everyone else, guests included, stays Denied
public class AnalyticsManager : Singleton<AnalyticsManager>
{
    public static bool collecting { get; private set; }

    // per account, so one player's choice doesn't carry over to another account on the same device
    static string ChoiceKey => "analyticsOptIn_" + AuthenticationService.Instance.PlayerId;
    public static bool choiceNeeded => AuthManager.hasAccount && !PlayerPrefs.HasKey(ChoiceKey);
    public static bool optedIn => AuthManager.hasAccount && PlayerPrefs.GetInt(ChoiceKey, 0) == 1;

    protected override void Awake()
    {
        base.Awake();
        if (instance != this) return;
        Apply(); // explicitly Denied until a consenting account signs in
    }

    void Start()
    {
        if (instance != this) return;
        AuthManager.instance.onLogIn.AddListener(Apply);
        AuthManager.instance.onLogOut.AddListener(Apply);
    }

    public static void SetChoice(bool adult, bool consent)
    {
        PlayerPrefs.SetInt(ChoiceKey, adult && consent ? 1 : 0);
        PlayerPrefs.Save();
        Apply();
    }

    // revoke first: the SDK refuses to delete while consent is granted. Unity retries the request until it succeeds, even across sessions
    public static void DeleteData()
    {
        SetChoice(false, false);
        AnalyticsService.Instance.RequestDataDeletion();
    }

    static void Apply()
    {
        collecting = optedIn;
        ConsentState state = EndUserConsent.GetConsentState();
        state.AnalyticsIntent = collecting ? ConsentStatus.Granted : ConsentStatus.Denied;
        EndUserConsent.SetConsentState(state);
    }

    // each event and its parameters must also exist in the dashboard's Event Manager, or Unity drops it
    public static void Record(CustomEvent e)
    {
        if (collecting) AnalyticsService.Instance.RecordEvent(e);
    }
}
