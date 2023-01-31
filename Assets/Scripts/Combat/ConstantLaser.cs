using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantLaser : WeaponShoot
{
    #region Private Variables

    private GameObject _constantLaser;
    private GameObject _laserBeam;
    private GameObject _laserBall;
    private ShootLaserState _shootLaserState = ShootLaserState.CanShoot;
    [SerializeField] private float _laserBallScaleTime = 0;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _constantLaser = Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation);
        _laserBeam = _constantLaser.transform.GetChild(0).gameObject;
        _laserBall = _constantLaser.transform.GetChild(1).gameObject;

        _laserBeam.SetActive(false);
        _laserBall.transform.localScale = new Vector3(0, 0, 0);
    }

    public override void Update()
    {
        base.Update();

        if (_shootLaserState == ShootLaserState.ChargingUp && _laserBall.transform.localScale != new Vector3(1.5f, 1.5f, 1.5f))
        {
            _laserBallScaleTime = _laserBallScaleTime + Time.deltaTime;
            _laserBall.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1.5f, 1.5f, 1.5f), _laserBallScaleTime);
        }
        if (_shootLaserState == ShootLaserState.ChargingDown && _laserBall.transform.localScale != new Vector3(0, 0, 0))
        {
            _laserBallScaleTime = _laserBallScaleTime - Time.deltaTime;
            _laserBall.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1.5f, 1.5f, 1.5f), _laserBallScaleTime);
        }
    }

    #endregion

    public override void CheckShootInput()
    {
        _constantLaser.transform.rotation = _weapon.TurretHead.transform.rotation;
        _constantLaser.transform.position = _weapon.TurretHead.transform.position;

        if (_weapon.CurrentPlayer.IsUsing && !_laserBeam.activeSelf && (_shootLaserState == ShootLaserState.CanShoot || _shootLaserState == ShootLaserState.ChargingUp))
        {
            ShootLaserBeam();
        }
        else if (!_weapon.CurrentPlayer.IsUsing && _laserBeam.activeSelf && (_shootLaserState == ShootLaserState.CanStop || _shootLaserState == ShootLaserState.ChargingDown))
        {
            StopLaserBeam();
        }
    }

    private void ShootLaserBeam()
    {
        _shootLaserState = ShootLaserState.ChargingUp;
        if (_laserBallScaleTime >= 1)
        {
            _laserBeam.SetActive(true);
            _shootLaserState = ShootLaserState.CanStop;
        }
    }

    private void StopLaserBeam()
    {
        _shootLaserState = ShootLaserState.ChargingDown;
        _laserBeam.SetActive(false);

        if (_laserBall.transform.localScale == new Vector3(0, 0, 0))
        {
            _shootLaserState = ShootLaserState.CanShoot;
        }
    }

    private enum ShootLaserState
    {
        CanShoot,
        ChargingUp,
        ChargingDown,
        CanStop
    }
}