using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LootUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private PlayableDirector _itemsRetrievedPanelTimeline;
    [SerializeField] private GameObject _lootItemUIPrefab;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private GameObject _lootPanel;

    #endregion

    #region Private Variables

    private static LootUI _instance;
    private float _displayItemTime = 1f;

    #endregion

    #region Public Properties

    public static LootUI Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    public void DisplayLootedItems(List<ItemQuantity> lootedItems)
    {
        DestroyPrevListedItems();

        _itemsRetrievedPanelTimeline.Play();

        StartCoroutine(DisplayLootedItemsSlowly(lootedItems));

        _lootPanel.SetActive(true);

        StartCoroutine(DisableLootPanel());
    }

    private IEnumerator DisplayLootedItemsSlowly(List<ItemQuantity> lootedItems)
    {
        List<ItemQuantity> copiedList = new List<ItemQuantity>();
        copiedList.AddRange(lootedItems);

        WaitForSeconds wait = new WaitForSeconds(_displayItemTime);

        foreach (ItemQuantity itemQuantity in copiedList)
        {
            GameObject lootItemUI = Instantiate(_lootItemUIPrefab, _contentTransform);
            LootItemUI lootItem = lootItemUI.GetComponent<LootItemUI>();
            lootItem.Initialize(itemQuantity);

            yield return wait;
        }
    }

    private IEnumerator DisableLootPanel()
    {
        yield return new WaitForSeconds(5f);

        _lootPanel.SetActive(false);
    }

    private void DestroyPrevListedItems()
    {
        foreach (Transform child in _contentTransform)
        {
            if(child == _contentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }
}
