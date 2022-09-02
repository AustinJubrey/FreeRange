using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Example.Prediction.CharacterControllers;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class ChickPlayerController : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameLabel;
    
    [SerializeField]
    private Camera _camera;
    
    private CharacterControllerPrediction _characterPrediction;
    private PlayerInventory _inventory;
    private PlayerAnimatorHelper _animatorHelper;

    private float _equipCooldown = 1f;
    private float _equipCooldownCounter;
    private bool _canEquip = true;

    private void Awake()
    {
        _characterPrediction = GetComponent<CharacterControllerPrediction>();
        _inventory = GetComponent<PlayerInventory>();
        _animatorHelper = GetComponent<PlayerAnimatorHelper>();
    }

    private void Start()
    {
        PiersEvent.Post(PiersEventKey.EventKey.CameraTargetBroadcast, _camera);
    }

    private void Update()
    {
        if (!_canEquip)
        {
            _equipCooldownCounter -= Time.deltaTime;

            if (_equipCooldownCounter <= 0)
            {
                _canEquip = true;
            }
        }
    }

    public Camera GetCamera()
    {
        return _camera;
    }

    public void EquipPickUp(string pickupID)
    {
        if (IsServer && _canEquip)
        {
            if (_inventory.GetEquippedItem() != EPickUpID.Nothing)
            {
                Debug.Log("picking up with something in hand, dropping item");
                _inventory.DropEquippedItem();
            }

            _inventory.EquipPickUp(pickupID);

            OnEquip();
        }
    }
    
    [ObserversRpc]
    public void SetNameLabel(string name)
    {
        _nameLabel.text = name;
        Debug.Log("set name to " + name);
    }

    private void OnEquip()
    {
        _canEquip = false;
        _equipCooldownCounter = _equipCooldown;
    }

    public EPickUpID GetEquippedPickUp()
    {
        return _inventory.GetEquippedItem();
    }

    public void TeleportToLocation(Vector3 position)
    {
        _characterPrediction.GetCharacterController().enabled = false;
        transform.position = position;
        _characterPrediction.GetCharacterController().enabled = true;
    }

    public void BackStabFarmer(Transform attackTarget)
    {
        // Do a little stab animation on the weapon slot, so we can "use" things
    }
}
