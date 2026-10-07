using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;

[DefaultExecutionOrder(-100)]
public class LeaderboardManager : Singleton<LeaderboardManager>
{
    [SerializeField] string leaderboardId;
    private int playersPerPage = 5;
    public int totalPages {get; private set;} = 0;
    // read the real state: AuthManager's login can finish before our own await resumes
    public bool servicesReady => UnityServices.State == ServicesInitializationState.Initialized;

    new private async void Awake()
    {
        base.Awake();
        await UnityServices.InitializeAsync();
    }

    void Start()
    {
        // login also fires on app start with a cached session, which retries offline/guest bests
        if (AuthManager.instance != null) AuthManager.instance.onLogIn.AddListener(PushHighScore);
    }

    public async void PushHighScore()
    {
        // guests can play, but only signed-in accounts submit
        if (!SaveData.HighScorePending || !servicesReady || !AuthManager.hasAccount) return;

        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, SaveData.HighScore);
            SaveData.HighScorePending = false;
        }
        catch (Exception e)
        {
            // stays pending for the next login
            Debug.Log(e.Message);
        }
    }

    public async Task<LeaderboardScoresPage> LoadPlayers(int page)
    {
        if (!servicesReady || !AuthenticationService.Instance.IsSignedIn)
            return null;

        try
        {
            GetScoresOptions options = new GetScoresOptions
            {
                Offset = (page - 1) * playersPerPage,
                Limit = playersPerPage
            };

            var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);

            totalPages = Mathf.Max(1, Mathf.CeilToInt((float)scores.Total / scores.Limit));

            return scores;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
