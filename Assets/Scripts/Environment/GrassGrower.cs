using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrassController))]
public class GrassGrower : MonoBehaviour
{
    public float Interval = 3f;
    public float Radius = 10.0f;

    private float countdown = 0.0f;

    private Vector3 lastPoint;

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown > 0) return;
        countdown += Interval;

        var controller = GetComponent<GrassController>();
        controller.GrowPatchy(0.5f);
        return;

        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;
        var point = new Vector3(
            Random.Range(0f, data.size.x),
            0,
            Random.Range(0f, data.size.z)
        );

        controller.PlantPatchy(point + terrain.transform.position, Radius, 0.7f);

        lastPoint = point + terrain.transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(lastPoint, Radius);
    }
}
