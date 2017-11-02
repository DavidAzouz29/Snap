///<summary>
/// Author: David Azouz
/// Date: 31/10/17
/// -------------------------------------------------
/// Brief: Using the built-in Unity's in-editor WebView
/// 
/// viewed: http://wiki.unity3d.com/index.php/FindGameObjects
/// https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor.Web/WebViewEditorWindowTabs.cs
/// https://docs.unity3d.com/Manual/RunningEditorCodeOnLaunch.html 
/// cf. http://qiita.com/kyusyukeigo/items/a7d3940b95c4ec224d12 (in Japanese)
/// http://www.codegist.net/snippet/c/twitterwindowcs_rraallvv_c
/// 
/// https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor.Web/JSProxyMgr.cs
/// 
/// https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/AssetStoreLoginWindow.cs
/// https://github.com/MattRix/UnityDecompiled/blob/82e03c823811032fd970ffc9a75246e95c626502/UnityEditor/UnityEditor/AssetStoreAssetInspector.cs
/// </summary>

using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Text;
//using System.Xml;

//typedef webView = WebView;

public class BlocklyPlayground : ScriptableObject
{

	static object webView;
	//static Type webView;
	static Type webViewType;

    //[EditorWindowTitle(title = "Snapper Playground", useTypeNameAsIconName = true)]
    [MenuItem("Window/Snapper/Snapper Playground &s")]
    static void OpenSnapperPlayground()
    {
        OpenWebViewEditorWindowTabs("Snapper Playground",
            "file:///Assets/Snapper/blockly_playground.html");
    }

    //[EditorWindowTitle(title = "Blockly Playground", useTypeNameAsIconName = true)]
    [MenuItem("Window/Snapper/Blockly Playground &b")]
    static void OpenBlocklyPlayground()
    {
        OpenWebViewEditorWindowTabs("Blockly Playground",
            "https://blockly-demo.appspot.com/static/tests/playground.html");
    }

    //[EditorWindowTitle(title = "Block Factory", icon = "Snap.png")]
    [MenuItem("Window/Snapper/Block Factory &f")]
    static void OpenBlockFactory()
    {
        OpenWebViewEditorWindowTabs("Block Factory",
            "https://blockly-demo.appspot.com/static/demos/blockfactory/index.html");
    }

    static void OpenWebViewEditorWindowTabs(string a_windowTitle, string a_path)
    {
        var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
#if UNITY_5_4_OR_NEWER
        webViewType = (typeof(Editor).Assembly).GetType("UnityEditor.Web.WebViewEditorWindowTabs");
        webView = (typeof(Editor).Assembly).GetType("UnityEditor.WebView");
        //var type = Types.GetType("UnityEditor.Web.WebViewEditorWindowTabs", "UnityEditor.dll");
#else
        webViewType = Types.GetType("UnityEditor.Web.WebViewEditorWindow", "UnityEditor.dll");
        webView = Types.GetType("UnityEditor.WebView", "UnityEditor.dll");
#endif
        webView = ScriptableObject.CreateInstance(webViewType);
        webView = webViewType.GetMember("m_WebView", flags);

        var methodInfo = webViewType.GetMethod("Create", flags);
        methodInfo = methodInfo.MakeGenericMethod(webViewType);
        if (a_path == "")
            a_path = "https://www.google.com.au/";
        methodInfo.Invoke(
            webView,
            new object[]
            {
                a_windowTitle,
                a_path,
                200, 500, 800, 600
            });
    }

    public void OnDownloadProgress(string id, string message, ulong bytes, ulong total)
    {
        this.InvokeJSMethod("document.AssetStore.pkgs", "OnDownloadProgress", new object[]
        {
                id,
                message,
                bytes,
                total
        });
    }

    #region Serialisation
    //this.InvokeJSMethod("document.AssetStore.login", "logout", new object[0]);
    //https://blockly-demo.appspot.com/static/tests/playground.html toXML()
    public void ExportToXML()
    {
        this.InvokeJSMethod("document", "toXML", new object[0]);
        Debug.Log("To XML");
        //TODO: JSONProxyMgr?
    }

    public void ImportFromXML()
    {
        this.InvokeJSMethod("document", "fromXML", new object[0]);
        Debug.Log("From XML");
    }

    //JavaScript, Python, PHP, Lua, Dart
    public void ToCode(string a_lang)
    {
        this.InvokeJSMethod("document", "toCode", new object[]
            {
                a_lang
            });
        Debug.LogFormat("Translated to: {0}.", a_lang);
    }

    //TODO: get either output or textarea from taChange
    // Disable the "Import from XML" button if the XML is invalid.
    // Preserve text between page reloads.
    public void taChange()
    {
        //XmlDocument document = new XmlDocument();
        Application.ExternalCall("document.getElementById('importExport')");
        /*var textarea = document.getElementById("importExport");
        if (sessionStorage)
        {
            sessionStorage.setItem("textarea", textarea.value);
        }
        var valid = true;
        try
        {
            Blockly.Xml.textToDom(textarea.value);
        }
        catch (e)
        {
            valid = false;
        }
        document.getElementById("import").disabled = !valid;*/
    }
#endregion

    /*function GetUserName()
    {
        if (document.getElementById("fw-member-presence").firstChild.innerText != null)
        {
            var unity = GetUnity();
            unity.SendMessage("WebPhone", "PickUp", document.getElementById("fw-member-presence").firstChild.innerText);
        }
    }

    <head> <title>Unity Web Player - comm</title> 
        <script type = "text/javascript" language="javascript"> 
        <!-- function SaySomethingToUnity()
    { document.getElementById("UnityContent").SendMessage("MyObject", "MyFunction", 
        "Hello from a web page!"); } --> </script>*/
    private void InvokeJSMethod(string objectName, string name, params object[] args)
    {
        if (webView != null) //TODO: uncomment!
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(objectName);
            stringBuilder.Append('.');
            stringBuilder.Append(name);
            stringBuilder.Append('(');
            bool flag = true;
            for (int i = 0; i < args.Length; i++)
            {
                object obj = args[i];
                if (!flag)
                {
                    stringBuilder.Append(',');
                }
                bool flag2 = obj is string;
                if (flag2)
                {
                    stringBuilder.Append('"');
                }
                stringBuilder.Append(obj);
                if (flag2)
                {
                    stringBuilder.Append('"');
                }
                flag = false;
            }
            stringBuilder.Append(");");
            //this.webView.ExecuteJavascript(stringBuilder.ToString());

            //
            var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var webViewT = webView as Type;
            var methodInfo = webViewT.GetMethod("ExecuteJavascript", flags);
            methodInfo = methodInfo.MakeGenericMethod(webViewT);
            
            methodInfo.Invoke(
                webView,
                new object[]
                {
                stringBuilder.ToString()
                });
        }
    }
}