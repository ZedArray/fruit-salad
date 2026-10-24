using System;
using CompositeCurves;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShakerSpawner : StageComponent<ShakerSpawner>
{
    [Header("Spawning")]
    public Shaker[] shakers;
    public Transform boundtr;
    public Transform boundbl;
    public Vector2 spawnTimeRange;
    public int maxShakers;
    public float spawnHeight = 10f;

    [Header("Shaker")]
    [Tooltip("Health by spawn number this run (x = 0 for the first shaker, 1 for the next...).")]
    public CompositeCurveDefinition healthCurve;
    public float fallSpeed = 15f;
    public float landShake = 0.1f;
    [Tooltip("Seconds between the killing hit and despawn, so the hurt animation can play.")]
    public float despawnDelay = 0.15f;

    [Header("Landing warning")]
    [Tooltip("Needs a trigger Collider2D; overlapping it on landing damages the fruit.")]
    public GameObject warningZone;
    [Tooltip("Seconds the warning shows before the shaker starts falling (fall time adds to this).")]
    public float warningTime = 1f;
    public int landDamage = 1;

    [Header("Coin drop")]
    public GameObject coin;
    public int coinsDropped = 3;
    public float coinScatter = 1f;

    [NonSerialized] public int currShakers;
    private int spawned;
    private bool scheduled;

    new void Awake()
    {
        base.Awake();
    }

    public override void Enable()
    {
        base.Enable();
        // a timer from a previous Enable may still be pending; don't start a second chain
        if (!scheduled) Schedule();
    }

    void Schedule()
    {
        scheduled = true;
        GlobalTimer.instance.AddTimer(new TimerRequest(Random.Range(spawnTimeRange.x, spawnTimeRange.y), TrySpawn));
    }

    public void TrySpawn()
    {
        scheduled = false;
        if (!doStage || Fruit.dead) return;
        Schedule();
        if (currShakers >= maxShakers || shakers.Length == 0) return;

        Vector2 location = new Vector2(Random.Range(boundbl.position.x, boundtr.position.x),
            Random.Range(boundbl.position.y, boundtr.position.y));
        Shaker s = Instantiate(shakers[Random.Range(0, shakers.Length)], new Vector2(location.x, spawnHeight), Quaternion.identity);
        // at least 1: a 0-health shaker could never be broken (TakeDamage ignores already-dead ones)
        s.init(this, location.y, Mathf.Max(1, Mathf.RoundToInt(healthCurve.Evaluate(spawned))));
        spawned++;
        currShakers++;
    }

    public Vector2 ClampToBounds(Vector2 pos)
    {
        return new Vector2(Mathf.Clamp(pos.x, boundbl.position.x, boundtr.position.x),
            Mathf.Clamp(pos.y, boundbl.position.y, boundtr.position.y));
    }
}
