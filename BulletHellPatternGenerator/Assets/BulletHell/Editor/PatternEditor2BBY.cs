﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using BulletHellGenerator;

public class PatternEditor2BBY : EditorWindow
{
    public SerializedObject sData { get { return sdata; } set 
        {
            sdata = value;

            if (value != null)
            {
                BulletHellPattern pattern = (BulletHellPattern)value.targetObject;

                //Verify that the object has a pattern
                if (pattern != null)
                {
                    if (pattern.PatternLayers == null)
                    {
                        pattern.PatternLayers = new List<BulletHellPattern.PatternLayer>();
                        pattern.PatternLayers.Add(new BulletHellPattern.PatternLayer());
                    }

                    if (pattern.PatternLayers.Count <= 0)
                    {
                        pattern.PatternLayers.Add(new BulletHellPattern.PatternLayer());
                    }
                }
            }
        }
    }

    public int SelectedLayer
    {
        get { return selectedLayer; }

        set
        {
            if(sData != null)
            {
                selectedLayer = Mathf.Clamp(value,0, sData.FindProperty("PatternLayers").arraySize);
            }
            else
            {
                selectedLayer = 0;
            }
            sLayer = null;
        }
    }

    private int selectedLayer = 0;

    private SerializedObject sdata;

    private BulletHellPattern data;

    public readonly Color HighlightColor = new Color(1f,1f,1f,1f);

    GenericMenu BulletChooseMenu;
    GenericMenu TimingChooseMenu;
    GenericMenu PatternChooseMenu;

    [MenuItem("Pattern Editor/Editor 2 BBY")]
    public static PatternEditor2BBY OpenWindow()
    {
        PatternEditor2BBY window = GetWindow<PatternEditor2BBY>("Pattern Editor 2");
        return window;
    }

    private void OnEnable()
    {
        SetupContextMenus();
    }

    private Vector2 scrollPos;

