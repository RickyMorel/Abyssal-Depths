namespace Tests.Factories
{
    public class UpgradableBuilder
    {
        private UpgradeChip[] _upgradeSockets = { null, null }; 
        private bool _isAIOnlyInteractable;

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
            return new UpgradableHumble(_upgradeSockets, _isAIOnlyInteractable);
        }
    }

    public static class UpgradableFactory
    {
        public static UpgradableBuilder AnUpgradable => new UpgradableBuilder();
    }
}
