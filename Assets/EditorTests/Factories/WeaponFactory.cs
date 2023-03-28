using System.Collections.Generic;
using UnityEngine;

namespace Tests.Factories
{
    public class WeaponBuilder
    {
        private bool _isAIOnlyInteractable;

        public WeaponBuilder WithIsAiOnlyInteractable(bool IsAiOnlyInteractable)
        {
            _isAIOnlyInteractable = IsAiOnlyInteractable;
            return this;
        }

        public WeaponHumble Build()
        {
            return new WeaponHumble(_isAIOnlyInteractable);
        }
    }

    public static class WeaponFactory
    {
        public static WeaponBuilder AWeapon => new WeaponBuilder();
    }
}
