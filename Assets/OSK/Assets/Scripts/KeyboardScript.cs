using Rewired.Integration.UnityUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardScript : MonoBehaviour
{
    #region Private Variables

    private RewiredEventSystem _eventSystem;

    #endregion

    #region EditorFields

    [SerializeField] private TMP_InputField _textField;
    [SerializeField] private GameObject _engLayoutSml, _engLayoutBig, _symbLayout;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _eventSystem = GameObject.FindGameObjectWithTag(GameTagsManager.EVENT_SYSTEM).GetComponent<RewiredEventSystem>();
    }

    #endregion

    public void alphabetFunction(string alphabet)
    {
        _textField.text = _textField.text + alphabet;
    }

    public void BackSpace()
    {
        if(_textField.text.Length>0) _textField.text = _textField.text.Remove(_textField.text.Length-1);
    }

    public void CloseAllLayouts()
    {
        _engLayoutSml.SetActive(false);
        _engLayoutBig.SetActive(false);
        _symbLayout.SetActive(false);
    }

    public void ShowLayout(GameObject SetLayout)
    {
        CloseAllLayouts();
        SetLayout.SetActive(true);
    }

    public void SetGameObjectAsSelected(GameObject gameObjectToBeSelected)
    {
        _eventSystem.SetSelectedGameObject(gameObjectToBeSelected);
    }

    public void Enter()
    {
        _eventSystem.SetSelectedGameObject(_textField.gameObject);
        CloseAllLayouts();
    }
}