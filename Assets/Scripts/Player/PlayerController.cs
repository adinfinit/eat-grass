using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    
    public float speed = 6.0f;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion cameraDirection;
    private Component[] weaponControllers;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraDirection = Quaternion.Euler(0, 45, 0);

        weaponControllers = GetComponentsInChildren<WeaponController>();
        print(weaponControllers[0]);
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        moveDirection = new Vector3 (moveHorizontal, 0.0f, moveVertical);
        characterController.Move((cameraDirection * moveDirection) * Time.deltaTime * speed);

        // Attack animation
        if (Input.GetKeyDown (KeyCode.Space)) { 
            print("attack!");
            foreach (WeaponController wc in weaponControllers) {
                wc.StartAttack();
            }
        }
    }
}
