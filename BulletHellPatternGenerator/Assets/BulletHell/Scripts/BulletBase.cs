using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BulletHellGenerator
{
    [System.Serializable]
    public abstract class BulletBase
    {
        //Edits the prefab instance to contain the bullet script
        private void SetupPrefab()
        {

        }

        //Returns a bullet
        public virtual GameObject GetBullet()
        {
            return null;
        }

        #if UNITY_EDITOR
        public virtual void OnGUI()
        {

        }
        #endif
    }

    [System.Serializable]
    public class AlternatingBullets : BulletBase
    {
        public List<GameObject> Bullets;

        private int ToPick = 0;

        public AlternatingBullets()
        {
            //Creates a new lis if bullets == null
            Bullets = Bullets ?? new List<GameObject>();
        }

        public override GameObject GetBullet()
        {
            return Bullets[ToPick++ % Bullets.Count];
        }

        #if UNITY_EDITOR
        public override void OnGUI()
        {
            if (Bullets == null) Bullets = new List<GameObject>();

            EditorGUILayout.BeginVertical("Box");
            {
                for(int i = 0; i < Bullets.Count; i++)
                {
                    Bullets[i] = EditorGUILayout.ObjectField(new GUIContent("Bullet Prefab"), Bullets[i],typeof(GameObject),false) as GameObject;
                }

                EditorGUILayout.EndVertical();
            }

            //GUILayout.FlexibleSpace();
            if(GUILayout.Button("Add Bullet", GUILayout.MaxWidth(128)))
            {
                Bullets.Add(null);
            }
        }
        #endif
    }
}