    public void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "Box", GUILayout.MaxWidth(position.width));
        {
            // Load Input
            EditorGUI.BeginChangeCheck();
            data = (BulletHellPattern)EditorGUILayout.ObjectField(data, typeof(BulletHellPattern), false);
            if (EditorGUI.EndChangeCheck())
            {
                sLayer = null;
                sData = null;
                sData = new SerializedObject(data);
            }

            DrawLayerBar();

            if (sData != null)
            {
                sData.Update();

                DrawBoard();

                sData.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    SerializedProperty sLayer;
    SerializedProperty sBullet;
    SerializedProperty sTiming;
    SerializedProperty sPattern;

    private void DrawBoard()
    {
        if (sLayer == null)
        {
            if(sData.FindProperty("PatternLayers").arraySize > 0)
            sLayer = sData.FindProperty("PatternLayers").GetArrayElementAtIndex(SelectedLayer);

            if (sLayer != null)
            {
                sBullet  = sLayer.FindPropertyRelative("Bullet");
                sTiming  = sLayer.FindPropertyRelative("Timing");
                sPattern = sLayer.FindPropertyRelative("Pattern");

                sBullet.isExpanded  = true;
                sTiming.isExpanded  = true;
                sPattern.isExpanded = true;
            }
        }

        //Bullets
        GUILayout.Label("Bullet Selection", EditorStyles.centeredGreyMiniLabel);
        if (sBullet != null) EditorGUILayout.PropertyField(sBullet, true);
        if (GUILayout.Button("Change Bullet Mode", GUILayout.Width(192))) BulletChooseMenu.ShowAsContext();

        DrawSeparator();

        //Timing
        GUILayout.Label("Timing Selection", EditorStyles.centeredGreyMiniLabel);
        if (sTiming != null) EditorGUILayout.PropertyField(sTiming, true);
        if (GUILayout.Button("Change Timing Mode", GUILayout.Width(192))) TimingChooseMenu.ShowAsContext();

        DrawSeparator();

        //Pattern
        GUILayout.Label("Pattern Selection", EditorStyles.centeredGreyMiniLabel);
        if (sPattern != null) EditorGUILayout.PropertyField(sPattern, true);
        if (GUILayout.Button("Change Pattern Mode", GUILayout.Width(192))) PatternChooseMenu.ShowAsContext();

        DrawSeparator();
    }

    private void DrawSeparator()
    {
        GUI.backgroundColor = HighlightColor;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUI.backgroundColor = Color.white;
    }

    private void DrawLayerBar()
    {
        if (sData == null) return;
        if (sData.FindProperty("PatternLayers") == null) return;

        sData.Update();

        SerializedProperty sLayers = sData.FindProperty("PatternLayers");

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            //Generate Names and values for popups
            int[] values = new int[sLayers.arraySize];
            GUIContent[] names = new GUIContent[sLayers.arraySize];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = i;
                names[i] = new GUIContent(i.ToString());
            }

            SelectedLayer = EditorGUILayout.IntPopup(new GUIContent("Layer"),SelectedLayer, names, values, EditorStyles.toolbarPopup);

            //Add Remove Patten Layer
            if(GUILayout.Button(new GUIContent("Add Layer","Adds a new pattern.")))
            {
                data.PatternLayers.Add(new BulletHellPattern.PatternLayer());
                SelectedLayer = sLayers.arraySize - 1;
                sLayer = null;
            }

            if (GUILayout.Button(new GUIContent("Remove Layer", "Removes the selected pattern")))
            {
                data.PatternLayers.RemoveAt(SelectedLayer);
                SelectedLayer--;

                if(data.PatternLayers.Count <= 0)
                {
                    data.PatternLayers.Add(new BulletHellPattern.PatternLayer());
                }
                sLayer = null; // Forces it to reget the layer
            }

            //Pattern Duration
            EditorGUILayout.PropertyField(sData.FindProperty("PatternDuration"), new GUIContent("Duration","The total duration of the pattern, used to calculate curves x positions."));

            EditorGUILayout.EndHorizontal();

            sData.ApplyModifiedProperties();
        }

    }

    #region // CONTEXT MENUS
    private static System.Type[] bulletChooseTypes;
    private static System.Type[] timingChooseTypes;
    private static System.Type[] patternChooseTypes;

    private void SetupContextMenus()
    {
        BulletChooseMenu  = new GenericMenu();
        TimingChooseMenu  = new GenericMenu();
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
        BulletHellPattern pattern = (BulletHellPattern)sData.targetObject;
        
        //Make sure pattern exsits
        if(pattern != null)
        {
            sdata.Update();
            BulletHellPattern.PatternLayer layer = pattern.PatternLayers[0];
            layer.Pattern = (PatternBase)Activator.CreateInstance(patternChooseTypes[(int)typeIndex]);
            sLayer = null;

            pattern.PatternLayers[0] = layer;
            sData.ApplyModifiedProperties();
        }

        Repaint();
    }

    private void TimingChooseOnClick(object typeIndex)
    {
        BulletHellPattern pattern = (BulletHellPattern)sData.targetObject;

        //Make sure pattern exsits
        if (pattern != null)
        {
            sdata.Update();
            BulletHellPattern.PatternLayer layer = pattern.PatternLayers[0];
            layer.Timing = (TimingBase)Activator.CreateInstance(timingChooseTypes[(int)typeIndex]);
            sLayer = null;

            pattern.PatternLayers[0] = layer;
            sData.ApplyModifiedProperties();
        }

        Repaint();
    }

    private void BulletChooseOnClick(object typeIndex)
    {
        BulletHellPattern pattern = (BulletHellPattern)sData.targetObject;

        //Make sure pattern exsits
        if (pattern != null)
        {
            sdata.Update();
            BulletHellPattern.PatternLayer layer = pattern.PatternLayers[0];
            layer.Bullet = (BulletBase)Activator.CreateInstance(bulletChooseTypes[(int)typeIndex]);
            sLayer = null;

            pattern.PatternLayers[0] = layer;
            sData.ApplyModifiedProperties();
        }

        Repaint();
    }
    #endregion
}
