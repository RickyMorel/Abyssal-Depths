using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace AbyssalDepths.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Editor Fields

        [Header("Panels")]
        [SerializeField] private GameObject _headerPanel;
        [SerializeField] private GameObject _playPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _newGamePanel;
        [SerializeField] private GameObject _onScreenKeyboard;
        [SerializeField] private GameObject _loadPanel;
        [SerializeField] private GameObject _gameplaySettingsPanel;
        [SerializeField] private GameObject _videoSettingsPanel;
        [SerializeField] private GameObject _audioSettingsPanel;

        [Header("Buttons")]
        [SerializeField] private GameObject _loadGameMenuButtonPrefab;

        [Header("Transforms")]
        [SerializeField] private Transform _loadMenuContentTransform;

        [Header("Misc")]
        [SerializeField] private TMP_InputField _newGameNameText;

        #endregion

        private void Start()
        {
            LoadGame();
        }

        public void NewGame()
        {
            if(_newGameNameText.text == string.Empty) { return; }

            int saveIndex = SaveSystem.CreateNewSave(_newGameNameText.text);
            CreateLoadObj(saveIndex);

            EnableNewGamePanel(false);
            SceneManager.LoadScene(1);
        }

        public static void CreateLoadObj(int saveIndex)
        {
            GameObject saveDataObj = new GameObject($"Loaded Data:{saveIndex}");
            saveDataObj.tag = "LoadIndex";
            DontDestroyOnLoad(saveDataObj);
        }

        public void EnableNewGamePanel(bool isEnabled)
        {
            DisableAllPanels();

            _newGamePanel.SetActive(isEnabled);
            
            EnableOnScreenKeyboard(isEnabled);
        }

        public void EnableOnScreenKeyboard(bool isEnabled)
        {
            _onScreenKeyboard.SetActive(isEnabled);
        }

        public void LoadGame()
        {
            DestroyPrevLoadGameButtons();

            List<SaveData> allSaves = SaveSystem.LoadAllSaves();

            if(allSaves.Count < 1) { return; }

            int i = 0;
            foreach (SaveData save in allSaves)
            {
                GameObject loadButtonInstance = Instantiate(_loadGameMenuButtonPrefab, _loadMenuContentTransform);
                LoadGameMenuButton loadButtonInstanceScript = loadButtonInstance.GetComponent<LoadGameMenuButton>();
                loadButtonInstanceScript.DisplayData(save, i);
                i++;
            }
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
            _newGamePanel.SetActive(false);
            _onScreenKeyboard.SetActive(false);
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