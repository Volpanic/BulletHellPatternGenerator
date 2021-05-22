using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace BulletHellGenerator
{
    [System.Serializable]
    public class Stack
    {
        [Min(1)]
        public int StackAmount = 2;

        public bool UseStack = false;

        public int MinSpeed = 4;
        public int MaxSpeed = 8;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Stack))]
    public class StackDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("UseStack"), new GUIContent("Use Stack"));

            //Open
            if (property.FindPropertyRelative("UseStack").boolValue)
            {
                EditorGUILayout.LabelField("Stack Data");
                EditorGUILayout.PropertyField(property.FindPropertyRelative("StackAmount"), new GUIContent("Stack Amount"));

                EditorGUILayout.PropertyField(property.FindPropertyRelative("MinSpeed"), new GUIContent("Min Bullet Speed"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("MaxSpeed"), new GUIContent("Max Bullet Speed"));
            }
            else // Closed
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("MinSpeed"), new GUIContent("Bullet Speed"));

            }
        }
    }
#endif
}