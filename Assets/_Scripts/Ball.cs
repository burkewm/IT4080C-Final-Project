using System;
using System.Collections;
using UnityEngine;



    public class Ball : MonoBehaviour {
        [SerializeField] private float m_MovePower = 5; // The force added to the ball to move it.
        [SerializeField] private bool m_UseTorque = true; // Whether or not to use torque to move the ball.
        [SerializeField] private float m_MaxAngularVelocity = 25; // The maximum velocity the ball can rotate at.
        [SerializeField] private float m_JumpPower = 2; // The force added to the ball when it jumps.
        [SerializeField] private float m_AirMoveForce = 3; //Controls amount of movement added to the ball in the air
        public LayerMask groundedMask;
        public bool grounded;
        private float k_GroundRayLength = 1f; // The length of the ray to check if the ball is grounded.
        public Rigidbody m_Rigidbody;
        public float jumpForce = 220;
        public bool hasSpeed;
        public bool hasGiant;
        public bool hasGravity;
        public bool hasJump;
        public BallUserControl userControl;

        private void Start() {
            m_Rigidbody = GetComponent<Rigidbody>();
            // Set the maximum angular velocity.
            GetComponent<Rigidbody>().maxAngularVelocity = m_MaxAngularVelocity;
            Physics.gravity = Physics.gravity * 1.5f;
        }


        void Update() {
            if (grounded) {
                if (Input.GetButtonDown("Jump")) {
                    m_Rigidbody.AddForce(transform.up * jumpForce);
                    
                }
            }
            if (Input.GetButtonDown("PowerUp")) {
                UsePower();
            }
            /* if (Physics.Raycast(transform.position, -transform.up)&&grounded)
             {
                 // ... add force in upwards.
                 m_Rigidbody.AddForce(Vector3.up*m_JumpPower, ForceMode.Impulse);
             }*/
            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask)) {
                grounded = true;
            } else {
                grounded = false;
            }

        }
        public void Move(bool jump) {
            if (!grounded) {
                m_Rigidbody.AddForceAtPosition(userControl.move * m_AirMoveForce, transform.position);
                // If on the ground and jump is pressed...
                if (Physics.Raycast(transform.position, -Vector3.up, k_GroundRayLength) && jump) {
                    // ... add force in upwards.
                    m_Rigidbody.AddForce(Vector3.up * m_JumpPower, ForceMode.Impulse);
                }
            }
        }
        public void JustMove(Vector3 moveDir) {
            if (m_UseTorque) {
                // ... add torque around the axis defined by the move direction.
                    m_Rigidbody.AddTorque(new Vector3(moveDir.z, 0, -moveDir.x) * m_MovePower);

            } else {
                // Otherwise add force in the move direction.
                m_Rigidbody.AddForce(moveDir * m_MovePower);
            }
        }

        public IEnumerator SizePower() {
            this.transform.localScale = this.transform.localScale * 2;
            jumpForce = jumpForce * 2;
            m_JumpPower = m_JumpPower * 2;
            k_GroundRayLength = k_GroundRayLength * 2f;
            hasGiant = false;
            yield return new WaitForSeconds(5.0f);
            this.transform.localScale = this.transform.localScale / 2;
            jumpForce = jumpForce / 2;
            m_JumpPower = m_JumpPower / 2;
            k_GroundRayLength = k_GroundRayLength / 2f;
        }
        public IEnumerator GravityPower() {
            Physics.gravity = Physics.gravity / 4;
            hasGravity = false;
            yield return new WaitForSeconds(5f);
            Physics.gravity = Physics.gravity * 4;
        }
        public IEnumerator JumpPower() {
           
            m_Rigidbody.AddForceAtPosition(Vector3.up * 1000, transform.position);
            hasJump = false;
            yield return new WaitForSeconds(0.3f);
            
        }

         private void UsePower() {
        if (hasGiant) {
            StartCoroutine(SizePower());    
        }
        if (hasGravity) {
            StartCoroutine(GravityPower());
        }
        if (hasJump) {
            StartCoroutine(JumpPower());
        }
      }

    }

