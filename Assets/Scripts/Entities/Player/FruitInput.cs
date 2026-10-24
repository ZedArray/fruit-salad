using EnhancedOnScreenControls;
using UnityEngine;
using UnityEngine.InputSystem;

// turns touches (tap mode) or the floating on-screen stick (joystick mode) into a world-space target for Fruit to follow
public class FruitInput : MonoBehaviour
{
    // world units ahead of the fruit the stick aims at full tilt; top speed is roughly Fruit.moveSpeed * stickReach
    [SerializeField]
    private float stickReach = 1.5f;

    private Camera cam;
    private EnhancedOnScreenStick stick;
    public bool joystick { get; private set; }
    private float reach;

    public Vector3 Target { get; private set; }
    public bool Held { get; private set; }
    public bool PressedThisFrame { get; private set; }

    void Awake()
    {
        cam = Camera.main;
        var settings = SaveManager.LoadSettings();
        joystick = settings.JoystickMovement;
        reach = stickReach * settings.JoystickSensitivity;
        // the stick's canvas is inactive in the scene; only show it in joystick mode
        stick = FindAnyObjectByType<EnhancedOnScreenStick>(FindObjectsInactive.Include);
        if (stick != null) stick.GetComponentInParent<Canvas>(true).gameObject.SetActive(joystick);
    }

    // Fruit calls this each frame before moving
    public void Read()
    {
        var touch = Touchscreen.current?.primaryTouch;
        Held = touch != null && touch.press.isPressed;
        PressedThisFrame = Held && touch.press.wasPressedThisFrame;

        Vector3 target = transform.position;
        if (joystick)
        {
            // stick touches also land on primaryTouch, so position comes only from the stick here
            if (stick != null && stick.control is InputControl<Vector2> c)
                target += (Vector3)(c.ReadValue() * reach);
        }
        else if (Held)
        {
            target = cam.ScreenToWorldPoint(touch.position.ReadValue());
        }
        target.z = 0f;
        Target = target;
    }
}
