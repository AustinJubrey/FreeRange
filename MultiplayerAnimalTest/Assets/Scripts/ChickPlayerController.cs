using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Example.Prediction.CharacterControllers;
using FishNet.Object;
using UnityEngine;

public class ChickPlayerController : NetworkBehaviour
{
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
