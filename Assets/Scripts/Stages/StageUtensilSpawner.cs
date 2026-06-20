using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageUtensilSpawner : StageComponent<StageUtensilSpawner>

{

    public UtensilPatternData patterns;
    private float? spawnRad;

    new void Awake()
    {
        base.Awake();
    }
    

    void UpdateSpawnRad()
    {
        Vector3 corner = Camera.main.ViewportToWorldPoint(
            new Vector3(1, 1, Camera.main.nearClipPlane)
        );

        spawnRad = 3f + Vector2.Distance(
            Camera.main.transform.position,
            corner
        );
    }



    [ContextMenu("Test Spawn Utensil")]
    public void SpawnUtensilTest()
    {
        SpawnPatternByIndex(0, 3);
    }

    public override void Enable()
    {
        base.Enable();
        print("Works Lmao = " + doStage);
        if (spawnRad == null)
        {
            UpdateSpawnRad();
        }
        UtensilSpawnerWorker();

    }

    private float GetPatternTime()
    {
        //TODO: Populate this
        return 3f;
        
    }

    private float GetDelayBetweenPatterns()
    {
        //TODO: Populate this
        return 4f;
    }

    private void UtensilSpawnerWorker()
    {
        if (doStage == false) return;
        
        float patternTime = GetPatternTime(); 
        SpawnPatternByIndex(Random.Range(0,patterns.patterns.Length), patternTime);
        GlobalTimer.instance.AddTimer(new TimerRequest(patternTime+GetDelayBetweenPatterns(), UtensilSpawnerWorker));
    }

    public void SpawnPatternByIndex(int index, float totalPatternTime)
    {
        UtensilPattern pattern = patterns.patterns[index];
        foreach (var wave in pattern.utensilWaves)
        {
            GlobalTimer.instance.AddTimer(new TimerRequest(totalPatternTime*wave.relativeTimeToStart, () => {SpawnWave(wave);}));
        }


    }

    void SpawnWave(UtensilWave wave)
    {
        Vector2Pol point = new Vector2Pol(spawnRad.GetValueOrDefault(0f), (wave.angleRange.x+wave.angleOffset)*Mathf.Deg2Rad);
        float delta = (wave.angleRange.y - wave.angleRange.x)/(wave.utensilAmount);
        for(int i = 0; i < wave.utensilAmount; i++)
        {
            int toSpawn = i % wave.utensilTypes.Length;
            if (wave.randomize)
            {
                toSpawn = Random.Range(0, wave.utensilTypes.Length);
            }

            Arrow2D arr = Instantiate(wave.utensilTypes[toSpawn], (Vector2)point, Quaternion.identity);
            arr.speed = wave.utensilSpeed;
            if (wave.targetCenterNotPlayer)
            {
                arr.UpdateTarget(Vector3.zero);
            }
            else
            {
                arr.UpdateTarget(Fruit.instance.transform);
            }

            point.RotateByDegrees(delta);
        }
    }
}
