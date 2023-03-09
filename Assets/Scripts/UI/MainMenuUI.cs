using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _headerPanel;
    [SerializeField] private GameObject _playPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _loadPanel;
    [SerializeField] private GameObject _gameplaySettingsPanel;
    [SerializeField] private GameObject _videoSettingsPanel;
    [SerializeField] private GameObject _audioSettingsPanel;

    #endregion

    public void NewGame()
    {
        SceneManager.LoadScene("MainScene");
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
}
