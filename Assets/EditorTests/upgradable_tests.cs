using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests.Factories;
using UnityEngine;

public class upgradable_tests
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
}
