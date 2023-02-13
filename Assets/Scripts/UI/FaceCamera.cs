using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        if(Camera.main == null) { return; }

        Vector3 position = transform.position + Camera.main.transform.rotation * Vector3.forward;
        Vector3 rotation = Camera.main.transform.rotation * Vector3.up;

        transform.LookAt(position, rotation);
    }
}
