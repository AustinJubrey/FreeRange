using UnityEngine;

public class HayBaleInteractionPoint : InteractionPoint
{
    [SerializeField] private Collider _collider;

    private Vector3 _hiderOriginalPosition;
    
    protected override void OnPlayerInteraction(Transform playerTransform)
    {
        // Set character as hidden
        // Move character to hidden position
        var player = playerTransform.GetComponent<ChickPlayerController>();
        if (player.GetIsHidden())
        {
            // Player is hidden, should be booted out of spot
            player.TeleportToLocation(_hiderOriginalPosition);
            player.SetHidden(false);
            _collider.enabled = true;
        }
        else
        {
            // Player is trying to hide, move them into the spot
            player.SetHidden(true);
            _hiderOriginalPosition = playerTransform.position;
            _collider.enabled = false;
            player.TeleportToLocation(transform.position, false);
        }
    }
}
