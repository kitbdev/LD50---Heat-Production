using UnityEngine;

public class FaceCamera : MonoBehaviour {
    Transform cam;

    void Awake() {
        cam = Camera.main.transform;
    }

    void Update() {
        // transform.forward = cam.forward;
        // transform.rotation = Quaternion .LookRotation( transform.position - cam.position );
        transform.LookAt(cam, Vector3.up);
    }
}