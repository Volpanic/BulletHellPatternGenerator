using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BulletHellGenerator
{
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

        protected override void GeneratePattern(BulletHellPatternGenerator generator, BulletBase bulletChooser)
        {
            if (!Sequentially)
            {
                float seg = (Mathf.PI * 2) / BulletAmount;

                for (int i = 0; i < BulletAmount; i++)
                {
                    CreateBulletAtDirection(generator.transform.position,BulletSpeed, seg * i, bulletChooser.GetBullet());
                }
            }
        }

        private BH_Bullet CreateBulletAtDirection(Vector3 position,float Speed, float Angle, GameObject BulletPrefab)
        {
            Vector3 dir = new Vector3(Mathf.Cos(Angle), Mathf.Sin(Angle), 0);

            BH_Bullet pulse = GameObject.Instantiate(BulletPrefab, position, Quaternion.identity).GetComponent<BH_Bullet>();
            pulse.Direction = dir;
            pulse.MoveSpeed = Speed;

            return pulse;
        }

        #if UNITY_EDITOR
        public override void OnGUI()
        {
            BulletAmount = EditorGUILayout.IntField(new GUIContent("Bullet Amount"), BulletAmount);
            BulletSpeed  = EditorGUILayout.FloatField(new GUIContent("Bullet Speed"), BulletSpeed);
            Sequentially = EditorGUILayout.Toggle(new GUIContent("Sequentially"),Sequentially);

            BulletAmount = Mathf.Clamp(BulletAmount,1,int.MaxValue);
        }
        #endif
    }
}