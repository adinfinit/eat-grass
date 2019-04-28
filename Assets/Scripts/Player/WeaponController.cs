using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Generic")]

    public float attackSpeed = 0.75f;
    public float damage = 30.0f;

    [Header("Forward Slash Settings")]
    [Range(0.0f, 360f)] public float forwardSlashArcAngle = 75.0f;
    [Range(2.0f, 15f)] public float forwardSlashArcArea = 7.0f;


    [Header("Forward Slash Settings")]
    [Range(4.0f, 20.0f)] public float areaSlashArcArea = 10.0f;

    [Header("Particles")]

    public GameObject bloodSplatter;
    public GameObject grassAreaSplatter;
    public GameObject grassConeSplatter;

    public AnimationCurve forwardSlashCurve;
    public AnimationCurve areaSlashCurve;


    public enum AttackType { None, ForwardSlash, AreaSlash };
    private AttackType attackType = AttackType.None;

    private float targetRotation;
    private float attackStartTime;

    private Animator anim;
    private Component sword;

    private GameObject mainCamera;
    private Transform weaponPivot;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        targetRotation = transform.localEulerAngles.y;
        anim = GetComponentInChildren<Animator>();
        sword = GetComponentInChildren<WeaponPivot>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        weaponPivot = transform.parent;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float attackTimer = Time.time - attackStartTime;

        // Vector3 currentRotationEuler = weaponPivot.rotation.eulerAngles;

        // initialize variables
        float curveValue;

        if (attackTimer > attackSpeed)
            attackType = AttackType.None;

        switch (attackType)
        {
            case AttackType.ForwardSlash:
                curveValue = forwardSlashCurve.Evaluate(attackTimer / attackSpeed);
                weaponPivot.localRotation = Quaternion.Euler(0, -curveValue * targetRotation, 0);

                break;

            case AttackType.AreaSlash:
                curveValue = areaSlashCurve.Evaluate(attackTimer / attackSpeed);
                weaponPivot.localRotation = Quaternion.Euler(0, -curveValue * targetRotation, 0);
                break;

            case AttackType.None:
                break;
        }
    }

    public void StartAttack()
    {
        anim.SetTrigger("Attack");
        attackStartTime = Time.time;
        targetRotation = 360f;

        // Player forward;
        Vector3 direction = playerTransform.forward;

        var grass = Terrain.activeTerrain.GetComponent<GrassController>();

        grass.CutArc(transform.position, forwardSlashArcArea, direction, forwardSlashArcAngle);
        grass.CutArc(transform.position, forwardSlashArcArea - 2.0f, direction, forwardSlashArcAngle * 0.7f);

        CreateParticleGameobject(grassConeSplatter, direction);
        attackType = AttackType.ForwardSlash;
    }


    public void StartAreaSlash()
    {
        anim.SetTrigger("Attack");
        attackStartTime = Time.time;
        targetRotation = 360f;

        var grass = Terrain.activeTerrain.GetComponent<GrassController>();
        var total = 0;
        total += grass.CutCounted(transform.position, areaSlashArcArea);
        total += grass.CutCounted(transform.position, areaSlashArcArea - 2);
        total += grass.CutCounted(transform.position, areaSlashArcArea - 4);

        if (WorldController.instance != null)
            WorldController.instance.PlayerCut(total);

        CreateParticleGameobject(grassAreaSplatter, Vector3.left);

        attackType = AttackType.AreaSlash;
    }

    private void CreateParticleGameobject(GameObject vfx, Vector3 lookAt)
    {
        if (vfx)
        {
            GameObject _go_vfx = Instantiate(vfx, this.transform.position,
                Quaternion.LookRotation(lookAt));

            _go_vfx.AddComponent<ParticleSystemAutoDestroy>();

        }
    }
}
