using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using GoogleMobileAds.Editor;

namespace HOW
{
    public class IHBuildTool : EditorWindow
    {
        public eStore selectStore;
        public eServer selectServer;
        public eLanguage selectLang;
        public string version;
        public string bundleVersion;
        public bool splitBinary;

        public Texture2D googleAdaptiveBackground;
        public Texture2D googleAdaptiveForeground;
        public Texture2D googleRound;
        public Texture2D googleLegacy;

        private Texture2D onestoreAdaptiveBackground;
        private Texture2D onestoreAdaptiveForeground;
        private Texture2D onestoreRound;
        private Texture2D onestoreLegacy;

        private const string INFO_PATH = "Assets/BuildInfo/";
        private const string DEFAULT_OUT_PATH = "./";

        private Vector2 scrollPos;

        [MenuItem( "Dev/BuildTool" )]
	    public static void ShowTool()
	    {
		    EditorWindow.GetWindow( typeof ( IHBuildTool ) );
	    }

        private Texture2D TextureField( string name, Texture2D texture )
        {
            EditorGUILayout.BeginVertical();
            var result = (Texture2D)EditorGUILayout.ObjectField( texture, typeof( Texture2D ), false, GUILayout.Width( 80 ), GUILayout.Height( 80 ) );
            var style = new GUIStyle( GUI.skin.label );
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = 80;
            style.wordWrap = true;
            GUILayout.Label( name, style );
            EditorGUILayout.EndVertical();
            GUILayout.Space( 20 );
            return result;
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView( scrollPos, false, false );
            selectStore = (eStore)EditorGUILayout.EnumPopup( "Select Store : ", selectStore );
            selectServer = (eServer)EditorGUILayout.EnumPopup( "Select Server : ", selectServer );
            selectLang = (eLanguage)EditorGUILayout.EnumPopup( "Select Language : ", selectLang );
            
            EditorGUILayout.BeginHorizontal();
            version = EditorGUILayout.TextField( "Version : ", version );
            bundleVersion = EditorGUILayout.TextField( "BundleVersion : ", bundleVersion );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            splitBinary = EditorGUILayout.Toggle( "Split Binary", splitBinary );
            if( GUILayout.Button( "LOAD" ) ) { Load(); }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 20 );
            
