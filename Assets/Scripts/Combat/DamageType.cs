using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/DamageType")]
public class DamageMultipliers : ScriptableObject
{
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Base = new Vector2(2, 3);
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Laser = new Vector2(2, 0);
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Electric = new Vector2(2.5f, 0);
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Fire = new Vector2(3, 3);
}

public enum DamageType
{
    None,
    Base,
    Fire,
    Laser,
    Electric
}