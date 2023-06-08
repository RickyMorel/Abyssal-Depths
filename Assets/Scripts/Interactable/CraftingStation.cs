using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class CraftingStation : Interactable
{
    #region Editor Fields

    [SerializeField] private Transform _itemSpawnTransform;
    [SerializeField] private ParticleSystem _craftingParticle;
    [SerializeField] private PlayableDirector _craftingTimeline;
    [Header("Text related")]
    [SerializeField] private TextMeshPro _craftingText;
    [SerializeField] private string _waitingString = "Craft?";
    [SerializeField] private string _craftingString = "Crafting";

    #endregion

    #region Private Variables

    private int _counter = 0;
    private bool _shouldCountUp = true;
    private Coroutine _lastRoutine;

    #endregion

    #region Public Properties

    public static event Action OnCraft;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _craftingText.text = _waitingString;
    }

    public override void Awake()
    {
        base.Awake();

        Humble.OnInteract += HandleInteract;
        Humble.OnUninteract += HandleUnInteract;
    }

    private void OnDestroy()
    {
        Humble.OnInteract -= HandleInteract;
        Humble.OnUninteract -= HandleUnInteract;
    }

    #endregion

    public void TryCraft(CraftingRecipy craftingRecipy)
    {
        if (!CraftingManager.CanCraft(craftingRecipy)) { return; }

        ResetText(_craftingString);

        _lastRoutine = StartCoroutine(CraftingText(_craftingText));

        StartCoroutine(PlayCraftingAnimation(craftingRecipy, craftingRecipy.CraftingIngredients));
    }

    private void Craft(CraftingRecipy craftingRecipy, List<ItemQuantity> usedResources)
    {
        MainInventory.Instance.RemoveItems(usedResources);

        GameObject spawnedItem = craftingRecipy.CraftedItem.Item.SpawnItemPickup(_itemSpawnTransform);
        if(spawnedItem.TryGetComponent<ChipPickup>(out ChipPickup chipPickup)) { chipPickup.Initialize(craftingRecipy.CraftedItem.Item); }

        OnCraft?.Invoke();

        ResetText(_waitingString);

        RemoveCurrentPlayer();
    }

    private void ResetText(string text)
    {
        Debug.Log($"ResetText: {text}");
        StopAllCoroutines();
        _craftingText.text = text;
        _shouldCountUp = true;
        _counter = 0;

        //if (_lastRoutine != null) { StopCoroutine(_lastRoutine); StopAllCoroutines(); }
    }

    private void HandleInteract()
    {
        if(CurrentPlayer == null) { HandleUnInteract(); return; }

        CraftingManager.Instance.EnableCanvas(true, CurrentPlayer.GetComponent<PlayerInputHandler>(), this);
    }

    private void HandleUnInteract()
    {
        CraftingManager.Instance.EnableCanvas(false, null, this);
    }

    private IEnumerator PlayCraftingAnimation(CraftingRecipy craftingRecipy, List<ItemQuantity> usedResources)
    {
        _craftingTimeline.Play();

        yield return new WaitForSeconds(1.2f);

        _craftingParticle.Play();
        Craft(craftingRecipy, usedResources);
    }

    private IEnumerator CraftingText(TextMeshPro text)
    {
        Debug.Log("CraftingText");
        yield return new WaitForSeconds(0.2f);
        if (_shouldCountUp && _counter < 3) 
        { 
            _counter += 1;
            text.text = text.text + ".";

            StopCoroutine(_lastRoutine);

            _lastRoutine = StartCoroutine(CraftingText(text));

            yield break;
        }
        else if (_shouldCountUp && _counter >= 3) 
        {
            _shouldCountUp = false;

            StopCoroutine(_lastRoutine);

            _lastRoutine = StartCoroutine(CraftingText(text));

            yield break; }
        else if (!_shouldCountUp && _counter > 0)
        {
            _counter -= 1;
            text.text = text.text.Remove(text.text.Length-1);

            StopCoroutine(_lastRoutine);

            _lastRoutine = StartCoroutine(CraftingText(text));

            yield break;
        }
        else if (!_shouldCountUp && _counter <= 0) 
        { 
            _shouldCountUp = true;

            StopCoroutine(_lastRoutine);

            _lastRoutine = StartCoroutine(CraftingText(text));

            yield break; 
        }
    }
}