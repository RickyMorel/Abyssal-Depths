using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickableUI
{
    bool GotClicked { get; set; }
    PlayerInputHandler CurrentPlayer { get; set; }

    private void HandleClick(PlayerInputHandler playerThatClicked)
    {
        if (playerThatClicked != CurrentPlayer) { return; }

        GotClicked = true;
    }

    public void OnClick()
    {
        if (!GotClicked) { return; }

        GotClicked = false;
    }
}