            EditorGUILayout.BeginHorizontal( GUILayout.Width( 80 ) );
            GUILayout.Label( "Google Icon : ", new GUIStyle( GUI.skin.label ) );
            GUILayout.Space( 14 );
            googleAdaptiveBackground = TextureField( "Adaptive Background", googleAdaptiveBackground );
            googleAdaptiveForeground = TextureField( "Adaptive Foreground", googleAdaptiveForeground );
            googleRound = TextureField( "Round", googleRound );
            googleLegacy = TextureField( "Legacy", googleLegacy );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal( GUILayout.Width( 80 ) );
            GUILayout.Label( "OneStore Icon : ", new GUIStyle( GUI.skin.label ) );
            onestoreAdaptiveBackground = TextureField( "Adaptive Background", onestoreAdaptiveBackground );
            onestoreAdaptiveForeground = TextureField( "Adaptive Foreground", onestoreAdaptiveForeground );
            onestoreRound = TextureField( "Round", onestoreRound );
            onestoreLegacy = TextureField( "Legacy", onestoreLegacy );
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 20 );
            EditorGUILayout.HelpBox( "Edit Order\n1.Choose Store.\n2.Choose Server.\n3.Choose Language.\n4.Click LOAD Button.\n5.Enter Version And BundleVersion.\n6.Click BUILD Button", MessageType.Info );
            if( GUILayout.Button( "BUILD" ) ) { Build(); }
            EditorGUILayout.EndScrollView();
        }

        private void LoadIcon()
        {
            IHStoreInfo googleInfo = GetStoreInfo( eStore.GOOGLE );
            IHStoreInfo onestoreInfo = GetStoreInfo( eStore.ONESTORE );

            googleAdaptiveBackground = googleInfo.icons[ 0 ];
            googleAdaptiveForeground = googleInfo.icons[ 1 ];
            googleRound = googleInfo.icons[ 2 ];
            googleLegacy = googleInfo.icons[ 3 ];

            onestoreAdaptiveBackground = onestoreInfo.icons[ 0 ];
            onestoreAdaptiveForeground = onestoreInfo.icons[ 1 ];
            onestoreRound = onestoreInfo.icons[ 2 ];
            onestoreLegacy = onestoreInfo.icons[ 3 ];
        }

        private void LoadVersion()
        {
            SetVersion( GetStoreInfo() );
            GUI.FocusControl( null );
        }

        private void Load()
        {
            LoadIcon();
            LoadVersion();
        }

        private void UpdateVersion()
        {
            UpdateVersion( GetStoreInfo() );
            UpdateBuildInfo( GetStoreInfo() );
            GUI.FocusControl( null );
        }

        private void Build()
        {
            if( Check() == true )
            {
                bool usePath = System.IO.File.Exists( "Assets/BuildInfo/BuildPath.asset" );

                switch( selectStore )
                {
                    case eStore.GOOGLE:
                        #if UNITY_ANDROID
                        GoogleBuild( usePath );
                        #elif UNITY_IOS
                        Debug.LogError( "BuildTool Error : Wrong Platform = Current Target Platform is IOS" );
                        #endif
                        break;
                    case eStore.ONESTORE:
                        #if UNITY_ANDROID
                        OneStoreBuild( usePath );
                        #elif UNITY_IOS
                        Debug.LogError( "BuildTool Error : Wrong Platform = Current Target Platform is IOS" );
                        #endif
                        break;
                    case eStore.APPSTORE:
                        #if UNITY_ANDROID
                        Debug.LogError( "BuildTool Error : Wrong Platform = Current Target Platform is ANDROID" );
                        #elif UNITY_IOS
                        AppStoreBuild( usePath );
                        #endif
                        break;
                }
            }
        }

        // 체크함수들
        // 스토어, 서버, 랭귀지 enum 체크
        private bool Check()
        {
            if( CheckStore() == true && CheckServer() == true && CheckLang() == true && CheckVersion() == true && CheckBundleVersion() == true )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckStore()
        {
            switch( selectStore )
            {
                case eStore.APPSTORE:
                case eStore.GOOGLE:
                case eStore.ONESTORE:
                    return true;
                default:
                    Debug.LogError( "BuildTool Error : Select Store is Wrong = " + selectStore );
                    return false;
            }
        }

        private bool CheckServer()
        {
            switch( selectServer )
            {
                case eServer.DEV:
                case eServer.QA:
                case eServer.STAGING:
                case eServer.REVIEW:
                case eServer.LIVE:
                    return true;
                default:
                    Debug.LogError( "BuildTool Error : Select Server is Wrong = " + selectServer );
                    return false;
            }
        }

        private bool CheckLang()
        {
            switch( selectLang )
            {
                case eLanguage.KR:
                case eLanguage.EN:
                case eLanguage.JP:
                    return true;
                default:
                    Debug.LogError( "BuildTool Error : Select Language is Wrong = " + selectLang );
                    return false;
            }
        }

        private bool CheckVersion()
        {
            bool returnValue = false;
            string[] words = version.Split( '.' );

            if( CheckStore() == true )
            {
                if( words.Length == 3 )
                {
                    int mainVer = 0;
                    int subVer = 0;
                    int fixVer = 0;
                    

                    if( int.TryParse( words[ 0 ], out mainVer ) == true && int.TryParse( words[ 1 ], out subVer ) == true && int.TryParse( words[ 2 ], out fixVer ) == true )
                    {
                        returnValue = true;
                    }
                }

                if( returnValue == false )
                {
                    Debug.LogError( "BuildTool Error : Wrong Version Format = " + version );
                    SetVersion( GetStoreInfo() );
                }

                return returnValue;
            }
            else
            {
                return false;
            }
        }

        private bool CheckBundleVersion()
        {
            bool returnValue = false;

            if( CheckStore() == true )
            {
                int bundleVer = 0;

                if( int.TryParse( bundleVersion, out bundleVer ) == true )
                {
                    returnValue = true;
                }

                if( returnValue == false )
                {
                    Debug.LogError( "BuildTool Error : BundleVersion is not number = " + version );
                    SetVersion( GetStoreInfo() );
                }

                return returnValue;
            }
            else
            {
                return false;
            }
        }

        private IHStoreInfo GetStoreInfo( eStore _store )
        {
            string path = INFO_PATH;

            switch( _store )
            {
                case eStore.GOOGLE:
                    path += "GoogleStoreInfo.asset";
                    break;
                case eStore.APPSTORE:
                    path += "AppStoreInfo.asset";
                    break;
                case eStore.ONESTORE:
                    path += "OneStoreInfo.asset";
                    break;
                default:
                    Debug.LogError( "BuildTool Error : Select Store is Wrong = " + selectStore );
                    break;
            }

            return AssetDatabase.LoadAssetAtPath( path, typeof( IHStoreInfo ) ) as IHStoreInfo;
        }

        private IHStoreInfo GetStoreInfo()
        {
            string path = INFO_PATH;

            switch( selectStore )
            {
                case eStore.GOOGLE:
                    path += "GoogleStoreInfo.asset";
                    break;
                case eStore.APPSTORE:
                    path += "AppStoreInfo.asset";
                    break;
                case eStore.ONESTORE:
                    path += "OneStoreInfo.asset";
                    break;
                default:
                    Debug.LogError( "BuildTool Error : Select Store is Wrong = " + selectStore );
                    break;
            }

            return AssetDatabase.LoadAssetAtPath( path, typeof( IHStoreInfo ) ) as IHStoreInfo;
        }

        private void SetVersion( IHStoreInfo _storeInfo )
        {
            version = _storeInfo.version;
            bundleVersion = _storeInfo.bundleVersion;
            Repaint();
        }

        private void UpdateIcon()
        {
            IHStoreInfo googleInfo = GetStoreInfo( eStore.GOOGLE );
            IHStoreInfo onestoreInfo = GetStoreInfo( eStore.ONESTORE );

            googleInfo.icons[ 0 ] = googleAdaptiveBackground;
            googleInfo.icons[ 1 ] = googleAdaptiveForeground;
            googleInfo.icons[ 2 ] = googleRound;
            googleInfo.icons[ 3 ] = googleLegacy;

            onestoreInfo.icons[ 0 ] = onestoreAdaptiveBackground;
            onestoreInfo.icons[ 1 ] = onestoreAdaptiveForeground;
            onestoreInfo.icons[ 2 ] = onestoreRound;
            onestoreInfo.icons[ 3 ] = onestoreLegacy;

            EditorUtility.SetDirty( googleInfo );
            EditorUtility.SetDirty( onestoreInfo );
            Repaint();
        }

        private void UpdateVersion( IHStoreInfo _storeInfo )
        {
            string[] words = version.Split( '.' );

            if( words.Length == 3 )
            {
                int mainVer = 0;
                int subVer = 0;
                int fixVer = 0;
                int bundleVer = 0;

                if( int.TryParse( words[ 0 ], out mainVer ) == true && int.TryParse( words[ 1 ], out subVer ) == true && int.TryParse( words[ 2 ], out fixVer ) == true )
                {
                    if( int.TryParse( bundleVersion, out bundleVer ) == true )
                    {
                        _storeInfo.version = version;
                        _storeInfo.bundleVersion = bundleVersion;
                        EditorUtility.SetDirty( _storeInfo );
                        Repaint();
                    }
                }
            }
        }

        private IHBuildInfo UpdateBuildInfo( IHStoreInfo _storeInfo )
        {
            IHBuildInfo buildInfo = Resources.Load( "BuildInfo" ) as IHBuildInfo;
            buildInfo.startServer = selectServer;
            buildInfo.store = selectStore;
            buildInfo.lang = selectLang;
            buildInfo.publicKey = _storeInfo.publicKey;
            buildInfo.admobID = _storeInfo.admobID;
            buildInfo.admobKeyMain = _storeInfo.admobKeyMain;

            for( int i = 0; i < _storeInfo.adRewardKey.Count; i++ )
            {
                buildInfo.adRewardKey[ i ] = _storeInfo.adRewardKey[ i ];
            }

            Debug.Log( "UpdateBuildInfo : FILE = " + buildInfo.name + " STORE = " + buildInfo.store + " SERVER = " + buildInfo.startServer + " LANG = " + buildInfo.lang + " KEY = " + buildInfo.publicKey );

            EditorUtility.SetDirty( buildInfo );

            if( buildInfo.store != eStore.APPSTORE )
            {
                GoogleMobileAdsSettings.Instance.AdMobAndroidAppId = buildInfo.admobID;
                EditorUtility.SetDirty( GoogleMobileAdsSettings.Instance );
                GoogleMobileAdsSettings.Instance.WriteSettingsToFile();
            }

            return buildInfo;
        }

        private static string[] FindEnabledEditorScenes()
        {
            List<string> EditorScenes = new List<string>();
 
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                EditorScenes.Add(scene.path);
            }
 
            return EditorScenes.ToArray();
        }

        

        private string GetPath( bool _usePath )
        {
            IHStoreInfo storeInfo = GetStoreInfo();

            string name = string.Empty;
            string path = DEFAULT_OUT_PATH;
            bool pathFile = System.IO.File.Exists( "Assets/BuildInfo/BuildPath.asset" );

            switch( selectStore )
            {
                case eStore.GOOGLE:
                    name = "SMASTER_AOS_v" + storeInfo.version + "b" + storeInfo.bundleVersion + "_" + selectServer.ToString() + ".apk";
                    if( _usePath == true && pathFile == true )
                    {
                        IHBuildPath buildPath = AssetDatabase.LoadAssetAtPath( "Assets/BuildInfo/BuildPath.asset", typeof( IHBuildPath ) ) as IHBuildPath;
                        path = buildPath.googleStoreAPKPath + "/";
                    }
                    break;
                case eStore.ONESTORE:
                    name = "SMASTER_ONE_v" + storeInfo.version + "b" + storeInfo.bundleVersion + "_" + selectServer.ToString() + ".apk";
                    if( _usePath == true && pathFile == true )
                    {
                        IHBuildPath buildPath = AssetDatabase.LoadAssetAtPath( "Assets/BuildInfo/BuildPath.asset", typeof( IHBuildPath ) ) as IHBuildPath;
                        path = buildPath.oneStoreAPKPath + "/";
                    }
                    break;
                case eStore.APPSTORE:
                    name = "SMASTER_XCODE_v" + storeInfo.version + "b" + storeInfo.bundleVersion + "_" + selectServer.ToString();
                    if( _usePath == true && pathFile == true )
                    {
                        IHBuildPath buildPath = AssetDatabase.LoadAssetAtPath( "Assets/BuildInfo/BuildPath.asset", typeof( IHBuildPath ) ) as IHBuildPath;
                        path = buildPath.xcodeProjectPath + "/";
                    }
                    break;
                default:
                    Debug.LogError( "BuildTool Error : Wrong Store[ " + selectStore.ToString() + " ]" );
                    break;
            }

            Debug.Log( "path : " + path );
            Debug.Log( "buildName : " + name );

            return path + name;
        }

        private void GoogleBuild( bool _usePath )
        {
            UpdateIcon();

            IHStoreInfo storeInfo = GetStoreInfo();
            UpdateVersion( storeInfo );
            UpdateBuildInfo( storeInfo );

            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            buildOption.scenes = FindEnabledEditorScenes();
            buildOption.locationPathName = GetPath( _usePath );
            buildOption.target = BuildTarget.Android;
            buildOption.targetGroup = BuildTargetGroup.Android;
            buildOption.options = BuildOptions.None;

            PlayerSettings.productName = storeInfo.appName;
            PlayerSettings.SetApplicationIdentifier( BuildTargetGroup.Android, storeInfo.package );
            PlayerSettings.bundleVersion = storeInfo.version;
            PlayerSettings.Android.bundleVersionCode = int.Parse( storeInfo.bundleVersion );
            PlayerSettings.Android.keystoreName = "./keystore_smaster/keystore_smaster.keystore";
            PlayerSettings.Android.keystorePass = "Beatrain0801!";
            PlayerSettings.Android.keyaliasName = "smaster";
            PlayerSettings.Android.keyaliasPass = "Beatrain0801!";
            PlayerSettings.Android.useAPKExpansionFiles = splitBinary;

            PlatformIcon[] icons;
            Texture2D[] iconTex;

            icons = PlayerSettings.GetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Adaptive );
            iconTex = icons[ 0 ].GetTextures();
            iconTex[ 0 ] = googleAdaptiveBackground;
            iconTex[ 1 ] = googleAdaptiveForeground;
            icons[ 0 ].SetTextures( iconTex );
            PlayerSettings.SetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Adaptive, icons );

            icons = PlayerSettings.GetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Round );
            iconTex = icons[ 0 ].GetTextures();
            iconTex[ 0 ] = googleRound;
            icons[ 0 ].SetTextures( iconTex );
            PlayerSettings.SetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Round, icons );

            icons = PlayerSettings.GetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Legacy );
            iconTex = icons[ 0 ].GetTextures();
            iconTex[ 0 ] = googleLegacy;
            icons[ 0 ].SetTextures( iconTex );
            PlayerSettings.SetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Legacy, icons );

            BuildStart( buildOption );
        }

        private void OneStoreBuild( bool _usePath )
        {
            UpdateIcon();

            IHStoreInfo storeInfo = GetStoreInfo();
            UpdateVersion( storeInfo );
            UpdateBuildInfo( storeInfo );

            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            buildOption.scenes = FindEnabledEditorScenes();
            buildOption.locationPathName = GetPath( _usePath );
            buildOption.target = BuildTarget.Android;
            buildOption.targetGroup = BuildTargetGroup.Android;
            buildOption.options = BuildOptions.None;

            PlayerSettings.productName = storeInfo.appName;
            PlayerSettings.SetApplicationIdentifier( BuildTargetGroup.Android, storeInfo.package );
            PlayerSettings.bundleVersion = storeInfo.version;
            PlayerSettings.Android.bundleVersionCode = int.Parse( storeInfo.bundleVersion );
            PlayerSettings.Android.keystoreName = "./keystore_smaster/keystore_smaster.keystore";
            PlayerSettings.Android.keystorePass = "Beatrain0801!";
            PlayerSettings.Android.keyaliasName = "smaster";
            PlayerSettings.Android.keyaliasPass = "Beatrain0801!";
            PlayerSettings.Android.useAPKExpansionFiles = splitBinary;

            PlatformIcon[] icons;
            Texture2D[] iconTex;

            icons = PlayerSettings.GetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Adaptive );
            iconTex = icons[ 0 ].GetTextures();
            iconTex[ 0 ] = onestoreAdaptiveBackground;
            iconTex[ 1 ] = onestoreAdaptiveForeground;
            icons[ 0 ].SetTextures( iconTex );
            PlayerSettings.SetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Adaptive, icons );

            icons = PlayerSettings.GetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Round );
            iconTex = icons[ 0 ].GetTextures();
            iconTex[ 0 ] = onestoreRound;
            icons[ 0 ].SetTextures( iconTex );
            PlayerSettings.SetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Round, icons );

            icons = PlayerSettings.GetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Legacy );
            iconTex = icons[ 0 ].GetTextures();
            iconTex[ 0 ] = onestoreLegacy;
            icons[ 0 ].SetTextures( iconTex );
            PlayerSettings.SetPlatformIcons( BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Legacy, icons );

            BuildStart( buildOption );
        }

        private void AppStoreBuild( bool _usePath )
        {
            IHStoreInfo storeInfo = GetStoreInfo();
            UpdateVersion( storeInfo );
            UpdateBuildInfo( storeInfo );

            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            buildOption.scenes = FindEnabledEditorScenes();
            buildOption.locationPathName = GetPath( _usePath );
            buildOption.target = BuildTarget.iOS;
            buildOption.targetGroup = BuildTargetGroup.iOS;
            buildOption.options = BuildOptions.None;

            PlayerSettings.productName = storeInfo.appName;
            PlayerSettings.SetApplicationIdentifier( BuildTargetGroup.iOS, storeInfo.package );
            PlayerSettings.bundleVersion = storeInfo.version;
            PlayerSettings.iOS.buildNumber = storeInfo.bundleVersion;

            BuildStart( buildOption );
        }

        private void BuildStart( BuildPlayerOptions _options )
        {
            BuildReport report = BuildPipeline.BuildPlayer( _options );
            BuildSummary summary = report.summary;

            if( summary.result == BuildResult.Succeeded )
            {
                Debug.Log( "Build succeeded : " + summary.platform + " SIZE : " + summary.totalSize + " bytes" );
            }
            else if( summary.result == BuildResult.Failed )
            {
                Debug.Log( "Build failed" );
            }

            GUI.FocusControl( null );
        }
    }
}
