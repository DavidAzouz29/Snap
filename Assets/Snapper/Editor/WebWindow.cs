///<summary>
/// Author: David Azouz
/// Date: 31/10/17
/// viewed: https://gist.github.com/drawcode/4596778 31/10/17
/// https://stackoverflow.com/questions/45616562/use-the-dynamic-keyword-net-4-6-feature-in-unity
/// https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor.Web/WebViewEditorWindowTabs.cs
/// </summary>

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class WebWindow : EditorWindow {
  
	static Rect windowRect = new Rect(100,100,800,600);
	static BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
	static StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;
	
	object webView;
	dynamic x;
	Type webViewType;
	MethodInfo doGUIMethod;
	MethodInfo loadURLMethod;
	MethodInfo focusMethod;
	MethodInfo unFocusMethod;
	
	Vector2 resizeStartPos;
	Rect resizeStartWindowSize;
	MethodInfo dockedGetterMethod;

    string urlText = "http://www.google.com"; // https://blockly-demo.appspot.com/static/tests/playground.html";
	

	[MenuItem ("Tools/Web Window %#w")]
    static void Load() {
        WebWindow window = WebWindow.GetWindow<WebWindow>();
		//window.Show();
		window.Init();
    }
	
	void Init() {
		//Set window rect
		this.position = windowRect;
		//Get WebView type
		webViewType = GetTypeFromAllAssemblies("WebView");
		//Init web view
		InitWebView();
		//Get docked property getter MethodInfo
		dockedGetterMethod = typeof(EditorWindow).GetProperty("docked", fullBinding).GetGetMethod(true);
	}
	
	private void InitWebView() {
		webView = ScriptableObject.CreateInstance(webViewType);
		webViewType.GetMethod("InitWebView").Invoke(webView, new object[] {(int)position.width,(int)position.height,false});
		webViewType.GetMethod("set_hideFlags").Invoke(webView, new object[] {13});
		
		loadURLMethod = webViewType.GetMethod("LoadURL");
		loadURLMethod.Invoke(webView, new object[] {urlText});
		webViewType.GetMethod("SetDelegateObject").Invoke(webView, new object[] {this});
		
		doGUIMethod = webViewType.GetMethod("DoGUI");
		focusMethod = webViewType.GetMethod("Focus");
		unFocusMethod = webViewType.GetMethod("UnFocus");
		
		this.wantsMouseMove = true;
	}
	
	void OnGUI() {
		if(GUI.GetNameOfFocusedControl().Equals("urlfield"))
			unFocusMethod.Invoke(webView, null);
		
		bool isDocked = (bool)(dockedGetterMethod.Invoke(this, null));
		Rect webViewRect = new Rect(0,20,position.width,position.height - ((isDocked) ? 20 : 40));
		if(Event.current.isMouse && Event.current.type == EventType.MouseDown && webViewRect.Contains(Event.current.mousePosition)) {
			GUI.FocusControl("hidden");
			focusMethod.Invoke(webView, null);
		}
		
		//Hidden, disabled, button for taking focus away from urlfield
		GUI.enabled = false;
		GUI.SetNextControlName("hidden");
		GUI.Button(new Rect(-20,-20,5,5), string.Empty);
		GUI.enabled = true;
		
		//URL Label
		GUI.Label(new Rect(0,0,30,20), "URL:");
		
		//URL text field
		GUI.SetNextControlName("urlfield"); 
		urlText = GUI.TextField(new Rect(30,0, position.width-30, 20), urlText);
		
		//Focus on web view if return is pressed in URL field
		if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl().Equals("urlfield")) {
			loadURLMethod.Invoke(webView, new object[] {urlText});
			GUI.FocusControl("hidden");
			focusMethod.Invoke(webView, null);
		}
		
		//Web view
		if(webView != null)
			doGUIMethod.Invoke(webView, new object[] {webViewRect});
	}
	
	private void OnWebViewDirty() {
		this.Repaint();
	}
	
	public static Type GetTypeFromAllAssemblies(string typeName) {
		Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		foreach(Assembly assembly in assemblies) {
			Type[] types = assembly.GetTypes();
			foreach(Type type in types) {
				if(type.Name.Equals(typeName, ignoreCase) || type.Name.Contains('+' + typeName)) //+ check for inline classes
					return type;
			}
		}
		return null;
	}
	
	void OnDestroy() {
		//Destroy web view
		webViewType.GetMethod("DestroyWebView", fullBinding).Invoke(webView, null);
	}
}
