using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[CustomEditor(typeof(BH_Bullet))]
public class BulletEditor : Editor
{
    private BH_Bullet bullet;
    private GenericMenu addEventWindow;
    private Vector2 scrollPosition;

    private SerializedObject bulletObject;

    private SerializedProperty AHHHH;

    private void OnEnable()
    {
        bullet = (BH_Bullet)target;

        bulletObject = new SerializedObject(target);
        AHHHH = bulletObject.FindProperty("AHHHHH");

        SetupContextMenu();
    }

    public override void OnInspectorGUI()
    {
        if (bullet == null) return;

        EditorGUI.BeginChangeCheck();
        {
            //Draw regular bullet data
            bullet.MaxLifeTime = EditorGUILayout.FloatField(new GUIContent("Max Life Time", "In seconds"), bullet.MaxLifeTime);
            bullet.SpeedModifier = EditorGUILayout.FloatField(new GUIContent
                ("Speed Modifier", "Effects Movespeed, used to change inital speed of bullet in patterns."), bullet.SpeedModifier);
            bullet.OrbitalVelcoity = Quaternion.Euler(0,0,
                EditorGUILayout.Slider(bullet.OrbitalVelcoity.eulerAngles.z,0,360));
            bullet.RotateRelativeToDirection = EditorGUILayout.Toggle(new GUIContent("Rotate Relative to Direction"),bullet.RotateRelativeToDirection);
            bullet.RotationOffset = Quaternion.Euler(EditorGUILayout.Vector3Field(new GUIContent(""),bullet.RotationOffset.eulerAngles));

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);  
            }
        }

        //Draw Bullet events
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,GUILayout.Height(256),GUILayout.ExpandHeight(true));
        {
            if (bullet.bulletEvents != null)
            {
                for (int i = 0; i < bullet.bulletEvents.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");
                    {
                        // Display data for each node
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(bullet.bulletEvents[i].GetType().Name);

                            //Remove Button
                            if(GUILayout.Button("✘"))
                            {
                                Undo.RecordObject(target,"Bullet Event Removed");
                                bullet.bulletEvents.RemoveAt(i);
                                i--;
                                EditorUtility.SetDirty(target);
                                continue;
                            }

                            //MoveUp Button
                            if (GUILayout.Button("▲") && i != 0)
                            {
                                Undo.RecordObject(target, "Bullet Event Moved Up");
                                bullet.bulletEvents.Reverse(i-1, 2);
                                i--;
                                EditorUtility.SetDirty(target);
                                continue;
                            }

                            //MoveDown Button
                            if (GUILayout.Button("▼") && i != bullet.bulletEvents.Count-1)
                            {
                                Undo.RecordObject(target, "Bullet Event Moved Down");
                                bullet.bulletEvents.Reverse(i,2);
                                i--;
                                EditorUtility.SetDirty(target);
                                continue;
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUI.BeginChangeCheck();

                        bullet.bulletEvents[i].OnInspectorGUI(bullet);

                        //If bullet event edited
                        if(EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(target);
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        //Add Button
        if(GUILayout.Button("Add Bullet Event"))
        {
            addEventWindow.ShowAsContext();
        }

        if (GUILayout.Button("Clear ALL Bullet Events"))
        {
            if (bullet.bulletEvents != null)
            {
                Undo.RecordObject(bullet, "Cleared bullet events.");
                bullet.bulletEvents.Clear();
                EditorUtility.SetDirty(target);
            }
        }
    }

    private void SetupContextMenu()
    {
        addEventWindow = new GenericMenu();

        addEventWindow.AddItem(new GUIContent("Wait For x Seconds"),false, () => AddBulletEvent<BEWaitForSeconds>());
        addEventWindow.AddItem(new GUIContent("Transition to Speed"),false, () => AddBulletEvent<BEToMoveSpeed>());
        addEventWindow.AddItem(new GUIContent("Transition to Direction"),false, () => AddBulletEvent<BERotateTowardsDirection>());
    }

    private void AddBulletEvent<BE>() where BE : BulletEvents, new()
    {
        if(bullet != null)
        {
            //Undo event logs before the object changes
            Undo.RecordObject(bullet, "Added new bullet event.");

            //Crate the list
            if (bullet.bulletEvents == null)
            {
                bullet.bulletEvents = new List<BulletEvents>();
            }
            bullet.bulletEvents.Add(new BE());

            //Set dirty so editor knows to save it
            EditorUtility.SetDirty(target);
        }
    }
}
