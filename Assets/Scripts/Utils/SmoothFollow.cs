using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private bool _useInitialOffset; 
    [SerializeField] private Vector3 _offset;     
    [SerializeField] private float _smoothSpeed = 5f;

    private void Start()
    {
        if (_useInitialOffset) {
            _offset = transform.position-_target.position ;
        }
    }
    private void LateUpdate()
    {
        FollowPlayer();
    }
    public void SetTarget(Transform target)
    {
        this._target = target;
    }
    private void FollowPlayer()
    {
        if (_target == null)
        {
            Debug.LogWarning("Target Transform is not assigned in SmoothFollow!");
            return;
        }
        Vector3 targetPosition = _target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
    }
}
