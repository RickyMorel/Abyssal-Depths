using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    class ChipSOProvider : IEnumerable<UpgradeChip>
    {
        public IEnumerator<UpgradeChip> GetEnumerator()
        {
            UpgradeChip[] chips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");

            foreach (var chip in chips)
            {
                yield return chip;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<UpgradeChip>)this).GetEnumerator();
    }

    [TestFixture]
    [TestFixtureSource(typeof(ChipSOProvider))]
    public class chipSo_tests
    {
        private readonly UpgradeChip _chipSo;

        public chipSo_tests(UpgradeChip chipSo)
        {
            _chipSo = chipSo;
        }

        [Test]
        public void check_if_itemPrefab_not_null()
        {
            Assert.IsTrue(_chipSo.ItemPrefab != null, "ItemPrefab was null");
        }

        [Test]
        public void check_if_itemPickupPrefab_not_null()
        {
            Assert.IsTrue(_chipSo.ItemPickupPrefab != null, "ItemPickupPrefab was null");
        }

        [Test]
        public void check_if_chipType_matches_name()
        {
            string chipName = _chipSo.name.ToLower();
            string chipTypeString = _chipSo.ChipType.ToString().ToLower();
            Debug.Log(chipName);
            Debug.Log(_chipSo.ChipType.ToString());

            Assert.IsTrue(chipName.Contains(chipTypeString), "Chip name and chip type don't match, or there is a spelling error");
        }

        [Test]
        public void check_if_chipMkName_matches_level()
        {
            string chipName = _chipSo.name.ToLower();
            string chiplevelString = "MK" + _chipSo.Level.ToString();
            Debug.Log(chipName);
            Debug.Log(_chipSo.ChipType.ToString());

            Assert.IsTrue(chipName.Contains(chiplevelString), "Chip name and chip level don't match, or there is a spelling error");
        }
    }
}

