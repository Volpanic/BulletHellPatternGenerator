using BulletHellGenerator;
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

    private void OnEnable()
    {
        bullet = (BH_Bullet)target;

        bulletObject = new SerializedObject(target);

        SetupContextMenu();
    }

    public override void OnInspectorGUI()
    {
        if (bullet == null) return;

        bulletObject.Update();

        EditorGUI.BeginChangeCheck();
        {
            //Draw regular bullet data
            EditorGUILayout.LabelField("Lifetime", EditorStyles.centeredGreyMiniLabel);
            bullet.MaxLifeTime = EditorGUILayout.FloatField(new GUIContent("Max Life Time", "In seconds"), bullet.MaxLifeTime);

            //Rotatinal
            EditorGUILayout.LabelField("Rotational and Direction",EditorStyles.centeredGreyMiniLabel);
            bullet.OrbitalVelcoity = Quaternion.Euler(0,0,
                EditorGUILayout.Slider(bullet.OrbitalVelcoity.eulerAngles.z,0,360));
            bullet.RotateRelativeToDirection = EditorGUILayout.Toggle(new GUIContent("Rotate Relative to Direction"),bullet.RotateRelativeToDirection);
            bullet.RotationOffset = Quaternion.Euler(EditorGUILayout.Vector3Field(new GUIContent("Rotation Offset"),bullet.RotationOffset.eulerAngles));
            bullet.RotationalVelocity = EditorGUILayout.Vector3Field(new GUIContent("Rotation Velocity"),bullet.RotationalVelocity);
            EditorGUILayout.PropertyField(bulletObject.FindProperty("RotatinalVelocityModifier"),new GUIContent("Rotational Velocity Modifier"));

            EditorGUILayout.LabelField("Speeds", EditorStyles.centeredGreyMiniLabel);
            bullet.SpeedModifier = EditorGUILayout.FloatField(new GUIContent
                ("Speed Modifier", "Effects Movespeed, used to change inital speed of bullet in patterns."), bullet.SpeedModifier);
            bullet.HomingSpeed = EditorGUILayout.FloatField(new GUIContent
                ("Homing Speed", "Target is set when spawned by BH_BulletHellPatternGenerator."), bullet.HomingSpeed);

            bullet.DisableWhenOffscreen = EditorGUILayout.Toggle(new GUIContent("Disable When Offscreen","If the bullet should be made inactive when offscreen"),bullet.DisableWhenOffscreen);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);  
            }
        }

        bulletObject.ApplyModifiedProperties();
    }

    private void SetupContextMenu()
    {
        addEventWindow = new GenericMenu();

        addEventWindow.AddItem(new GUIContent("Wait For x Seconds"),false, () => AddBulletEvent<BEWaitForSeconds>());
        addEventWindow.AddItem(new GUIContent("Transition to Speed"),false, () => AddBulletEvent<BEToMoveSpeed>());
        addEventWindow.AddItem(new GUIContent("Transition to Homing Speed"), false, () => AddBulletEvent<BEToHomingSpeed>());
        addEventWindow.AddItem(new GUIContent("Transition to Direction"),false, () => AddBulletEvent<BERotateTowardsDirection>());
    }

    private void AddBulletEvent<BE>() where BE : BulletEvent, new()
    {
        if(bullet != null)
        {
            //Undo event logs before the object changes
            Undo.RecordObject(bullet, "Added new bullet event.");

            //Set dirty so editor knows to save it
            EditorUtility.SetDirty(target);
        }
    }
}
