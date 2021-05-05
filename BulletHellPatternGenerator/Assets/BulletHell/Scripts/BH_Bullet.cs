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


    public float MaxLifeTime = 1;
    private float lifeTimer = 0;

    public Quaternion RelativeDirection;
    public Quaternion OrbitalVelcoity = Quaternion.identity;

    [HideInInspector]
    public Transform Target;

    [Min(0)]
    public float HomingSpeed = 0;
    public float SpeedModifier = 1;

    private float heatTimer = 0;

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

    //public Vector3 Velocity
    //{
    //    get
    //    {
    //        // Rigidbodies take priority in case user wants to use them instead
    //        if (Body != null) return Body.velocity;
    //        if (Body2D != null) return Body2D.velocity;
    //        return Direction * MoveSpeed;
    //    }

    //    set
    //    {
    //        if (Body != null) Body.velocity = value;
    //        if (Body2D != null) Body2D.velocity = value;
    //        Direction = value.normalized;
    //        MoveSpeed = value.magnitude;
    //    }
    //}

    public Vector3 Direction = Vector3.right;
    public float MoveSpeed = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        //Bullet Event stack
        if (bulletEvents != null && bulletEvents.Count > 0)
        {
            if (bulletEvents[bulletEventIndex].OnUpdate(this))
            {
                bulletEventIndex++;

                //Still in bounds
                if (bulletEventIndex >= bulletEvents.Count)
                {
                    bulletEventIndex = 0;
                }

                bulletEvents[bulletEventIndex].OnReset();
            }
        }


        //Apply Orbital velcoity
        Direction += ((OrbitalVelcoity) * Direction) * Time.fixedDeltaTime;

        //Homing
        if (HomingSpeed != 0 && Target != null)
        {
            Direction = Vector3.RotateTowards(Direction,(Target.position - transform.position).normalized,HomingSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime,0.0f);
        }

        transform.position += ((RelativeDirection * Direction).normalized * (MoveSpeed * SpeedModifier)) * Time.fixedDeltaTime;

        lifeTimer += Time.fixedDeltaTime;
        heatTimer += Time.fixedDeltaTime;

        if (lifeTimer >= MaxLifeTime)
        {
            Destroy(gameObject);
        }

        RotationOffset = Quaternion.Euler(RotationOffset.eulerAngles + (RotationalVelocity
            * Extension.MinMaxEvaluate(RotatinalVelocityModifier, lifeTimer / MaxLifeTime)) * Time.deltaTime);
        transform.localRotation = Quaternion.LookRotation(Vector3.forward, (RotationOffset * (RelativeDirection * Direction)).normalized);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

// Pre JOBS 30~ FPS
