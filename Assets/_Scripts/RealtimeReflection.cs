using UnityEngine;
using System.Collections;
 
public class RealtimeReflection : MonoBehaviour {
    
    ReflectionProbe probe;
    public Camera cam;
    void Awake() {
        probe = GetComponent<ReflectionProbe>();
    }
    
    void Update () {
        probe.transform.position = new Vector3(
            cam.transform.position.x, 
            cam.transform.position.y * -1, 
            cam.transform.position.z
        );
 
        probe.RenderProbe();
    }
}