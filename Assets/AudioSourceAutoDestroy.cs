using UnityEngine;
using System.Collections;

public class AudioSourceAutoDestroy : MonoBehaviour
{
    private AudioSource _as;


    public void Start()
    {
        _as = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (_as)
        {
            if (!_as.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}