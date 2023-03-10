using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AbyssalDepths.UI
{
    public class OptionSelectorButton : MonoBehaviour
    {
        #region Editor Fields

        [SerializeField] private List<string> _options = new List<string>();
        [SerializeField] private TextMeshProUGUI _optionText;

        #endregion

        #region Public Properties

        public UnityEvent<int> OnSelected;

        #endregion

        #region Getters & Setters

        public int Index { get; private set; }

        #endregion

        private void Start()
        {
            SetIndex(Index);
        }

        public void Left()
        {
            SetIndex(Index-1);
        }

        public void Right()
        {
            SetIndex(Index+1);
        }

        public void InitializeOptions(string[] newOptions)
        {
            _options.Clear();

            foreach (string newOption in newOptions)
            {
                _options.Add(newOption);
            }
        }

        public void SetIndex(int wantedIndex)
        {
            Index = Mathf.Clamp(wantedIndex, 0, _options.Count - 1);

            _optionText.text = _options[Index];

            OnSelected?.Invoke(Index);
        }
    }
}
