using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantLaser : WeaponShoot
{
    #region Editor Fields

    [SerializeField] private float _laserBallSize;
    [SerializeField] private float _scaleDuration;

    #endregion

    #region Private Variables

    private GameObject _constantLaser;
    private GameObject _laserBeam;
    private GameObject _laserBall;
    private ShootLaserState _shootLaserState = ShootLaserState.CanShoot; 
    private float _laserBallScaleTime = 0;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _constantLaser = Instantiate(_weapon.ProjectilePrefab, _shootTransforms[0].position, _turretHead.rotation);
        _laserBeam = _constantLaser.transform.GetChild(0).gameObject;
        _laserBall = _constantLaser.transform.GetChild(1).gameObject;
        _laserBall.transform.GetChild(1).gameObject.SetActive(false);

        _laserBeam.SetActive(false);
        _laserBall.transform.localScale = new Vector3(0, 0, 0);

        _laserBeam.GetComponent<ProjectileLaser>().WeaponReference = _weapon;
    }

    public override void Update()
    {
        base.Update();

        if (_shootLaserState == ShootLaserState.ChargingUp && _laserBall.transform.localScale != new Vector3(_laserBallSize, _laserBallSize, _laserBallSize))
        {
            _laserBallScaleTime += Time.deltaTime;
            _laserBall.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(_laserBallSize, _laserBallSize, _laserBallSize), _laserBallScaleTime/_scaleDuration);
            _laserBall.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (_shootLaserState == ShootLaserState.ChargingDown && _laserBall.transform.localScale != new Vector3(0, 0, 0))
        {
            _laserBallScaleTime -= Time.deltaTime;
            _laserBall.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(_laserBallSize, _laserBallSize, _laserBallSize), _laserBallScaleTime);
            
            if (_laserBallScaleTime <= 0) { _shootLaserState = ShootLaserState.CanShoot; _laserBall.transform.GetChild(1).gameObject.SetActive(false); }
        }
    }

    #endregion

    public override void CheckShootInput()
    {
        if(_weapon.CurrentPlayer == null) { StopLaserBeam(); return; }
        else if(_weapon.CurrentPlayer != null 
            && !_laserBeam.activeSelf
            && _shootLaserState == ShootLaserState.ChargingDown) { Debug.Log("Set Can Shoot"); _shootLaserState = ShootLaserState.CanShoot; }

        _constantLaser.transform.rotation = _turretHead.transform.rotation;
        _constantLaser.transform.position = _turretHead.transform.position;

        if (_weapon.CurrentPlayer.IsUsing && !_laserBeam.activeSelf && (_shootLaserState == ShootLaserState.CanShoot || _shootLaserState == ShootLaserState.ChargingUp)) { ShootLaserBeam(); }
        else if (!_weapon.CurrentPlayer.IsUsing && _laserBeam.activeSelf && (_shootLaserState == ShootLaserState.CanStop || _shootLaserState == ShootLaserState.ChargingDown)) { StopLaserBeam(); }
    }

    private void ShootLaserBeam()
    {
        _shootLaserState = ShootLaserState.ChargingUp;
        if (_laserBallScaleTime/_scaleDuration >= 1)
        {
            _laserBeam.SetActive(true);
            _shootLaserState = ShootLaserState.CanStop;
            PlayShootFX();
        }
    }

    private void StopLaserBeam()
    {
        Debug.Log("StopLaserBeam");
        _shootLaserState = ShootLaserState.ChargingDown;
        _laserBeam.SetActive(false);
    }

    private enum ShootLaserState
    {
        CanShoot,
        ChargingUp,
        ChargingDown,
        CanStop
    }
}