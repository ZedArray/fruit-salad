using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private LeaderboardItem leaderboardItemPrefab = null;
    [SerializeField] private RectTransform leaderboardItemContainer = null;
    [SerializeField] private TMP_Text pageText = null;
    [SerializeField] private Button increment, decrement;
    private List<LeaderboardItem> items = new List<LeaderboardItem>();
    private int currentPage = 1;
    private Coroutine refreshRoutine;
    private bool isLoading = false;

    void OnEnable()
    {
        refreshRoutine = StartCoroutine(AutoRefresh());
        increment.onClick.AddListener(NextPage);
        decrement.onClick.AddListener(PreviousPage);
    }

    void OnDisable()
    {
        if (refreshRoutine != null)
            StopCoroutine(refreshRoutine);

        increment.onClick.RemoveListener(NextPage);
        decrement.onClick.RemoveListener(PreviousPage);
    }

    System.Collections.IEnumerator AutoRefresh()
    {
        yield return new WaitWhile(() =>
        {
            return LeaderboardManager.instance == null ||
                   !LeaderboardManager.instance.servicesReady;
        });

        while (isActiveAndEnabled)
        {
            LoadPlayers();
            yield return new WaitForSeconds(3f);
        }
    }


    public async void LoadPlayers()
    {
        // if (isLoading) return;
        // isLoading = true;
        LeaderboardScoresPage temp = await LeaderboardManager.instance.LoadPlayers(currentPage);

        if (temp == null || temp.Results == null)
        {
            Debug.LogError("Failed to load leaderboard");
            return;
        }

        List<LeaderboardEntry> scores = temp.Results;

        ClearPlayersList();

        for (int i = 0; i < scores.Count; i++)
        {
            LeaderboardItem item = Instantiate(leaderboardItemPrefab, leaderboardItemContainer);
            item.Initialize(scores[i]);
            items.Add(item);
        }

        pageText.text = currentPage + "/" + LeaderboardManager.instance.totalPages;
        isLoading = false;
    }


    public void NextPage()
    {

        currentPage++;
        if (currentPage >= LeaderboardManager.instance.totalPages) currentPage = LeaderboardManager.instance.totalPages;

        LoadPlayers();
    }

    public void PreviousPage()
    {
        currentPage--;
        if (currentPage <= 1) currentPage = 1;

        LoadPlayers();
    }

    private void ClearPlayersList()
    {
        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Destroy(items[i].gameObject);
            }

            items.Clear();
        }
    }
}
