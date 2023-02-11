using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    #region Private Variables

    private ShipHealth _shipHealth;
    private float _timeSinceDeath;
    private bool _isInSafeZone = false;
    private bool _isLoadingScene = false;

    #endregion

    #region Getters & Setters

    public bool IsInSafeZone { get { return _isInSafeZone; } set { _isInSafeZone = value; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
    }

    private void Start()
    {
        _shipHealth = Ship.Instance.GetComponent<ShipHealth>();
    }

    private void Update()
    {
        Debug.Log($"_shipHealth.IsDead(): {_shipHealth.IsDead()} || IsInSafeZone: {IsInSafeZone}");
        if (!_shipHealth.IsDead() || IsInSafeZone) { UpdateDeathTime(-1f); return; }

        UpdateDeathTime(1f);

        if (_timeSinceDeath < Ship.Instance.TimeTillDeath) { return; }

        if (_isLoadingScene) { return; }

        _isLoadingScene = true;

        StartCoroutine(DeathCoroutine());
    }

    #endregion

    private void OnSceneLoaded(Scene arg0, Scene arg1)
    {
        _isInSafeZone = false;
        _isLoadingScene = false;
    }

    private void UpdateDeathTime(float multiplier)
    {
        _timeSinceDeath = Mathf.Clamp(_timeSinceDeath + (Time.deltaTime * multiplier), 0f, Ship.Instance.TimeTillDeath);
        Debug.Log("_timeSinceDeath: " + _timeSinceDeath + $" multiplier: {multiplier}");
        AddEyeClosingFX();
    }

    private IEnumerator DeathCoroutine()
    {
        KillAllPlayers();

        yield return new WaitForSeconds(2f);

        ShowDeathScreen();

        yield return new WaitForSeconds((float)DeathPanelUI.Instance.DeathPanelTimeLine.duration);

        ReloadScene();
    }

    private void ShowDeathScreen()
    {
        DeathPanelUI.Instance.PlayDeathTimeline();
    }

    private void ReloadScene()
    {
        Debug.Log("ReloadScene");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Ship.Instance.GetComponent<ShipData>().ReloadLevel();
        _timeSinceDeath = 0f;
    }

    private void KillAllPlayers()
    {
        PlayerHealth[] allPlayers = FindObjectsOfType<PlayerHealth>();

        foreach(PlayerHealth player in allPlayers)
        {
            player.Hurt(DamageType.Base);
        }
    }

    private void AddEyeClosingFX()
    {
        VolumeInterface.Instance.ChangeVignetteByPercentage((_timeSinceDeath / Ship.Instance.TimeTillDeath) * 0.6f);
    }
}
