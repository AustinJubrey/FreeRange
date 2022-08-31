using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 offset = new Vector3(horizontal, Physics.gravity.y, vertical) * moveSpeed * Time.deltaTime;
        _characterController.Move(offset);
    }
}
