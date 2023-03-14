using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AbyssalDepths.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Editor Fields

        [Header("Panels")]
        [SerializeField] private GameObject _headerPanel;
        [SerializeField] private GameObject _playPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _loadPanel;
        [SerializeField] private GameObject _gameplaySettingsPanel;
        [SerializeField] private GameObject _videoSettingsPanel;
        [SerializeField] private GameObject _audioSettingsPanel;

        [Header("Buttons")]
        [SerializeField] private GameObject _loadGameMenuButtonPrefab;

        [Header("Transforms")]
        [SerializeField] private Transform _loadMenuContentTransform;

        #endregion

        private void Start()
        {
            LoadGame();
        }

        public void NewGame()
        {
            SceneManager.LoadScene(1);
        }

        public void LoadGame()
        {
            DestroyPrevLoadGameButtons();

            SaveData saveData = SaveSystem.Load();

            if(saveData == null) { return; }

            GameObject loadButtonInstance = Instantiate(_loadGameMenuButtonPrefab, _loadMenuContentTransform);
            LoadGameMenuButton loadButtonInstanceScript = loadButtonInstance.GetComponent<LoadGameMenuButton>();

            loadButtonInstanceScript.DisplayData(saveData);
        }

        private void DestroyPrevLoadGameButtons()
        {
            foreach (Transform child in _loadMenuContentTransform)
            {
                if(child == _loadMenuContentTransform) { continue; }

                Destroy(child.gameObject);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void DisableAllPanels()
        {
            _headerPanel.SetActive(false);
            _playPanel.SetActive(false);
            _settingsPanel.SetActive(false);
            _loadPanel.SetActive(false);
            _gameplaySettingsPanel.SetActive(false);
            _videoSettingsPanel.SetActive(false);
            _audioSettingsPanel.SetActive(false);
        }

        public void SelectPlayPanel()
        {
            DisableAllPanels();

            _playPanel.SetActive(true);
            _headerPanel.SetActive(true);
        }

        public void SelectSettingsPanel()
        {
            DisableAllPanels();

            _settingsPanel.SetActive(true);
            _headerPanel.SetActive(true);
        }

        public void SelectGameplayPanel()
        {
            DisableAllPanels();

            _gameplaySettingsPanel.SetActive(true);
        }

        public void SelectVideoPanel()
        {
            DisableAllPanels();

            _videoSettingsPanel.SetActive(true);
        }

        public void SelectAudioPanel()
        {
            DisableAllPanels();

            _audioSettingsPanel.SetActive(true);
        }

        public void SelectLoadPanel()
        {
            DisableAllPanels();

            _loadPanel.SetActive(true);
        }
    }
}
