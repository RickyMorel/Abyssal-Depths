using AbyssalDepths.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionButton : MonoBehaviour
{
    [SerializeField] private int _mixerId = 0;
    [SerializeField] private OptionsMenu _optionsMenu;
    [SerializeField] private TextMeshProUGUI _percentageText;

    public void SetVolume(float volume)
    {
        _percentageText.text = $"{(int)(volume * 100)}%";

        _optionsMenu.SetVolume(volume, _mixerId);
    }
}
