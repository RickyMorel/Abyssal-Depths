using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetsManager : MonoBehaviour
{
    #region Editor Fields

    [Header("UI")]
    [SerializeField] private GameObject _damagePopup;
    [SerializeField] private GameObject _repairPopup;
    [SerializeField] private GameObject _repairCostsCanvas;

    [Header("Pickup Prefabs")]
    [SerializeField] private GameObject _chipPickup;
    [SerializeField] private GameObject _deathLootPickup;

    [Header("Building Prefabs")]
    [SerializeField] private GameObject _basePart;

    [Header("Particles")]
    [SerializeField] private GameObject _upgradeParticles;
    [SerializeField] private GameObject _electricParticles;
    [SerializeField] private GameObject _fireParticles;
    [SerializeField] private GameObject _meleeFloorHitParticles;
    [SerializeField] private GameObject _fireWhipCollisionParticles;
    [SerializeField] private GameObject _itemPickupCollisionParticles;
    [SerializeField] private GameObject _ragdollBubbleParticles;
    [SerializeField] private GameObject _minableHitDefault;
    [SerializeField] private GameObject _shootBubbleParticles;
    [SerializeField] private GameObject[] _stunnedParticles;
    [SerializeField] private GameObject _buildingSmokeParticle;
    [ColorUsageAttribute(false, true), SerializeField] private Color _laserHeatColor;

    [Header("Scriptable Objects")]
    [SerializeField] private ChipDataSO _chipDataSO;
    [SerializeField] private EnemyDamageDataSO _enemyDamageDataSOPrefab;
    [SerializeField] private EnemyDamageDataSO _buildingDamageDataSOPrefab;
    [SerializeField] private DamageTypeSO _damageType;

    #endregion

    #region Private Variables

    private static GameAssetsManager _instance;
    private EnemyDamageDataSO _enemyDamageDataSOInstance;
    private EnemyDamageDataSO _buildingDamageDataSOInstance;

    #endregion

    #region Public Properties

    public GameObject DamagePopup => _damagePopup;
    public GameObject RepairPopup => _repairPopup;
    public GameObject RepairCostsCanvas => _repairCostsCanvas;
    public GameObject ChipPickup => _chipPickup;
    public GameObject DeathLootPickup => _deathLootPickup;
    public GameObject BasePart => _basePart;
    public GameObject UpgradeParticles => _upgradeParticles;
    public GameObject ElectricParticles => _electricParticles;
    public GameObject FireParticles => _fireParticles;
    public GameObject MeleeFloorHitParticles => _meleeFloorHitParticles;
    public GameObject FireWhipCollisionParticles => _fireWhipCollisionParticles;
    public GameObject ItemPickupCollisionParticles => _itemPickupCollisionParticles;
    public GameObject RagdollBubbleParticles => _ragdollBubbleParticles;
    public GameObject MinableHitDefault => _minableHitDefault;
    public GameObject ShootBubbleParticles => _shootBubbleParticles;
    public GameObject[] StunnedParticles => _stunnedParticles;
    public GameObject BuildingSmokeParticle => _buildingSmokeParticle;
    public Color LaserHeatColor => _laserHeatColor;
    public ChipDataSO ChipDataSO => _chipDataSO;
    public EnemyDamageDataSO EnemyDamageDataSO => _enemyDamageDataSOInstance;
    public EnemyDamageDataSO BuildingDamageDataSO => _buildingDamageDataSOInstance;
    public DamageTypeSO DamageType => _damageType;

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
        //_buildingDamageDataSOInstance = new EnemyDamageDataSO(_buildingDamageDataSOPrefab);
    }
}
