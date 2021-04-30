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
        public virtual bool CheckTime(float duration)
        {
            return true;
        }

        #if UNITY_EDITOR
        public virtual void OnGUI(SerializedProperty pattern)
        {

        }
        #endif
    }

    #region Every X Seconds

    [System.Serializable]
    public class EveryXSecondTiming : TimingBase
    {
        public float Interval = 0.2f;
        private float timing = 0;

        public override bool CheckTime(float duration)
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
        public override void OnGUI(SerializedProperty pattern)
        {
            Interval = EditorGUILayout.FloatField(new GUIContent("Interval"), Interval);
            Interval = Mathf.Clamp(Interval,0, float.MaxValue);
        }
        #endif
    }
    #endregion

    #region Start of pattern

    [System.Serializable]
    public class StartOfPatternTiming : TimingBase
    {
        private bool hasRan = false;
        private float oldTime = 0;
        private float timer = -1;

        public override bool CheckTime(float duration)
        {
            timer += Time.fixedDeltaTime;
            if(duration != 0) timer %= duration; // Makes sure not to divide by zero

            //Meaning we have restarted
            if(oldTime > timer)
            {
                hasRan = false;
            }

            oldTime = timer;

            if (!hasRan)
            {
                hasRan = true;
                return true;
            }
            return false;
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedProperty pattern)
        {
            EditorGUILayout.LabelField("No settings to adjust",EditorStyles.centeredGreyMiniLabel);
        }
#endif
    }
    #endregion


    #region On Off Duration Timer

    [System.Serializable]
    public class OnOffDurationTimer : TimingBase
    {
        public float Interval = 0.2f;
        private float timing = 0;
        public AnimationCurve OnOffPoints = new AnimationCurve();

        public override bool CheckTime(float duration)
        {
            timing += Time.fixedDeltaTime;
            timing %= duration;
            //failsafe
            if (Interval == 0 || OnOffPoints.Evaluate(timing /duration) < 0.5f)
                return false;


            if (timing >= Interval)
            {
                if (timing <= 0)
                    return false;

                timing -= Interval;

                return true;
            }
            return false;
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedProperty pattern)
        {
            Interval = EditorGUILayout.FloatField(new GUIContent("Interval"), Interval);
            OnOffPoints = EditorGUILayout.CurveField(new GUIContent("On Off Points"), OnOffPoints);
            Interval = Mathf.Clamp(Interval, 0, float.MaxValue);
        }
#endif
    }
    #endregion
}