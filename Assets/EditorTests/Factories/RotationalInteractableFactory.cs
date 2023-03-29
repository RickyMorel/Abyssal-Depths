namespace Tests.Factories
{
    public class RotationalInteractableBuilder
    {
        private bool _isAIOnlyInteractable;

        public RotationalInteractableBuilder WithIsAiOnlyInteractable(bool IsAiOnlyInteractable)
        {
            _isAIOnlyInteractable = IsAiOnlyInteractable;
            return this;
        }

        public RotationalInteractableHumble Build()
        {
            return new RotationalInteractableHumble(_isAIOnlyInteractable);
        }
    }

    public static class RotationalInteractableFactory
    {
        public static RotationalInteractableBuilder ARotationalInteractable => new RotationalInteractableBuilder();
    }
}
