using System.Collections;
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
            BulletHellPattern pattern = (BulletHellPattern)sdata.targetObject;

            //Verifly that the object has a pattern
            if(pattern != null)
            {
                if(pattern.PatternLayers == null)
                {
                    pattern.PatternLayers = new List<BulletHellPattern.PatternLayer>();
                    pattern.PatternLayers.Add(   new BulletHellPattern.PatternLayer());
                }

                if(pattern.PatternLayers.Count < 0)
                {
                    pattern.PatternLayers.Add(new BulletHellPattern.PatternLayer());
                }
            }
        }
    }
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

    public void OnGUI()
    {
        GUILayout.BeginVertical("Box", GUILayout.MaxWidth(512));
        {
            // Load Input
            EditorGUI.BeginChangeCheck();
            data = (BulletHellPattern)EditorGUILayout.ObjectField(data, typeof(BulletHellPattern), false);
            if (EditorGUI.EndChangeCheck()) sData = new SerializedObject(data);

            if (sData != null)
            {
                sData.Update();

                DrawBoard();

                sData.ApplyModifiedProperties();
            }

            GUILayout.EndVertical();
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
            sLayer = sData.FindProperty("PatternLayers").GetArrayElementAtIndex(0);

            if (sLayer != null)
            {
                sBullet  = sData.FindProperty("PatternLayers").GetArrayElementAtIndex(0).FindPropertyRelative("Bullet");
                sTiming  = sData.FindProperty("PatternLayers").GetArrayElementAtIndex(0).FindPropertyRelative("Timing");
                sPattern = sData.FindProperty("PatternLayers").GetArrayElementAtIndex(0).FindPropertyRelative("Pattern");

                sBullet.isExpanded  = true;
                sTiming.isExpanded  = true;
                sPattern.isExpanded = true;
            }
        }

        if (sLayer == null) return;
  
        //Bullets
        if (GUILayout.Button("Change Bullet Mode")) BulletChooseMenu.ShowAsContext();
        GUILayout.Label("Bullet Selection", EditorStyles.centeredGreyMiniLabel);
        if (sBullet != null) EditorGUILayout.PropertyField(sBullet, true);

        DrawSeparator();

        //Timing
        if (GUILayout.Button("Change Timing Mode")) TimingChooseMenu.ShowAsContext();
        GUILayout.Label("Timing Selection", EditorStyles.centeredGreyMiniLabel);
        if (sTiming != null) EditorGUILayout.PropertyField(sTiming, true);

        DrawSeparator();

        //Pattern
        if (GUILayout.Button("Change Pattern Mode")) PatternChooseMenu.ShowAsContext();
        GUILayout.Label("Pattern Selection", EditorStyles.centeredGreyMiniLabel);
        if (sPattern != null) EditorGUILayout.PropertyField(sPattern, true);

        DrawSeparator();
    }

    private void DrawSeparator()
    {
        GUI.backgroundColor = HighlightColor;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUI.backgroundColor = Color.white;
    }

    #region // CONTEXT MENUS
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
