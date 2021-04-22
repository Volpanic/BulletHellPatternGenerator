using BulletHellGenerator.BulletEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BH_Bullet))]
public class BulletEditor : Editor
{
    private BH_Bullet bullet;
    private GenericMenu addEventWindow;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        bullet = (BH_Bullet)target;
        SetupContextMenu();
    }

    public override void OnInspectorGUI()
    {
        if (bullet == null) return;

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
                                bullet.bulletEvents.RemoveAt(i);
                                i--;
                                continue;
                            }

                            //MoveUp Button
                            if (GUILayout.Button("▲") && i != 0)
                            {
                                bullet.bulletEvents.Reverse(i-1, 2);
                                i--;
                                continue;
                            }

                            //MoveDown Button
                            if (GUILayout.Button("▼") && i != bullet.bulletEvents.Count-1)
                            {
                                bullet.bulletEvents.Reverse(i,2);
                                i--;
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
