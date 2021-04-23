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
            PatternEditor window = GetWindow<PatternEditor>("Pattern Editor");
            return window;
        }

        public static PatternEditor OpenWindowWithAsset(BulletHellPattern asset)
        {
            PatternEditor window =  GetWindow<PatternEditor>("Pattern Editor");
            window.Data = asset;

            return window;
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
                    EditorGUI.BeginChangeCheck();
                    DrawBoard();

                    if (EditorGUI.EndChangeCheck()) SetDirtyAndSave();

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
            if (Data == null) return;

            //Choosing bullet selection type
            GUILayout.Label("Bullets");

            if (GUILayout.Button("Change Bullet Mode"))
                BulletChooseMenu.ShowAsContext();
            if(Data.Bullet != null)
            {
                Data.Bullet.OnGUI();
            }

            DrawSeparator();

            //Choosing timing type
            GUILayout.Label("Timing");

            if (GUILayout.Button("Change Timing Mode"))
                TimingChooseMenu.ShowAsContext();
            if (Data.Timing != null)
            {
                Data.Timing.OnGUI();
            }
            DrawSeparator();

            //Choosing pattern type
            GUILayout.Label("Pattern");

            if (GUILayout.Button("Change Pattern Mode"))
                PatternChooseMenu.ShowAsContext();
            if (Data.Pattern != null)
            {
                Data.Pattern.OnGUI();
            }

            DrawSeparator();
        }

        System.Type[] bulletChooseTypes;
        System.Type[] timingChooseTypes;
        System.Type[] patternChooseTypes;

        private void SetupContextMenus()
        {
            BulletChooseMenu = new GenericMenu();
            TimingChooseMenu = new GenericMenu();
            PatternChooseMenu = new GenericMenu();

            int count = 0;

            //Setup bullet choose menu, by getting all compiled 
            bulletChooseTypes = CullTypeArray(Assembly.GetAssembly(typeof(BulletBase)).GetTypes(), typeof(BulletBase));
            for (int i = 0; i < bulletChooseTypes.Length; i++)
            {
                BulletChooseMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(bulletChooseTypes[i].Name)), false, () => BulletChooseOnClick(bulletChooseTypes[count-1]));
                count++;
            }

            count = 0;

            //Setup Timing choose menu, by getting all compiled 
            timingChooseTypes = CullTypeArray(Assembly.GetAssembly(typeof(TimingBase)).GetTypes(), typeof(TimingBase));
            for (int i = 0; i < timingChooseTypes.Length; i++)
            {
                TimingChooseMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(timingChooseTypes[i].Name)), false, () => TimingChooseOnClick(timingChooseTypes[count-1]));
                count++;
            }

            count = 0;

            //Setup Pattern choose menu, by getting all compiled 
            patternChooseTypes = CullTypeArray(Assembly.GetAssembly(typeof(PatternBase)).GetTypes(), typeof(PatternBase));
            for (int i = 0; i < patternChooseTypes.Length; i++)
            {
                PatternChooseMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(patternChooseTypes[i].Name)), false, () => PatternChooseOnClick(patternChooseTypes[count-1]));
                count++;
            }

        }

        private System.Type[] CullTypeArray(System.Type[] toCull, System.Type desiredParentType)
        {
            List<System.Type> tempList = new List<Type>();

            for(int i = 0; i < toCull.Length; i++)
            {
                if (toCull[i].IsSubclassOf(desiredParentType))
                    tempList.Add(toCull[i]);
            }

            return tempList.ToArray();
        }

        private void PatternChooseOnClick(Type type)
        {
            if (Data == null) return;

            Undo.RecordObject(Data, "Changed Pattern Choose Mode");
            //This code is kind of yucky, by makes modularity really easy
            Data.Pattern = (PatternBase)Activator.CreateInstance(type);
            SetDirtyAndSave();
            Repaint();
        }

        private void TimingChooseOnClick(Type type)
        {
            if (Data == null) return;

            Undo.RecordObject(Data, "Changed Timing Choose Mode");
            //This code is kind of yucky, by makes modularity really easy
            Data.Timing = (TimingBase)Activator.CreateInstance(type);
            SetDirtyAndSave();
            Repaint();
        }

        private void BulletChooseOnClick(Type type)
        {
            if (Data == null) return;

            Undo.RecordObject(Data,"Changed Bullet Choose Mode");
            //This code is kind of yucky, by makes modularity really easy
            Data.Bullet = (BulletBase)Activator.CreateInstance(type);
            SetDirtyAndSave();
            Repaint();
        }

        private void DrawMenuBar()
        {
            Rect menuBar = new Rect(0, 0, position.width, 24);

            GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
            {
                //Draw menu bar content
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    Data = (BulletHellPattern)EditorGUILayout.ObjectField(Data, typeof(BulletHellPattern), false);

                    if(GUILayout.Button("Save"))
                    {
                        SetDirtyAndSave();
                    }

                    if(Data != null)
                    {
                        Debug.Log(Data.GetType().IsSerializable);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndArea();
            }
        }

        private void SetDirtyAndSave()
        {
            if(Data != null)
            {
                EditorUtility.SetDirty(Data);
            }
        }

        private void DrawSeparator()
        {
            GUI.backgroundColor = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.backgroundColor = Color.white;
        }
    }
}