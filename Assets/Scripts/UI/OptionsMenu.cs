using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbyssalDepths.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        #region Editor Fields

        [Header("SO's")]
        [SerializeField] private SettingsSO _defaultSettingsSO;

        [Header("Video Buttons")]
        [SerializeField] private OptionSelectorButton _resolutionSelector;
        [SerializeField] private OptionSelectorButton _fullscreenSelector;
        [SerializeField] private OptionSelectorButton _frameRateLimitSelector;
        [SerializeField] private OptionSelectorButton _vsyncSelector;
        [SerializeField] private OptionSelectorButton _brightnessSelector;
        [SerializeField] private OptionSelectorButton _qualitySelector;
        [SerializeField] private OptionSelectorButton _textureQualitySelector;
        [SerializeField] private OptionSelectorButton _shadowQualitySelector;
        [SerializeField] private OptionSelectorButton _antiAliasingSelector;

        [Header("Audio Buttons")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _dialogueVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _ambientVolumeSlider;

        #endregion

        #region Private Variables

        private Resolution[] _resolutions;
        private FrameRateLimits _frameRateLimit;
        private SettingsData _finalSettings;

        #endregion

        private void Awake()
        {
            SettingsData savedSettings = SaveSystem.LoadSettings();
            _finalSettings = savedSettings != null ? savedSettings: new SettingsData(_defaultSettingsSO);

            PopulateResolutionsSelector();
            PopulateFrameRateRestrictionSelector();
        }

        private void Start()
        {
            LoadSettings(_finalSettings);
        }

        public void LoadSettings(SettingsData settingsData)
        {
            SetResolution(settingsData.ResolutionIndex);
            SetFrameRate(settingsData.FrameRateIndex);
            SetQuality(settingsData.QualityIndex);
            SetFullScreen(settingsData.FullScreenIndex);
            SetVsync(settingsData.VsyncIndex);
            SetAntiAliasing(settingsData.AntiAliasIndex);
            SetTextureQuality(settingsData.TextureQualityIndex);
            SetShadowsQuality(settingsData.ShadowQualityIndex);
            SetVolume(settingsData.MasterVolumePercentage, 0);
            SetVolume(settingsData.MusicVolumePercentage, 1);
            SetVolume(settingsData.DialogueVolumePercentage, 2);
            SetVolume(settingsData.SfxVolumePercentage, 3);
            SetVolume(settingsData.AmbientVolumePercentage, 4);
        }

        public void Apply()
        {

            SaveSystem.SaveSettings(_finalSettings);
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
                else if (limitValue == 1) { wantedString = $"Monitor Rate({Screen.currentResolution.refreshRate})"; }
                else { wantedString = limitValue.ToString(); }

                limitsList.Add(wantedString);
            }

            _frameRateLimitSelector.InitializeOptions(limitsList.ToArray());
        }

        public void SetResolution(int resolutionIndex)
        {
            int wantedIndex = resolutionIndex == -1 ? _resolutions.Length - 1 : resolutionIndex;

            _finalSettings.ResolutionIndex = wantedIndex;
            _resolutionSelector.SetIndex(wantedIndex, false);

            if (_resolutions.Length - 1 < wantedIndex) { return; }

            Resolution resolution = _resolutions[wantedIndex];

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
                    _frameRateLimit = FrameRateLimits.Monitor;
                    break;
                case 2:
                    _frameRateLimit = FrameRateLimits.limit30;
                    break;
                case 3:
                    _frameRateLimit = FrameRateLimits.limit60;
                    break;
                case 4:
                    _frameRateLimit = FrameRateLimits.limit90;
                    break;
                case 5:
                    _frameRateLimit = FrameRateLimits.limit120;
                    break;
                case 6:
                    _frameRateLimit = FrameRateLimits.limit144;
                    break;
                case 7:
                    _frameRateLimit = FrameRateLimits.limit240;
                    break;
            }

            _finalSettings.FrameRateIndex = index;
            _frameRateLimitSelector.SetIndex(index, false);

            int wantedFrameRate = _frameRateLimit == FrameRateLimits.Monitor ? Screen.currentResolution.refreshRate : (int)_frameRateLimit;

            Application.targetFrameRate = wantedFrameRate;
        }

        public void SetQuality(int qualityIndex)
        {
            _finalSettings.QualityIndex = qualityIndex;
            _qualitySelector.SetIndex(qualityIndex, false);

            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullScreen(int isFullScreen)
        {
            _finalSettings.FullScreenIndex = isFullScreen;
            _fullscreenSelector.SetIndex(isFullScreen, false);

            bool isFullScreenBool = isFullScreen == 0 ? true : false;

            Screen.fullScreen = isFullScreenBool;
        }

        public void SetVsync(int isEnabled)
        {
            _finalSettings.VsyncIndex = isEnabled;
            _vsyncSelector.SetIndex(isEnabled, false);

            int vSyncCount = isEnabled == 0 ? 1 : 0;
            QualitySettings.vSyncCount = vSyncCount;
        }

        public void SetAntiAliasing(int index)
        {
            _finalSettings.AntiAliasIndex = index;
            _antiAliasingSelector.SetIndex(index, false);

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
            _finalSettings.TextureQualityIndex = index;
            _textureQualitySelector.SetIndex(index, false);

            if (index == 0)         //Full res
                QualitySettings.masterTextureLimit = 0;
            else if (index == 1)    //1/2 res    
                QualitySettings.masterTextureLimit = 1;
            else                    //1/4 res
                QualitySettings.masterTextureLimit = 2;
        }

        public void SetShadowsQuality(int index)
        {
            _finalSettings.ShadowQualityIndex = index;
            _shadowQualitySelector.SetIndex(index, false);

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

        public void SetVolume(float volume, int mixerIndex)
        {
            switch (mixerIndex)
            {
                case 0:
                    _finalSettings.MasterVolumePercentage = volume;
                    _masterVolumeSlider.value = volume;
                    break;
                case 1:
                    _finalSettings.MusicVolumePercentage = volume;
                    _musicVolumeSlider.value = volume;
                    break;
                case 2:
                    _finalSettings.DialogueVolumePercentage = volume;
                    _dialogueVolumeSlider.value = volume;
                    break;
                case 3:
                    _finalSettings.SfxVolumePercentage = volume;
                    _sfxVolumeSlider.value = volume;
                    break;
                case 4:
                    _finalSettings.AmbientVolumePercentage = volume;
                    _ambientVolumeSlider.value = volume;
                    break;
            }
        }
    }

    public enum FrameRateLimits
    {
        Unlimited = 0,
        Monitor = 1,
        limit30 = 30,
        limit60 = 60,
        limit90 = 90,
        limit120 = 120,
        limit144 = 144,
        limit240 = 240,
    }
}
