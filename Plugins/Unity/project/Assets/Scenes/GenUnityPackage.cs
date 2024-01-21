using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenUnityPackage 
{
    public static void Export () {
        var dir = new FileInfo(exportPath).Directory;
        if (dir != null && !dir.Exists) {
            dir.Create();
        }
        AssetDatabase.ExportPackage("../../Artifact", "BocchiTracker/BocchiTracker.unitypackage", ExportPackageOptions.Recurse);
    }
}
