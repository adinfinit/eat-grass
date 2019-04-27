using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float attackSpeed = 2f;
    private float targetRotation;
    private float attackTimer;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        targetRotation = transform.localEulerAngles.y;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += attackSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, targetRotation, attackTimer), 0);
    }

    public void StartAttack()
    {
        print("Hyaaa");
        print(targetRotation);
        anim.SetTrigger("Attack");
        attackTimer = 0;
        transform.eulerAngles = Vector3.zero;
        targetRotation = 360f;
    }
}
