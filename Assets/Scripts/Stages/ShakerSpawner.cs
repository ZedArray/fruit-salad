using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShakerSpawner : StageComponent<ShakerSpawner>
{
    public Shaker[] shakers;
    public Transform boundtr;
    public Transform boundbl;
    public Vector2 spawnTimeRange;
    public int maxShakers;
    private int currShakers;
    new void Awake()
    {
        base.Awake();
    }

    public override void Enable()
    {
        base.Enable();
        GlobalTimer.instance.AddTimer(new TimerRequest(Random.Range(spawnTimeRange.x,spawnTimeRange.y),TrySpawn,false));
    }

    public void TrySpawn()
    {
        if (!doStage || currShakers >= maxShakers)
        {
            return;
        }
        Vector2 location = new Vector2(Random.Range(boundbl.position.x,boundtr.position.x),Random.Range(boundbl.position.y,boundtr.position.y))
        Instantiate(shakers[Random.Range(0, shakers.Length)], new Vector2(location.x, 10),Quaternion.identity);
        

    }
}
