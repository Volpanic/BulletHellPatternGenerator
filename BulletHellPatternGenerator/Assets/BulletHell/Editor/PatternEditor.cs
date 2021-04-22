using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

namespace BulletHellGenerator
{
    public class PatternEditor : EditorWindow
    {
        // Rect is dynamically sized, but to get that size we need a 
        // rect that the dynamic one falls in, So we just give it a huge one
        Rect BoardRect = new Rect(0, 0, 1, 1);
        Vector2 boardPosition;

        BulletHellPattern Data;

        GenericMenu BulletChooseMenu;
        GenericMenu TimingChooseMenu;
        GenericMenu PatternChooseMenu;

        [MenuItem("Pattern Editor/Editor")]
        public static PatternEditor OpenWindow()
        {
            return GetWindow<PatternEditor>("Pattern Editor");
        }

        private void OnEnable()
        {
            SetupContextMenus();
        }

        private void OnGUI()
        {

            GUI.Box(BoardRect, "Box Tho ╚(•⌂•)╝");

            GUILayout.BeginArea(new Rect(BoardRect.x + 16, BoardRect.y + 32, float.MaxValue, float.MaxValue));
            {
                //Begin vertical area, used to measure the rect of the node
                GUILayout.BeginVertical(GUILayout.MaxWidth(512), GUILayout.ExpandWidth(false));
                {

                    DrawBoard();

                    GUILayout.EndVertical();
                }

                //Get size of the above vertical only works during repaint
                if (Event.current.type == EventType.Repaint)
                {
                    Rect dynamicRect = GUILayoutUtility.GetLastRect();
                    dynamicRect.width += 32;
                    dynamicRect.height += 32;

                    //If rect has changed in size force repaint with new rect
                    if (BoardRect != dynamicRect)
                    {
                        BoardRect = dynamicRect;
                        Repaint();
                    }
                }

                GUILayout.EndArea();
            }
            DrawMenuBar();
        }

        private void DrawBoard()
        {
            GUILayout.TextField("Bullets");

            if (GUILayout.Button("Yo"))
            {
                BulletChooseMenu.ShowAsContext();
            }

            DrawSeparator();

            GUILayout.TextField("Timing");
            DrawSeparator();

            GUILayout.TextField("Pattern");
            DrawSeparator();
        }

        private void SetupContextMenus()
        {
            BulletChooseMenu = new GenericMenu();
            TimingChooseMenu = new GenericMenu();
            PatternChooseMenu = new GenericMenu();

            //Setup bullet choose menu, by getting all compiled 
            System.Type[] bulletChooseTypes = Assembly.GetAssembly(typeof(BulletBase)).GetTypes();
            for (int i = 0; i < bulletChooseTypes.Length; i++)
            {
                if(bulletChooseTypes[i].IsSubclassOf(typeof(BulletBase)))
                    BulletChooseMenu.AddItem(new GUIContent(bulletChooseTypes[i].Name), false, () => BulletChooseOnClick(bulletChooseTypes[i]));
            }
        }
        private void BulletChooseOnClick(Type type)
        {
            if (Data == null) return;

            Undo.RecordObject(Data,"Changed Bullet Choose Mode");
            //This code is kind of yucky, by makes modularity really easy
            Data.Pattern.Bullet = (BulletBase)Activator.CreateInstance(type);
            EditorUtility.SetDirty(Data);
        }

        private void DrawMenuBar()
        {
            Rect menuBar = new Rect(0, 0, position.width, 24);

            GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
            {
                //Draw menu bar content
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    EditorGUI.BeginChangeCheck();
                    Data = (BulletHellPattern)EditorGUILayout.ObjectField(Data, typeof(BulletHellPattern), false);

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndArea();
            }
        }

        private void DrawSeparator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}