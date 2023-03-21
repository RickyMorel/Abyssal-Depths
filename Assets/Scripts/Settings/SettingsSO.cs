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
    public float MasterVolumePercentage;
    public float MusicVolumePercentage;
    public float DialogueVolumePercentage;
    public float SfxVolumePercentage;
    public float AmbientVolumePercentage;

    public SettingsSO(SettingsSO settingsSO)
    {
        ResolutionIndex = settingsSO.ResolutionIndex;
        FrameRateIndex = settingsSO.FrameRateIndex;
        QualityIndex = settingsSO.QualityIndex;
        FullScreenIndex = settingsSO.FullScreenIndex;
        VsyncIndex = settingsSO.VsyncIndex;
        AntiAliasIndex = settingsSO.AntiAliasIndex;
        TextureQualityIndex = settingsSO.TextureQualityIndex;
        ShadowQualityIndex = settingsSO.ShadowQualityIndex;
        MasterVolumePercentage = settingsSO.MasterVolumePercentage;
        MusicVolumePercentage = settingsSO.MusicVolumePercentage;
        DialogueVolumePercentage = settingsSO.DialogueVolumePercentage;
        SfxVolumePercentage = settingsSO.SfxVolumePercentage;
        AmbientVolumePercentage = settingsSO.AmbientVolumePercentage;
    }
}

[System.Serializable]
public class SettingsData
{
    public int ResolutionIndex;
    public int FrameRateIndex;
    public int QualityIndex;
    public int FullScreenIndex;
    public int VsyncIndex;
    public int AntiAliasIndex;
    public int TextureQualityIndex;
    public int ShadowQualityIndex;
    public float MasterVolumePercentage;
    public float MusicVolumePercentage;
    public float DialogueVolumePercentage;
    public float SfxVolumePercentage;
    public float AmbientVolumePercentage;

    public SettingsData(SettingsSO settingsSO)
    {
        ResolutionIndex = settingsSO.ResolutionIndex;
        FrameRateIndex = settingsSO.FrameRateIndex;
        QualityIndex = settingsSO.QualityIndex;
        FullScreenIndex = settingsSO.FullScreenIndex;
        VsyncIndex = settingsSO.VsyncIndex;
        AntiAliasIndex = settingsSO.AntiAliasIndex;
        TextureQualityIndex = settingsSO.TextureQualityIndex;
        ShadowQualityIndex = settingsSO.ShadowQualityIndex;
        MasterVolumePercentage = settingsSO.MasterVolumePercentage;
        MusicVolumePercentage = settingsSO.MusicVolumePercentage;
        DialogueVolumePercentage = settingsSO.DialogueVolumePercentage;
        SfxVolumePercentage = settingsSO.SfxVolumePercentage;
        AmbientVolumePercentage = settingsSO.AmbientVolumePercentage;
    }
}
