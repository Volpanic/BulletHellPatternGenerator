using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bullet))]
public class BulletEditor : Editor
{
    private Bullet bullet;
    private GenericMenu addEventWindow;

    private void OnEnable()
    {
        bullet = (Bullet)target;
        SetupContextMenu();
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
                    bullet.bulletEvents[i].OnInspectorGUI(bullet);
                    EditorGUILayout.EndVertical();
                }
            }
        }
    }

    private void SetupContextMenu()
    {
        addEventWindow = new GenericMenu();
        addEventWindow.AddItem(new GUIContent("Wait For x Seconds"),false, () => AddBulletEvent<BEWaitForSeconds>());
    }

    private void AddBulletEvent<BE>() where BE : BulletEvents, new()
    {
        if(bullet != null)
        {
            //Crate the list
            if(bullet.bulletEvents == null)
            {
                bullet.bulletEvents = new List<BulletEvents>();
            }
            bullet.bulletEvents.Add(new BE());
        }
    }
}
