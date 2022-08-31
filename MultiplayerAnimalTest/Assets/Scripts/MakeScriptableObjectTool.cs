using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
public class MakeScriptableObject
{
    [MenuItem("Assets/Create/CustomData/PickUp Data")]
    public static void CreatePickupDataAsset()
    {
        PickUpData asset = ScriptableObject.CreateInstance<PickUpData>();

        AssetDatabase.CreateAsset(asset, "Assets/Data/PickUpData/NEWPickUpData.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif