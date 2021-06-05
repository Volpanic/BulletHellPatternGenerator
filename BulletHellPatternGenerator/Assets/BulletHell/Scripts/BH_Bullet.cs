using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHellGenerator.Wrappers;
using static UnityEngine.ParticleSystem;

namespace BulletHellGenerator
{
    public class BH_Bullet : MonoBehaviour
    {
        public float MaxLifeTime = 1;
        private float lifeTimer = 0;

        public bool DisableWhenOffscreen = true;

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

        //Crust
        public Rigidbody Body;
        public Rigidbody2D Body2D;

        public Vector3 Direction = Vector3.right;
        public float MoveSpeed = 0;


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

            if (RotateRelativeToDirection)
                transform.localRotation = Quaternion.LookRotation(Vector3.forward, (RotationOffset * (RelativeDirection * Direction)).normalized);
            else
                transform.localRotation = RotationOffset;
        }

        private void OnBecameInvisible()
        {
            if (!DisableWhenOffscreen) return;
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}