using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests.Factories;
using UnityEngine;

public class upgradable_tests
{
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void check_if_LoadUpgrade_correctly(int socketIndex)
    {
        UpgradableHumble upgradableHumble = UpgradableFactory.AnUpgradable.Build();
        UpgradeChip chip = new UpgradeChip();

        upgradableHumble.LoadUpgrade(chip, socketIndex);

        Assert.That(upgradableHumble.UpgradeSockets[socketIndex], Is.Not.Null);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void check_if_RemovedUpgrades_correctly(int socketIndex)
    {
        UpgradableHumble upgradableHumble = UpgradableFactory.AnUpgradable.Build();
        GameObject[] upgradeSocketObjs = { new GameObject(), new GameObject() };
        //UpgradeChip chip = new UpgradeChip();
        //upgradableHumble.LoadUpgrade(chip, 0);
        //upgradableHumble.LoadUpgrade(chip, 1);
        //upgradableHumble.PlaceChip()

        Assert.That(upgradableHumble.UpgradeSockets[0], Is.Not.Null, "Did not correctly load chip 0");
        Assert.That(upgradableHumble.UpgradeSockets[1], Is.Not.Null, "Did not correctly load chip 1");



        Assert.That(upgradableHumble.UpgradeSockets[socketIndex], Is.Not.Null);
    }

    [Test]
    public void check_if_PlaceChip_correctly()
    {
        UpgradableHumble upgradableHumble = UpgradableFactory.AnUpgradable.Build();
        GameObject[] upgradeSocketObjs = { new GameObject(), new GameObject() };
        UpgradeChip chip = new UpgradeChip();
        chip.ItemPrefab = new GameObject();

        upgradableHumble.PlaceChip(chip, upgradeSocketObjs, 0);

        Assert.That(upgradableHumble.ChipInstances[0], Is.Not.Null);
    }

    [Test]
    [TestCase(1, 1, true)]
    [TestCase(1, 2, false)]
    [TestCase(2, 1, false)]
    [TestCase(3, 3, true)]
    public void check_if_IsSameLevel_returns_false_if_not_same_level(int chip_1_level, int chip_2_level, bool expectedResult)
    {
        UpgradeChip chip_1 = new UpgradeChip();
        chip_1.Level = chip_1_level;

        UpgradeChip[] upgradeSockets = { chip_1, null };

        UpgradableHumble upgradableHumble = UpgradableFactory.AnUpgradable.WithUpgradeSockets(upgradeSockets).Build();

        UpgradeChip chip_2 = new UpgradeChip();
        chip_2.Level = chip_2_level;

        bool isSameLevel = upgradableHumble.IsSameMkLevel(chip_2);

        Assert.AreEqual(isSameLevel, expectedResult);
    }
}
