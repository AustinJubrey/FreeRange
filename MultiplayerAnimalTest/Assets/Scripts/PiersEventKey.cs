using System;
using UnityEngine;

public class PiersEventKey : MonoBehaviour
{
    public enum EventKey
    {
        ClientJoined,
        ClientExited,
        PlayerCacheUpdated,
        CameraTargetBroadcast,
        PlayersWin,
        SendBackToLobby,
        PlayersAllCaught,
        
    }

    [Serializable]
    public class PiersEventEnum : SerializableEnum<EventKey> { }

    public PiersEventEnum m_EnumVariable;
}
