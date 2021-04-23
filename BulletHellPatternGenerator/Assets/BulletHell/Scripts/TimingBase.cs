using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        public virtual void OnGUI()
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
            timing += Time.deltaTime;
            if (timing >= Interval)
            {
                timing -= Interval;
                return true;
            }
            return false;
        }

        #if UNITY_EDITOR
        public override void OnGUI()
        {
            Interval = EditorGUILayout.FloatField(new GUIContent("Interval"), Interval);
            Interval = Mathf.Clamp(Interval,0, float.MaxValue);
        }
        #endif
    }
}