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
    public float attackDistance = 2.0f;
    public float wanderingMinDuration = 100.0f;
    public float wanderingMaxDuration = 150.0f;
    private Rigidbody rb;
    private SphereCollider triggerCollider;
    private float newDirectionCountdown = 0.0f;
    private bool randomStanding = false;
    private Vector3 chargePosition; 
    public enum State { Wandering, Aggressive, Charging, Attacking};
    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        currentState = State.Wandering;

        rb = gameObject.GetComponent<Rigidbody>();

        triggerCollider = gameObject.GetComponent<SphereCollider>();

        if((player.transform.position - transform.position).magnitude < triggerCollider.radius)
        {
            currentState = State.Aggressive;
        }
    }

    // Update is called once per frame
    void Update() {
        if (currentState == State.Aggressive)
        {
            chargePosition = player.transform.position;
            Vector3 chargeDirection = (chargePosition - transform.position).normalized;
            currentState = State.Charging;

            transform.rotation = Quaternion.LookRotation(chargeDirection);

        }
        if (currentState == State.Charging)
        {

            Vector3 chargeDirection = (chargePosition - transform.position).normalized;


            rb.velocity = chargeDirection * speed;

            if((chargePosition - transform.position).magnitude < 0.15f
                & (player.transform.position - transform.position).magnitude > attackDistance)
            {
                currentState = State.Aggressive;
            }
            if((player.transform.position - transform.position).magnitude <= attackDistance)
            {
                currentState = State.Attacking;
            }
        }

        if (currentState == State.Wandering)
        {   
            if (newDirectionCountdown <= 0.0f)
            {
                randomDirection = (new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f))).normalized;
                transform.rotation = Quaternion.LookRotation(randomDirection);

                newDirectionCountdown = Random.Range(wanderingMinDuration, wanderingMaxDuration);
                randomStanding = (Random.value > 0.5f);
            }
            if (randomStanding == false)
            {
                transform.position += randomDirection * wanderSpeed * Time.deltaTime;
            }

            newDirectionCountdown -= 1.0f ;

        }
        if (currentState == State.Attacking)
        {
            if ((player.transform.position - transform.position).magnitude > attackDistance 
            & (player.transform.position - transform.position).magnitude < triggerCollider.radius)
            {
                currentState = State.Aggressive;
            }
            if ((player.transform.position - transform.position).magnitude >= triggerCollider.radius)
            {
                currentState = State.Wandering;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) {
            currentState = State.Aggressive;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            currentState = State.Wandering;
        }
    }
}
