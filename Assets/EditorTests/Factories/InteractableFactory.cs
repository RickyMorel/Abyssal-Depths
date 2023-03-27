namespace Tests.Factories
{
    public class InteractableBuilder
    {
        private bool _isAIOnlyInteractable;

        public InteractableBuilder WithIsAiOnlyInteractable(bool IsAiOnlyInteractable)
        {
            _isAIOnlyInteractable = IsAiOnlyInteractable;
            return this;
        }

        public InteractableHumble Build()
        {
            return new InteractableHumble(_isAIOnlyInteractable);
        }
    }

    public static class InteractableFactory
    {
        public static InteractableBuilder AnInteractable => new InteractableBuilder();
    }
}
