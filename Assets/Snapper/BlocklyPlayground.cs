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
/// https://github.com/MattRix/UnityDecompiled/blob/82e03c823811032fd970ffc9a75246e95c626502/UnityEditor/UnityEditor/AddComponentWindow.cs#L168
/// https://github.com/MattRix/UnityDecompiled/blob/b4b209f8d1c93d66f560bf23c81bc0910cef177c/UnityEditor/UnityEditor/ProjectWindowUtil.cs#L193
/// 
/// https://answers.unity.com/questions/1430364/savecopy-a-txt-file-a-runtime.html
/// https://docs.unity3d.com/ScriptReference/EditorGUIUtility-systemCopyBuffer.html
/// https://stackoverflow.com/a/6055620
/// https://forum.unity.com/threads/c-script-template-how-to-make-custom-changes.273191/ 
/// 
/// </summary>

using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Linq;
//using System.Xml;

//typedef webView = WebView;

public class BlocklyPlayground : ScriptableObject
{

    static object webView;
    //static Type webView;
    static Type webViewType;

    public static string LastSnapperScript { get; private set; }

    #region SnapperWindows
    public const string snapperPath = "Window/Snapper/";

    //[EditorWindowTitle(title = "Snapper Editor", useTypeNameAsIconName = true)]
    [MenuItem(snapperPath + "Snapper Editor &s")]
    public static void OpenSnapperEditor()
    {
        OpenWebViewEditorWindowTabs("Snapper Editor",
            "file:///Assets/StreamingAssets/snapper/tests/snapper_editor.html");
    }
    /*
    //[EditorWindowTitle(title = "Snapper Playground", useTypeNameAsIconName = true)]
    [MenuItem(snapperPath + "Snapper Playground %#&s")]
    static void OpenSnapperPlayground()
    {
        OpenWebViewEditorWindowTabs("Snapper Playground",
            "file:///Assets/StreamingAssets/blockly_playground.html");
    }*/

    //[EditorWindowTitle(title = "Scratch Playground", useTypeNameAsIconName = true)]
    [MenuItem(snapperPath + "Scratch Playground &#s", priority = 2)]
    static void OpenScratchPlayground()
    {
        OpenWebViewEditorWindowTabs("Scratch Playground",
            "https://scratch.mit.edu/projects/editor/?tip_bar=home");
    }

    //[EditorWindowTitle(title = "Blockly Playground", useTypeNameAsIconName = true)]
    [MenuItem(snapperPath + "Blockly Playground &b")]
    static void OpenBlocklyPlayground()
    {
        OpenWebViewEditorWindowTabs("Blockly Playground",
            "https://blockly-demo.appspot.com/static/tests/playground.html");
    }

    //[EditorWindowTitle(title = "Block Factory", icon = "Snap.png")]
    [MenuItem(snapperPath + "Block Factory &f")]
    static void OpenBlockFactory()
    {
        OpenWebViewEditorWindowTabs("Block Factory",
            "https://blockly-demo.appspot.com/static/demos/blockfactory/index.html");
    }

    [MenuItem(snapperPath + "Test Webpage &t")]
    static void OpenTestWebpage()
    {
        OpenWebViewEditorWindowTabs("Test Webpage",
            "http://output.jsbin.com/awenaq/3");
        //"https://clipboardjs.com/");
    }
    #endregion

    [MenuItem(snapperPath + "Save/Save as C# %#&c")]
    public static void SaveAsCSharp()
    {
        SaveToFile(".cs"); // File extension
    }

    [MenuItem(snapperPath + "Save/Save as JS %#&j")]
    public static void SaveAsJavaScript()
    {
        SaveToFile(".js"); // File extension
    }

