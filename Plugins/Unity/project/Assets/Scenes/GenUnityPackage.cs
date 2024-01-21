using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenUnityPackage 
{
    [MenuItem("BocchiTracker/Export Package")]
    public static void Export () {

        var exportedPackageAssetList = new List<string>();
        foreach (var guid in AssetDatabase.FindAssets("", new[] { "Assets/BocchiTracker" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            exportedPackageAssetList.Add(path);
        }

        foreach (var PackFile in exportedPackageAssetList) { Debug.Log(PackFile); }

        AssetDatabase.ExportPackage(exportedPackageAssetList.ToArray(), "../Artifact/BocchiTracker.unitypackage", ExportPackageOptions.Recurse);
    }
}
