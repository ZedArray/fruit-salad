using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// binds a slider to the Settings volume field for its category; applies live, saves on release
[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private AudioCategory category;
    private Slider _slider;

    void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(v => UserManager.instance.settings.UpdateSetting($"{category}Volume", v, save: false));
    }

    void OnEnable() => _slider.SetValueWithoutNotify(SFXManager.VolumeFor(category));

    // touch release fires this even if the finger slid off the slider
    public void OnPointerUp(PointerEventData eventData) => SaveManager.SaveSettings(UserManager.instance.settings);
}
