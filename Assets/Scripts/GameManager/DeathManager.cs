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
    private bool _isReloadingScene = false;

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
        if (!_isReloadingScene) { return; }

        _isInSafeZone = false;
        _isLoadingScene = false;
        _timeSinceDeath = 0f;
        Ship.Instance.GetComponent<ShipData>().ReloadLevel();
        _isReloadingScene = false;
        Ship.Instance.FireRespawnEvent();
    }

    private void UpdateDeathTime(float multiplier)
    {
        _timeSinceDeath = Mathf.Clamp(_timeSinceDeath + (Time.deltaTime * multiplier), 0f, Ship.Instance.TimeTillDeath);
        AddEyeClosingFX();
    }

    private IEnumerator DeathCoroutine()
    {
        KillAllPlayers();
        ShipInventory.Instance.DropAllItems();

        yield return new WaitForSeconds(2f);

        SaveSystem.Save(Ship.Instance.ShipData.CurrentSaveIndex);

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
        _isReloadingScene = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void KillAllPlayers()
    {
        PlayerHealth[] allPlayers = FindObjectsOfType<PlayerHealth>();

        foreach(PlayerHealth player in allPlayers)
        {
            player.Hurt(DamageTypes.Base);
        }
    }

    private void AddEyeClosingFX()
    {
        VolumeInterface.Instance.ChangeVignetteByPercentage((_timeSinceDeath / Ship.Instance.TimeTillDeath) * 0.6f);
    }
}
