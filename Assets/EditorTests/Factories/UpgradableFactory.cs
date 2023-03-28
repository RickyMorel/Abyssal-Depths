using System.Collections.Generic;
using UnityEngine;

namespace Tests.Factories
{
    public class UpgradableBuilder
    {
        private UpgradeChip[] _upgradeSockets = { null, null };
        private List<GameObject> _chipInstances = new List<GameObject>();
        private bool _isAIOnlyInteractable;

        public UpgradableBuilder WithChipInstances(List<GameObject> ChipInstances)
        {
            _chipInstances = ChipInstances;
            return this;
        }

        public UpgradableBuilder WithUpgradeSockets(UpgradeChip[] UpgradeSockets)
        {
            _upgradeSockets = UpgradeSockets;
            return this;
        }

        public UpgradableBuilder WithIsAiOnlyInteractable(bool IsAiOnlyInteractable)
        {
            _isAIOnlyInteractable = IsAiOnlyInteractable;
            return this;
        }

        public UpgradableHumble Build()
        {
            return new UpgradableHumble(_upgradeSockets, _chipInstances, _isAIOnlyInteractable);
        }
    }

    public static class UpgradableFactory
    {
        public static UpgradableBuilder AnUpgradable => new UpgradableBuilder();
    }
}
