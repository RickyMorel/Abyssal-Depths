using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "AbyssalDepths/DayNightSO", order = 1)]
public class DayNightSO : ScriptableObject
{
    public List<Color> DayLightColors = new List<Color>();
    public List<Color> NightLightColors = new List<Color>();
    public float LowFogDensity;
    public float HighFogDensity;

    public void SetLightColors(Light dirLight_1, Light dirLight_2, Light dirLight_3, bool IsNight)
    {
        dirLight_1.color = IsNight ? NightLightColors[0] : DayLightColors[0];
        dirLight_2.color = IsNight ? NightLightColors[1] : DayLightColors[1];
        dirLight_3.color = IsNight ? NightLightColors[2] : DayLightColors[2];

        RenderSettings.fogColor = IsNight ? NightLightColors[3] : DayLightColors[3];
    }
}
