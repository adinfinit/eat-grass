using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuupController : MonoBehaviour
{
    public GameObject[] bloodPrefabs;

    public GameObject player;
    public float speed = 2.0f;
    public float knockbackSpeed = 3.0f;
    public float attackDistance = 2.0f;
    public float health = 100.0f;

    public float wanderSpeed = 0.5f;
    public float wanderingMinDuration = 100.0f;
    public float wanderingMaxDuration = 150.0f;
    public enum State { Wandering, Aggressive, Charging, Attacking, Knockback };
    public State currentState;

    private Vector3 randomDirection;
    private Rigidbody rb;
    private SphereCollider triggerCollider;
    private float newDirectionCountdown = 0.0f;
    private bool randomStanding = false;
    private Vector3 chargePosition;
    private float knockBackTime = -1.0f;
    private Animator anim;
    private ShoopAttack shoopAttack;
    private HealthBar healthBar;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        currentState = State.Wandering;

        rb = gameObject.GetComponent<Rigidbody>();
        shoopAttack = gameObject.GetComponentInChildren<ShoopAttack>();

        triggerCollider = gameObject.GetComponent<SphereCollider>();

        if ((player.transform.position - transform.position).magnitude < triggerCollider.radius)
        {
            currentState = State.Aggressive;
        }

        anim = GetComponentInChildren<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
    }

    // Update is called once per frame
    void Update()
    {

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }

        if (currentState == State.Aggressive)
        {
            chargePosition = player.transform.position;
            Vector3 chargeDirection = (chargePosition - transform.position).normalized;
            currentState = State.Charging;

            rb.MoveRotation(Quaternion.LookRotation(new Vector3(chargeDirection.x, 0f, chargeDirection.z)));

        }
        else if (currentState == State.Charging)
        {
            anim.SetBool("Running", true);
            anim.SetFloat("Speed", 3f);
            Vector3 chargeDirection = (chargePosition - transform.position).normalized;


            rb.MovePosition(transform.position + chargeDirection * speed * Time.deltaTime);

            if ((chargePosition - transform.position).magnitude < 0.15f
                & (player.transform.position - transform.position).magnitude > attackDistance)
            {
                currentState = State.Aggressive;
            }
            if ((player.transform.position - transform.position).magnitude <= attackDistance)
            {
                currentState = State.Attacking;
                anim.SetTrigger("Attack");
                shoopAttack.Attack();
            }
        }

        else if (currentState == State.Wandering)
        {
            if (newDirectionCountdown <= 0.0f)
            {
                randomDirection = (new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f))).normalized;

                rb.MoveRotation(Quaternion.LookRotation(randomDirection));

                newDirectionCountdown = Random.Range(wanderingMinDuration, wanderingMaxDuration);
                randomStanding = (Random.value > 0.5f);
            }
            if (randomStanding == false)
            {
                anim.SetBool("Running", true);
                anim.SetFloat("Speed", 0.5f);
                rb.MovePosition(transform.position + randomDirection * wanderSpeed * Time.deltaTime);
            }
            else
            {
                anim.SetBool("Running", false);
                anim.SetFloat("Speed", 0.5f);
            }

            newDirectionCountdown -= 1.0f;

            var grass = Terrain.activeTerrain.GetComponent<GrassController>();
            var count = 0;
            count += grass.CutCounted(transform.position, 3f);

            if (WorldController.instance != null) WorldController.instance.SheepCut(count);
        }
        else if (currentState == State.Attacking)
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
        else if (currentState == State.Knockback)
        {
            if (Time.time - knockBackTime > 0.5f)
            {
                currentState = State.Wandering;
            }

        }


        healthBar.transform.localScale = new Vector3(2 * (Mathf.Clamp(health, 0f, 100f) / 100f), 0.2f, 0f);
        //transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
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

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Weapon")
        {

            if (Time.time - knockBackTime > 0.1f)
            {
                // damage
                this.health -= other.gameObject.GetComponent<WeaponInfo>().weaponController.damage;

                // sound
                EventManager.TriggerEvent("SheepBaa");

                // blood spawn
                GameObject randomBloodPrefab = SelectRandomBloodPrefab();
                if (randomBloodPrefab)
                    Instantiate(randomBloodPrefab, this.transform.position, Quaternion.LookRotation(other.contacts[0].normal));


                // knockback 
                Vector3 velocityChange = -(player.transform.position - transform.position);
                rb.MovePosition(transform.position + velocityChange * 1.0f);

                velocityChange.y = 0.0f;
                rb.velocity = velocityChange * knockbackSpeed;
                knockBackTime = Time.time;
                currentState = State.Knockback;

            }
        }
    }

    GameObject SelectRandomBloodPrefab()
    {
        if (bloodPrefabs.Length > 0)
        {
            int index = Random.Range(0, bloodPrefabs.Length - 1);
            return bloodPrefabs[index];
        }

        return null;

    }
}
