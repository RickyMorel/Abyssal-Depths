using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests.Factories;
using UnityEngine;

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
    [TestCase(false)]
    [TestCase(true)]
    public void check_if_sets_playerInteractable_when_calling_SetCurrentInteractable(bool isSetting)
    {
        Interactable interactable = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Interactable>();
        Interactable interactableInstance = GameObject.Instantiate(interactable.gameObject).GetComponent<Interactable>();
        InteractableHealth interactableHealth = new InteractableHealth();
        var interactableHumble = InteractableFactory.AnInteractable.Build();

        bool eventRaised = false;

        interactableHumble.OnSetInteractable += delegate ()
        {
            eventRaised = true;
        };


        GameObject playerObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playerObj.AddComponent<BaseInteractionController>();
        Collider collider = playerObj.AddComponent<Collider>();
        GameObject instanceObj = GameObject.Instantiate(playerObj);

        //interactableHumble.SetCurrentInteractable(isSetting, out bool setOutline);


        Assert.AreEqual(eventRaised, true);
    }

    //[Test]
    //public void check_if_removes_player_when_calling_RemoveCurrentPlayer()
    //{
    //    InteractableHumble interactableHumble = new InteractableHumble();

    //    BaseInteractionController controller = new BaseInteractionController();
    //    interactableHumble.SetCurrentPlayer(controller);


    //    Assert.AreEqual(controller, interactableHumble.CurrentPlayer);
    //}
}
