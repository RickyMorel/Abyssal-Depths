using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "Buildings/Farm", order = 1)]
public class BuildingFarmSO : ScriptableObject
{
    public string DisplayName;
    public string Description;
    public List<ItemQuantity> ResourcesGenerated;
    public float GenerationMinutes = 10f;
}
