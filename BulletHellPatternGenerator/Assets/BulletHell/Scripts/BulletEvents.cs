using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHellGenerator.BulletEvents
{
    [System.Serializable]
    public abstract class BulletEvents
    {
        public abstract void OnReset(); // Reset all values in case events repeat
        public abstract bool OnUpdate(Bullet bullet); // Update the event, returns true if event done.

#if UNITY_EDITOR
        public virtual void OnInspectorGUI() { }
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

            if (timer >= Time.deltaTime) return true;
            return false;
        }
    }

    [System.Serializable]
    public class BEStopVelocity : BulletEvents
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

            if (timer >= Time.deltaTime) return true;
            return false;
        }
    }
}
