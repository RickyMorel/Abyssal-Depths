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
        if(other.transform.root.gameObject.tag != "MainShip") { return; }

        if (_startedLoadingScene) { return; }

        LoadScene(_sceneToLoad);

        Ship.Instance.transform.position = _shipPosition;

        _startedLoadingScene = true;
    }

    public static void LoadScene(int sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        LoadScreenUI.Instance.LoadScene(operation);
    }

    public static bool IsInGarageScene()
    {
        return SceneManager.GetActiveScene().name == "SpaceStations";
    }
}
