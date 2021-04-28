using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BulletHellGenerator
{

    [System.Serializable]
    public class AngleRange
    {
        public float MinAngle
        {
            get
            {
                return AnglePosition - AngleSize;
            }
        }

        public float MaxAngle
        {
            get
            {
                return AnglePosition + AngleSize;
            }
        }

        public float Range
        {
            get
            {
                return AngleSize * 2f;
            }
        }


        [SerializeField]
        [Range(0,Mathf.PI*2)]
        public float AnglePosition = 0;

        [SerializeField]
        [Range(0, Mathf.PI)]
        public float AngleSize = Mathf.PI;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AngleRange))]
    public class AngleRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //EditorGUI.LabelField(position, label);

            //oxymoron name, i know
            Rect circleRect = new Rect(position.position, new Vector2(EditorGUIUtility.singleLineHeight * 6, EditorGUIUtility.singleLineHeight * 4));
            Vector2 circRectCenter = circleRect.center - circleRect.position;

            EditorGUI.HelpBox(circleRect, "", MessageType.None);

            //Draw the circle in the box
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(circleRect);
                GL.PushMatrix();
                GL.Clear(true, false, Color.black);

                //Draw the actual circle
                int percision = 32; // Circle percision
                float range = property.FindPropertyRelative("AngleSize").floatValue * 2f;

                float sectionSize = (range) / percision;

                float segments = ((range / (Mathf.PI * 2f)) * percision);

                if (segments > 1)
                {
                    GL.Begin(GL.TRIANGLE_STRIP);
                    GL.Color(Color.white);

                    Vector3 circPoint = new Vector3(float.NaN, float.NaN,0);

                    //Actually draw the circle
                    for (int i = 0; i <= segments; i++)
                    {
                        var len = (i * sectionSize) + property.FindPropertyRelative("AnglePosition").floatValue;
                        circPoint.x = Mathf.Cos(len) * EditorGUIUtility.singleLineHeight * 1.5f;
                        circPoint.y = Mathf.Sin(len) * EditorGUIUtility.singleLineHeight * 1.5f;
                        GL.Vertex((Vector3)circRectCenter + circPoint);
                        GL.Vertex((Vector3)circRectCenter + circPoint * 0.5f);

                        len = ((i+1) * sectionSize) + property.FindPropertyRelative("AnglePosition").floatValue;
                        circPoint.x = Mathf.Cos(len) * EditorGUIUtility.singleLineHeight * 1.5f;
                        circPoint.y = Mathf.Sin(len) * EditorGUIUtility.singleLineHeight * 1.5f;
                        GL.Vertex((Vector3)circRectCenter + circPoint);
                        GL.Vertex((Vector3)circRectCenter + circPoint*0.5f);
                    }
                    GL.End();
                   
                }
                GL.PopMatrix();
                GUI.EndClip();
            }

            EditorGUI.PropertyField(new Rect(circleRect.xMax + EditorGUIUtility.singleLineHeight, circleRect.center.y - EditorGUIUtility.singleLineHeight
                , position.width - circleRect.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),property.FindPropertyRelative("AnglePosition"));
            EditorGUI.PropertyField(new Rect(circleRect.xMax + EditorGUIUtility.singleLineHeight, circleRect.center.y, position.width - circleRect.width - EditorGUIUtility.singleLineHeight
                , EditorGUIUtility.singleLineHeight),property.FindPropertyRelative("AngleSize"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4;
        }
    }
#endif

}