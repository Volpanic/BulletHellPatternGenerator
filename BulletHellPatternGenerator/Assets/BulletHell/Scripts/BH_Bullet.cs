using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHellGenerator.Wrappers;
using static UnityEngine.ParticleSystem;

public class BH_Bullet : MonoBehaviour
{
    // Positinal Velocity
    // Rotatinal Velocity
    // Orbital Velocity
    // LIFE

    public enum BulletEventLoopMode
    {
        Stop = 0,
        Repeat = 1
    }

    public float MaxLifeTime = 1;
    private float lifeTimer = 0;

    public Quaternion RelativeDirection;
    public Quaternion OrbitalVelcoity = Quaternion.identity;

    [HideInInspector]
    public Transform Target;

    [Min(0)]
    public float HomingSpeed = 0;
    public float SpeedModifier = 1;


    //Rotation Stuff
    public bool RotateRelativeToDirection;
    public Quaternion RotationOffset;
    public Vector3 RotationalVelocity;
    public MinMaxCurve RotatinalVelocityModifier = new MinMaxCurve(1);

    //Crust
    public Rigidbody Body;
    public Rigidbody2D Body2D;

    [SerializeReference]
    public List<BulletEvents> bulletEvents = new List<BulletEvents>();
    private int bulletEventIndex;

    public Vector3 Direction = Vector3.right;
    public float MoveSpeed = 0;

    public BulletEventLoopMode LoopMode = BulletEventLoopMode.Repeat;

    private void Start()
    {
        transform.localRotation = RotationOffset;

        Body = GetComponent<Rigidbody>();
        Body2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        lifeTimer = 0;
        if (Body != null) Body.velocity = Vector3.zero;
        if (Body2D != null) Body2D.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Bullet Event stack
        if (bulletEvents != null && bulletEvents.Count > 0 && bulletEventIndex >= 0)
        {
            if (bulletEvents[bulletEventIndex].OnUpdate(this))
            {
                bulletEventIndex++;

                //Still in bounds
                if (bulletEventIndex >= bulletEvents.Count)
                {
                    switch (LoopMode)
                    {
                        case BulletEventLoopMode.Stop:
                        {
                            bulletEventIndex = -1;
                            break;
                        }

                        default:
                        {
                            bulletEventIndex = 0;
                            bulletEvents[bulletEventIndex].OnReset();
                            break;
                        }
                    }
                }
            }
        }

        //Apply Orbital velcoity
        Direction += ((OrbitalVelcoity) * Direction) * Time.fixedDeltaTime;

        //Homing
        if (HomingSpeed != 0 && Target != null)
        {
            Direction = Vector3.RotateTowards(Direction, (Target.position - transform.position).normalized, HomingSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0f);
        }

        transform.position += ((RelativeDirection * Direction).normalized * (MoveSpeed * SpeedModifier)) * Time.fixedDeltaTime;

        lifeTimer += Time.fixedDeltaTime;

        if (lifeTimer >= MaxLifeTime)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);

        }

        if (RotationalVelocity != Vector3.zero)
        {
            RotationOffset = Quaternion.Euler(RotationOffset.eulerAngles + (RotationalVelocity
                * Extension.MinMaxEvaluate(RotatinalVelocityModifier, lifeTimer / MaxLifeTime)) * Time.deltaTime); 
        }

        if(RotateRelativeToDirection)
        transform.localRotation = Quaternion.LookRotation(Vector3.forward, (RotationOffset * (RelativeDirection * Direction)).normalized);
        else
        transform.localRotation = RotationOffset;
    }

    private void OnBecameInvisible()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
