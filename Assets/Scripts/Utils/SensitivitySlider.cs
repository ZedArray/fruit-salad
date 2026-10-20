using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// binds a slider to Settings.JoystickSensitivity; applies live, saves on release
[RequireComponent(typeof(Slider))]
public class SensitivitySlider : MonoBehaviour, IPointerUpHandler
{
    private Slider _slider;

    void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(v => SaveManager.LoadSettings().UpdateSetting(nameof(Settings.JoystickSensitivity), v, save: false));
    }

    void OnEnable() => _slider.SetValueWithoutNotify(SaveManager.LoadSettings().JoystickSensitivity);

    // touch release fires this even if the finger slid off the slider
    public void OnPointerUp(PointerEventData eventData) => SaveManager.SaveSettings(SaveManager.LoadSettings());
}
