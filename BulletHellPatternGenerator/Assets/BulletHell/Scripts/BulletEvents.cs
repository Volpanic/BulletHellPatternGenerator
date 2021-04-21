using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BulletHellGenerator.BulletEvents
{
    [System.Serializable]
    public abstract class BulletEvents
    {
        public abstract void OnReset(); // Reset all values in case events repeat
        public abstract bool OnUpdate(Bullet bullet); // Update the event, returns true if event done.

#if UNITY_EDITOR
        public virtual void OnInspectorGUI(Bullet bullet) { }
#endif
    }

    [System.Serializable]
    public class BEWaitForSeconds : BulletEvents
    {
        private float MaxTime = 1;
        private float timer = 0;

        public override void OnReset()
        {
            timer = 0;
        }

        public override bool OnUpdate(Bullet bullet)
        {
            timer += Time.deltaTime;

            if (timer >= MaxTime) return true;
            return false;
        }

        public override void OnInspectorGUI(Bullet bullet)
        {
            MaxTime = Mathf.Abs(EditorGUILayout.FloatField(MaxTime));
            if (MaxTime == 0) MaxTime = 0.01f;
        }
    }

    [System.Serializable]
    public class BEToMoveSpeed : BulletEvents
    {
        private float TransitonDuration = 1;
        private float TargetSpeed = 0;
        private float timer = 0;

        private float initalSpeed = 0;
        private bool hasGottenInitalSpeed = false;

        public override void OnReset()
        {
            timer = 0;
            initalSpeed = 0;
            hasGottenInitalSpeed = false;
        }

        // Edit this later
        public override bool OnUpdate(Bullet bullet)
        {
            if(!hasGottenInitalSpeed)
            {
                hasGottenInitalSpeed = true;
                initalSpeed = bullet.MoveSpeed;
            }

            timer += Time.deltaTime;

            if (timer >= TransitonDuration) return true;
            return false;
        }

        public override void OnInspectorGUI(Bullet bullet)
        {
            TransitonDuration = Mathf.Abs(EditorGUILayout.FloatField(TransitonDuration));
            TargetSpeed = EditorGUILayout.FloatField(TargetSpeed);
            if (TransitonDuration == 0) TransitonDuration = 0.01f;
        }
    }
}
