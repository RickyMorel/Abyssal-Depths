using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent (typeof(Rigidbody))]  
public class SceneLoader : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private int _sceneToLoad = -1;
    [SerializeField] private Vector3 _shipPosition;

    #endregion

    #region Private Variables

    private bool _startedLoadingScene = false;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.gameObject.GetComponentInChildren<Ship>() == null) { return; }

        LoadScene(_sceneToLoad);
    }

    public void LoadScene(int sceneToLoad)
    {
        if (_startedLoadingScene) { return; }

        LoadSceneOperation(sceneToLoad);

        Ship.Instance.transform.position = _shipPosition;
    }

    public static void LoadSceneOperation(int sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        LoadScreenUI.Instance.LoadScene(operation);
    }

    public static bool IsInGarageScene()
    {
        return SceneManager.GetActiveScene().name == "SpaceStations";
    }
}
