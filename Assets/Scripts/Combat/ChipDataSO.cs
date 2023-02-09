using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

[CreateAssetMenu(fileName = "Data", menuName = "Combat/Chip")]

public class ChipDataSO : ScriptableObject
{
    public List<ChipData> ChipData = new List<ChipData>();

    #region Editor
#if UNITY_EDITOR

    [CustomEditor(typeof(ChipDataSO))]
    public class ChipDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

#endif
    #endregion
}
[System.Serializable]
public class ChipData
{
    public ChipType Chiptype;
    public int ChipLevel;
    public float ShootAfterSeconds;
    public int Damage;
    public int DamageMultiplierWeakness;
    public int DamageMultiplierResistance;
}