using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerBallUserController : NetworkBehaviour {
    public AudioSource RollSound;
    private PlayerBall ball; // Reference to the ball controller.

    [SerializeField]public NetworkVariable<Vector3> move;
    [SerializeField] public Vector2 inputVector;
    [SerializeField] public Transform cam;
    [SerializeField] private Vector3 camForward;
    
    public bool hasSpeed;
    private PlayerInputAction playerInputAction;
    

    public override void OnNetworkSpawn()
    {
        cam.transform.parent.gameObject.SetActive(IsOwner);
        this.enabled = IsOwner;
        ball = transform.GetComponent<PlayerBall>();
        if (IsOwner)
        {
            playerInputAction = new PlayerInputAction();
            playerInputAction.Player.Enable();
        }
    }
    
    private void FixedUpdate() {
        //Movement is Physics based, so logic must be done in a fixed update! 
        {
            if(!IsOwner)
                return;
            
            inputVector = playerInputAction.Player.Movement.ReadValue<Vector2>();
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            RequestMovementServerRpc(inputVector, camForward, cam.right, playerInputAction.Player.Jump.triggered);
        }
    }

    [ServerRpc]
    void RequestMovementServerRpc(Vector2 playerInput, Vector3 cameraT, Vector3 right, bool isJump)
    {
        if (!IsServer && !IsHost) return;
        
        move.Value = (playerInput.y * cameraT + playerInput.x * right).normalized;
        if (isJump)
        {
            ball.Jump();
        }
        ball.JustMove(move.Value);
    }
}
