﻿using System.Collections;
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
        public void UpdatePattern(BulletHellPatternGenerator generator, BulletHellPattern data)
        {
            // Base Example
            if (data.Timing.CheckTime())
            {
                GeneratePattern(generator, data.Bullet);
            }
        }

        protected virtual void GeneratePattern(BulletHellPatternGenerator generator, BulletBase bulletChooser)
        {
            //Spawn the bullet things here
        }

        #if UNITY_EDITOR
        public virtual void OnGUI()
        {

        }
        #endif
    }

    [System.Serializable]
    public class RingPattern : PatternBase
    {
        public int BulletAmount = 8;
        public float BulletSpeed = 4;
        public bool Sequentially = false;
        public float AngleOffset = 0;

        private int SeqentialCount;

        protected override void GeneratePattern(BulletHellPatternGenerator generator, BulletBase bulletChooser)
        {
            float seg = (Mathf.PI * 2) / BulletAmount;
            if (!Sequentially)
            {
                for (int i = 0; i < BulletAmount; i++)
                {
                    generator.CreateBulletAtDirection(generator.transform.position,BulletSpeed, (seg * i) + AngleOffset, bulletChooser.GetBullet());
                }
            }
            else
            {
                generator.CreateBulletAtDirection(generator.transform.position, BulletSpeed,(seg * SeqentialCount++) + AngleOffset, bulletChooser.GetBullet());
            }
        }

        #if UNITY_EDITOR
        public override void OnGUI()
        {
            BulletAmount = EditorGUILayout.IntField(new GUIContent("Bullet Amount"), BulletAmount);
            BulletSpeed  = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"),Sequentially);
            AngleOffset  = EditorGUILayout.Slider(new GUIContent("Angle Offset"),AngleOffset * Mathf.Rad2Deg,0,360) * Mathf.Deg2Rad;

            BulletAmount = Mathf.Clamp(BulletAmount,1,int.MaxValue);
            AngleOffset = Mathf.Clamp(AngleOffset,0,Mathf.PI*2);
        }
        #endif
    }
}