using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace BulletHellGenerator
{
    public class PatternEditor : EditorWindow
    {
        // Rect is dynamically sized, but to get that size we need a 
        // rect that the dynamic one falls in, So we just give it a huge one
        Rect BoardRect = new Rect(0, 0, 1, 1);
        Vector2 boardPosition;

        public static BulletHellPattern Data;
        public static SerializedObject sData;
        public static int SelectedLayer = 0;

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
            PatternEditor window = GetWindow<PatternEditor>("Pattern Editor");
            PatternEditor.Data = asset;
            PatternEditor.sData = new SerializedObject(PatternEditor.Data);

            return window;
        }

        private void OnEnable()
        {
            SetupContextMenus();
        }

        private Vector2 scrollPos;

        private void OnGUI()
        {
            GUI.Box(new Rect(BoardRect.x, BoardRect.y + 40, BoardRect.width, BoardRect.height), "Box Tho ╚(•⌂•)╝");

            GUILayout.BeginArea(new Rect(BoardRect.x + 16, BoardRect.y + 80, float.MaxValue, float.MaxValue));
            {
                //Begin vertical area, used to measure the rect of the node
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxWidth(512), GUILayout.ExpandWidth(false), GUILayout.MaxHeight(position.height));
                {
                    if (Data != null)
                    {
                        titleContent.text = Data.name;

                        //Make sure theres data to fill out
                        if (Data.PatternLayers == null || Data.PatternLayers.Count == 0)
                        {
                            Data.PatternLayers = new List<BulletHellPattern.PatternLayer>();
                            Data.PatternLayers.Add(new BulletHellPattern.PatternLayer());
                        }

                        SelectedLayer = Mathf.Clamp(SelectedLayer, 0, sData.FindProperty("PatternLayers").arraySize-1);

                        EditorGUI.BeginChangeCheck();

                        //if(SelectedLayer < sData.FindProperty("PatternLayers").arraySize)
                        DrawBoard(Data.PatternLayers[SelectedLayer], sData.FindProperty("PatternLayers").GetArrayElementAtIndex(SelectedLayer));

                        if (EditorGUI.EndChangeCheck()) SetDirtyAndSave();
                    }
                    else
                    {
                        titleContent.text = "Pattern Editor";
                        EditorGUILayout.LabelField("No Data Loaded.", EditorStyles.centeredGreyMiniLabel);
                    }
                    GUILayout.EndScrollView();
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

        private void DrawBoard(BulletHellPattern.PatternLayer layer, SerializedProperty sLayer)
        {
            if (Data == null) return;
            if (sData == null) return;

            sData.Update();

            EditorGUILayout.PropertyField(sData.FindProperty("PatternDuration"));

            //Choosing bullet selection type
            GUILayout.Label("Bullets", EditorStyles.boldLabel);

            if (GUILayout.Button("Change Bullet Mode"))
                BulletChooseMenu.ShowAsContext();
            if (layer.Bullet != null)
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(layer.Bullet.GetType().Name), EditorStyles.centeredGreyMiniLabel);

                if (sLayer != null)
                    layer.Bullet.OnGUI(sLayer);
            }

            DrawSeparator();

            //Choosing timing type
            GUILayout.Label("Timing", EditorStyles.boldLabel);

            if (GUILayout.Button("Change Timing Mode"))
                TimingChooseMenu.ShowAsContext();
            if (layer.Timing != null)
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(layer.Timing.GetType().Name), EditorStyles.centeredGreyMiniLabel);

                if(sLayer != null)
                    layer.Timing.OnGUI(sLayer);
            }
            DrawSeparator();

            //Choosing pattern type
            GUILayout.Label("Pattern", EditorStyles.boldLabel);

            if (GUILayout.Button("Change Pattern Mode"))
                PatternChooseMenu.ShowAsContext();
            if (layer.Pattern != null)
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(layer.Pattern.GetType().Name), EditorStyles.centeredGreyMiniLabel);
                if (sLayer != null)
                    layer.Pattern.OnGUI(sLayer);
            }

            DrawSeparator();
            SetDirtyAndSave();
            sData.ApplyModifiedProperties();
        }

        private static System.Type[] bulletChooseTypes;
        private static System.Type[] timingChooseTypes;
        private static System.Type[] patternChooseTypes;

        private void SetupContextMenus()
        {
            BulletChooseMenu = new GenericMenu();
            TimingChooseMenu = new GenericMenu();
            PatternChooseMenu = new GenericMenu();

            //Setup bullet choose menu, by getting all compiled 
            bulletChooseTypes = CullTypeArray(Assembly.GetAssembly(typeof(BulletBase)).GetTypes(), typeof(BulletBase));
            for (int i = 0; i < bulletChooseTypes.Length; i++)
            {
                BulletChooseMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(bulletChooseTypes[i].Name)), false, BulletChooseOnClick, i);
            }

            //Setup Timing choose menu, by getting all compiled 
            timingChooseTypes = CullTypeArray(Assembly.GetAssembly(typeof(TimingBase)).GetTypes(), typeof(TimingBase));
            for (int i = 0; i < timingChooseTypes.Length; i++)
            {
                TimingChooseMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(timingChooseTypes[i].Name)), false, TimingChooseOnClick, i);
            }

            //Setup Pattern choose menu, by getting all compiled 
            patternChooseTypes = CullTypeArray(Assembly.GetAssembly(typeof(PatternBase)).GetTypes(), typeof(PatternBase));
            for (int i = 0; i < patternChooseTypes.Length; i++)
            {
                PatternChooseMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(patternChooseTypes[i].Name)), false, PatternChooseOnClick, i);
            }
        }

        private System.Type[] CullTypeArray(System.Type[] toCull, System.Type desiredParentType)
        {
            List<System.Type> tempList = new List<Type>();

            for (int i = 0; i < toCull.Length; i++)
            {
                if (toCull[i].IsSubclassOf(desiredParentType))
                    tempList.Add(toCull[i]);
            }

            return tempList.ToArray();
        }

        private void PatternChooseOnClick(object typeIndex)
        {
            if (Data == null) return;
            //if (patternChooseTypes == null || patternChooseTypes.Length - 1 > typeIndex) return;

            Undo.RecordObject(Data, "Changed Pattern Choose Mode");
            //This code is kind of yucky, by makes modularity really easy
            var p = Data.PatternLayers[SelectedLayer];
            p.Pattern = null;
            p.Pattern = (PatternBase)Activator.CreateInstance(patternChooseTypes[(int)typeIndex]);
            Data.PatternLayers[SelectedLayer] = p;

            SetDirtyAndSave();
            Repaint();
        }

        private void TimingChooseOnClick(object typeIndex)
        {
            if (Data == null) return;
            //if (timingChooseTypes == null || timingChooseTypes.Length - 1 > typeIndex) return;

            Undo.RecordObject(Data, "Changed Timing Choose Mode");
            //This code is kind of yucky, by makes modularity really easy
            var p = Data.PatternLayers[SelectedLayer];
            p.Timing = (TimingBase)Activator.CreateInstance(timingChooseTypes[(int)typeIndex]);
            Data.PatternLayers[SelectedLayer] = p;
            SetDirtyAndSave();
            Repaint();
        }

        private void BulletChooseOnClick(object typeIndex)
        {
            if (Data == null) return;
            //if (bulletChooseTypes == null || bulletChooseTypes.Length-1 > typeIndex) return;

            Undo.RecordObject(Data, "Changed Bullet Choose Mode");
            //This code is kind of yucky, by makes modularity really easy
            var p = Data.PatternLayers[SelectedLayer];
            p.Bullet = (BulletBase)Activator.CreateInstance(bulletChooseTypes[(int)typeIndex]);
            Data.PatternLayers[SelectedLayer] = p;
            SetDirtyAndSave();
            Repaint();
        }

        private void DrawMenuBar()
        {
            Rect menuBar = new Rect(0, 0, position.width, EditorGUIUtility.singleLineHeight * 2);

            GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
            {
                //Draw menu bar content
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    EditorGUI.BeginChangeCheck();
                    Data = (BulletHellPattern)EditorGUILayout.ObjectField(Data, typeof(BulletHellPattern), false);

                    if (EditorGUI.EndChangeCheck() && Data != null)
                    {
                        sData = new SerializedObject(Data);
                    }

                    if (GUILayout.Button("Save"))
                    {
                        SetDirtyAndSave();
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndArea();
            }

            // Layer Bar
            if (Data != null && Data.PatternLayers != null)
            {
                menuBar.y += menuBar.height;

                //Generate Names and values for popups
                int[] values = new int[Data.PatternLayers.Count];
                GUIContent[] names = new GUIContent[Data.PatternLayers.Count];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = i;
                    names[i] = new GUIContent(i.ToString() + " - " + Data.PatternLayers[i].LayerName);
                }

                GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
                {

                    GUILayout.BeginHorizontal(EditorStyles.toolbar,GUILayout.ExpandWidth(false));
                    {
                        SelectedLayer = EditorGUILayout.IntPopup(SelectedLayer, names, values, EditorStyles.toolbarPopup);

                        // Add new bullet layer
                        if (GUILayout.Button("+", EditorStyles.toolbarButton))
                        {
                            Data.PatternLayers.Add(new BulletHellPattern.PatternLayer());
                            SelectedLayer = Data.PatternLayers.Count - 1;
                            SetDirtyAndSave();
                        }

                        //Remove bullet layer
                        if (GUILayout.Button("X", EditorStyles.toolbarButton))
                        {
                            if (Data.PatternLayers.Count > 1)
                            {
                                Data.PatternLayers.RemoveAt(SelectedLayer);
                                SelectedLayer--;
                                SelectedLayer = Mathf.Clamp(SelectedLayer, 0, Data.PatternLayers.Count);
                            }
                            else
                            {
                                Data.PatternLayers[0] = new BulletHellPattern.PatternLayer();
                                SelectedLayer = 0;
                            }
                            SetDirtyAndSave();
                        }

                        //if(GUILayout.Button("Duplicate Layer",EditorStyles.toolbarButton))
                        //{
                        //    Data.PatternLayers.Add(DuplicateLayer(Data.PatternLayers[SelectedLayer]));
                        //    SelectedLayer = Data.PatternLayers.Count - 1;
                        //    SetDirtyAndSave();
                        //}
                       
                        var sPattern = Data.PatternLayers[SelectedLayer];
                        EditorGUILayout.LabelField("Layer Name",GUILayout.Width(70));
                        sPattern.LayerName = EditorGUILayout.TextField(Data.PatternLayers[SelectedLayer].LayerName);
                        Data.PatternLayers[SelectedLayer] = sPattern;

                        GUILayout.EndHorizontal();
                    }

                }
                GUILayout.EndArea();
            }

        }

        private void SetDirtyAndSave()
        {
            if (Data != null)
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

    //Draw inspector for scriptable object
    [CustomEditor(typeof(BulletHellPattern))]
    class BulletHellPatternInspector : Editor
    {

        BulletHellPattern pattern;
        private void OnEnable()
        {
            pattern = (BulletHellPattern)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open in Pattern Editor"))
            {
                PatternEditor.OpenWindowWithAsset(pattern);
            }
        }
    }

}

