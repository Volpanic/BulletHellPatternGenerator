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

public class PatternEditor : EditorWindow
{
    // Big get and set to make sure sData is setup properly
    // Creates and fills lists if they are empty
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

    //Clamps the layer so we can't get one out of index
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
    private Vector2 scrollPos;
    public readonly Color HighlightColor = new Color(1f,1f,1f,1f); // Color of the seperators

    //Menus that hold different bases to create
    GenericMenu BulletChooseMenu;
    GenericMenu TimingChooseMenu;
    GenericMenu PatternChooseMenu;

    [MenuItem("Bullet Hell/Pattern Editor")]
    public static PatternEditor OpenWindow()
    {
        PatternEditor window = GetWindow<PatternEditor>("Pattern Editor");
        return window;
    }

    //Opens the editor with an asset already loaded
    public static PatternEditor OpenWindowWithAsset(BulletHellPattern asset)
    {
        PatternEditor window = GetWindow<PatternEditor>("Pattern Editor");
        window.sData = new SerializedObject(asset);
        window.data = asset;
        return window;
    }

    private void OnEnable()
    {
        SetupContextMenus();
    }

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

    //Data used for displaying the pattern board
    SerializedProperty sLayer;
    SerializedProperty sBullet;
    SerializedProperty sTiming;
    SerializedProperty sPattern;

    private void DrawBoard()
    {

        if(sData.FindProperty("PatternLayers").arraySize > 0)
        sLayer = sData.FindProperty("PatternLayers").GetArrayElementAtIndex(SelectedLayer);

        //Set the board data if layer is not null
        if (sLayer != null)
        {
            sBullet  = sLayer.FindPropertyRelative("Bullet");
            sTiming  = sLayer.FindPropertyRelative("Timing");
            sPattern = sLayer.FindPropertyRelative("Pattern");

            sBullet.isExpanded  = true;
            sTiming.isExpanded  = true;
            sPattern.isExpanded = true;
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

    //Draws a line through the window to separate sections
    private void DrawSeparator()
    {
        GUI.backgroundColor = HighlightColor;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUI.backgroundColor = Color.white;
    }

    //Draws a bar at the top of the window for controlling layers
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

            //Removes selected layer, if only one layer exists it replaces it with a fresh one
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

    //Culls an array of all types that are not subclasses of the desired type
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
    
    //Replaces the currently selected layers pattern with the selected one
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

            pattern.PatternLayers[SelectedLayer] = layer;
            sData.ApplyModifiedProperties();
        }

        Repaint();
    }

    //Replaces the currently selected layers timing with the selected one
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

            pattern.PatternLayers[SelectedLayer] = layer;
            sData.ApplyModifiedProperties();
        }

        Repaint();
    }

    //Replaces the currently selected layers bullet with the selected one
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

            pattern.PatternLayers[SelectedLayer] = layer;
            sData.ApplyModifiedProperties();
        }

        Repaint();
    }
    #endregion
}
