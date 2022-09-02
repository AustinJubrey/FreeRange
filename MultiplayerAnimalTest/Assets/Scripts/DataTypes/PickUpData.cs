using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPickUpID
{
    Nothing = 0,
    Knife,
    TruckKeys,
    CoopCageKey,
}

public class PickUpData : ScriptableObject
{
    public EPickUpID _id;
    public GameObject _worldPrefab;
    public GameObject _equippedPrefab;
}
