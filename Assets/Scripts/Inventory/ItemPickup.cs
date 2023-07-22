using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Outline))]
public class ItemPickup : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] protected Item _itemSO;

    #endregion

    #region Public Properties

    public Item ItemSO => _itemSO;
    public Rigidbody Rb => _rb;
    public PlayerCarryController PrevPlayerCarryController => _prevPlayerCarryController;

    #endregion

    #region Private Variables

    private Outline _outline;
    private Rigidbody _rb;
    private PlayerCarryController _prevPlayerCarryController = null;
    private ParticleSystem _collisionParticles;
    private float _maxDistanceFromStaticShip = 15f;

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        _outline = GetComponent<Outline>();
        _rb = GetComponent<Rigidbody>();
        _collisionParticles = Instantiate(GameAssetsManager.Instance.ItemPickupCollisionParticles, transform).GetComponent<ParticleSystem>();

        EnableOutline(false);
    }

    private void FixedUpdate()
    {
        if (SceneLoader.IsInGarageScene()) { return; }

        _rb.AddForce(-Ship.Instance.Rb.velocity, ForceMode.Force);

        //Makes sure to teleport back items that clipped through the ship
        if(Vector3.Distance(ShipMovingStaticManager.Instance.ShipStaticObj.transform.position, transform.position) > _maxDistanceFromStaticShip)
        {
            transform.position = ShipMovingStaticManager.Instance.ShipStaticObj.transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _collisionParticles.transform.position = collision.contacts[0].point;
        _collisionParticles.Play();
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.ItemDropSfx, transform.position);
    }

    #endregion

    public virtual void Initialize(Item item)
    {

    }

    public void EnableOutline(bool isEnabled)
    {
        _outline.enabled = isEnabled;
    }

    public void PickUp(PlayerCarryController playerCarryController)
    {
        _prevPlayerCarryController = playerCarryController;
        playerCarryController.CarryItem(this);
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.ItemPickUpSfx, transform.position);
    }

    public void PickUpSingle(PlayerCarryController playerCarryController)
    {
        _prevPlayerCarryController = playerCarryController;
        playerCarryController.CarrySingle(this);
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.ItemPickUpSfx, transform.position);
    }
}
