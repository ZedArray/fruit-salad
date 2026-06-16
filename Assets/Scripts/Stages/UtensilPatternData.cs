using LazySquirrelLabs.MinMaxRangeAttribute;
using UnityEngine;

[System.Serializable]
public struct UtensilWave
{
    [SerializeField] private string name;
    public int utensilAmount;
    public float utensilSpeed;
    public bool randomize;
    public bool targetCenterNotPlayer;
    [MinMaxRange(0,360)] public Vector2 angleRange;
    [Range(0,360)] public float angleOffset;
    [Range(0, 1)] public float relativeTimeToStart;
    public Arrow2D[] utensilTypes;
}

[System.Serializable]
public struct UtensilPattern
{
    [SerializeField] private string name;
    public UtensilWave[] utensilWaves;
}

[CreateAssetMenu(fileName = "UtensilPatternData", menuName = "Scriptable Objects/UtensilPatternData")]
public class UtensilPatternData : ScriptableObject
{
    public UtensilPattern[] patterns;
}
