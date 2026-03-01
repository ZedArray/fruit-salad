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
    public bool servicesReady {get; private set;} = false;

    new private async void Awake()
    {
        base.Awake();
        await UnityServices.InitializeAsync();
        servicesReady = true;
    }

    public async void AddPlayerScore(int score)
    {
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
        }
        catch (Exception e)
        {
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

            totalPages = Mathf.CeilToInt((float)scores.Total / scores.Limit);

            return scores;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
