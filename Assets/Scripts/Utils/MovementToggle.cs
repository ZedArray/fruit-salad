using UnityEngine;
using UnityEngine.UI;

// binds a toggle to Settings.JoystickMovement: on = floating joystick, off = tap to move
[RequireComponent(typeof(Toggle))]
public class MovementToggle : MonoBehaviour
{
    private Toggle _toggle;

    void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(on => SaveManager.LoadSettings().UpdateSetting(nameof(Settings.JoystickMovement), on));
    }

    void OnEnable() => _toggle.SetIsOnWithoutNotify(SaveManager.LoadSettings().JoystickMovement);
}
