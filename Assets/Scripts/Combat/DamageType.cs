using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/DamageType")]
public class DamageType : ScriptableObject
{
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Base = new Vector2(2, 3);
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Laser = new Vector2(2, 0);
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Electric = new Vector2(2.5f, 0);
    [Tooltip("X is weakness, Y resistance")]
    public Vector2 Fire = new Vector2(3, 3);
    public void GetWeaknessAndResistance(DamageTypes damageType, out float weakness, out float resistance)
    {
        if (damageType is DamageTypes.Base)
        {
            weakness = Base[0];
            resistance = Base[1];
            return;
        }
        if (damageType is DamageTypes.Fire)
        {
            weakness = Fire[0];
            resistance = Fire[1];
            return;
        }
        if (damageType is DamageTypes.Electric)
        {
            weakness = Electric[0];
            resistance = Electric[1];
            return;
        }
        if (damageType is DamageTypes.Laser)
        {
            weakness = Laser[0];
            resistance = Laser[1];
            return;
        }
        weakness = 0;
        resistance = 0;
        return;
    }
}

public enum DamageTypes
{
    None,
    Base,
    Fire,
    Laser,
    Electric
}