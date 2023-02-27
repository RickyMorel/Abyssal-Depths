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
        _damageMultipliers = GameAssetsManager.Instance.DamageType;
        if (damageType is DamageTypes.Base)
        {
            weakness = _damageMultipliers.Base[0];
            resistance = _damageMultipliers.Base[1];
            return;
        }
        if (damageType is DamageTypes.Fire)
        {
            weakness = _damageMultipliers.Fire[0];
            resistance = _damageMultipliers.Fire[1];
            return;
        }
        if (damageType is DamageTypes.Electric)
        {
            weakness = _damageMultipliers.Electric[0];
            resistance = _damageMultipliers.Electric[1];
            return;
        }
        if (damageType is DamageTypes.Laser)
        {
            weakness = _damageMultipliers.Laser[0];
            resistance = _damageMultipliers.Laser[1];
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