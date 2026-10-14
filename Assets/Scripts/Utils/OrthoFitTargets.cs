using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrthoFitTargets : MonoBehaviour
{
    [SerializeField] private GameObject[] _targets = new GameObject[4];
    [SerializeField] private float _padding = 1f;
    [SerializeField] private float _minSize = 1f;
    [SerializeField] private float _maxSize = 50f;
    [SerializeField] private float _smoothSpeed = 5f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographic = true;
    }

    private void LateUpdate()
    {
        float target = Mathf.Clamp(GetRequiredSize() + _padding, _minSize, _maxSize);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, target, _smoothSpeed * Time.deltaTime);
    }

    // Camera stays where it is; size grows until every target's bounds fit around the view center.
    private float GetRequiredSize()
    {
        float size = 0f;
        foreach (GameObject go in _targets)
        {
            if (go == null) continue;

            Bounds b = go.TryGetComponent(out Renderer r) ? r.bounds : new Bounds(go.transform.position, Vector3.zero);
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = new Vector3(
                    (i & 1) == 0 ? b.min.x : b.max.x,
                    (i & 2) == 0 ? b.min.y : b.max.y,
                    (i & 4) == 0 ? b.min.z : b.max.z);
                Vector3 local = transform.InverseTransformPoint(corner);
                size = Mathf.Max(size, Mathf.Abs(local.y), Mathf.Abs(local.x) / _camera.aspect);
            }
        }
        return size;
    }
}
