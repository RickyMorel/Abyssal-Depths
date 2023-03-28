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
            WeaponHumble weaponHumble = WeaponFactory.AWeapon.Build();
            GameObject meshObj = new GameObject();
            GameObject rotationObj = new GameObject();
            rotationObj.transform.SetParent(meshObj.transform);
            GameObject[] shootTransforms = { new GameObject() };
            Upgrade upgrade = new Upgrade(meshObj, shootTransforms, _weaponSO);

            weaponHumble.HandleUpgrade(upgrade);

            Debug.Log("weaponHumble: " + weaponHumble);

            Assert.That(weaponHumble.WeaponShoot, Is.Not.Null);
        }
    }
}
