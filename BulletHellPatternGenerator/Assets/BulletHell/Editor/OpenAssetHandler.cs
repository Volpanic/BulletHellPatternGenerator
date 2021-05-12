using BulletHellGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class OpenAssetHandler
{
    [OnOpenAssetAttribute(1)]
    public static bool OpenFile(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);

        //Should be null if cast fails
        if (obj == null || obj.GetType() != typeof(BulletHellPattern)) return false;

        PatternEditor.OpenWindowWithAsset((BulletHellPattern)obj);
        return true;
    }
}
