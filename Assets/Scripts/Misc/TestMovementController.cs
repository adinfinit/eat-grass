using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementController : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public Vector3 direction = Vector3.zero;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb == null) return;

        var dx = Input.GetAxis("Horizontal");
        var dy = Input.GetAxis("Vertical");

        direction = new Vector3(dx, 0, dy);
        if (direction.magnitude > 1.0f) direction.Normalize();
    }

    void FixedUpdate()
    {
        // rb.AddForce(direction * maxSpeed);
        rb.velocity = direction * maxSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}
