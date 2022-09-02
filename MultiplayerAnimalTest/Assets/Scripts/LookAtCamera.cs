using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        PiersEvent.Listen<Camera>(PiersEventKey.EventKey.CameraTargetBroadcast, OnClientJoined);
    }

    private void OnClientJoined(Camera camera)
    {
        _camera = camera;
    }

    // Update is called once per frame
    void Update()
    {
        if (_camera == null)
            return;
        
        transform.LookAt(_camera.transform);
    }
}
