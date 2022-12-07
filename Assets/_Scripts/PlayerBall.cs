using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerBall : NetworkBehaviour {
    [SerializeField] public float movePower; 
    [SerializeField] private float maxAngularVelocity; 
    [SerializeField] private float jumpPower; 
    [SerializeField] private float airMoveForce;
    [SerializeField] private bool useTorque;
    public LayerMask groundedMask;
    public bool grounded;
    private float _groundRayLength = 1f;
    public Rigidbody mRigidbody;
    public float jumpForce = 220;
    public bool hasSpeed;
    public bool hasGiant;
    public bool hasGravity;
    public bool hasJump;
    public PlayerBallUserController userControl;
    public GameObject rayCaster;
    public AudioSource mAudio;
    public static event Action highVelocity;
    public static event Action lowVelocity;
    public static event Action<int> playerWon;
    
 
    //Multiplayer Attributes//
    public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    [SerializeField]public NetworkVariable<int> PlayerID = new NetworkVariable<int>(0);

    private void Start()
    {
        this.enabled = IsOwner;
        mRigidbody = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
        mAudio.volume = 0;
        ApplyPlayerColor();
        PlayerColor.OnValueChanged += OnPlayerColorChanged;
    }
    


    void Update()
    {
        var position = rayCaster.transform.position;
        var ray = new Ray(position, -Vector3.up);
        Debug.DrawRay(position, -Vector3.up, Color.green);

        grounded = Physics.Raycast(ray, out _, 1f, groundedMask);
        

    }
    

    public void Jump() {
        
        if (Physics.Raycast(transform.position, -Vector3.up, _groundRayLength))
        {
            mRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
    
    public void JustMove(Vector3 moveDir) {
        if (useTorque)
        {
            mRigidbody.AddTorque(new Vector3(moveDir.z, 0, -moveDir.x) * movePower);
        }
        else if(grounded) {
            mRigidbody.AddForce(moveDir * movePower);
        }
        else
        {
            var increasedFall = new Vector3(moveDir.x, -0.75f, moveDir.z);
            mRigidbody.AddForceAtPosition(increasedFall * (movePower * airMoveForce), transform.position);
        }

        if (mRigidbody.angularVelocity.magnitude >= maxAngularVelocity / 3)
        {
            highVelocity?.Invoke();
        }
        else
        {
            lowVelocity?.Invoke();
        }
    }

    public IEnumerator SizePower() {
        
        Transform localTrans = this.transform;
        var localScale = localTrans.localScale;
        localScale *=  2;
        jumpForce *= 2;
        jumpPower *= 2;
        _groundRayLength *= 2f;
        hasGiant = false;
        
        yield return new WaitForSeconds(5.0f);
        localScale /=  2;
        localTrans.localScale = localScale;
        jumpForce /= 2;
        jumpPower /= 2;
        _groundRayLength /= 2f;
    }
    public IEnumerator GravityPower() {
        Physics.gravity /= 4;
        hasGravity = false;
        
        yield return new WaitForSeconds(5f);
        Physics.gravity *= 4;
    }
    
    public void SpeedPower() {
        mRigidbody.AddForceAtPosition(GetComponent<PlayerBallUserController>().move.Value * 2000, transform.position);
    }
    public void JumpPower() {
        mRigidbody.AddForceAtPosition(Vector3.up * 1000, transform.position);
        hasJump = false;
    }

    private void UsePower() {
        if (hasGiant) {
            StartCoroutine(SizePower());
        }
        if (hasGravity) {
            StartCoroutine(GravityPower());
        }
        if (hasJump) {
            JumpPower();
        }
    }

    public void ApplyPlayerColor()
    {
        GetComponent<MeshRenderer>().material.SetColor("_Color", PlayerColor.Value);
        Debug.Log("Color Changed");
    }
    

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        ApplyPlayerColor();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsHost)
        {
            if (other.CompareTag("SpeedBoost"))
            {
                HostHandleSpeed(other);
            }
            
        }
        if (other.CompareTag("Teleport"))
        {
            TeleportServerRPC();
        }
    }

    private void HostHandleSpeed(Collider other)
    {
        SpeedPower();
    }

    [ServerRpc]
    public void TeleportServerRPC()
    {
        var go = GameObject.FindGameObjectsWithTag("Spawn");
        transform.position = go[Random.Range(0, go.Length)].transform.position;
    }

    
        

}


