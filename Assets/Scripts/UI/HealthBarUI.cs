using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    #region Editor Fields

    [Header("Scripts")]
    [SerializeField] private Damageable _ownHealth;

    [Header("UI")]
    [SerializeField] private Image _healthFill;
    [SerializeField] private Image _damageFill;

    #endregion

    #region Private Variables

    private float _currentDamageFill = 1f;
    private bool _isUpdatingDamageFill = false;
    private bool _isFlashingWhite = false;

    #endregion

    private void Awake()
    {
        _ownHealth.OnDamaged += HandleDamaged;
        _ownHealth.OnUpdateHealth += HandleUpdateHealth;
    }

    private void Start()
    {
        _healthFill.fillAmount = _ownHealth.CurrentHealth / _ownHealth.MaxHealth;
    }

    private void OnDestroy()
    {
        _ownHealth.OnDamaged -= HandleDamaged;
    }

    private void Update()
    {
        if (!_isUpdatingDamageFill) { return; }

        float wantedFill = _ownHealth.CurrentHealth / _ownHealth.MaxHealth;
        _currentDamageFill = Mathf.Lerp(_currentDamageFill, wantedFill, Time.deltaTime);
        _damageFill.fillAmount = _currentDamageFill;

        if(_currentDamageFill <= wantedFill) { _isUpdatingDamageFill = false; }
    }

    private void HandleDamaged(DamageTypes damageType, int damage)
    {
        _healthFill.fillAmount = _ownHealth.CurrentHealth / _ownHealth.MaxHealth;

        _isUpdatingDamageFill = true;
    }

    private void HandleUpdateHealth(int amountAdded)
    {
        _healthFill.fillAmount = _ownHealth.CurrentHealth / _ownHealth.MaxHealth;
        _currentDamageFill = _ownHealth.CurrentHealth / _ownHealth.MaxHealth;
    }

    private IEnumerator FlashWhite()
    {
        _isFlashingWhite = true;

        Color initialColor = _healthFill.color;
        Color whiteColor = initialColor;
        whiteColor.a = 0.3f;

        float elapsedTime = 0f;
        float waitTime = 0.3f;

        while(elapsedTime < waitTime)
        {
            _healthFill.color = Color.Lerp(_healthFill.color, whiteColor, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < waitTime*1.5f)
        {
            _healthFill.color = Color.Lerp(_healthFill.color, initialColor, (elapsedTime / waitTime*1.5f));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _isFlashingWhite = false;

        yield return null; 
    }
}
