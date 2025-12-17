using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // new input system
using UnityEngine.InputSystem.Android;
using UnityEngine.SceneManagement;

public class Fruit : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Camera cam;

    //0:topright 1:bottomleft
    [SerializeField]
    private Transform[] boundaries;

    [SerializeField]
    private Transform shadow;
    private Vector3 shadowOffset;
    [SerializeField]
    private GameObject warning;

    [SerializeField]
    private Vector2 padding;

    private Vector3 LastPos = Vector2.zero;

    [SerializeField]
    private float rotationThresholdMagnitude;
    [SerializeField]
    private float angularSpeedFactor;

    [SerializeField]
    private TextMeshProUGUI coinCounter;
    [SerializeField]
    private scoreCounter sc;

    private Rigidbody2D rb;
    private Animator anim;
    private int coinCaught;
    public static Fruit instance;
    public static bool dead = false;
    public static float idle;

    [SerializeField] private bool godMode = false;
    void Start()
    {
        instance = this;
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shadowOffset = shadow.position - this.transform.position;
        dead = false;
        idle = 0f;
        coinCaught = 0;
        #if !UNITY_EDITOR
            godMode = false;
        #endif
    }

    void Update()
    {

        if (dead)
        {
            return;
        }
        // get current mouse position from new Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();
        //Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        Vector3 worldPos = transform.position;
        worldPos.z = 0f;

        var gamepad = AndroidJoystick.current;

        //print(gamepad.stick.ReadValue());

        transform.position += moveSpeed * Time.deltaTime * new Vector3(gamepad.stick.ReadValue().x, gamepad.stick.ReadValue().y, 0);

        if (Mathf.Abs(Vector3.Distance(LastPos, worldPos)) >= rotationThresholdMagnitude)
        {

            rb.AddTorque((angularSpeedFactor * Vector3.Magnitude(worldPos - LastPos)), ForceMode2D.Impulse);

        }

        // smoothly move fruit to cursor
        //transform.position = Vector3.Lerp(transform.position, worldPos, moveSpeed * Time.deltaTime);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, boundaries[1].position.x + padding.x, boundaries[0].position.x - padding.x), Mathf.Clamp(transform.position.y, boundaries[1].position.y + padding.y, boundaries[0].position.y - padding.y));

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
        shadow.position = transform.position+shadowOffset;
        LastPos = worldPos;

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") && !godMode && !dead)
        {
            dead = true;
            dieAnim();
        }
        if (collision.CompareTag("Coin") && !dead)
        {
            Destroy(collision.gameObject);
            coinCaught += 1;
            coinCounter.text = coinCaught.ToString();
            sc.TryAddScore();
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
