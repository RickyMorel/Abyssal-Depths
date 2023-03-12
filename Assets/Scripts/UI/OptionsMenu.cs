using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssalDepths.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        #region Editor Fields

        [SerializeField] private SettingsSO _defaultSettingsSO;
        [SerializeField] private OptionSelectorButton _resolutionSelector;
        [SerializeField] private OptionSelectorButton _frameRateLimitSelector;

        #endregion

        #region Private Variables

        private Resolution[] _resolutions;
        private FrameRateLimits _frameRateLimit;

        #endregion

        private void Awake()
        {
            PopulateResolutionsSelector();
            PopulateFrameRateRestrictionSelector();
        }

        private void Start()
        {
            SetResolution(_defaultSettingsSO.ResolutionIndex);
            SetFrameRate(_defaultSettingsSO.FrameRateIndex);
            SetQuality(_defaultSettingsSO.QualityIndex);
            SetFullScreen(_defaultSettingsSO.FullScreenIndex);
            SetVsync(_defaultSettingsSO.VsyncIndex);
            SetAntiAliasing(_defaultSettingsSO.AntiAliasIndex);
            SetTextureQuality(_defaultSettingsSO.TextureQualityIndex);
            SetShadowsQuality(_defaultSettingsSO.ShadowQualityIndex);
            SetVolume(_defaultSettingsSO.VolumePercentage);
        }

        private void PopulateResolutionsSelector()
        {
            _resolutions = Screen.resolutions;
            List<string> options = new List<string>();

            int currentResolutionIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height) { currentResolutionIndex = i; }
            }

            _resolutionSelector.InitializeOptions(options.ToArray());
            _resolutionSelector.SetIndex(currentResolutionIndex);
        }

        private void PopulateFrameRateRestrictionSelector()
        {
            FrameRateLimits[] allSomeEnumValues = (FrameRateLimits[])Enum.GetValues(typeof(FrameRateLimits));
            List<string> limitsList = new List<string>();

            foreach (FrameRateLimits limit in allSomeEnumValues)
            {
                int limitValue = (int)limit;
                string wantedString = " ";

                if (limitValue == 0) { wantedString = "Unlimited"; }
                else { wantedString = limitValue.ToString(); }

                limitsList.Add(wantedString);
            }

            _frameRateLimitSelector.InitializeOptions(limitsList.ToArray());
            _frameRateLimitSelector.SetIndex(0);
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];

            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetFrameRate(int index)
        {
            switch (index)
            {
                case 0:
                    _frameRateLimit = FrameRateLimits.Unlimited;
                    break;
                case 1:
                    _frameRateLimit = FrameRateLimits.limit30;
                    break;
                case 2:
                    _frameRateLimit = FrameRateLimits.limit60;
                    break;
                case 3:
                    _frameRateLimit = FrameRateLimits.limit90;
                    break;
                case 4:
                    _frameRateLimit = FrameRateLimits.limit120;
                    break;
                case 5:
                    _frameRateLimit = FrameRateLimits.limit144;
                    break;
                case 6:
                    _frameRateLimit = FrameRateLimits.limit240;
                    break;
            }

            Application.targetFrameRate = (int)_frameRateLimit;
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullScreen(int isFullScreen)
        {
            bool isFullScreenBool = isFullScreen == 0 ? true : false;

            Screen.fullScreen = isFullScreenBool;
        }

        public void SetVsync(int isEnabled)
        {
            int vSyncCount = isEnabled == 0 ? 1 : 0;
            QualitySettings.vSyncCount = vSyncCount;
        }

        public void SetAntiAliasing(int index)
        {
            if (index == 2)
                QualitySettings.antiAliasing = 2;
            else if (index == 3)
                QualitySettings.antiAliasing = 4;
            else if (index == 4)
                QualitySettings.antiAliasing = 8;
            else
                QualitySettings.antiAliasing = 0; // If none of FXAA turn off MSAA
        }

        public void SetTextureQuality(int index)
        {
            if (index == 0)         //Full res
                QualitySettings.masterTextureLimit = 0;
            else if (index == 1)    //1/2 res    
                QualitySettings.masterTextureLimit = 1;
            else                    //1/4 res
                QualitySettings.masterTextureLimit = 2;
        }

        public void SetShadowsQuality(int index)
        {
            switch (index)
            {
                case 0://Very Low
                    ChangeShadows(ShadowmaskMode.Shadowmask, ShadowQuality.Disable, ShadowResolution.Low, ShadowProjection.StableFit, 15, 3, 0);
                    break;
                case 1://Low
                    ChangeShadows(ShadowmaskMode.Shadowmask, ShadowQuality.HardOnly, ShadowResolution.Low, ShadowProjection.StableFit, 20, 3, 0);
                    break;
                case 2://Medium
                    ChangeShadows(ShadowmaskMode.DistanceShadowmask, ShadowQuality.All, ShadowResolution.Medium, ShadowProjection.StableFit, 40, 3, 2);
                    break;
                case 3://High
                    ChangeShadows(ShadowmaskMode.DistanceShadowmask, ShadowQuality.All, ShadowResolution.High, ShadowProjection.StableFit, 70, 3, 4);
                    break;
                case 4://Very High
                    ChangeShadows(ShadowmaskMode.DistanceShadowmask, ShadowQuality.All, ShadowResolution.VeryHigh, ShadowProjection.StableFit, 150, 3, 4);
                    break;
            }
        }

        private void ChangeShadows(ShadowmaskMode mask, ShadowQuality quality, ShadowResolution res,
            ShadowProjection projection, float shadowDistance, float shadowNearPlaneOffset, int shadowCascades)
        {
            QualitySettings.shadowmaskMode = mask;
            QualitySettings.shadows = quality;
            QualitySettings.shadowResolution = res;
            QualitySettings.shadowProjection = projection;
            QualitySettings.shadowDistance = shadowDistance;
            QualitySettings.shadowNearPlaneOffset = shadowNearPlaneOffset;
            QualitySettings.shadowCascades = shadowCascades;
        }

        public void SetVolume(float volume)
        {

        }
    }

    public enum FrameRateLimits
    {
        Unlimited = 0,
        limit30 = 30,
        limit60 = 60,
        limit90 = 90,
        limit120 = 120,
        limit144 = 144,
        limit240 = 240,
    }
}
