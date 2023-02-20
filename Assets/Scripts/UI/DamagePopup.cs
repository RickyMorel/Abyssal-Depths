using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DamagePopup : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _fireColor;
    [SerializeField] private Color _electricColor;
    [SerializeField] private Color _laserColor;
    [SerializeField] private Color _normalTextColor;
    [SerializeField] private Color _criticalTextColor;

    #endregion

    #region Private Variables

    private static int _sortingOrder;

    private const float DISSAPEAR_TIMER_MAX = 1f;

    private TextMeshPro _damageText;
    private float _disappearTimer;
    private Color _textColor;
    private Vector3 _moveVector;

    #endregion

    #region Public Properties

    public static DamagePopup Create(Vector3 position, int damage, DamageType damageType, bool isCriticalHit, bool isSmall = true)
    {
        Vector3 offset = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0);
        GameObject damagePopupObj = Instantiate(GameAssetsManager.Instance.DamagePopup, position + offset, Quaternion.identity);
        DamagePopup damagePopup = damagePopupObj.GetComponent<DamagePopup>();
        damagePopup.Setup((int)damage, damageType, isCriticalHit, isSmall);

        return damagePopup;
    }

    #endregion

    private void Awake()
    {
        _damageText = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        transform.position += _moveVector * Time.deltaTime;
        _moveVector -= _moveVector * 8f * Time.deltaTime;

        DoScalingFX();

        _disappearTimer -= Time.deltaTime;

        if (_disappearTimer < 0) { StartDisappear(); }
    }

    private void DoScalingFX()
    {
        if (_disappearTimer > DISSAPEAR_TIMER_MAX * 0.5f)
        {
            //First half if the popup
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            //Second half
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }
    }

    private void StartDisappear()
    {
        float disappearSpeed = 3f;
        _textColor.a -= disappearSpeed * Time.deltaTime;
        _damageText.color = _textColor;
        if(_textColor.a < 0) { Destroy(gameObject); }
    }

    public void Setup(int damageAmount, DamageType damageType, bool isCriticalHit, bool isSmall)
    {
        isCriticalHit = UnityEngine.Random.Range(0, 4) == 1 ? true : false;
        _damageText.text = damageAmount.ToString();
        float fontDivider = isSmall ? 3.5f : 1f;
        _damageText.fontSize = isCriticalHit ? 80 / fontDivider : 40 / fontDivider;
        _disappearTimer = DISSAPEAR_TIMER_MAX;
        _moveVector = new Vector3(1, 1) * 30f;
        _sortingOrder++;
        _damageText.sortingOrder = _sortingOrder;

        if (isCriticalHit) { _damageText.fontStyle = FontStyles.Bold | FontStyles.Italic; }
        _textColor = _normalTextColor;

        switch (damageType)
        {
            case DamageType.Base:
                _textColor = _baseColor;
                break;
            case DamageType.Fire:
                _textColor = _fireColor;
                break;
            case DamageType.Electric:
                _textColor = _electricColor ;
                break;
            case DamageType.Laser:
                _textColor = _laserColor;
                break;
        }

        Color finalColor = isCriticalHit ? _criticalTextColor : _textColor;
        finalColor.a = 1f;
        _damageText.color = finalColor;
    }
}
