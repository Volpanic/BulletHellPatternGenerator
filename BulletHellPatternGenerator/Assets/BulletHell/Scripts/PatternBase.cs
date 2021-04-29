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

        public void UpdatePattern(BulletHellPatternGenerator generator, BulletHellPattern data)
        {
            // Base Example
            if(data.Timing.CheckTime())
            {
                GeneratePattern(generator, data.Bullet, data.PatternDuration);
            }

            durationTimer += Time.fixedDeltaTime;
            if (durationTimer >= data.PatternDuration)
            {
                durationTimer -= data.PatternDuration;
            }
        }

        protected virtual void GeneratePattern(BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            //Spawn the bullet things here
        }

#if UNITY_EDITOR
        public virtual void OnGUI(SerializedObject pattern)
        {

        }
#endif
    }

    #region Ring Pattern
    [System.Serializable]
    public class RingPattern : PatternBase
    {
        public MinMaxCurve BulletAmount = new MinMaxCurve(1, 64);
        public float BulletSpeed = 4;
        public bool Sequentially = false;
        public MinMaxCurve AngleOffset = new MinMaxCurve(0f, 1f);
        public AngleRange BulletArc;

        private int SeqentialCount;

        protected override void GeneratePattern(BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletAmount, durationTimer / duration)));

            float seg = BulletArc.Range / bulletAmount;
            if (!Sequentially)
            {
                for (int i = 0; i < bulletAmount; i++)
                {
                    generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed, BulletArc.Evaluate((float)i / (float)bulletAmount) + (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)), bulletChooser.GetBullet());
                }
            }
            else
            {
                generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed, BulletArc.Evaluate((float)SeqentialCount++ / (float)bulletAmount) + (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)), bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedObject pattern)
        {
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("BulletAmount"), new GUIContent("Bullet Amount"));
            BulletSpeed = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"), Sequentially);
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("AngleOffset"), new GUIContent("Angle Offset"));
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("BulletArc"), new GUIContent("Angle Arc"));
        }
#endif
    }
    #endregion


    #region Ring Pattern
    [System.Serializable]
    public class OctogonPattern : PatternBase
    {
        public MinMaxCurve BulletAmount = new MinMaxCurve(1, 64);
        public float BulletSpeed = 4;
        public bool Sequentially = false;
        public MinMaxCurve AngleOffset = new MinMaxCurve(0f, 1f);
        public AngleRange BulletArc;

        private int SeqentialCount;

        protected override void GeneratePattern(BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletAmount, durationTimer / duration)));

            float seg = BulletArc.Range / bulletAmount;
            if (!Sequentially)
            {
                for (int i = 0; i < bulletAmount; i++)
                {
                    generator.CreateBulletAtDirectionOct(generator.transform.position, BulletSpeed, BulletArc.Evaluate((float)i / (float)bulletAmount) + (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)), bulletChooser.GetBullet());
                }
            }
            else
            {
                generator.CreateBulletAtDirectionOct(generator.transform.position, BulletSpeed, BulletArc.Evaluate((float)SeqentialCount++ / (float)bulletAmount) + (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)), bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedObject pattern)
        {
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("BulletAmount"), new GUIContent("Bullet Amount"));
            BulletSpeed = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"), Sequentially);
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("AngleOffset"), new GUIContent("Angle Offset"));
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("BulletArc"), new GUIContent("Angle Arc"));
        }
#endif
    }
    #endregion 

    #region Square Pattern
    [System.Serializable]
    public class SquarePattern : PatternBase
    {
        public MinMaxCurve BulletAmount = new MinMaxCurve(1, 64);
        public float BulletSpeed = 4;
        public bool Sequentially = false;
        public MinMaxCurve AngleOffset = new MinMaxCurve(0f, 1f);

        private int SeqentialCount;

       // private float[] SquareSides = new []{ 0, Mathf.PI * 0.5f, Mathf.PI, Mathf.PI * 1.5f };

        protected override void GeneratePattern(BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletAmount, durationTimer / duration)));
            if (!Sequentially)
            {
                float seg = (Mathf.PI * 2f) / bulletAmount;
                for (int i = 0; i < bulletAmount; i++)
                {
                    generator.CreateBulletAtDirectionSquare(generator.transform.position, BulletSpeed, (seg*i) + (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)), bulletChooser.GetBullet());
                }
            }
            else
            {
                generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed,(((Mathf.PI * 2f) / bulletAmount) * SeqentialCount++) + (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)), bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }

#if UNITY_EDITOR
        public override void OnGUI(SerializedObject pattern)
        {
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("BulletAmount"), new GUIContent("Bullet Amount"));
            BulletSpeed = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"), Sequentially);
            EditorGUILayout.PropertyField(pattern.FindProperty("Pattern").FindPropertyRelative("AngleOffset"), new GUIContent("Angle Offset"));
        }
#endif
    }
    #endregion 
}