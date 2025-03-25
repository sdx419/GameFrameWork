using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    [MenuItem("Tools/Build AssetBundles (PC)")]
    static void Build()
    {
        string outputPath = Path.Combine(Application.streamingAssetsPath, "assets");
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
        Debug.LogError($"build finish");
    }

}
