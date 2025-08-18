using UnityEngine;
using UnityEngine.InputSystem; // new input system

public class FruitFollow : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // get current mouse position from new Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        // smoothly move fruit to cursor
        transform.position = Vector3.Lerp(transform.position, worldPos, moveSpeed * Time.deltaTime);
    }
}
