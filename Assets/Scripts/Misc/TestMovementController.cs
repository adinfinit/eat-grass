using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementController : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public Vector3 direction = Vector3.zero;

    public Terrain terrain;

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
        direction = Quaternion.Euler(0, 45f, 0) * direction;
        if (direction.magnitude > 1.0f) direction.Normalize();

        if (Input.GetButtonDown("Fire1"))
        {
            var mower = terrain.GetComponent<GrassController>();
            mower.Cut(transform.position, 6f);
            mower.Cut(transform.position, 3f);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            var mower = terrain.GetComponent<GrassController>();
            // mower.Plant(transform.position, 5f);
            mower.Grow();
        }
    }

    void FixedUpdate()
    {
        if (direction.magnitude > 0.05f)
        {
            var velocity = rb.velocity;
            velocity.x = direction.x * maxSpeed;
            velocity.z = direction.z * maxSpeed;
            rb.velocity = velocity;
        }
        rb.velocity.Scale(new Vector3(0.9f, 1.0f, 0.9f));
    }
}
