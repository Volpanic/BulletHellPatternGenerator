using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_Bullet : MonoBehaviour
{
    // Positinal Velocity
    // Rotatinal Velocity
    // Orbital Velocity
    // LIFE

    public Vector3 Forward;

    //LIFE
    public float MaxLifeTime = 1;
    private float lifeTimer = 0;

    //Positinal
    public Vector2 PositionalVelocity;

    //Rotation
    public float RotationalVelocity;

    //Oribtal
    public float OrbitalVelocity;

    //Crust
    public Rigidbody Body;
    public Rigidbody2D Body2D;

    [SerializeReference]
    public List<BulletEvents> bulletEvents = new List<BulletEvents>();
    private int bulletEventIndex;

    public Vector3 Velocity
    {
        get
        {
            // Rigidbodies take priority in case user wants to use them instead
            if (Body != null) return Body.velocity;
            if (Body2D != null) return Body2D.velocity;
            return Direction * MoveSpeed;
        }

        set
        {
            if (Body != null) Body.velocity = value;
            if (Body2D != null) Body2D.velocity = value;
            Direction = value.normalized;
            MoveSpeed = value.magnitude;
        }
    }

    public Vector3 Direction = Vector3.right;
    public float MoveSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        MoveSpeed = 1;
        MaxLifeTime = 8;
    }

    // Update is called once per frame
    void Update()
    {
        //Bullet Event stack
        if (bulletEvents != null)
        {
            if(bulletEvents[bulletEventIndex].OnUpdate(this))
            {
                bulletEventIndex++;

                //Still in bounds
                if(bulletEventIndex >= bulletEvents.Count)
                {
                    bulletEventIndex = 0;
                }

                bulletEvents[bulletEventIndex].OnReset();
            }
        }

        transform.position += (Direction * MoveSpeed) * Time.deltaTime;

        lifeTimer += Time.deltaTime;

        if(lifeTimer >= MaxLifeTime)
        {
            Destroy(gameObject);
        }
    }
}

// Pre JOBS 30~ FPS
