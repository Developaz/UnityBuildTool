using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace HOW
{
    public class IHBuildPath : ScriptableObject
    {
        [SerializeField]public string googleStoreAPKPath;
        [SerializeField]public string oneStoreAPKPath;
        [SerializeField]public string xcodeProjectPath;

        #if UNITY_EDITOR
        [MenuItem("Dev/Create/BuildPath")]
        static void CreateStoreInfoAsset()
        {
            const string buildInfoAssetPath = "Assets/BuildInfo/BuildPath.asset";

            if (File.Exists(buildInfoAssetPath))
                return;

            var buildPath = CreateInstance<IHBuildPath>();
            AssetDatabase.CreateAsset(buildPath, buildInfoAssetPath);
        }
        #endif
    }
}
