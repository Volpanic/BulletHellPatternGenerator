using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using System;

namespace BulletHellGenerator
{
    [System.Serializable]
    public class BH_BulletList 
    {
        public GameObject[] Bullets = new GameObject[0];
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BH_BulletList))]
    public class BH_BulletListEditor : PropertyDrawer
    {
        ReorderableList list;
        SerializedProperty bullets;

        private void DrawHeaderBullet(Rect rect)
        {
            EditorGUI.LabelField(rect, "Bullet List");
        }

        private void DrawBulletItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if(bullets.GetArrayElementAtIndex(index) != null)
            EditorGUI.PropertyField(rect, bullets.GetArrayElementAtIndex(index));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bullets = property.FindPropertyRelative("Bullets");
            if (list == null)
            {
                list = new ReorderableList(property.serializedObject, property.FindPropertyRelative("Bullets"), true, true, true, true);

                list.drawElementCallback = DrawBulletItem;
                list.drawHeaderCallback = DrawHeaderBullet;
            }

            list.serializedProperty = property.FindPropertyRelative("Bullets");
            if(list != null && list.serializedProperty != null)
            list.DoLayoutList();
        }

        //private void AddBullet(ReorderableList list)
        //{
        //    throw new NotImplementedException();
        //}
    }
#endif
}