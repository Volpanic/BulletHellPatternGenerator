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
            if (data.Timing.CheckTime())
            {
                GeneratePattern(generator, data.Bullet, data.PatternDuration);
            }

            durationTimer += Time.deltaTime;
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
                    generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed,BulletArc.MinAngle + (seg * i), bulletChooser.GetBullet());
                }
            }
            else
            {
                //generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed, initAngle + (seg * SeqentialCount++) + (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration) * (range)), bulletChooser.GetBullet());
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
}