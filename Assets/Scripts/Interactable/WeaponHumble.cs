using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHumble : UpgradableHumble
{
    public GameObject ProjectilePrefab { get; private set; }
    public List<Transform> ShootTransforms { get; private set; } = new List<Transform>();
    public Transform TurretHead { get; private set; }
    public WeaponShoot WeaponShoot { get; private set; }

    public WeaponHumble(bool isAIOnlyInteractable) : base(isAIOnlyInteractable)
    {

    }

    public Vector3 CalculateWeaponLocalRotation(ref float rotationX, float movDirX, float rotationSpeed, Vector2 rotationLimits)
    {
        rotationX += rotationSpeed * movDirX * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, rotationLimits.x, rotationLimits.y);
        return new Vector3(rotationX, 0f, 0f);
    }

    public void HandleUpgrade(Upgrade upgrade)
    {
        Transform rotationChild = upgrade.UpgradeMesh.transform.GetChild(0);

        TurretHead = rotationChild;

        ProjectilePrefab = upgrade.UpgradeSO.ProjectilePrefab;

        ShootTransforms.Clear();

        for (int i = 0; i < upgrade.ShootTransform.Length; i++)
        {
            ShootTransforms.Add(upgrade.ShootTransform[i].transform);
        }

        WeaponShoot = upgrade.UpgradeMesh.GetComponent<WeaponShoot>();
    }
}