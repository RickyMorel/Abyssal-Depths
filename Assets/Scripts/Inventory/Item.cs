using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectIdAttribute : PropertyAttribute { }

//Generates new id
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
        }
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    [ScriptableObjectId]
    public string Id;
    public Sprite Icon;
    public string DisplayName;
    public string Description;
    public int Value;
    public GameObject ItemPrefab;
    public GameObject ItemPickupPrefab;
    public int ItemSize;
    public bool IsSingleHold = false;

    public GameObject SpawnItemPickup(Transform spawnTransform)
    {
        GameObject itemInstance = Instantiate(ItemPickupPrefab, spawnTransform.position, spawnTransform.rotation);

        return itemInstance;
    }

    public string GetItemName()
    {
        string name = DisplayName;

        if(this is UpgradeChip)
        {
            UpgradeChip chip = (UpgradeChip)this;
            chip.GetChipName();
        }

        return name;
    }
}
