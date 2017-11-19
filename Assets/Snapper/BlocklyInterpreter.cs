///<summary>
/// Author: David Azouz
/// Date: 05/11/17
/// -------------------------------------------------
/// Brief: Calls Funcs from HTML
/// 
/// viewed: https://docs.unity3d.com/Manual/JSONSerialization.html
/// https://answers.unity.com/questions/290489/how-do-i-call-a-unity-function-from-html-using-jav.html
/// https://groups.google.com/forum/#!searchin/blockly/unity%7Csort:relevance/blockly/ZB4eL6U-4lI/qGz5sNvTAAAJ
/// https://forum.unity.com/threads/super-fast-javascript-interaction-on-webgl.382734/ 
/// 
/// https://github.com/MattRix/UnityDecompiled/blob/b4b209f8d1c93d66f560bf23c81bc0910cef177c/UnityEditor/UnityEditor.Web/PurchasingAccess.cs
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
//using System;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditor.Connect;
using UnityEditor.Purchasing;
using UnityEditor.Web;

public class BlocklyInterpreter : MonoBehaviour
{
    private const string kServiceName = "Purchasing";

    private const string kServiceDisplayName = "In App Purchasing";

    private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/purchasing";

    private const string kETagPath = "Assets/Plugins/UnityPurchasing/ETag";

    private const string kUnknownPackageETag = "unknown";

    private static readonly System.Uri kPackageUri;

    private bool m_InstallInProgress;

    //var unity = GetUnity();
    //unity.SendMessage("ObjectWithFunctionToCall", "FunctionName", paramsToSend);

    #region Blockly JS
    // BlocklyInterpreter Singleton 
    private static BlocklyInterpreter _instance;
    public static BlocklyInterpreter Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            // If we're null
            BlocklyInterpreter BlocklyInterpreterManager = FindObjectOfType<BlocklyInterpreter>();
            if (BlocklyInterpreterManager != null)
            {
                _instance = BlocklyInterpreterManager;
            }
            return _instance;
        }
    }

    /*// Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }*/

    

    public void ToXML(string a_output, object a_classname, object a_func)
    {
		// TODO: filepath?
        using (var fileStream = new FileStream(System.String.Format("Snapper_{0}.xml", a_classname), FileMode.OpenOrCreate))
        using (var streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.Write(JsonUtility.FromJson<string> (a_output));
        }
    }

    void ToJavaScript(string a_classname, string a_func)
    {
        SendMessage("Cube", "ToXML", SendMessageOptions.RequireReceiver);
    }

    void DebugLogTest(string a_var)
    {
        Debug.Log(a_var);
    }
    #endregion

    // Heavily inspired by how Unity install assets within the editor via the InstallUnityPackage function.
    public void InstallUnityPackage(string a_extension)
    {
        if (!this.m_InstallInProgress)
        {
            RemoteCertificateValidationCallback originalCallback = ServicePointManager.ServerCertificateValidationCallback;
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                ServicePointManager.ServerCertificateValidationCallback = ((object a, X509Certificate b, X509Chain c, SslPolicyErrors d) => true);
            }
            this.m_InstallInProgress = true;
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Assets/Snapper/Code")))
            {
                AssetDatabase.CreateFolder("Assets", "Snapper");
                AssetDatabase.CreateFolder("Assets/Snapper", "Code");
            }
            string location = Path.Combine(Application.dataPath, "Assets/Snapper/Code");// FileUtil.GetUniqueTempPathInProject();
            if(a_extension == "")
                a_extension = ".js";
            location = Path.ChangeExtension(location, a_extension); //".unitypackage"
            WebClient client = new WebClient();
            client.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs args)
            {
                EditorApplication.CallbackFunction handler = null;
                handler = delegate
                {
                    ServicePointManager.ServerCertificateValidationCallback = originalCallback;
                    EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Remove(EditorApplication.update, handler);
                    this.m_InstallInProgress = false;
                    if (args.Error == null)
                    {
                        this.SaveETag(client); //AssetDatabase.ImportPackage(location, false);
                        //Add Script downloaded to GO
                        BlocklyInterpreter go = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<BlocklyInterpreter>();
                        go.name = "Snapper";
                    }
                    else
                    {
                        Debug.LogWarning("Failed to download IAP package. Please check connectivity and retry.");
                        Debug.LogException(args.Error);
                    }
                };
                EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.update, handler);
            };
            client.DownloadFileAsync(BlocklyInterpreter.kPackageUri, location);
        }
    }

    private void SaveETag(WebClient client)
    {
        string text = client.ResponseHeaders.Get("ETag");
        if (text != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName("Assets/Plugins/UnityPurchasing/ETag"));
            File.WriteAllText("Assets/Plugins/UnityPurchasing/ETag", text);
        }
    }
}
