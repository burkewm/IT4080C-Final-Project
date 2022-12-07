using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObj : MonoBehaviour {
    public float turnSpeed = 4.0f;
    public Transform player;

    public float height = 1f;
    public float distance = 2f;

    private Vector3 offsetX;
    private Vector3 offsetY;

    public bool usingMouse;

    void Start() {

        offsetX = new Vector3(0, height, distance);
        offsetY = new Vector3(0, 0, distance);
    }

    void FixedUpdate() {


        if (Input.GetKeyDown(KeyCode.Y)) {

        }
        if (Input.GetKeyDown(KeyCode.Y)) {

        }
        if (usingMouse) {
            offsetX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
            offsetY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offsetY;
        } else {
            offsetX = Quaternion.AngleAxis(Input.GetAxis("JoystickX") * turnSpeed, Vector3.up) * offsetX;
            offsetY = Quaternion.AngleAxis(Input.GetAxis("JoystickY") * turnSpeed, Vector3.right) * offsetY;
        }
    }
    private void Update() {
        transform.position = player.position + offsetX;
        //transform.position = player.position + offsetY;
        transform.LookAt(player.position);
    }
}
	

