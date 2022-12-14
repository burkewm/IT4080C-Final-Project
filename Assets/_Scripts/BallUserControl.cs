using System;
using System.Collections;
using UnityEngine;



public class BallUserControl : MonoBehaviour {
        public AudioSource RollSound;
        private Ball ball; // Reference to the ball controller.

        public Vector3 move;
        // the world-relative desired move direction, calculated from the camForward and user input.

        private Transform cam; // A reference to the main camera in the scenes transform
        private Vector3 camForward; // The current forward direction of the camera
        private bool jump; // whether the jump button is currently pressed
        public bool hasSpeed;


        private void Awake() {
            // Set up the reference.
            ball = GetComponent<Ball>();


            // get the transform of the main camera
            if (Camera.main != null) {
                cam = Camera.main.transform;
            } else {
                Debug.LogWarning(
                    "Warning: no main camera found. Ball needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use world-relative controls in this case, which may not be what the user wants, but hey, we warned them!
            }
        }


        private void Update() {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            jump = Input.GetButton("Jump");


            // calculate move direction
            if (!cam) 
                return;

            // calculate camera relative direction to move:
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            move = (v * camForward + h * cam.right).normalized;
           
            if (Input.GetKeyUp(KeyCode.W)) {
             
            }
            ball.JustMove(move);
        }


        private void FixedUpdate() {
            ball.Move(jump);
        }

    }
