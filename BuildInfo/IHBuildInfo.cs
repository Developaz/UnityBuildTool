using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace HOW
{
    public class IHBuildInfo : ScriptableObject
    {
        [SerializeField]public eStore store;                // store 정보( 구글, ios, 원스토어 )
        [SerializeField]public eServer startServer;         // 선택된 서버( dev, qa, staging, review, live )
        [SerializeField]public eLanguage lang;              // 언어( kr, en, jp )
        [SerializeField]public string publicKey;            // 스토어 퍼블릭키( 원스토어에서만 사용 )
        [SerializeField]public string admobID;              // 광고 ID
        [SerializeField]public string admobKeyMain;         // 광고 key의 앞부분( ex : ca-app-pub-9429808070250988/1150010542 이라면 ca-app-pub-9429808070250988 부분 )
        [SerializeField]public List<string> adRewardKey;    // 광고 key

        #if UNITY_EDITOR
        [MenuItem("Dev/Create/BuildInfo")]
        static void CreateBuildInfoAsset()
        {
            const string buildInfoAssetPath = "Assets/BuildInfo/Resources/BuildInfo.asset";

            if (File.Exists(buildInfoAssetPath))
                return;

            var buildInfo = CreateInstance<IHBuildInfo>();
            AssetDatabase.CreateAsset(buildInfo, buildInfoAssetPath);
        }
        #endif
    }
}
