using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetsManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _damagePopup;
    [SerializeField] private GameObject _repairPopup;
    [SerializeField] private GameObject _repairCostsCanvas;
    [SerializeField] private GameObject _chipPickup;
    [SerializeField] private GameObject _deathLootPickup;
    [SerializeField] private GameObject _upgradeParticles;
    [SerializeField] private GameObject _electricParticles;
    [SerializeField] private GameObject _fireParticles;
    [SerializeField] private GameObject _meleeFloorHitParticles;
    [SerializeField] private GameObject[] _stunnedParticles;
    [ColorUsageAttribute(false, true), SerializeField] private Color _laserHeatColor;
    [SerializeField] private ChipDataSO _chipDataSO;
    [SerializeField] private EnemyDamageDataSO _enemyDamageDataSOPrefab;
    [SerializeField] private DamageType _damageType;

    #endregion

    #region Private Variables

    private static GameAssetsManager _instance;
    private EnemyDamageDataSO _enemyDamageDataSOInstance;

    #endregion

    #region Public Properties

    public GameObject DamagePopup => _damagePopup;
    public GameObject RepairPopup => _repairPopup;
    public GameObject RepairCostsCanvas => _repairCostsCanvas;
    public GameObject ChipPickup => _chipPickup;
    public GameObject DeathLootPickup => _deathLootPickup;
    public GameObject UpgradeParticles => _upgradeParticles;
    public GameObject ElectricParticles => _electricParticles;
    public GameObject FireParticles => _fireParticles;
    public GameObject MeleeFloorHitParticles => _meleeFloorHitParticles;
    public GameObject[] StunnedParticles => _stunnedParticles;
    public Color LaserHeatColor => _laserHeatColor;
    public ChipDataSO ChipDataSO => _chipDataSO;
    public EnemyDamageDataSO EnemyDamageDataSO => _enemyDamageDataSOInstance;
    public DamageType DamageType => _damageType;

    public static GameAssetsManager Instance { get { return _instance; } }

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        _enemyDamageDataSOInstance = new EnemyDamageDataSO(_enemyDamageDataSOPrefab);
    }
}
