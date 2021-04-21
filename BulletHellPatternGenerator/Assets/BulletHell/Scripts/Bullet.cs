using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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

    public Vector3 Velocity
    {
        get
        {
            // Rigidbodies take priority in case user wants to use them instead
            if (Body   != null) return   Body.velocity;
            if (Body2D != null) return Body2D.velocity;
            return direction * MoveSpeed;
        }

        set
        {
            if (Body   != null) Body.velocity  = value; 
            if (Body2D != null)Body2D.velocity = value;
            direction = value.normalized;
            MoveSpeed = value.magnitude;
        }
    }

    private Vector3 direction = Vector3.right;
    public float MoveSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * MoveSpeed;
    }
}
