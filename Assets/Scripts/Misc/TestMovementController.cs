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
            RemoveGrass(terrain, transform.position, 3f);
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

    // Set all pixels in a detail map to zero.
    void RemoveGrass(Terrain terrain, Vector3 at, float radius)
    {
        if (terrain == null) return;

        float dpx = terrain.terrainData.detailResolution / terrain.terrainData.size.x;
        float dpz = terrain.terrainData.detailResolution / terrain.terrainData.size.z;

        // todo use matrix
        float cx = (at.x - terrain.transform.position.x) * dpx;
        float cz = (at.z - terrain.transform.position.z) * dpz;

        int x0 = (int)Mathf.Floor(cx - radius * dpx);
        int x1 = (int)Mathf.Ceil(cx + radius * dpx);

        int y0 = (int)Mathf.Floor(cz - radius * dpz);
        int y1 = (int)Mathf.Ceil(cz + radius * dpz);

        Debug.LogFormat("{0}:{1}  {2}:{3}", x0, x1, y0, y1);

        // Get all of layer zero.
        var cut0 = terrain.terrainData.GetDetailLayer(x0, y0, x1 - x0, y1 - y0, 0);
        var cut1 = terrain.terrainData.GetDetailLayer(x0, y0, x1 - x0, y1 - y0, 1);
        var cut2 = terrain.terrainData.GetDetailLayer(x0, y0, x1 - x0, y1 - y0, 2);

        // For each pixel in the detail map...
        //for (var y = 0; y < terrain.terrainData.detailHeight; y++)
        //for (var y = y0; y < y1; y++)
        for (var y = 0; y < cut0.GetLength(1); y++)
        {
            // for (var x = 0; x < terrain.terrainData.detailWidth; x++)
            //for (var x = x0; x < x1; x++)
            for (var x = 0; x < cut0.GetLength(0); x++)
            {
                if (cut1[x, y] > 0) cut2[x, y] = 1;
                cut1[x, y] = cut0[x, y];
                cut0[x, y] = 0;
            }
        }

        // Assign the modified map back.
        terrain.terrainData.SetDetailLayer(x0, y0, 0, cut0);
        terrain.terrainData.SetDetailLayer(x0, y0, 1, cut1);
        terrain.terrainData.SetDetailLayer(x0, y0, 2, cut2);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }


    ///
    /// Cuts the grass. BE CAREFULL: Grass may not respawn after game reset!!!!!! 
    ///
    /// The effected Terrain, if only one Terrain pass null
    /// The world position you want to cut the grass
    /// the radius of the square
    public void CutGrass(Terrain t, Vector3 position, float radius)
    {
        int TerrainDetailMapSize = t.terrainData.detailResolution;
        if (t.terrainData.size.x != t.terrainData.size.z)
        {
            Debug.Log("X and Y Size of terrain have to be the same (RemoveGrass.CS Line 43)");
            return;
        }

        float PrPxSize = TerrainDetailMapSize / t.terrainData.size.x;

        Vector3 TexturePoint3D = position - t.transform.position;
        TexturePoint3D = TexturePoint3D * PrPxSize;

        float[] xymaxmin = new float[4];
        xymaxmin[0] = TexturePoint3D.z + radius;
        xymaxmin[1] = TexturePoint3D.z - radius;
        xymaxmin[2] = TexturePoint3D.x + radius;
        xymaxmin[3] = TexturePoint3D.x - radius;

        int[,] map = t.terrainData.GetDetailLayer(0, 0, t.terrainData.detailWidth, t.terrainData.detailHeight, 0);

        for (int y = 0; y < t.terrainData.detailHeight; y++)
        {
            for (int x = 0; x < t.terrainData.detailWidth; x++)
            {

                if (xymaxmin[0] > x && xymaxmin[1] < x && xymaxmin[2] > y && xymaxmin[3] < y)
                    map[x, y] = 0;
            }
        }
        t.terrainData.SetDetailLayer(0, 0, 0, map);
    }
}
