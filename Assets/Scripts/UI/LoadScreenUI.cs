using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreenUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _loadingBarSlider;

    #endregion

    #region Private Variables

    private static LoadScreenUI _instance;

    #endregion

    #region Public Properties

    public static LoadScreenUI Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    public void LoadScene(AsyncOperation operation)
    {
        StartCoroutine(LoadSceneAsync(operation));
    }

    private IEnumerator LoadSceneAsync(AsyncOperation operation)
    {
        _loadingPanel.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            _loadingBarSlider.value = progressValue;

            yield return null;
        }

        _loadingPanel.SetActive(false);
    }

}
