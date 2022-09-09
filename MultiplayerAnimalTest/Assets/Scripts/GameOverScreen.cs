using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class GameOverScreen : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _countdownNumber;

    private float _countdownDuration = 10;
    private float _countdownCount;

    public void SetTitle(string text)
    {
        _title.text = text;
    }

    public void SetDescription(string text)
    {
        _description.text = text;
    }

    private void Update()
    {
        if (_countdownCount >= _countdownDuration)
        {
            _countdownNumber.text = "loading...";
            // next scene
            PiersEvent.Post(PiersEventKey.EventKey.SendBackToLobby);
        }
        else
        {
            _countdownCount += Time.deltaTime;
            _countdownNumber.text = (_countdownDuration - ((int) Math.Ceiling(_countdownCount))).ToString();
        }
    }
    
    
}
