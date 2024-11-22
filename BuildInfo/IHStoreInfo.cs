using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace HOW
{
    public class IHStoreInfo : ScriptableObject
    {
        [SerializeField]public string appName;              // 앱 이름
        [SerializeField]public string package;              // 패키지명
        [SerializeField]public string version;              // 버전
        [SerializeField]public string bundleVersion;        // 번들버전
        [SerializeField]public string publicKey;            // 스토어 라이센스
        [SerializeField]public string admobID;              // 광고 ID
        [SerializeField]public string admobKeyMain;         // 광고 key의 앞부분( ex : ca-app-pub-9429808070250988/1150010542 이라면 ca-app-pub-9429808070250988 부분 )
        [SerializeField]public List<string> adRewardKey;    // 광고 key
        [SerializeField]public List<Texture2D> icons;       // 아이콘

        // TODO : 광고 필요!!

        #if UNITY_EDITOR
        [MenuItem("Dev/Create/StoreInfo")]
        static void CreateStoreInfoAsset()
        {
            const string buildInfoAssetPath = "Assets/BuildInfo/StoreInfo.asset";

            if (File.Exists(buildInfoAssetPath))
                return;

            var buildInfo = CreateInstance<IHStoreInfo>();
            AssetDatabase.CreateAsset(buildInfo, buildInfoAssetPath);
        }
        #endif
    }
}
