using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent (typeof(Rigidbody))]  
public class SceneLoader : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private int _sceneToLoad = -1;

    #endregion

    #region Private Variables

    private bool _startedLoadingScene = false;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.gameObject.tag != "MainShip") { return; }

        if (_startedLoadingScene) { return; }

        LoadScene(_sceneToLoad);

        _startedLoadingScene = true;
    }

    public static void LoadScene(int sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        LoadScreenUI.Instance.LoadScene(operation);
    }
}
