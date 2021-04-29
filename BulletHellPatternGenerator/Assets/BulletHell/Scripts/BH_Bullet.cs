using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BH_Bullet : MonoBehaviour
{
    // Positinal Velocity
    // Rotatinal Velocity
    // Orbital Velocity
    // LIFE

    public Quaternion RelativeDirection;
    public float SpeedModifier = 1;

    public Quaternion OrbitalVelcoity = Quaternion.identity;

    [HideInInspector]
    public BulletHellPatternGenerator Creator;

    //LIFE
    public float MaxLifeTime = 1;
    private float lifeTimer = 0;

    private float heatTimer = 0;

    //Rotation Stuff
    public bool RotateRelativeToDirection;
    public Quaternion RotationOffset;

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

    // Start is called before the first frame update
    void Start()
    {
    }

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

        Direction += ((OrbitalVelcoity) * Direction) * Time.fixedDeltaTime;
        transform.position += ((RelativeDirection * Direction).normalized * (MoveSpeed * SpeedModifier)) * Time.fixedDeltaTime;

        lifeTimer += Time.fixedDeltaTime;
        heatTimer += Time.fixedDeltaTime;

        if (lifeTimer >= MaxLifeTime)
        {
            Destroy(gameObject);
        }

        if (heatTimer >= 0.01f && Creator.HeatmapGen != null && Creator.HeatmapGen.isActiveAndEnabled)
        {
            Creator.HeatmapGen.AddHeatWorldPos(transform.position);
            heatTimer = 0;
        }

        transform.localRotation = Quaternion.LookRotation(Vector3.forward, (RotationOffset * (RelativeDirection * Direction)).normalized);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

// Pre JOBS 30~ FPS
