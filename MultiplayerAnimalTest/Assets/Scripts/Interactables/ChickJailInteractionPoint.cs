using UnityEngine;

public class ChickJailInteractionPoint : InteractionPoint
{
    [SerializeField]
    private ChickJail _chickJail;

    private bool _hasChecked;
    private float _doorCooldown = 1f;
    private float _doorCooldownCount = 0;
    
    protected override void OnPlayerInteraction(Transform playerTransform)
    {
        if(playerTransform.GetComponent<ChickPlayerController>().GetEquippedPickUp() == EPickUpID.CoopCageKey)
            _chickJail.OpenDoor();
    }
}
