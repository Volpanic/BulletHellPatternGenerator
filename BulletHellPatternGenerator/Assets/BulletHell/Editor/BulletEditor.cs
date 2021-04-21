using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bullet))]
public class BulletEditor : Editor
{
    private Bullet bullet;

    private void OnEnable()
    {
        bullet = (Bullet)target;
    }

    public override void OnInspectorGUI()
    {
        if (bullet == null) return;

        if(bullet.bulletEvents != null)
        {
            for(int i = 0; i < bullet.bulletEvents.Count; i++)
            {
                EditorGUILayout.BeginVertical("Box");
                {
                    bullet.bulletEvents[i].OnInspectorGUI();
                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}
