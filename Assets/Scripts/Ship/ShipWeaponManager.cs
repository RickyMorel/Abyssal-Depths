using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWeaponManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Transform _weaponWheelTransform;
    [SerializeField] private Weapon[] _weapons;
    [SerializeField] private Transform _currentPosTransform;
    [SerializeField] private List<Transform> _snapPoints = new List<Transform>();
    [SerializeField] private float _rotationSpeed = 20f;

    #endregion

    #region Private Variables

    private float _snapStopDistance = 1f;
    private int _currentPosIndex = 0;

    #endregion

    #region Public Properties

    public Weapon[] Weapons => _weapons;

    #endregion

    private void Start()
    {
        CreateSnapPoints();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            RotateWeapons();
        }
    }

    private void CreateSnapPoints()
    {
        foreach (Weapon weapon in _weapons)
        {
            GameObject snapPoint = new GameObject("SnapPoint");

            GameObject snapPointInstance = Instantiate(snapPoint, Ship.Instance.transform);
            snapPointInstance.transform.position = weapon.WeaponHeadIdObj.transform.position;

            _snapPoints.Add(snapPointInstance.transform);
        }
    }

    public void RotateWeapons()
    {
        foreach (Weapon weapon in _weapons)
        {
            weapon.SwapWeapon();
        }

        StartCoroutine(RotateCoroutine());
    }

    private IEnumerator RotateCoroutine()
    {
        IncrementCurrentPosIndex();

        EnableWeapons(false);

        while(Vector3.Distance(_currentPosTransform.position, _snapPoints[_currentPosIndex].position) > _snapStopDistance)
        {
            _weaponWheelTransform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);

            yield return null;
        }

        EnableWeapons(true);
    }

    private void IncrementCurrentPosIndex()
    {
        _currentPosIndex = Mathf.Clamp(_currentPosIndex + 1, 0, Weapons.Length);

        if (_currentPosIndex == Weapons.Length) { _currentPosIndex = 0; }
    }

    private void EnableWeapons(bool isEnabled)
    {
        foreach (Weapon weapon in _weapons)
        {
            weapon.CanShoot = isEnabled;
        }
    }
}
