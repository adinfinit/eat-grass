using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    
    public float speed = 10.0f;
    public float dashMultiplier = 3.0f;
    public float dashDuration = 0.10f;
    public float doubleTapCooldown = 0.3f;
    private float doubleTapTimer = 0f;
    private float dashTimer = 0f;
    private bool currentlyDashing = false;
    private Vector3 dashDir =Vector3.zero;
    private KeyCode dashKey = KeyCode.Space;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion cameraDirection;
    private Component[] weaponControllers;

    public float MaxHealth = 200f;
    public float CurrentHealth = 200f;
    public float InvulnerableTime = 0.5f;  // time between taking damage
    private float InvulnerableTimer= 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraDirection = Quaternion.Euler(0, 45, 0);

        weaponControllers = GetComponentsInChildren<WeaponController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DoubleTap()) {
            currentlyDashing = true;
            print("DASH!");
        }

        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        moveDirection = new Vector3 (moveHorizontal, 0.0f, moveVertical);

        if (currentlyDashing) {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashDuration) {
                dashTimer = 0f;
                currentlyDashing = false;
            }

            moveDirection = dashMultiplier * dashDir;
        }

        characterController.Move((cameraDirection * moveDirection) * Time.deltaTime * speed);

        // Attack animation
        if (Input.GetKeyDown (KeyCode.Space)) { 
            foreach (WeaponController wc in weaponControllers) {
                wc.StartAttack();
            }
        }

        // constrain elevation
        transform.position = new Vector3 (transform.position.x, 1f, transform.position.z);
        InvulnerableTimer += Time.deltaTime;
    }

    bool DoubleTap() {
        if (Input.GetKeyDown(KeyCode.W)) {
            if (doubleTapTimer > 0 && doubleTapTimer < doubleTapCooldown && dashKey == KeyCode.W){
                doubleTapTimer = 0;
                dashDir = new Vector3(0, 0, 1);
                return true;
            } else {
                doubleTapTimer = 0;
                doubleTapTimer += Time.deltaTime;
                dashKey = KeyCode.W;
            }
        } 
        else if (Input.GetKeyDown(KeyCode.S)) {
            if (doubleTapTimer > 0 && doubleTapTimer < doubleTapCooldown && dashKey == KeyCode.S){
                doubleTapTimer = 0;
                dashDir = new Vector3(0, 0, -1);
                return true;
            } else {
                doubleTapTimer = 0;
                doubleTapTimer += Time.deltaTime;
                dashKey = KeyCode.S;
            }
        } 
        if (Input.GetKeyDown(KeyCode.D)) {
            if (doubleTapTimer > 0 && doubleTapTimer < doubleTapCooldown && dashKey == KeyCode.D){
                doubleTapTimer = 0;
                dashDir = new Vector3(1, 0, 0);
                return true;
            } else {
                doubleTapTimer = 0;
                doubleTapTimer += Time.deltaTime;
                dashKey = KeyCode.D;
            }
        } if (Input.GetKeyDown(KeyCode.A)) {
            if (doubleTapTimer > 0 && doubleTapTimer < doubleTapCooldown && dashKey == KeyCode.A){
                doubleTapTimer = 0;
                dashDir = new Vector3(-1, 0, 0);
                return true;
            } else {
                doubleTapTimer = 0;
                doubleTapTimer += Time.deltaTime;
                dashKey = KeyCode.A;
            }
        } 
        else {
            if (doubleTapCooldown > 0) {
                doubleTapTimer += Time.deltaTime;
            }
        }

        return false;
    }

    public void getAttacked() {
        print("Got hit!");
        if (InvulnerableTimer > InvulnerableTime) {
            InvulnerableTimer = 0f;
            CurrentHealth -= 40;
            print("Health left " + CurrentHealth);
        }
    }

}
