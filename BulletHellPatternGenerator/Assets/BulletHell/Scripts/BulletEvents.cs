﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BulletHellGenerator.BulletEvents
{
    [System.Serializable]
    public abstract class BulletEvent
    {
        public abstract void OnReset(); // Reset all values in case events repeat
        public abstract bool OnUpdate(BH_Bullet bullet); // Update the event, returns true if event done.

#if UNITY_EDITOR
        public virtual void OnInspectorGUI(BH_Bullet bullet) { }
#endif
    }

    #region // Wait for X Seconds

    [System.Serializable]
    public class BEWaitForSeconds : BulletEvent
    {
        public float MaxTime = 1;
        private float timer = 0;

        public override void OnReset()
        {
            timer = 0;
        }

        public override bool OnUpdate(BH_Bullet bullet)
        {
            timer += Time.fixedDeltaTime;

            if (timer >= MaxTime) return true;
            return false;
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI(BH_Bullet bullet)
        {
            MaxTime = Mathf.Abs(EditorGUILayout.FloatField("Wait for time duration", MaxTime));
            if (MaxTime == 0) MaxTime = 0.01f;
        }
#endif
    }
    #endregion

    #region // Transition To move speed
    [System.Serializable]
    public class BEToMoveSpeed : BulletEvent
    {
        public float TransitonDuration = 1;
        public float TargetSpeed = 0;
        public bool Relative = false;
        private float timer = 0;

        private float initalSpeed = 0;
        private float targetSpeed = 0;
        private bool hasGottenInitalSpeed = false;

        public override void OnReset()
        {
            timer = 0;
            initalSpeed = 0;
            targetSpeed = 0;
            hasGottenInitalSpeed = false;
        }

        // Edit this later
        public override bool OnUpdate(BH_Bullet bullet)
        {
            if (!hasGottenInitalSpeed)
            {
                hasGottenInitalSpeed = true;
                initalSpeed = bullet.MoveSpeed;

                //Adjust to be relative if should
                targetSpeed = TargetSpeed;

                if (Relative)
                {
                    targetSpeed += bullet.MoveSpeed;
                }
            }

            timer += Time.deltaTime;
            bullet.MoveSpeed = Mathf.Lerp(initalSpeed, targetSpeed, timer / TransitonDuration);

            if (timer >= TransitonDuration) return true;
            return false;
        }

#if UNITY_EDITOR
        public override void OnInspectorGUI(BH_Bullet bullet)
        {
            TransitonDuration = Mathf.Abs(EditorGUILayout.FloatField("Transition Duration", TransitonDuration));
            TargetSpeed = EditorGUILayout.FloatField("Target Speed", TargetSpeed);
            if (TransitonDuration == 0) TransitonDuration = 0.01f;
            Relative = EditorGUILayout.Toggle("Relative", Relative);
        }
#endif
    }
    #endregion

    #region // Transition To move speed
    [System.Serializable]
    public class BEToHomingSpeed : BulletEvent
    {
        public float TransitonDuration = 1;
        public float TargetSpeed = 0;
        public bool Relative = false;
        private float timer = 0;

        private float initalSpeed = 0;
        private float targetSpeed = 0;
        private bool hasGottenInitalSpeed = false;

        public override void OnReset()
        {
            timer = 0;
            initalSpeed = 0;
            targetSpeed = 0;
            hasGottenInitalSpeed = false;
        }

        // Edit this later
        public override bool OnUpdate(BH_Bullet bullet)
        {
            if (!hasGottenInitalSpeed)
            {
                hasGottenInitalSpeed = true;
                initalSpeed = bullet.HomingSpeed;

                //Adjust to be relative if should
                targetSpeed = TargetSpeed;

                if (Relative)
                {
                    targetSpeed += bullet.HomingSpeed;
                }
            }

            timer += Time.deltaTime;
            bullet.HomingSpeed = Mathf.Lerp(initalSpeed, targetSpeed, timer / TransitonDuration);

            if (timer >= TransitonDuration) return true;
            return false;
        }

#if UNITY_EDITOR
        public override void OnInspectorGUI(BH_Bullet bullet)
        {
            TransitonDuration = Mathf.Abs(EditorGUILayout.FloatField("Transition Duration", TransitonDuration));
            TargetSpeed = EditorGUILayout.FloatField("Target Homing Speed", TargetSpeed);
            if (TransitonDuration == 0) TransitonDuration = 0.01f;
            Relative = EditorGUILayout.Toggle("Relative", Relative);
        }
#endif
    }
    #endregion

    #region // Rotate Direction

    [System.Serializable]
    public class BERotateTowardsDirection : BulletEvent
    {
        public float TransitonDuration = 1;
        public Vector3 TargetDirection = Vector3.right;
        public bool Relative = true;

        //Used to store the relative direction if needed
        private Vector3 targetDirection = Vector3.zero;
        private float timer = 0;

        private Vector3 initalDirection = Vector3.zero;
        private bool hasGottenInitalSpeed = false;

        public override void OnReset()
        {
            timer = 0;
            initalDirection = Vector3.zero;
            hasGottenInitalSpeed = false;
        }

        // Edit this later
        public override bool OnUpdate(BH_Bullet bullet)
        {
            if (!hasGottenInitalSpeed)
            {
                hasGottenInitalSpeed = true;
                initalDirection = bullet.Direction;

                targetDirection = TargetDirection;

                if (Relative)
                {
                    float ang = Mathf.Atan2(bullet.Direction.y, bullet.Direction.x) + Mathf.Atan2(targetDirection.y, targetDirection.x);
                    targetDirection = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), targetDirection.z);
                }
            }

            timer += Time.deltaTime;
            //bullet.MoveSpeed = Mathf.Lerp(initalSpeed, TargetSpeed, timer / TransitonDuration);
            bullet.Direction = Vector3.Slerp(initalDirection, targetDirection, timer / TransitonDuration);

            if (timer >= TransitonDuration) return true;
            return false;
        }

#if UNITY_EDITOR
        public override void OnInspectorGUI(BH_Bullet bullet)
        {
            TransitonDuration = Mathf.Abs(EditorGUILayout.FloatField("Transition Duration", TransitonDuration));
            TargetDirection = EditorGUILayout.Vector3Field("Target Direction", TargetDirection);
            Relative = EditorGUILayout.Toggle("Relative", Relative);
            if (TransitonDuration == 0) TransitonDuration = 0.01f;
        }
#endif
    }
    #endregion
}
