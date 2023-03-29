using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests.Factories;
using UnityEngine;

namespace InteractableTests
{
    class WeaponSOProvider : IEnumerable<WeaponSO>
    {
        public IEnumerator<WeaponSO> GetEnumerator()
        {
            WeaponSO[] weaponSOs = Resources.LoadAll<WeaponSO>("ScriptableObjs/Weapons");

            foreach (var weaponSO in weaponSOs)
            {
                yield return weaponSO;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<WeaponSO>)this).GetEnumerator();
    }

    [TestFixture]
    [TestFixtureSource(typeof(WeaponSOProvider))]
    public class weapon_tests
    {
        private readonly WeaponSO _weaponSO;

        public weapon_tests(WeaponSO weaponSO)
        {
            _weaponSO = weaponSO;
        }

        [Test]
        public void check_if_HandleUpgrade_updates_weaponShoot()
        {
            //Arrange
            WeaponHumble weaponHumble = WeaponFactory.AWeapon.Build();
            GameObject meshObj = GameObject.Instantiate(new GameObject());
            GameObject rotationObj = new GameObject();
            meshObj.AddComponent<WeaponShoot>();
            rotationObj.transform.SetParent(meshObj.transform);
            GameObject[] shootTransforms = { new GameObject() };
            Upgrade upgrade = new Upgrade(meshObj, shootTransforms, _weaponSO);

            //Act
            weaponHumble.HandleUpgrade(upgrade);

            //Assert
            Assert.That(weaponHumble.WeaponShoot, Is.Not.Null);
        }

        [Test]
        [TestCase(0f, 0f, 0f, 0f, 0f, 0f)]
        [TestCase(10f, 1f, 300f, -190f, 10f, 10f)]
        [TestCase(-90.34481f, -1f, 300f, -190f, 10f, -92.7848f)]
        public void check_if_HandleUpgrade_updates_weaponShoot(float rotationX, float movDirX, float rotationSpeed, float limitX, float limitY, float expectedRotX)
        {
            //Arrange
            WeaponHumble weaponHumble = WeaponFactory.AWeapon.Build();
            Vector2 limits = new Vector2(limitX, limitY);
            float maxDelta = 5;

            //Act
            Vector3 finalRotation = weaponHumble.CalculateWeaponLocalRotation(ref rotationX, movDirX, rotationSpeed, limits);
            Vector3 expectedRotation = new Vector3(expectedRotX, 0f, 0f);

            //Assert
            Assert.IsTrue(Mathf.Abs(finalRotation.x - expectedRotX) < maxDelta, "Calculated rotation and expected rotation are not the same");
        }
    }
}
