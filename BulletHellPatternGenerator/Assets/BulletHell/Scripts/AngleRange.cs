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

        public float Evaluate(float normalizedPosition)
        {
            return (MinAngle + (Range * normalizedPosition)) + Mathf.PI;
        }

        public float EvaluateArc(float initalAngle,float normalizedPosition)
        {
            return ((initalAngle - AngleSize) + (Range * normalizedPosition));
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

            EditorGUI.HelpBox(circleRect, "", MessageType.None);

            //Draw the circle in the box
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(circleRect);
                GL.PushMatrix();
                GL.Clear(true, false, Color.black);

                //Draw the actual circle
                float minAngle = property.FindPropertyRelative("AnglePosition").floatValue - property.FindPropertyRelative("AngleSize").floatValue;
                float angleSize = property.FindPropertyRelative("AngleSize").floatValue * 2f;

                float angleSegs = 32;

                float radius = EditorGUIUtility.singleLineHeight * 1.5f;

                if (angleSegs > 0)
                {
                    GL.Begin(GL.QUADS);
                    GL.Color(Color.white);

                    Vector3 p1 = new Vector3(0,0,0);
                    Vector3 p2 = new Vector3(0,0,0);
                    Vector3 p3 = new Vector3(0,0,0);
                    Vector3 p4 = new Vector3(0,0,0);

                    Vector3 pos = circleRect.size / 2f;

                    for (int i = 0; i < angleSegs; i++)
                    {
                        float angle = minAngle + (angleSize * (i / angleSegs));
                        angle -= Mathf.PI;

                        p1.x = Mathf.Cos(angle) * radius;
                        p1.y =- Mathf.Sin(angle) * radius;

                        p2.x = Mathf.Cos(angle) * (radius * 0.75f);
                        p2.y = -Mathf.Sin(angle) * (radius * 0.75f);

                        angle = minAngle + (angleSize * ((i+1f) / angleSegs));
                        angle -= Mathf.PI;

                        p4.x = Mathf.Cos(angle) * radius;
                        p4.y = -Mathf.Sin(angle) * radius;

                        p3.x = Mathf.Cos(angle) * (radius * 0.75f);
                        p3.y = -Mathf.Sin(angle) * (radius * 0.75f);

                        GL.Vertex(pos + p1);
                        GL.Vertex(pos + p2);
                        GL.Vertex(pos + p3);
                        GL.Vertex(pos + p4);

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