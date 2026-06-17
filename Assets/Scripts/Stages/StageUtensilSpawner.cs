using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageUtensilSpawner : StageComponent
{
    
    public static StageUtensilSpawner instance;
    public UtensilPatternData patterns;
    private float? spawnRad;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void UpdateSpawnRad()
    {
        Vector3 corner = Camera.main.ViewportToWorldPoint(
            new Vector3(1, 1, Camera.main.nearClipPlane)
        );

        spawnRad = 3f+Vector2.Distance(
            Camera.main.transform.position,
            corner
        );
    }
    
    

    [ContextMenu("Test Spawn Utensil")] 
    public void SpawnUtensilTest(){
        SpawnPatternByIndex(0, 3);
    }

    public void SpawnPatternByIndex(int index, float totalPatternTime)
    {
        UtensilPattern pattern = patterns.patterns[index];
        if (spawnRad == null)
        {
            UpdateSpawnRad();
        }

        foreach (var wave in pattern.utensilWaves)
        {
            if (doStage == false) return;
            GlobalTimer.instance.AddTimer(new TimerRequest(totalPatternTime*wave.relativeTimeToStart, () => {SpawnWave(wave);}));
        }


    }

    void SpawnWave(UtensilWave wave)
    {
        if (doStage == false) return;
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
