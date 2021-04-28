using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace BulletHellGenerator.Wrappers
{
    public class Extension
    {
        public static float MinMaxEvaluate(MinMaxCurve curve,float Time)
        {
            if (curve.mode == ParticleSystemCurveMode.TwoConstants) return Random.Range(curve.constantMin, curve.constantMax);
            if (curve.mode == ParticleSystemCurveMode.Curve) return curve.curve.Evaluate(Time);
            if (curve.mode == ParticleSystemCurveMode.TwoCurves) return Random.Range(curve.curveMin.Evaluate(Time), curve.curveMax.Evaluate(Time));

            return curve.constant;
        }
    }
}