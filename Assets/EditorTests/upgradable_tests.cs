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
    [TestCase(0, 100, true)]
    [TestCase(1, 100, true)]
    [TestCase(2, 100, false)]
    [TestCase(0, 0, false)]
    [TestCase(1, 0, false)]
    [TestCase(2, 0, false)]
    public void check_if_TryUpgrade_returns_correct_bool(int chipsInSockets, int interactableHealth, bool expectedResult)
    {
        //Arrange
        UpgradeChip[] upgradeSockets = { null, null };
        UpgradeChip chip = new UpgradeChip();

        for (int i = 0; i < chipsInSockets; i++)
        {
            upgradeSockets[i] = chip;
        }

        UpgradableHumble upgradableHumble = UpgradableFactory.AnUpgradable.WithUpgradeSockets(upgradeSockets).Build();
        GameObject interactableObj = GameObject.Instantiate(new GameObject());
        interactableObj.AddComponent<BoxCollider>();
        InteractableHealth health = interactableObj.AddComponent<InteractableHealth>();
        health.CurrentHealth = interactableHealth;

        //Act
        bool canUpgrade = upgradableHumble.TryUpgrade(chip, interactableObj, out int socketIndex, out bool isSameMKLevel);

        //Assert
        Assert.AreEqual(expectedResult, canUpgrade);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void check_if_RemovedUpgrades_correctly(int chipAmount)
    {
        //Arrange
        UpgradeChip chip = new UpgradeChip();
        UpgradeChip[] upgradeSockets = { null, null };
        List<GameObject> chipInstances = new List<GameObject>();

        for (int i = 0; i < chipAmount; i++)
        {
            upgradeSockets[i] = chip;
            chipInstances.Add(new GameObject());
        }

        UpgradableHumble upgradableHumble = UpgradableFactory.AnUpgradable.WithUpgradeSockets(upgradeSockets).WithChipInstances(chipInstances).Build();

        for (int i = 0; i < chipAmount; i++)
        {
            Assert.That(upgradableHumble.UpgradeSockets[i], Is.Not.Null, $"Did not correctly load chip {i}");
            Assert.That(upgradableHumble.ChipInstances[i], Is.Not.Null, "Did not correctly load chip instance");
        }

        //Act
        upgradableHumble.RemoveUpgrades();

        //Assert
        Assert.That(upgradableHumble.ChipInstances.Count == 0);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void check_if_RemovedUpgrades_set_upgradeSockets_to_null(int chipAmount)
    {
        //Arrange
        UpgradeChip chip = new UpgradeChip();
        UpgradeChip[] upgradeSockets = { null, null };

        for (int i = 0; i < chipAmount; i++)
        {
            upgradeSockets[i] = chip;
        }

        UpgradableHumble upgradableHumble = UpgradableFactory.AnUpgradable.WithUpgradeSockets(upgradeSockets).Build();

        for (int i = 0; i < chipAmount; i++)
        {
            Assert.That(upgradableHumble.UpgradeSockets[i], Is.Not.Null, $"Did not correctly load chip {i}");
        }

        //Act
        upgradableHumble.RemoveUpgrades();

        //Assert
        Assert.That(upgradableHumble.UpgradeSockets[0] == null, "UpgradeSockets[0] did not get set to null");
        Assert.That(upgradableHumble.UpgradeSockets[1] == null, "UpgradeSockets[1] did not get set to null");
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
