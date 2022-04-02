using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class FollowTransform : MonoBehaviour {

    public Transform target;
    public Vector3 offset = Vector3.zero;

    private void Update() {
        transform.position = target.position + offset;
    }
}