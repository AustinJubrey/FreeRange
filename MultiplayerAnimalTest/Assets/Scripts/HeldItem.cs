using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class HeldItem : NetworkBehaviour
{
    public void DeactivateItem()
    {
        Despawn();
    }
}
