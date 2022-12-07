using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleCamera : MonoBehaviour
{
    public Camera playerCam;
    //Layer to hit if needed for camera collision
    public LayerMask clippable;
    //Smoothening to reach target
    public float smoothTimeIn = 0.3F;
    //Smoothening to get out of target
    public float smoothTimeOut = 0.5F;
    //Current velocity
    private float yVelocity = 0.0F;
    //Clamp camera
    float yMinLimit = -90;
    float yMaxLimit = 30;

    //Check if using controller
    public bool usingMouse;
    //Inputs of the mouse
    public float horizontalMouse;
    public float verticalMouse;

    public Vector2 aimDelta;
    //Sensivity of mouse
    public float mouseSensitivity = 10;
    //Lock the cursor to the middle of the screen
    public bool lockCursor;

    //target to follow with the camera
    public Transform target;
    //Distance the camera has from the target
    public float distanceFromTarget = 2;
    public float idealDistanceFromTarget = 2;

    //Smoothening of the rotation
    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currenRotation;

    //Structure of all clip plane points
    public struct ClipPlanePoints {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public ClipPlanePoints cameraClipPlanePoints(float distance) {

        ClipPlanePoints clipPlanePoints = new ClipPlanePoints();

        Transform transform = playerCam.transform;
        Vector3 pos = transform.position;
        float halfFOV = (playerCam.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = playerCam.aspect;

        float height = Mathf.Tan(halfFOV) * distance;
        float width = height * aspect;

        //Lower Right
        clipPlanePoints.LowerRight = pos + transform.forward * distance;
        clipPlanePoints.LowerRight += transform.right * width;
        clipPlanePoints.LowerRight -= transform.up * height;

        //Lower Left
        clipPlanePoints.LowerLeft = pos + transform.forward * distance;
        clipPlanePoints.LowerLeft -= transform.right * width;
        clipPlanePoints.LowerLeft -= transform.up * height;

        //Upper Right
        clipPlanePoints.UpperRight = pos + transform.forward * distance;
        clipPlanePoints.UpperRight += transform.right * width;
        clipPlanePoints.UpperRight += transform.up * height;

        //Upper Left
        clipPlanePoints.UpperLeft = pos + transform.forward * distance;
        clipPlanePoints.UpperLeft -= transform.right * width;
        clipPlanePoints.UpperLeft += transform.up * height;

        return clipPlanePoints;
    }

    public void OnEnable()
    {
        // PlayerBall.highVelocity += delegate { SetFov(90); };
        // PlayerBall.lowVelocity += delegate { SetFov(70); };
    }

    public void OnDisable()
    {
        // PlayerBall.highVelocity -= delegate { SetFov(90); };
        // PlayerBall.lowVelocity -= delegate { SetFov(70); };
    }

    public void SetFov(float fov)
    {
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, fov, 2* Time.deltaTime);
    }

    // Use this for initialization
    void Start() {
        //Lock and hide cursor, escape to make cursor appear
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void FixedUpdate() {
        //Check if a collision happened


        bool checkIfCollision = false;

        //Clip points
        ClipPlanePoints nearPlanePointsCheck = cameraClipPlanePoints(0.35f);
        //Debug.Log (nearPlanePointsCheck.LowerLeft + ", " + nearPlanePointsCheck.LowerRight + ", " + nearPlanePointsCheck.UpperLeft + ", " + nearPlanePointsCheck.UpperRight);
        Debug.DrawLine(target.position, nearPlanePointsCheck.LowerLeft);
        Debug.DrawLine(target.position, nearPlanePointsCheck.LowerRight);
        Debug.DrawLine(target.position, nearPlanePointsCheck.UpperLeft);
        Debug.DrawLine(target.position, nearPlanePointsCheck.UpperRight);

        //Check where the ray hit
        RaycastHit hit;
        //List of points
        float[] hitDistances = { 0f, 0f, 0f, 0f };

        //Check if lower left hit
        if (Physics.Raycast(target.position, (nearPlanePointsCheck.LowerLeft - target.position), out hit, distanceFromTarget, clippable.value)) {
            Debug.DrawLine(target.position, hit.point, Color.black);
            checkIfCollision = true;
            hitDistances[0] = (hit.distance);
        }
        //Check if lower right hit
        if (Physics.Raycast(target.position, (nearPlanePointsCheck.LowerRight - target.position), out hit, distanceFromTarget, clippable.value)) {
            Debug.DrawLine(target.position, hit.point, Color.black);
            checkIfCollision = true;
            hitDistances[1] = (hit.distance);
        }
        //Check if upper left hit
        if (Physics.Raycast(target.position, (nearPlanePointsCheck.UpperLeft - target.position), out hit, distanceFromTarget, clippable.value)) {
            Debug.DrawLine(target.position, hit.point, Color.black);
            checkIfCollision = true;
            hitDistances[2] = (hit.distance);
        }
        //Check if upper right hit
        if (Physics.Raycast(target.position, (nearPlanePointsCheck.UpperRight - target.position), out hit, distanceFromTarget, clippable.value)) {
            Debug.DrawLine(target.position, hit.point, Color.black);
            checkIfCollision = true;
            hitDistances[3] = (hit.distance);
        }

        //Sort points to get the shortest one
        if (hitDistances.Length != 0 && hitDistances != null) {
            for (int x = 0; x < hitDistances.Length; x++) {
                if (hitDistances[0] >= hitDistances[x] && (hitDistances[0] - hitDistances[x]) > 0.005f && hitDistances[x] != 0f) {
                    hitDistances[0] = hitDistances[x];
                   // Debug.Log(hitDistances[0]);
                }

            }
        }

        //Debug.Log(checkIfCollision);

        //Check if camera collids with Clippable layer
        if (checkIfCollision == true) {
            //Get the distance between player and hit point
            float dis = hitDistances[0];
            //Debug.LogWarning (dis);
            float newPosition = Mathf.SmoothDamp(distanceFromTarget, dis, ref yVelocity, smoothTimeIn);
            //New distance to place camera in
            distanceFromTarget = newPosition;

        } else if (checkIfCollision == false) {
            //Dezoom from collision
            float normalPosition = Mathf.SmoothDamp(distanceFromTarget, idealDistanceFromTarget, ref yVelocity, smoothTimeOut);
            distanceFromTarget = normalPosition;
        }
        currenRotation = Vector3.SmoothDamp(currenRotation, new Vector3(-verticalMouse, horizontalMouse), ref rotationSmoothVelocity, rotationSmoothTime);

    }

    // Update is called once per frame
    void LateUpdate() {

        if (Input.GetKeyDown(KeyCode.C) && usingMouse) {
            usingMouse = false;
        }
        if (Input.GetKeyDown(KeyCode.X) && !usingMouse) {
            usingMouse = true;
        }

        //Movement of the camera which influences the player if he's moving, x or y
        
        if (usingMouse) {
            horizontalMouse += Input.GetAxis("Mouse X");
            verticalMouse += Input.GetAxis("Mouse Y");
        }
        if (!usingMouse) {
            horizontalMouse += Input.GetAxis("JoystickX");
            verticalMouse += Input.GetAxis("JoystickY");
        }
        //Clamp y
        verticalMouse = ClampAngle(verticalMouse, yMinLimit, yMaxLimit);

        //Calculating the rotation of the camera with smooth transition
        
        //Rotate the camera
        transform.eulerAngles = currenRotation;

        //Position of the camera
        transform.position = target.position - transform.forward * distanceFromTarget;

    }
    
    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}

