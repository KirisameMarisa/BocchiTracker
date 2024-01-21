using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenUnityPackage 
{
    public static void Export () {
        AssetDatabase.ExportPackage("../../Artifact", "BocchiTracker/BocchiTracker.unitypackage", ExportPackageOptions.Recurse);
    }
}
