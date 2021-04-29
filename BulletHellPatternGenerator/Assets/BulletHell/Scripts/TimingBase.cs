using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace BulletHellGenerator
{
    [System.Serializable]
    public abstract class TimingBase
    {
        public virtual bool CheckTime()
        {
            return true;
        }

        #if UNITY_EDITOR
        public virtual void OnGUI(SerializedObject pattern)
        {

        }
        #endif
    }

    [System.Serializable]
    public class EveryXSecondTiming : TimingBase
    {
        public float Interval = 0.2f;
        private float timing = 0;

        public override bool CheckTime()
        {
            //failsafe
            if (Interval == 0)
                return false;

            timing += Time.fixedDeltaTime;
            if (timing >= Interval)
            {
                if(timing <= 0)
                    return false;

                timing -= Interval;

                return true;
            }
            return false;
        }

        #if UNITY_EDITOR
        public override void OnGUI(SerializedObject pattern)
        {
            Interval = EditorGUILayout.FloatField(new GUIContent("Interval"), Interval);
            Interval = Mathf.Clamp(Interval,0, float.MaxValue);
        }
        #endif
    }
}