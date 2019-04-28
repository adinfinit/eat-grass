using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFlip : MonoBehaviour
{
    void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.flipX = Random.Range(0.0f, 1.0f) < 0.5f;
        enabled = false;
    }
}
