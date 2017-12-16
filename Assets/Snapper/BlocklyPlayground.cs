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
//using System.Linq;

public class BlocklyPlayground : ScriptableObject
{

    static object webView;
    static Type webViewType;

    // LastSnapperComponent for GO Primitive
    public static string LastSnapperComponent
    {
        get
        {
            if(EditorPrefs.HasKey("SnapperLastComponent"))
            {
                return EditorPrefs.GetString("SnapperLastComponent");
            }
            // If we're null
            else
            {
                return EditorPrefs.GetString("SnapperLastComponent", "PlayerMovement");
            }
        }
        private set
        {
            if (value == LastSnapperComponent)
            {
                return;
            }

            EditorPrefs.SetString("SnapperLastComponent", value);
        }
    }

    #region SnapperWindows
    public const string snapperPath = "Window/Snapper/";

    //[EditorWindowTitle(title = "Snapper Editor", useTypeNameAsIconName = true)]
    [MenuItem(snapperPath + "Snapper Editor &s", priority = 2)]
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
    [MenuItem(snapperPath + "Scratch Playground &#s")]
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
            EditorPrefs.SetInt("ScriptCount", EditorPrefs.GetInt("ScriptCount") + 1);
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
            // If the script template hasn't been altered...
            if (!fileContent.Contains("\r\n"))
            {
                // Cross Platform New Line Character.
                fileContent = fileContent.Replace("\n", Environment.NewLine);
            }
            fileContent = fileContent.Replace("#SCRIPTNAME#", filename);
            // Variables
            string start = "";
            fileContent = GenerateVariables(fileContent, ref start);
            // Find the first "#NOTRIM#", this will fill in the Start Func.
            var regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape("#NOTRIM#"));
            var newText = regex.Replace(fileContent, start, 1); // Start Func.
            // Update - Replace 2nd NOTRIM with generated code.
            // If our generated code contains come kind of function e.g. void *xxx*() where *xxx* can be *Update*...
            if(EditorGUIUtility.systemCopyBuffer.Contains("void ") &&
                EditorGUIUtility.systemCopyBuffer.Contains("()") &&
                !EditorGUIUtility.systemCopyBuffer.Contains("void Start()"))
            {
                //... make room by getting rid of the Update func entirely within the template.
                //... although we need somewhere to paste our clipboard.
                newText = newText.Replace(string.Format("" +
                    "// Update is called once per frame{0}\tvoid Update () {{{0}\t\t#NOTRIM#{0}\t}}"
                    , Environment.NewLine), "#NOTRIM#");
            }
            string tabbedClipboard = EditorGUIUtility.systemCopyBuffer.Replace(Environment.NewLine, Environment.NewLine + "\t\t");
            fileContent = newText.Replace("#NOTRIM#", tabbedClipboard);
            //RECODE: Although rb is a special case, I don't like this... camelCase?
            fileContent = fileContent.Replace("rigidbody", "rb"); 
            fileContent = fileContent.Replace("meshrenderer", "meshRenderer");

            //----------------------------------------------
            // Include our comments at the top of the file.
            fileContent = File.ReadAllText(GenerateCommentsToFile()) + fileContent;
            //----------------------------------------------
            UTF8Encoding encoding = new UTF8Encoding(true);
            File.WriteAllText(path, fileContent, encoding); //TODO: why not Encoding.UTF8?
            AssetDatabase.Refresh();
            LastSnapperComponent = filename;
            //----------------------------------------------
            Debug.LogFormat("File saved at {0}.", path);
        }
    }

    /// <summary>
    /// e.g. fileContent = GenerateVariables(fileContent);
    /// </summary>
    /// <param name="a_fileContent">fileContent</param>
    /// <returns>Variables for Variable Declaration and Start Func.</returns>
    static string GenerateVariables(string a_fileContent, ref string start)
    {
        string variables = "";

        // Add components for physics
        // TODO: find a way to populate this array.
        string[] components = new string[] { "Rigidbody", "MeshRenderer" }; //, "BoxCollider"
        System.Collections.Generic.List<string> args = new System.Collections.Generic.List<string>(3);
        for (int i = 0; i < components.Length; i++)
        {
            // If our generated code contains any of our desired components.
            if (EditorGUIUtility.systemCopyBuffer.Contains(components[i].ToLower()))
            {
                args.Add(components[i]);
                // TODO: find a better way to do variables (naming).
                variables += string.Format("\t{0} {1};{2}", components[i], //RECODE: ToPascalCase? what about BoxCollider?
                    components[i].ToLower(), Environment.NewLine);
                start += string.Format("{0} = GetComponent<{1}>();{2}\t\t", components[i].ToLower(), components[i], Environment.NewLine);
            }
        }
        if (args.Count != 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[RequireComponent(");
            bool flag = true;
            // RequireComponent can only support up to three requirements.
            for (int i = 0; i < args.Count && i < 3; i++)
            {
                object obj = args[i];
                if (!flag)
                {
                    stringBuilder.Append(", ");
                }
                bool flag2 = obj is string;
                if (flag2)
                {
                    stringBuilder.Append("typeof(");
                }
                stringBuilder.Append(obj);
                if (flag2)
                {
                    stringBuilder.Append(')');
                }
                flag = false;
            }
            stringBuilder.AppendLine(")]");
            // "public class" for peace of mind.
            stringBuilder.Append("public class");
            a_fileContent = a_fileContent.Replace("public class", stringBuilder.ToString());
        }

        // Adds any other variables we'd like to the variable declaration. 
        //TODO: Populate this array somehow. List? Also, Insert variables based on ex?
        string[] vars = new string[] { "public Color color;" }; //{ "float foo = 1.0f;", "int bar = 0;" };
        for (int i = 0; i < vars.Length; i++)
        {
            variables += string.Format("\t{0}" + Environment.NewLine, vars[i]);
        }
        a_fileContent = a_fileContent.Replace("MonoBehaviour {", "MonoBehaviour {" +
            Environment.NewLine + Environment.NewLine + "\t#region Variables" +
            Environment.NewLine + variables +
            Environment.NewLine + "\t#endregion" + Environment.NewLine);
        return a_fileContent;
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
}