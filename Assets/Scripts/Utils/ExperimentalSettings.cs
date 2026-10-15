using TMPro;
using UnityEngine;
using UnityEngine.UI;

// experimental anti-idle controls; writes AntiIdleUtensilSpawner's session-only statics, never the save file
public class ExperimentalSettings : MonoBehaviour
{
    [SerializeField] private Toggle antiIdle;
    [SerializeField] private Slider interval;
    [SerializeField] private TMP_Text intervalText;

    void Awake()
    {
        antiIdle.onValueChanged.AddListener(on => AntiIdleUtensilSpawner.disabled = !on);
        interval.onValueChanged.AddListener(v =>
        {
            AntiIdleUtensilSpawner.cooldownOverride = v;
            ShowInterval();
        });
    }

    void OnEnable()
    {
        antiIdle.SetIsOnWithoutNotify(!AntiIdleUtensilSpawner.disabled);
        if (AntiIdleUtensilSpawner.cooldownOverride >= 0) interval.SetValueWithoutNotify(AntiIdleUtensilSpawner.cooldownOverride);
        ShowInterval();
    }

    void ShowInterval() => intervalText.text = AntiIdleUtensilSpawner.cooldownOverride < 0
        ? "DEFAULT"
        : $"{AntiIdleUtensilSpawner.cooldownOverride:0.0}s";
}
