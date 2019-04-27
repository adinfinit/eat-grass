using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target = null;

    public float distance = 10f;

    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    const float pitch = 20;
    const float yaw = 45;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (target == null) return;

        var offset = Quaternion.Euler(pitch, yaw, 0) * new Vector3(0, 0, distance);
        var targetPosition = target.transform.position - offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
