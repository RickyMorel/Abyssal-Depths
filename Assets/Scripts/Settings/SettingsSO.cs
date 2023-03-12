using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsSO", order = 5)]
public class SettingsSO : ScriptableObject
{
    public int AntiAliasIndex;

    public void SetAntiAliasing()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();

        foreach (Camera cam in cameras)
        {
            //cam.GetComponent<PostProcessLayer>();
        }
    }
}
