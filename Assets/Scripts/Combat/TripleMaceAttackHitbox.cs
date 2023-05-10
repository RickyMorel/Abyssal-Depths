using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMaceAttackHitbox : AttackHitBox
{
    #region Editors Field

    [SerializeField] private Weapon _weapon;
    [SerializeField] private TripleMace _tripleMace;
    [SerializeField] private int _whichRbsToUse;

    #endregion

    #region Private Variables

    private int _damage;

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        OnHit += HandleHitParticles;
    }

    public virtual void OnDestroy()
    {
        OnHit -= HandleHitParticles;
    }

    public override void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    #endregion

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) { InvokeHitParticles(other); }

        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (_ownHealth != null && enemyHealth == _ownHealth) { return; }

        InvokeHitParticles(other);

        if (enemyHealth is AIHealth)
        {
            CalculateDamage();

            DealDamageToEnemies((AIHealth)enemyHealth, _damage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out TripleMaceAttackHitbox otherMace)) { return; }

        Vector3 pos = otherMace.gameObject.transform.position;
        Vector3 dir = (transform.position - pos).normalized;
        _tripleMace.ApplyForceToMace(_whichRbsToUse, dir);
    }

    public virtual void CalculateDamage()
    {
        _damage = (int)_damageData.SecondaryValue[0];
        if (Mathf.Abs(_tripleMace.rbs[_whichRbsToUse].velocity.x) >= (_tripleMace.MaxMovementSpeed / 2) || Mathf.Abs(_tripleMace.rbs[_whichRbsToUse].velocity.y) >= (_tripleMace.MaxMovementSpeed / 2))
        {
            float damagePercentage;
            if (Mathf.Abs(_tripleMace.rbs[_whichRbsToUse].velocity.x) >= Mathf.Abs(_tripleMace.rbs[_whichRbsToUse].velocity.y))
            {
                damagePercentage = (Mathf.Abs(_tripleMace.rbs[_whichRbsToUse].velocity.x) * 100) / _tripleMace.MaxMovementSpeed;
            }
            else
            {
                damagePercentage = (Mathf.Abs(_tripleMace.rbs[_whichRbsToUse].velocity.y) * 100) / _tripleMace.MaxMovementSpeed;
            }
            _damage = (int)((_damage * damagePercentage) / 100);
        }
        else { _damage = _damageData.Damage[0]; }
    }

    public virtual void HandleHitParticles(GameObject obj)
    {
        if (obj.tag == "MainShip") { return; }

        Instantiate(GameAssetsManager.Instance.MeleeFloorHitParticles, transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(5f, 50f, 0.2f);
    }
}