using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuupController : MonoBehaviour
{
    public GameObject player;
    public float speed = 2.0f;
    public float wanderSpeed = 0.5f;
    public bool trigger;
    public Vector3 randomDirection;
    private Rigidbody rb;
    private SphereCollider triggerCollider;
    private float newDirectionCountdown = 0.0f;
    private bool randomStanding = false;
    private Vector3 chargePosition; 

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        triggerCollider = gameObject.GetComponent<SphereCollider>();

        if((player.transform.position - transform.position).magnitude < triggerCollider.radius)
        {
            trigger = true;
        }
        else { trigger = false; }
    }

    // Update is called once per frame
    void Update() {
        if (trigger)
        {

            chargePosition = player.transform.position;

            Vector3 chargeDirection = (chargePosition - transform.position).normalized;

            Debug.Log(chargeDirection);

            rb.velocity = chargeDirection * speed;
        }
        else
        {   
            if (newDirectionCountdown <= 0.0f)
            {
                randomDirection = (new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f))).normalized;
                newDirectionCountdown = Random.Range(50.0f, 100.0f);
                randomStanding = (Random.value > 0.5f);
            }
            if (randomStanding == false)
            {
                transform.position += randomDirection * wanderSpeed * Time.deltaTime;
            }

            newDirectionCountdown -= 1.0f ;

        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) {
            trigger = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            trigger = false;
        }
    }
}
