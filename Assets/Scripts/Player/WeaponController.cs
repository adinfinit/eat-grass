using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private float targetRotation;
    private float attackTimer;

    // Start is called before the first frame update
    void Start()
    {
        targetRotation = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += 10 * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, targetRotation, attackTimer), 0);
    }

    public void StartAttack()
    {
        print("Hyaaa");
        print(targetRotation);
        attackTimer = 0;
        transform.eulerAngles = Vector3.zero;
        targetRotation = 360f;
    }
}
