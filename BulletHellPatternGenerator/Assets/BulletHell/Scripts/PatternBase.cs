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
        protected float durationTimer = 0;
        public const float MaxRadians = Mathf.PI * 2f;

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

    }

    #region Ring Pattern
    [System.Serializable]
    public class RingPattern : PatternBase
    {
        //Stack
        [Header("Bullet Info")]
        public Stack Stack = new Stack();

        [Header("Spawning Info")]
        public MinMaxCurve BulletDensity = new MinMaxCurve(8);
        public bool Sequentially = false;

        [Header("Angle")]
        public MinMaxCurve AngleOffset = new MinMaxCurve(0);
        public AngleRange BulletArc = new AngleRange();
        public bool TargetPlayer = false;

        private int SeqentialCount;

        protected override void GeneratePattern(BH_BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletDensity, durationTimer / duration)));

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
                    AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)) * BulletArc.AngleSize;// With credence to the Angle Offset

                    if (!Stack.UseStack) generator.CreateBulletAtDirection(generator.transform.position, Stack.MinSpeed, AddAngle, bulletChooser.GetBullet());
                    else generator.CreateBulletAtDirectionStack(generator.transform.position, Stack.MinSpeed, Stack.MaxSpeed, Stack.StackAmount, AddAngle, bulletChooser.GetBullet());
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
                AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)) * BulletArc.AngleSize;// With credence to the Angle Offset

                if (!Stack.UseStack) generator.CreateBulletAtDirection(generator.transform.position, Stack.MinSpeed, AddAngle, bulletChooser.GetBullet());
                else generator.CreateBulletAtDirectionStack(generator.transform.position, Stack.MinSpeed, Stack.MaxSpeed, Stack.StackAmount, AddAngle, bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }
    }
    #endregion

    #region Oct Pattern
    [System.Serializable]
    public class OctogonPattern : PatternBase
    {
        [Header("Bullet Info")]
        public Stack Stack = new Stack();

        [Header("Spawning Info")]
        public MinMaxCurve BulletDensity = new MinMaxCurve(8);
        public bool Sequentially = false;

        [Header("Angle")]
        public MinMaxCurve AngleOffset = new MinMaxCurve(0);
        public AngleRange BulletArc = new AngleRange();
        public bool TargetPlayer = false;

        private int SeqentialCount;

        protected override void GeneratePattern(BH_BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletDensity, durationTimer / duration)));

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
                    AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)) * MaxRadians;// With credence to the Angle Offset

                    if(!Stack.UseStack) generator.CreateBulletAtDirectionOct(generator.transform.position, Stack.MinSpeed, AddAngle, bulletChooser.GetBullet());
                    else generator.CreateBulletAtDirectionOctStack(generator.transform.position, Stack.MinSpeed, Stack.MaxSpeed, Stack.StackAmount, AddAngle, bulletChooser.GetBullet());
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
                AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)) * MaxRadians;// With credence to the Angle Offset


                if(!Stack.UseStack) generator.CreateBulletAtDirectionOct(generator.transform.position, Stack.MinSpeed, AddAngle, bulletChooser.GetBullet());
                else generator.CreateBulletAtDirectionOctStack(generator.transform.position, Stack.MinSpeed, Stack.MaxSpeed, Stack.StackAmount, AddAngle, bulletChooser.GetBullet());
                SeqentialCount %= bulletAmount;
            }
        }

    }
    #endregion 

    #region Square Pattern
    [System.Serializable]
    public class SquarePattern : PatternBase
    {
        [Header("Bullet Info")]
        public Stack Stack = new Stack();

        [Header("Spawning Info")]
        public MinMaxCurve BulletDensity = new MinMaxCurve(8);
        public bool Sequentially = false;

        [Header("Angle")]
        public MinMaxCurve AngleOffset = new MinMaxCurve(0);
        public bool TargetPlayer = false;

        private int SeqentialCount;

        protected override void GeneratePattern(BH_BulletHellPatternGenerator generator, BulletBase bulletChooser, float duration)
        {
            int bulletAmount = Mathf.Max(1, Mathf.FloorToInt(Wrappers.Extension.MinMaxEvaluate(BulletDensity, durationTimer / duration)));

            float AddAngle = 0;
            if (!Sequentially)
            {
                float seg = (MaxRadians) / bulletAmount;
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
                    AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)) * MaxRadians;// With credence to the Angle Offset

                    if (!Stack.UseStack) generator.CreateBulletAtDirectionSquare(generator.transform.position, Stack.MinSpeed, AddAngle, bulletChooser.GetBullet());
                    else generator.CreateBulletAtDirectionSquareStack(generator.transform.position, Stack.MinSpeed, Stack.MaxSpeed, Stack.StackAmount, AddAngle, bulletChooser.GetBullet());
                }
            }
            else
            {
                float seg = (MaxRadians) / bulletAmount;
                if (!TargetPlayer)
                {
                    AddAngle = (seg * SeqentialCount++);
                }
                else
                {
                    AddAngle = (seg * SeqentialCount++) + generator.TargetAngle; // With credence to generator target
                }
                AddAngle += (Wrappers.Extension.MinMaxEvaluate(AngleOffset, durationTimer / duration)) * MaxRadians;// With credence to the Angle Offset

                if(!Stack.UseStack) generator.CreateBulletAtDirectionSquare(generator.transform.position, Stack.MinSpeed, AddAngle, bulletChooser.GetBullet());
                else generator.CreateBulletAtDirectionSquareStack(generator.transform.position, Stack.MinSpeed, Stack.MaxSpeed, Stack.StackAmount, AddAngle, bulletChooser.GetBullet());

                SeqentialCount %= bulletAmount;
            }
        }

    }
    #endregion 
}