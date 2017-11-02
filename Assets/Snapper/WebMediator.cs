// WebView-Unity mediator plugin script.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebMediator : MonoBehaviour
{
    private static WebMediator _instance;
    /*public static WebMediator Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            // If we're null
            return Install();
        }
    } */

    private static bool isClearCache;

    private string lastRequestedUrl;
    private bool loadRequest;
    private bool visibility;
    private int leftMargin;
    private int topMargin;
    private int rightMargin;
    private int bottomMargin;

    // Install the plugin.
    // Call this at least once before using the plugin.
    public static WebMediator Install()
    {
        if (_instance == null)
        {
            GameObject master = new GameObject("WebMediator");
            DontDestroyOnLoad(master);
            _instance = master.AddComponent<WebMediator>();
            InstallPlatform();
            return _instance;
        }
        return null;
    }

    // Set margins around the web view.
    public static void SetMargin(int left, int top, int right, int bottom)
    {
        _instance.leftMargin = left;
        _instance.topMargin = top;
        _instance.rightMargin = right;
        _instance.bottomMargin = bottom;
        ApplyMarginsPlatform();
    }

    // Visibility functions.
    public static void Show()
    {
        _instance.visibility = true;
    }
    public static void Hide()
    {
        _instance.visibility = false;
    }
    public static bool IsVisible()
    {
        return _instance.visibility;
    }

    public static void SetClearCache()
    {
        isClearCache = true;
    }

    public static void SetCache()
    {
        isClearCache = false;
    }

    // Load the page at the URL.
    public static void LoadUrl(string url)
    {
        _instance.lastRequestedUrl = url;
        _instance.loadRequest = true;
    }

    void Update()
    {
        UpdatePlatform();
        _instance.loadRequest = false;
    }

#if UNITY_EDITOR

    // Unity Editor implementation.

    private static void InstallPlatform() { }
    private static void UpdatePlatform() { }
    private static void ApplyMarginsPlatform() { }
    public static WebMediatorMessage PollMessage() { return null; }
    public static void MakeTransparentWebViewBackground() { }

#endif
}

// Message container class.
public class WebMediatorMessage
{
    public string path;      // Message path
    public Hashtable args;   // Argument table

    public void WebMediatorMessages(string rawMessage)
    {
        // Retrieve a path.
        var split = rawMessage.Split("?"[0]);
        path = split[0];
        // Parse arguments.
        args = new Hashtable();
        if (split.Length > 1)
        {
            foreach (var pair in split[1].Split("&"[0]))
            {
                var elems = pair.Split("="[0]);
                args[elems[0]] = WWW.UnEscapeURL(elems[1]);
            }
        }
    }
}