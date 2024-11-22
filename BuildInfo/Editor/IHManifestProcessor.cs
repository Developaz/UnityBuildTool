#if UNITY_ANDROID

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#else
using UnityEditor;
#endif
using UnityEditor.Build;
using UnityEngine;

#if UNITY_2018_1_OR_NEWER
public class IHManifestProcessor : IPreprocessBuildWithReport
#else
public class IHManifestProcessor : IPreprocessBuild
#endif
{
    //private const string META_AD_MANAGER_APP = "com.google.android.gms.ads.AD_MANAGER_APP";

    private const string META_ADBRIX_APPKEY_NAME = "AdBrixRmAppKey";
    private const string META_ADBRIX_SECERETKEY_NAME = "AdBrixRmSecretKey";

    private const string META_ADBRIX_APPKEY_GOOGLE = "eiqGFzIT7ECz3SFIvE7eLg";
    private const string META_ADBRIX_SECERETKEY_GOOGLE = "OjbFMv6IyUOKbdQR0GXL5w";

    private const string META_ADBRIX_APPKEY_ONESTORE = "x8PSe8I5u0Ozo67eH8jkAg";
    private const string META_ADBRIX_SECERETKEY_ONESTORE = "Gf5qqjVxbUiLWZiSiJXWBA";

    //private const string META_DELAY_APP_MEASUREMENT_INIT =
    //        "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT";

    private XNamespace ns = "http://schemas.android.com/apk/res/android";

    public int callbackOrder { get { return 0; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild( BuildReport report )
#else
    public void OnPreprocessBuild(BuildTarget target, string path)
#endif
    {
        HOW.IHBuildInfo buildInfo = Resources.Load( "BuildInfo" ) as HOW.IHBuildInfo;

        string manifestPath = Path.Combine(
                Application.dataPath, "Plugins/Android/AndroidManifest.xml" );

        XDocument manifest = null;
        try
        {
            manifest = XDocument.Load( manifestPath );
        }
#pragma warning disable 0168
        catch( IOException e )
#pragma warning restore 0168
        {
            Debug.LogError( "AndroidManifest.xml is not valid." );
            return;
        }

        XElement elemManifest = manifest.Element( "manifest" );

        if( elemManifest == null )
        {
            Debug.LogError( "AndroidManifest.xml is not valid." );
            return;
        }

        XElement elemApplication = elemManifest.Element( "application" );
        if( elemApplication == null )
        {
            Debug.LogError( "AndroidManifest.xml is not valid." );
            return;
        }

        IEnumerable<XElement> metas = elemApplication.Descendants()
                .Where( elem => elem.Name.LocalName.Equals( "meta-data" ) );
        
        string appKey = "";
        string secretKey = "";

        XElement elemAdbrixAppkey = GetMetaElement( metas, META_ADBRIX_APPKEY_NAME );

        switch( buildInfo.store )
        {
            case HOW.eStore.GOOGLE:
                appKey = META_ADBRIX_APPKEY_GOOGLE;
                secretKey = META_ADBRIX_SECERETKEY_GOOGLE;
                break;
            case HOW.eStore.ONESTORE:
                appKey = META_ADBRIX_APPKEY_ONESTORE;
                secretKey = META_ADBRIX_SECERETKEY_ONESTORE;
                break;
        }

        Debug.Log( "store : " + buildInfo.store );
        Debug.Log( "appKey : " + appKey );
        Debug.Log( "secretKey : " + secretKey );

        if( appKey.Length == 0 )
        {
            Debug.LogError( "Adbrix AppKey is empty." );
        }

        if( elemAdbrixAppkey == null )
        {
            elemApplication.Add( CreateMetaElement( META_ADBRIX_APPKEY_NAME, appKey ) );
        }
        else
        {
            elemAdbrixAppkey.SetAttributeValue( ns + "value", appKey );
        }

        XElement elemAdbrixSecretkey = GetMetaElement( metas, META_ADBRIX_SECERETKEY_NAME );
        

        if( secretKey.Length == 0 )
        {
            Debug.LogError( "Adbrix SecretKey is empty." );
        }

        if( elemAdbrixSecretkey == null )
        {
            elemApplication.Add( CreateMetaElement( META_ADBRIX_SECERETKEY_NAME, secretKey ) );
        }
        else
        {
            elemAdbrixSecretkey.SetAttributeValue( ns + "value", secretKey );
        }

        elemManifest.Save( manifestPath );
    }

    private XElement CreateMetaElement( string name, object value )
    {
        return new XElement( "meta-data",
                new XAttribute( ns + "name", name ), new XAttribute( ns + "value", value ) );
    }

    private XElement GetMetaElement( IEnumerable<XElement> metas, string metaName )
    {
        foreach( XElement elem in metas )
        {
            IEnumerable<XAttribute> attrs = elem.Attributes();
            foreach( XAttribute attr in attrs )
            {
                if( attr.Name.Namespace.Equals( ns )
                        && attr.Name.LocalName.Equals( "name" ) && attr.Value.Equals( metaName ) )
                {
                    return elem;
                }
            }
        }
        return null;
    }
}

#endif
