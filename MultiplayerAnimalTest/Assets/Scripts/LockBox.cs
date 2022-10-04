using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBox : MonoBehaviour
{
    [SerializeField] private ItemGiver _itemGiver;

    private int _password = 0000;

    private void EnterPassword(int passwordAttempt)
    {
        if (passwordAttempt == _password)
        {
            //unlocks box, spawns item from itemGiver
            _itemGiver.SpawnItem();
        }
    }
}
