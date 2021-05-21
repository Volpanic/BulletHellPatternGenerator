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
    }
    #endregion

    #region On Off Duration Timer

    [System.Serializable]
    public class OnOffDurationTimer : TimingBase
    {
        public float Interval = 0.2f;
        private float timing = 0;
        private float durationcheck = 0;
        private bool wasOff = true;
        public AnimationCurve OnOffPoints = new AnimationCurve();

        public override bool CheckTime(float duration)
        {
            if (durationcheck >= duration) durationcheck -= duration;

            durationcheck += Time.fixedDeltaTime;

            //failsafe
            if (Interval == 0 || OnOffPoints.Evaluate(durationcheck / duration) < 0.5f)
            {
                wasOff = true;
                return false;
            }

            if(wasOff)
            {
                wasOff = false;
                timing = 0;
            }

            timing += Time.fixedDeltaTime;

            if (timing >= Interval)
            {
                if (timing <= 0)
                    return false;

                timing -= Interval;

                return true;
            }
            return false;
        }
    }
    #endregion
}