using UnityEngine;
using UnityEngine.InputSystem; // new input system

public class FruitFollow : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Camera cam;

    //0:topright 1:bottomleft
    [SerializeField]
    private Transform[] boundaries;

    [SerializeField]
    private Vector2 padding;

    private Vector3 LastPos = Vector2.zero;

    [SerializeField]
    private float rotationThresholdMagnitude;
    [SerializeField]
    private float angularSpeedFactor;


    private Rigidbody2D rb;
    private Animator anim;
    
    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // get current mouse position from new Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        if (Mathf.Abs(Vector3.Distance(LastPos, worldPos)) >= rotationThresholdMagnitude) {

            print(LastPos);
            print(worldPos);

            print(Vector3.Distance(LastPos, worldPos));
            rb.AddTorque((angularSpeedFactor * Vector3.Magnitude(worldPos - LastPos)),ForceMode2D.Impulse);

            
        }

        // smoothly move fruit to cursor
        transform.position = Vector3.Lerp(transform.position, worldPos, moveSpeed * Time.deltaTime);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, boundaries[1].position.x +padding.x, boundaries[0].position.x-padding.x), Mathf.Clamp(transform.position.y, boundaries[1].position.y+padding.y, boundaries[0].position.y-padding.y));

        anim.SetFloat("velocity", ((Mathf.Approximately(Vector3.Distance(LastPos,worldPos),0))?0f:((Vector3.Distance(LastPos, worldPos)<0)?-1f:1f)));
        LastPos = worldPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash"))
        {
            Destroy(gameObject);
        }
    }
}
