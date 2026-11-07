using UnityEngine;

// all gameplay runs on scaled time, so timeScale alone freezes it
public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    private static RectTransform rect;

    void Awake() => rect = (RectTransform)transform;

    // the canvas is screen space overlay, so no camera
    public static bool Covers(Vector2 screenPoint) =>
        rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint);

    public void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        overlay.SetActive(paused);
    }
}
