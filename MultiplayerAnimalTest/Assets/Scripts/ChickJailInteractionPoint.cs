using FishNet.Object;
using UnityEngine;

public class ChickJailInteractionPoint : InteractionPoint
{
    [SerializeField]
    private ChickJail _chickJail;

    private bool _hasChecked;
    private float _doorCooldown = 1f;
    private float _doorCooldownCount = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (_doorCooldownCount > 0)
        {
            _doorCooldownCount -= Time.deltaTime;
        }

        if (_doorCooldownCount  <= 0 && Input.GetKeyDown(KeyCode.E))
        {
            CheckIfShouldOpen();
            _doorCooldownCount = _doorCooldown;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckIfShouldOpen()
    {
        if (_nearbyPlayers.Count > 0)
        {
            if (!_hasChecked && DoesNearbyPlayerHaveKey())
            {
                _chickJail.OpenDoor();
            }
        }
    }

    private bool DoesNearbyPlayerHaveKey()
    {
        _hasChecked = true;
        foreach (var playerTransform in _nearbyPlayers)
        {
            var chickController = playerTransform.GetComponent<ChickPlayerController>();
            if (chickController != null)
            {
                if (chickController.GetEquippedPickUp() == EPickUpID.CoopCageKey)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
