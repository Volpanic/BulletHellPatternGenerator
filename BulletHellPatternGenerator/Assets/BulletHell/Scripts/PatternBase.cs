using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace BulletHellGenerator
{
    enum PatternResetMode
    {
        Loop = 0,
        PingPong,

    }

    [System.Serializable]
    public abstract class PatternBase
    {
        protected float durationTimer = 0;

        public void UpdatePattern(BH_BulletHellPatternGenerator generator, BulletHellPattern data, BulletHellPattern.PatternLayer layerData)
        {
            // Base Example
            if (layerData.Timing != null && layerData.Timing.CheckTime(data.PatternDuration))
            {
                if (layerData.Bullet != null)
                {
                    GeneratePattern(generator, layerData.Bullet, data.PatternDuration);
                }
            }

            durationTimer += Time.fixedDeltaTime;
            if (durationTimer >= data.PatternDuration)
            {
                durationTimer -= data.PatternDuration;
            }
        }

        protected virtual void GeneratePattern(BH_BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            //Spawn the bullet things here
        }

#if UNITY_EDITOR
        public virtual void OnGUI(SerializedProperty pattern)
        {

        }
#endif
    }

    #region Ring Pattern
    [System.Serializable]
    public class RingPattern : PatternBase
    {
        public MinMaxCurve BulletAmount = new MinMaxCurve(8);
        public float BulletSpeed = 4;
        public bool Sequentially = false;
        public MinMaxCurve AngleOffset = new MinMaxCurve(0);
        public AngleRange BulletArc;
        public bool TargetPlayer = false;

        private int SeqentialCount;

        protected override void GeneratePattern(BH_BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletAmount, durationTimer / duration)));

            float AddAngle = 0;

            float seg = BulletArc.Range / bulletAmount;
            if (!Sequentially)
            {
                for (int i = 0; i < bulletAmount; i++)
                {
                    if (!TargetPlayer)
                    {
                        AddAngle = BulletArc.Evaluate(i / (float)bulletAmount); // With credence to the bullet arc 
                    }
                    else
                    {
                        AddAngle = BulletArc.EvaluateArc(generator.TargetAngle, (i + 0.5f) / (float)bulletAmount); // With credence to generator target
                    }
                    AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration));// With credence to the Angle Offset

                    generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed, AddAngle, bulletChooser.GetBullet());
                }
            }
            else
            {
                if (!TargetPlayer)
                {
                    AddAngle = BulletArc.Evaluate(SeqentialCount++ / (float)bulletAmount); // With credence to the bullet arc 
                }
                else
                {
                    AddAngle = BulletArc.EvaluateArc(generator.TargetAngle, (SeqentialCount++ + 0.5f) / (float)bulletAmount); // With credence to generator target
                }
                AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration));// With credence to the Angle Offset

                generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed, AddAngle, bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedProperty pattern)
        {
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("BulletAmount"), new GUIContent("Bullet Amount"));
            BulletSpeed = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"), Sequentially);
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("AngleOffset"), new GUIContent("Angle Offset"));
            TargetPlayer = EditorGUILayout.Toggle(new GUIContent("Aim at Target"), TargetPlayer);
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("BulletArc"), new GUIContent("Angle Arc"));
        }
#endif
    }
    #endregion

    #region Oct Pattern
    [System.Serializable]
    public class OctogonPattern : PatternBase
    {
        public MinMaxCurve BulletAmount = new MinMaxCurve(8);
        public float BulletSpeed = 4;
        public bool Sequentially = false;
        public MinMaxCurve AngleOffset = new MinMaxCurve(0);
        public AngleRange BulletArc;
        public bool TargetPlayer = false;

        private int SeqentialCount;

        protected override void GeneratePattern(BH_BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletAmount, durationTimer / duration)));

            float AddAngle = 0;

            float seg = BulletArc.Range / bulletAmount;
            if (!Sequentially)
            {
                for (int i = 0; i < bulletAmount; i++)
                {
                    if (!TargetPlayer)
                    {
                        AddAngle = BulletArc.Evaluate(i / (float)bulletAmount); // With credence to the bullet arc 
                    }
                    else
                    {
                        AddAngle = BulletArc.EvaluateArc(generator.TargetAngle, (i + 0.5f) / (float)bulletAmount); // With credence to generator target
                    }
                    AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration));// With credence to the Angle Offset

                    generator.CreateBulletAtDirectionOct(generator.transform.position, BulletSpeed, AddAngle, bulletChooser.GetBullet());
                }
            }
            else
            {
                if (!TargetPlayer)
                {
                    AddAngle = BulletArc.Evaluate(SeqentialCount++ / (float)bulletAmount); // With credence to the bullet arc 
                }
                else
                {
                    AddAngle = BulletArc.EvaluateArc(generator.TargetAngle, (SeqentialCount++ + 0.5f) / (float)bulletAmount); // With credence to generator target
                }
                AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration));// With credence to the Angle Offset


                generator.CreateBulletAtDirectionOct(generator.transform.position, BulletSpeed, AddAngle, bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedProperty pattern)
        {
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("BulletAmount"), new GUIContent("Bullet Amount"));
            BulletSpeed = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"), Sequentially);
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("AngleOffset"), new GUIContent("Angle Offset"));
            TargetPlayer = EditorGUILayout.Toggle(new GUIContent("Aim at Target"), TargetPlayer);
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("BulletArc"), new GUIContent("Angle Arc"));
        }
#endif
    }
    #endregion 

    #region Square Pattern
    [System.Serializable]
    public class SquarePattern : PatternBase
    {
        public MinMaxCurve BulletAmount = new MinMaxCurve(8);
        public float BulletSpeed = 4;
        public bool Sequentially = false;
        public MinMaxCurve AngleOffset = new MinMaxCurve(0);
        public bool TargetPlayer = false;

        private int SeqentialCount;

        // private float[] SquareSides = new []{ 0, Mathf.PI * 0.5f, Mathf.PI, Mathf.PI * 1.5f };

        protected override void GeneratePattern(BH_BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletAmount, durationTimer / duration)));

            float AddAngle = 0;
            if (!Sequentially)
            {
                float seg = (Mathf.PI * 2f) / bulletAmount;
                for (int i = 0; i < bulletAmount; i++)
                {

                    if (!TargetPlayer)
                    {
                        AddAngle = (seg * i);
                    }
                    else
                    {
                        AddAngle = (seg * i) + generator.TargetAngle; // With credence to generator target
                    }
                    AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration));// With credence to the Angle Offset


                    generator.CreateBulletAtDirectionSquare(generator.transform.position, BulletSpeed, AddAngle, bulletChooser.GetBullet());
                }
            }
            else
            {
                float seg = (Mathf.PI * 2f) / bulletAmount;
                if (!TargetPlayer)
                {
                    AddAngle = (seg * SeqentialCount++);
                }
                else
                {
                    AddAngle = (seg * SeqentialCount++) + generator.TargetAngle; // With credence to generator target
                }
                AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration));// With credence to the Angle Offset
                generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed, AddAngle, bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedProperty pattern)
        {
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("BulletAmount"), new GUIContent("Bullet Amount"));
            BulletSpeed = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"), Sequentially);
            TargetPlayer = EditorGUILayout.Toggle(new GUIContent("Aim at Target"), TargetPlayer);
            EditorGUILayout.PropertyField(pattern.FindPropertyRelative("Pattern").FindPropertyRelative("AngleOffset"), new GUIContent("Angle Offset"));
        }
#endif
    }
    #endregion 
}