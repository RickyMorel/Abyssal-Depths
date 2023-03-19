using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponSO", order = 1)]
public class WeaponSO : ScriptableObject
{
    public string WeaponName;
    public GameObject ProjectilePrefab;
    public ChipType Socket_1;
    public ChipType Socket_2;
}