    static void SaveToFile(string a_ex)
    {
        string location = Path.Combine(Application.dataPath, "Snapper/Code/");
        string filename = "SnapperCode";

        if (!Directory.Exists(location))
        {
            AssetDatabase.CreateFolder("Assets", "Snapper");
            AssetDatabase.CreateFolder("Assets/Snapper", "Code");
        } //location = Path.ChangeExtension(location, a_ex);//".js"); //".unitypackage"
        // Open Save File Panel to save filename at location.
        var path = EditorUtility.SaveFilePanel("Save code as " + a_ex, location, filename += EditorPrefs.GetInt("ScriptCount") + a_ex, a_ex.TrimStart('.'));
        //----------------------------------------------
        // Copy script from Unity templates.
        //----------------------------------------------
        if (path != "")
        {
            string templatePath = Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates");
            string result = "";
            if (a_ex != ".js")
            {
                if (a_ex != ".cs")
                {
                    throw new ArgumentOutOfRangeException();
                }
                result = Path.Combine(templatePath, "81-C# Script-NewBehaviourScript.cs.txt");
            }
            else
            {
                result = Path.Combine(templatePath, "82-Javascript-NewBehaviourScript.js.txt");
            }

            //----------------------------------------------
            // Replacing content within the script template.
            //----------------------------------------------
            string fileContent = File.ReadAllText(result);
            filename = Path.GetFileNameWithoutExtension(path);
            // Add components for physics
            if (EditorGUIUtility.systemCopyBuffer.Contains("Rigidbody"))
            {
                // "public class" for peace of mind.
                fileContent = fileContent.Replace("public class", 
                    "[RequireComponent(typeof(Rigidbody))]" + 
                    Environment.NewLine + "public class"); //, typeof(BoxCollider)
            }
            fileContent = fileContent.Replace("#SCRIPTNAME#", filename);
            // TODO: Insert variables based on ex.
            fileContent = fileContent.Replace("MonoBehaviour {", "MonoBehaviour {" +
                Environment.NewLine + Environment.NewLine + "\t// Variables" + Environment.NewLine);
            // Find the first "#NOTRIM#", this will fill in the Start Func.
            var regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape("#NOTRIM#"));
            var newText = regex.Replace(fileContent, "", 1); // Start Func.
            // Update - Replace 2nd NOTRIM with generated code.
            fileContent = newText.Replace("#NOTRIM#", EditorGUIUtility.systemCopyBuffer);
            //----------------------------------------------
            // Include our comments at the top of the file.
            fileContent = File.ReadAllText(GenerateCommentsToFile()) + fileContent;
            //----------------------------------------------
            UTF8Encoding encoding = new UTF8Encoding(true);
            File.WriteAllText(path, fileContent, encoding); //TODO: why not Encoding.UTF8?
            AssetDatabase.Refresh();
            LastSnapperScript = filename;
            //----------------------------------------------
            Debug.LogFormat("File saved at {0}.", path);
        }
    }

    /// <summary>
    /// e.g. File.ReadAllText(GenerateCommentsToFile())
    /// </summary>
    /// <returns>Path to where the comments will be temperarily stored.</returns>
    static string GenerateCommentsToFile()
    {
        string copyPath = FileUtil.GetUniqueTempPathInProject();
        using (StreamWriter comments = new StreamWriter(copyPath))
        {
            comments.WriteLine("/// ------------------------------------------------");
            comments.WriteLine("/// <summary>");
            comments.WriteLine("/// Author: " + SystemInfo.deviceName + " - Created with Snapper!");
            comments.WriteLine("/// Date: " + DateTime.Today.ToShortDateString());
            comments.WriteLine("/// ------------------------------------------------");
            comments.WriteLine("/// Brief: *Describe the script here*");
            comments.WriteLine("/// ");
            comments.WriteLine("/// viewed: ");
            comments.WriteLine("/// ");
            comments.WriteLine("/// </summary>");
            comments.WriteLine("/// ------------------------------------------------");
            comments.WriteLine("");
        } //File written
        return copyPath;
    }

    [MenuItem(snapperPath + "Publish/Steam Direct %#&p")]
    public static void PublishTo()//string a_platform, string a_cost)
    {
        string a_platform = "Steam Direct";
        string a_cost = "$100.00USD";
        if (EditorUtility.DisplayDialog("Ready to Publish?",
               string.Format("Are you sure you want to publish to {0} for {1}?", a_platform, a_cost)
               , "Heck Yeah!", "Not now"))
        {
            Debug.LogFormat("Your game {0} has successfully been published to {1}.", Application.productName, a_platform);
        }
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
                200, 450, 800, 600 //int minWidth, int minHeight, int maxWidth, int maxHeight
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