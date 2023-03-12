using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsSO", order = 5)]
public class SettingsSO : ScriptableObject
{
    public int ResolutionIndex;
    public int FrameRateIndex;
    public int QualityIndex;
    public int FullScreenIndex;
    public int VsyncIndex;
    public int AntiAliasIndex;
    public int TextureQualityIndex;
    public int ShadowQualityIndex;
    public float VolumePercentage;
}
