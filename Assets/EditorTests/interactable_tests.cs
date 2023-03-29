using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests.Factories;
using UnityEngine;

namespace InteractableTests
{
    public class interactable_tests
    {
        [Test]
        public void check_if_onInteract_is_raised_when_calling_SetCurrentPlayer()
        {
            var interactableHumble = InteractableFactory.AnInteractable.Build();

            bool eventRaised = false;

            interactableHumble.OnInteract += delegate ()
            {
                eventRaised = true;
            };

            interactableHumble.SetCurrentPlayer(null);

            Assert.AreEqual(true, eventRaised);
        }

        [Test]
        public void check_if_onUnInteract_is_raised_when_calling_Uninteract()
        {
            var interactableHumble = InteractableFactory.AnInteractable.Build();

            bool eventRaised = false;

            interactableHumble.OnUninteract += delegate ()
            {
                eventRaised = true;
            };

            interactableHumble.Uninteract();

            Assert.AreEqual(true, eventRaised);
        }

        [Test]
        public void check_if_sets_player_when_calling_SetCurrentPlayer()
        {
            var interactableHumble = InteractableFactory.AnInteractable.Build();

            BaseInteractionController controller = new BaseInteractionController();
            interactableHumble.SetCurrentPlayer(controller);

            Assert.AreEqual(controller, interactableHumble.CurrentPlayer);
        }

        [Test]
        [TestCase(false, 0, false)]
        [TestCase(true, 0, true)]
        [TestCase(false, 1, false)]
        [TestCase(true, 1, true)]
        public void check_if_sets_playerInteractable_when_calling_SetCurrentInteractable(bool isSetting, int controllerType, bool expectedOutput)
        {
            InteractableHumble interactableHumble = InteractableFactory.AnInteractable.Build();

            GameObject playerObj = new GameObject();
            GameObject playerInstance = GameObject.Instantiate(playerObj);
            playerInstance.AddComponent<BoxCollider>();

            if (controllerType == 0) { playerInstance.AddComponent<BaseInteractionController>(); }
            else if (controllerType == 1) { playerInstance.AddComponent<PlayerInteractionController>(); }

            interactableHumble.SetCurrentInteractable(playerInstance.GetComponent<Collider>(), isSetting, out BaseInteractionController interactionController, out bool setInteractable, out bool setOutline); ;

            Assert.AreEqual(setInteractable, expectedOutput);
        }
    }
}
