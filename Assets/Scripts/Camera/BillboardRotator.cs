using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardRotator : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(CameraController.Pitch, CameraController.Yaw, 0);
    }
}
