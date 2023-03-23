namespace Tests.Factories
{
    public class InteractableBuilder
    {
        private Interactable _interactableMono;
        private InteractableHealth _interactableHealth;
        private bool _isAIOnlyInteractable;

        public InteractableBuilder WithInteractableMono(Interactable interactable)
        {
            _interactableMono = interactable;
            return this;
        }

        public InteractableBuilder WithInteractableHealth(InteractableHealth interactableHealth)
        {
            _interactableHealth = interactableHealth;
            return this;
        }

        public InteractableBuilder WithIsAiOnlyInteractable(bool IsAiOnlyInteractable)
        {
            _isAIOnlyInteractable = IsAiOnlyInteractable;
            return this;
        }

        public InteractableHumble Build()
        {
            return new InteractableHumble(_interactableMono, _interactableHealth, _isAIOnlyInteractable);
        }
    }

    public static class InteractableFactory
    {
        public static InteractableBuilder AnInteractable => new InteractableBuilder();
    }
}
