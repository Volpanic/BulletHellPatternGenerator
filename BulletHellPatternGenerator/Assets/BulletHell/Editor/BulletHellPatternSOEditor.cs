using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BulletHellPattern))]
public class BulletHellPatternSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(target != null)
        {
            if(GUILayout.Button("Open In Pattern Editor"))
            {
                PatternEditor.OpenWindowWithAsset((BulletHellPattern)target);
            }
        }
    }
}
