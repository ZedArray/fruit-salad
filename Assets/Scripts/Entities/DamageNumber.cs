using TMPro;
using UnityEngine;

// floats up and fades out, then destroys itself
public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float riseSpeed = 1.5f;
    [Tooltip("{0} is the damage dealt.")]
    [SerializeField] private string format = "-{0}";
    private float age;

    public void init(int damage)
    {
        text.text = string.Format(format, damage);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        age += Time.deltaTime;
        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
        text.alpha = 1f - age / lifetime;
    }
}
