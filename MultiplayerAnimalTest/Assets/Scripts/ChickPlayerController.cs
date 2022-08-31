using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Example.Prediction.CharacterControllers;
using FishNet.Object;
using UnityEngine;

public class ChickPlayerController : NetworkBehaviour
{
    private CharacterControllerPrediction _characterPrediction;
    private PlayerInventory _inventory;
    private PlayerAnimatorHelper _animatorHelper;

    private void Awake()
    {
        _characterPrediction = GetComponent<CharacterControllerPrediction>();
        _inventory = GetComponent<PlayerInventory>();
        _animatorHelper = GetComponent<PlayerAnimatorHelper>();
    }

    public void EquipPickUp(string pickupID)
    {
        if (IsServer)
        {
            if (_inventory.GetEquippedItem() != EPickUpID.Nothing)
            {
                Debug.Log("picking up with something in hand, dropping item");
                _inventory.DropEquippedItem();
            }

            _inventory.EquipPickUp(pickupID);
        }
    }

    public EPickUpID GetEquippedPickUp()
    {
        return _inventory.GetEquippedItem();
    }

    public void BackStabFarmer(Transform attackTarget)
    {
        // Do a little stab animation on the weapon slot, so we can "use" things
    }
}
