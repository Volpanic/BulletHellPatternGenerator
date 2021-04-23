﻿using BulletHellGenerator.BulletEvents;
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
    public float SpeedModifier = 1;

    public float OrbitalVelcoity = 0;

    //LIFE
    public float MaxLifeTime = 1;
    private float lifeTimer = 0;

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

    private Vector3 orbitalDirection = Vector3.zero;
    private float orbitalAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
  
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

        if(OrbitalVelcoity != 0)
        {
            orbitalAngle += OrbitalVelcoity * Time.deltaTime;
            orbitalDirection.x = Mathf.Cos(orbitalAngle);
            orbitalDirection.y = Mathf.Sin(orbitalAngle);
        }

        transform.position += ((Direction + orbitalDirection).normalized * (MoveSpeed * SpeedModifier)) * Time.deltaTime;

        lifeTimer += Time.deltaTime;

        if(lifeTimer >= MaxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

// Pre JOBS 30~ FPS
