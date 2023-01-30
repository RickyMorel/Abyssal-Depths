using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantLaser : WeaponShoot
{
    #region Private Variables

    private GameObject _laserBeam;
    private ShootLaserState _shootLaserState = ShootLaserState.CanShoot;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _laserBeam = Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation);

        _laserBeam.transform.GetChild(0).gameObject.SetActive(false);
    }

    #endregion

    public override void CheckShootInput()
    {
        _laserBeam.transform.rotation = _weapon.TurretHead.transform.rotation;
        _laserBeam.transform.position = _weapon.TurretHead.transform.position;

        if (_weapon.CurrentPlayer.IsUsing && !_laserBeam.transform.GetChild(0).gameObject.activeSelf && _shootLaserState == ShootLaserState.CanShoot)
        {
            StartCoroutine(ShootLaserBeam());
        }
        else if (!_weapon.CurrentPlayer.IsUsing && _laserBeam.transform.GetChild(0).gameObject.activeSelf && _shootLaserState == ShootLaserState.CanStop)
        {
            StartCoroutine(StopLaserBeam());
        }
    }

    IEnumerator ShootLaserBeam()
    {
        _shootLaserState = ShootLaserState.WaitingPhase;
        yield return new WaitForSeconds(1);
        _laserBeam.transform.GetChild(0).gameObject.SetActive(true);
        _shootLaserState = ShootLaserState.CanStop;
    }

    IEnumerator StopLaserBeam()
    {
        _shootLaserState = ShootLaserState.WaitingPhase;
        _laserBeam.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        _shootLaserState = ShootLaserState.CanShoot;
    }

    private enum ShootLaserState
    {
        CanShoot,
        WaitingPhase,
        CanStop
    }
}