using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondRotate : MonoBehaviour {

    public float turnSpeed = 4.0f;
    public Transform player;

    public float height = 1f;
    public float distance = 2f;

    private Vector3 offsetX;
    private Vector3 offsetY;

    void Start() {

        offsetX = new Vector3(0, height, distance);
        offsetY = new Vector3(0, 0, distance);
    }

    void LateUpdate() {
       // offsetX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
        offsetY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offsetY;
        //transform.position = player.position + offsetX;
         transform.position = player.position + offsetY;
        transform.LookAt(player.position);
    }
}
