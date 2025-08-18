using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // new input system

public class Fruit : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Camera cam;

    //0:topright 1:bottomleft
    [SerializeField]
    private Transform[] boundaries;

    [SerializeField]
    private Transform shadow;

    [SerializeField]
    private GameObject warning;

    [SerializeField]
    private Vector2 padding;

    private Vector3 LastPos = Vector2.zero;

    [SerializeField]
    private float rotationThresholdMagnitude;
    [SerializeField]
    private float angularSpeedFactor;


    private Rigidbody2D rb;
    private Animator anim;
    public static Fruit instance;
    public static bool dead = false;
    public static float idle;
    void Start()
    {
        instance = this;
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        dead = false;
        idle = 0f;
    }

    void Update()
    {

        if (dead)
        {
            return;
        }
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


        if ((Mathf.Approximately(Vector3.Distance(LastPos, worldPos), 0)))
        {
            idle += Time.deltaTime;

            
        }
        else
        {
            idle = 0f;
        }

        warning.SetActive((idle>= UtensilSpawner.Instance.maxIdleTime));
        shadow.position = transform.position;
        LastPos = worldPos;

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash"))
        {
            dead = true;
            dieAnim();
        }
    }

    private void dieAnim() {
        CameraShake.shake(0.3f);
        anim.SetTrigger("Die");
    }

    public void animEnd() {
        Destroy(shadow.gameObject, 0.09f);
        Destroy(gameObject, 0.1f);
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene(0);
    }
}
