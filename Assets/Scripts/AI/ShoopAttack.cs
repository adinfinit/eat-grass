using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoopAttack : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider bc;
    private bool canAttack = false;
    private GameObject target;

    void Start()
    {
        bc = gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack() {
        if (canAttack) {
            print("Shoop hit!");
            target.GetComponent<PlayerController>().getAttacked();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            target = other.gameObject;
            canAttack = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            canAttack = false;
        }
    }
}
