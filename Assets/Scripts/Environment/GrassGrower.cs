using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrassController))]
public class GrassGrower : MonoBehaviour
{
    public float Interval = 3.0f;
    private float countdown = 0.0f;

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown > 0) return;
        countdown = Interval;

        var controller = GetComponent<GrassController>();
        controller.Grow();
    }
}
