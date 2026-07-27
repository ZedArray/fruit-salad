using UnityEngine;
using UnityEngine.UI;

// Attach this to the UI Image (RawImage or Image) that uses the "UI/CutoutMask" material.
// It animates the hole size by driving the shader's _Scale property.
//
// Setup:
// 1. Create a fullscreen UI Image, set its Material to a material using "UI/CutoutMask".
// 2. Assign your hole-shape sprite (e.g. a star, circle, logo) as _MainTex on that material.
// 3. Attach this script to the same GameObject.
// 4. Optionally drive Progress (0-1) from your actual loading state instead of auto-pulsing.

[RequireComponent(typeof(Graphic))]
public class CutoutLoadingScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Graphic targetGraphic; // Image or RawImage using the CutoutMask material

    [Header("Hole Size Range")]
    [Tooltip("Shader _Scale value when the hole is smallest")]
    [SerializeField] private float minScale = 0.5f;
    [Tooltip("Shader _Scale value when the hole is largest")]
    [SerializeField] private float maxScale = 3f;

    [Header("Auto Pulse (disable if driving manually)")]
    [SerializeField] private bool autoPulse = true;
    [SerializeField] private float pulseSpeed = 1f;

    private Material _materialInstance;
    private static readonly int ScaleProp = Shader.PropertyToID("_Scale");

    private void Awake()
    {
        if (targetGraphic == null)
            targetGraphic = GetComponent<Graphic>();

        // Instance the material so multiple loading screens don't fight over shared properties
        _materialInstance = Instantiate(targetGraphic.material);
        targetGraphic.material = _materialInstance;
    }

    private void Update()
    {
        if (!autoPulse) return;

        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f; // 0..1
        SetProgress(t);
    }

    /// <summary>
    /// Manually drive the hole size. 0 = smallest hole, 1 = largest hole.
    /// Call this from your actual loading progress if you don't want auto-pulsing.
    /// </summary>
    public void SetProgress(float t01)
    {
        t01 = Mathf.Clamp01(t01);
        float scale = Mathf.Lerp(minScale, maxScale, t01);
        _materialInstance.SetFloat(ScaleProp, scale);
    }
}
