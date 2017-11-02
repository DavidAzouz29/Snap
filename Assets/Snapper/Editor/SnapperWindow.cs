///<summary>
/// Author: David Azouz
/// Date: 3/07/17
/// viewed: http://wiki.unity3d.com/index.php/FindGameObjects
/// https://docs.unity3d.com/Manual/RunningEditorCodeOnLaunch.html 
/// 
/// </summary>

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.VR;
using UnityEditor;
using System.Linq;
using Polyglot;

using UnityEditorInternal;
//using UnityEngine.Experimental.UIElements;
//using UnityEngine.Experimental.UIElements.StyleEnums;
//using UnityEditor.Experimental.UIElements;
using UnityEditor.ProjectWindowCallback;

[InitializeOnLoad]
public class SnapperWindow : EditorWindow
{
    public static int SelGridInt
    {
        get
        {
            return EditorPrefs.GetInt("SelGridEditorInt", 0);
        }
        set
        {
            if (value == SelGridInt)
            {
                return;
            }

            EditorPrefs.SetInt("SelGridEditorInt", value);

            switch(value)
            {
                case 0:
                    //EditorPrefs.SetBool("SelGridEditorInt", false);
                    break;
                default:
                    break;
            }
        }
    }
    
    string[] selStrings = new string[] {
        "Mesh", "Effects", "Physics", "Physics 2D", "Navigation", "Audio", "Video", "Rendering",
        "Layout", "Playables", "AR", "Misc.", "Analytics", "Scripts", "Event", "Network", "UI" };

    public bool showPosition = true;

    // GUI
    //private string sceneCanvasName = "";
    private Rect loadSceneUIPos;
    private Rect createCanvasUIPos;
    private int sideWindowWidth = 400;
    private int sideWindowStartHeight = 48; // headingsRect.height; //TODO: Recode (create box (like div), then set at headings height)

    public Rect headingsRect { get { return new Rect(0, 0, position.width, position.height); } }
    public Rect sideWindowRect { get { return new Rect(position.width - sideWindowWidth, sideWindowStartHeight, sideWindowWidth, position.height); } }
    public Rect canvasWindowRect { get { return new Rect(0, headingsRect.height, position.width - sideWindowWidth, position.height); } }

	/* private float hierarchyWidth
	{
		get
		{
			return (float)this.m_HorizontalSplitter.realSizes[0];
		}
	}

	private float contentWidth
	{
		get
		{
			return (float)this.m_HorizontalSplitter.realSizes[1];
		}
	}

	[SerializeField]
	private SplitterState m_HorizontalSplitter; */

	[System.NonSerialized]
	private Rect m_Position;

	Color color;

    static SnapperWindow()
    {
        EditorApplication.update += UpdateContents;
    }

    static void UpdateContents()
    {
        //Debug.Log("Updating");
        //GenerateHeadings();
    }

    [MenuItem("Snapper/Snapper Old &s")]
    public static void ShowWindow()
    {
        // Shows an instance of custom window, SnapperWindow
        SnapperWindow window = GetWindow<SnapperWindow>();
        // Loads an icon from an image stored at the specified path
        Texture icon = (Texture)EditorGUIUtility.Load("Icons/Snapper.png");
        // Create the instance of GUIContent to assign to the window. Gives the title "SnapperWindow" and the icon
        GUIContent titleContent = new GUIContent("Snapper", icon);
        window.minSize = new Vector2(650, 200);
        window.titleContent = titleContent;
    }

	/*void OnEnable()
    {
		base.hideFlags = HideFlags.HideAndDontSave;
		this.InitializeHorizontalSplitter();

		AnimEditor.s_AnimationWindows.Add(this);
		if (this.m_State == null)
		{
			this.m_State = (ScriptableObject.CreateInstance(typeof(AnimationWindowState)) as AnimationWindowState);
			this.m_State.hideFlags = HideFlags.HideAndDontSave;
			this.m_State.animEditor = this;
			this.InitializeHorizontalSplitter();
			this.InitializeClipSelection();
			this.InitializeDopeSheet();
			this.InitializeEvents();
			this.InitializeCurveEditor();
			this.InitializeOverlay();
		}
		this.InitializeNonserializedValues();
		this.m_State.timeArea = ((!this.m_State.showCurveEditor) ? this.m_DopeSheet : this.m_CurveEditor);
		this.m_DopeSheet.state = this.m_State;
		this.m_ClipPopup.state = this.m_State;
		this.m_Overlay.state = this.m_State;
		CurveEditor expr_E9 = this.m_CurveEditor;
		expr_E9.curvesUpdated = (CurveEditor.CallbackFunction)Delegate.Combine(expr_E9.curvesUpdated, new CurveEditor.CallbackFunction(this.SaveChangedCurvesFromCurveEditor));
		this.m_CurveEditor.OnEnable();*/

	/*// Shows an instance of custom window, SnapperWindow
	SnapperWindow window = GetWindow<SnapperWindow>("Snapper");
	// Loads an icon from an image stored at the specified path
	Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor Default Resources/Icons/Snapper.png");
	// Create the instance of GUIContent to assign to the window. Gives the title "SnapperWindow" and the icon
	GUIContent titleContent = new GUIContent("SnapperWindow", icon);
	window.titleContent = titleContent;* /
}*/

	#region GUI

	void OnGUI()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Snapper");
        UISystemProfilerApi.BeginSample(UISystemProfilerApi.SampleType.Render);
		//Event currentEvent = Event.current;

		GUISkin _editorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

		//GUILayout.BeginArea(headingsRect, GUI.skin.box);
		DrawMenu();
		DrawHeadings();
        //GUILayout.EndArea();

        GUILayout.BeginHorizontal("Box");//, GUILayout.Width(GetWindow<SnapperWindow>().maxSize.x - 8));
        //EditorGUILayout.Space();

        DrawBlocks();

        DrawGrid();
        GUILayout.EndHorizontal();

        //scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        DrawColoriser();
        DrawGenHeadings();

		EditorGUILayout.BeginVertical();
		// https://github.com/MattRix/UnityDecompiled/blob/82e03c823811032fd970ffc9a75246e95c626502/UnityEditor/UnityEditor/ProjectWindowUtil.cs
		//Or you can simply use the style itself when rendering a control:
		if (GUILayout.Button("", _editorSkin.GetStyle("Icon.Mute")))
		{
			Debug.Log("Icon");
		}
		if(GUILayout.Button("", _editorSkin.GetStyle("VisibilityToggle")))
		{
			Debug.Log("Vis Tog");
			//Debug.Log("Assets/Editor Default Resources/" + EditorResourcesUtility.iconsPath);
		}
		GUILayout.Label("", _editorSkin.GetStyle("MeTransPlayhead"));
		GUILayout.Label(_editorSkin.GetStyle("MeTransPlayhead").normal.background);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Button(EditorGUIUtility.IconContent("Animation.EventMarker", "|Add."));
		GUILayout.Button(EditorGUIUtility.IconContent("LookDevClose", "|Add."));
		GUILayout.Button(EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc."));
		//GUILayout.Button(EditorGUIUtility.LoadIcon("animationkeyframe"));
		GUILayout.Button(EditorGUIUtility.IconContent("Mirror", "|Add.")); 
		GUILayout.Button(EditorGUIUtility.IconContent("LookDevSingle1", "|Add."));
		GUILayout.Button(EditorGUIUtility.IconContent("LookDevSingle2", "|Add."));
		GUILayout.Button(EditorGUIUtility.IconContent("LookDevSideBySide", "|Add."));
		GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus", "|Add."));
		GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus", "|Minus."));
		EditorGUILayout.EndHorizontal();

		/*using (new EditorGUI.DisabledScope(false))
		{
			GUILayout.BeginHorizontal();// new GUILayoutOption[0]);
			SplitterGUILayout.BeginHorizontalSplit(this.m_HorizontalSplitter, new GUILayoutOption[0]);
			GUILayout.BeginVertical();// new GUILayoutOption[0]);
			GUILayout.BeginHorizontal();// EditorStyles.toolbarButton, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			SplitterGUILayout.EndHorizontalSplit();
			GUILayout.EndHorizontal();
		}*/

		EditorGUILayout.BeginHorizontal();
		GUILayout.Button(EditorGUIUtility.IconContent("GameObject Icon", "|Game Object."));
		//GUILayout.Button(EditorResourcesUtility.emptyFolderIconName, "|Game Object"));
		GUILayout.Button(EditorGUIUtility.IconContent("cs Script Icon", "|C# Script Icon."));
		GUILayout.Button(EditorGUIUtility.IconContent("AnimatorController Icon", "|Add."));
		GUILayout.Button(_editorSkin.GetStyle("MeTransPlayhead").normal.background);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		// TODO: remove later
		System.Type[] categories = new System.Type[]// UnityEngine.Object[]
        {   typeof(Mesh), typeof(ParticleSystem), typeof(Physics), typeof(Physics2D),
			typeof(Behaviour), typeof(NavMeshAgent), typeof(AudioSource), typeof(VideoClip),
			typeof(Renderer), typeof(LayoutElement), typeof(VRDevice), typeof(Network),
			typeof(Transform), typeof(RectTransform), typeof(AudioLowPassFilter)
		};

		for (int i = 0; i < categories.Length; i++)
		{
			if (i == 6)//categories.Length * 0.5f)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}
			if (GUILayout.Button(EditorGUIUtility.ObjectContent(null, categories[i]).image))
			{
				Debug.Log(categories[i].Name + " Image."); //Debug.Log(nameof(categories[i]) + " Image");
			}
		}
		/*if (GUILayout.Button(EditorGUIUtility.ObjectContent(null, typeof(Transform)).image))
		{
			Debug.Log("Transform Image");
		}*/
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		// Draw Side Window
		/*sideWindowWidth = System.Math.Min(600, System.Math.Max(200, (int)(position.width / 5)));
        GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
        DrawSideWindow();
        GUILayout.EndArea(); */

        UISystemProfilerApi.EndSample(UISystemProfilerApi.SampleType.Render);
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void DrawSideWindow()
    {
        //GUILayout.Label(new GUIContent("Node Editor (" + canvasCache.nodeCanvas.name + ")", "Opened Canvas path: " + canvasCache.openedCanvasPath), NodeEditorGUI.nodeLabelBold);

        //			EditorGUILayout.ObjectField ("Loaded Canvas", canvasCache.nodeCanvas, typeof(NodeCanvas), false);
        //			EditorGUILayout.ObjectField ("Loaded State", canvasCache.editorState, typeof(NodeEditorState), false);

        if (GUILayout.Button(new GUIContent("New Canvas", "Loads an Specified Empty CanvasType")))
        { }
    }
        /*    NodeEditorFramework.Utilities.GenericMenu menu = new NodeEditorFramework.Utilities.GenericMenu();
            NodeCanvasManager.FillCanvasTypeMenu(ref menu, canvasCache.NewNodeCanvas);
            menu.Show(createCanvasUIPos.position, createCanvasUIPos.width);
        }
        if (Event.current.type == EventType.Repaint)
        {
            Rect popupPos = GUILayoutUtility.GetLastRect();
            createCanvasUIPos = new Rect(popupPos.x + 2, popupPos.yMax + 2, popupPos.width - 4, 0);
        }

        GUILayout.Space(6);

        if (GUILayout.Button(new GUIContent("Save Canvas", "Saves the Canvas to a Canvas Save File in the Assets Folder")))
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Node Canvas", "Node Canvas", "asset", "", NodeEditor.editorPath + "Resources/Saves/");
            if (!string.IsNullOrEmpty(path))
                canvasCache.SaveNodeCanvas(path);
        }

        if (GUILayout.Button(new GUIContent("Load Canvas", "Loads the Canvas from a Canvas Save File in the Assets Folder")))
        {
            string path = EditorUtility.OpenFilePanel("Load Node Canvas", NodeEditor.editorPath + "Resources/Saves/", "asset");
            if (!path.Contains(Application.dataPath))
            {
                if (!string.IsNullOrEmpty(path))
                    ShowNotification(new GUIContent("You should select an asset inside your project folder!"));
            }
            else
                canvasCache.LoadNodeCanvas(path);
        }

        GUILayout.Space(6);

        GUILayout.BeginHorizontal();
        sceneCanvasName = GUILayout.TextField(sceneCanvasName, GUILayout.ExpandWidth(true));
        if (GUILayout.Button(new GUIContent("Save to Scene", "Saves the Canvas to the Scene"), GUILayout.ExpandWidth(false)))
            canvasCache.SaveSceneNodeCanvas(sceneCanvasName);
        GUILayout.EndHorizontal();

        if (GUILayout.Button(new GUIContent("Load from Scene", "Loads the Canvas from the Scene")))
        {
            NodeEditorFramework.Utilities.GenericMenu menu = new NodeEditorFramework.Utilities.GenericMenu();
            foreach (string sceneSave in NodeEditorSaveManager.GetSceneSaves())
                menu.AddItem(new GUIContent(sceneSave), false, LoadSceneCanvasCallback, (object)sceneSave);
            menu.Show(loadSceneUIPos.position, loadSceneUIPos.width);
        }
        if (Event.current.type == EventType.Repaint)
        {
            Rect popupPos = GUILayoutUtility.GetLastRect();
            loadSceneUIPos = new Rect(popupPos.x + 2, popupPos.yMax + 2, popupPos.width - 4, 0);
        }

        GUILayout.Space(6);

        if (GUILayout.Button(new GUIContent("Recalculate All", "Initiates complete recalculate. Usually does not need to be triggered manually.")))
            NodeEditor.RecalculateAll(canvasCache.nodeCanvas);

        if (GUILayout.Button("Force Re-Init"))
            NodeEditor.ReInit(true);

        NodeEditorGUI.knobSize = EditorGUILayout.IntSlider(new GUIContent("Handle Size", "The size of the Node Input/Output handles"), NodeEditorGUI.knobSize, 12, 20);
        canvasCache.editorState.zoom = EditorGUILayout.Slider(new GUIContent("Zoom", "Use the Mousewheel. Seriously."), canvasCache.editorState.zoom, 0.6f, 2);

        if (canvasCache.editorState.selectedNode != null && Event.current.type != EventType.Ignore)
            canvasCache.editorState.selectedNode.DrawNodePropertyEditor();
    }

    public void LoadSceneCanvasCallback(object canvas)
    {
        canvasCache.LoadSceneNodeCanvas((string)canvas);
    } */

    #endregion


    void OnInspectorUpdate()
    {
        Repaint();
    }

	void DrawMenu()
	{
		string[] menuItems = new string[] { "File", Localization.Get("MENU_LABEL_CONFIRM_BUTTON") };
		EditorGUILayout.BeginHorizontal("Box");
		GUILayout.Toolbar(SelGridInt, menuItems, EditorStyles.toolbar);

		GUIContent currLangButtonContent = new GUIContent("\u25cc", "English"); //1F310
		//prevLangButtonContent = new GUIContent("<", "Prev Lang"),
		//nextLangButtonContent = new GUIContent(">", "Next Lang");

		GUILayoutOption miniButtonWidth = GUILayout.Width(50f);

		var loc = Localization.Instance;
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.PrevKey", "|Prev Lang."), EditorStyles.miniButtonLeft, miniButtonWidth))
		{
			loc.SelectLanguage(loc.SelectedLanguage - 1);
		}
		if (GUILayout.Button(currLangButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
		{
			loc.SelectLanguage(Language.English);
		}
		if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.NextKey", "|Next Lang."), EditorStyles.miniButtonRight, miniButtonWidth))
		{
			loc.SelectLanguage(loc.SelectedLanguage + 1);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndHorizontal();
	}

    void DrawHeadings()
    {
        // Allows for nice layout for headings
        int buffer = 0;
        buffer = (selStrings.Length % 2 == 0) ? 0 : 1;


        EditorGUILayout.BeginVertical("Box");
        SelGridInt = GUILayout.SelectionGrid(SelGridInt, selStrings,
            selStrings.Length / 2 + buffer, 
            EditorStyles.toolbarButton, GUILayout.MinWidth(630));
        EditorGUILayout.EndVertical();

        GUILayout.Label(selStrings[SelGridInt], EditorStyles.boldLabel);
    }

    public string status = "Select a GameObject";
    void DrawBlocks()
    {
        GUILayout.BeginVertical("Box");
        showPosition = EditorGUILayout.Foldout(showPosition, status);// new Rect(3, 103, position.width - 6, 15), showPosition, status);
        if (showPosition)
            if (Selection.activeTransform)
            {
                Selection.activeTransform.position = EditorGUILayout.Vector3Field("Position", Selection.activeTransform.position);
                status = Selection.activeTransform.name;
            }

        if (!Selection.activeTransform)
        {
            status = "Select a GameObject";
            showPosition = false;
        }
        GUILayout.EndVertical();
    }

    void Callback(object obj)
    {
        Debug.Log("Selected: " + obj);
    }

    void DrawGrid()
    {
        //Event currentEvent = Event.current;
        //Rect contextRect = new Rect(10, 10, 100, 100);
        EditorGUILayout.Space();
        GUILayout.BeginVertical("Box");
        //EditorGUILayout.DrawRect(contextRect, Color.gray);
        /*BeginWindows();

        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // Now create the menu, add items and show it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Block"), false, CreateBlock, "item 1");
                menu.AddItem(new GUIContent("MenuItem2"), false, Callback, "item 2");
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, Callback, "item 3");
                menu.ShowAsContext();
                currentEvent.Use();
            }
        }
        EndWindows(); */

        GUILayout.EndVertical();
    }

    void DrawColoriser()
    {
        GUILayout.Label("Color the selected objects!", EditorStyles.boldLabel);

        color = EditorGUILayout.ColorField("Color", color);

        if (GUILayout.Button("Snapper!"))
        {
            Colorize();
        }
    }

    void DrawGenHeadings()
    {
        GUILayout.Label("Generate Headings", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate \nHeadings!"))
        {
            GenerateHeadingsDebug();
        }
    }

    void CreateBlock()
    {

    }

    /// <summary>
    /// https://docs.unity3d.com/Manual/UsingComponents.html
    /// Gathers each Category from the component browser
    /// </summary>
    void GenerateHeadings()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(Component));
        var assemblyMesh = System.Reflection.Assembly.GetAssembly(typeof(Mesh));

    }

    private string[] category;
    private string[] _componentNames;
    private System.Type[] _componentTypes;
    private void GetComponentTypes()
    {
        // get types
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(Component));
        _componentTypes = assembly.GetTypes().Where(t => typeof(Component).IsAssignableFrom(t) && t.IsPublic && t != typeof(Component)).ToArray();

        // get names
        _componentNames = new string[_componentTypes.Length];
        for (int i = 0; i < _componentTypes.Length; i++)
        {
            _componentNames[i] = GetComponentName(_componentTypes[i]);
        }

        // add abstract self to path
        for (int i = 0; i < _componentNames.Length; i++)
        {
            string name = _componentNames[i];

            // is this name used as a path in another name?
            bool contained = false;
            for (int j = 0; j < _componentNames.Length; j++)
            {
                if (i == j) continue;

                if (_componentNames[j].Contains(name + "/"))
                {
                    contained = true;
                    break;
                }
            }

            string category = "";
            if (_componentTypes[i].Namespace != null && _componentTypes[i].Namespace != "UnityEngine")
            {
                category = _componentTypes[i].Namespace.Substring(0, 12) + "/";
            }
            else if (i > 1 && i <= 15)
            {
                category = "Rendering/";
            }
            else if (i > 24 && i <= 33)
            {
                category = "Physics/";
            }
            else
            {
                //string contains = _componentNames[i];
                System.Collections.Generic.List<string> subStrings = new System.Collections.Generic.List<string>
                { "Audio", "2D" };
                switch (subStrings.FirstOrDefault(_componentNames[i].Contains))
                {
                    case "Audio":
                        {
                            category = "Audio/";
                            break;
                        }
                    case "2D":
                        {
                            category = "Physics/Physics2D/";
                            break;
                        }

                    default:
                        {
                            category = "";
                            break;
                        }
                }
            }

            // add it to its own path
            if (contained)
            {
                _componentNames[i] = category + name + '/' + name.Split('/').Last();
                // TODO: category name here

            }
        }
    }

    private string GetComponentName(System.Type type)
    {
        string name = type.Name;
        while (type.BaseType != null && type.BaseType != typeof(Component) && type.BaseType != typeof(Behaviour))
        {
            name = type.BaseType.Name + "/" + name;
            type = type.BaseType;
        }

        return name;
    }

    private string GetComponentNamespace(System.Type type)
    {
        string @namespace = (type.Namespace != null) ? type.Namespace : "";
        while (type.BaseType != null && type.BaseType != typeof(Component) && type.BaseType != typeof(Behaviour))
        {
            @namespace = type.BaseType.Namespace + "/" + @namespace;
            type = type.BaseType;
        }

        return @namespace;
    }

    void Colorize()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.color = color;
            }
        }
    }

    void GenerateHeadingsDebug()
    {
        System.Type[] categories = new System.Type[]// UnityEngine.Object[]
        {   typeof(Mesh), typeof(ParticleSystem), typeof(Physics), typeof(Physics2D),
            typeof(Behaviour), typeof(NavMeshAgent), typeof(AudioSource), typeof(VideoClip),
            typeof(Renderer), typeof(LayoutElement), typeof(VRDevice), typeof(Network),
            typeof(Transform), typeof(RectTransform), typeof(AudioLowPassFilter)};

        for (int i = 0; i < categories.Length; i++)
        {
            Debug.Log(categories[i].Name.ToString());

            if (categories[i].ReflectedType != null)
                Debug.Log(categories[i].ReflectedType.ToString());

            if (categories[i].ReflectedType == typeof(Component))
            {
                Debug.Log(categories[i].ReflectedType.ToString());
                Debug.Log("Component");
            }
            if (categories[i].ReflectedType == typeof(Mesh))
            {
                Debug.Log("Mesh");
            }


            string sNamespace = categories[i].Namespace;
            // If we have a namespace...
            if(sNamespace.Contains("."))
            {
                sNamespace.Split('.').Last();
                //... place Block under that namespace's category heading
                switch (sNamespace) //Substring(0, 12)
                {
                    case "AI":
                        {
                            // Navigation
                            break;
                        }

                    case "Animations":
                        {
                            /*"Audio"
           "Events"
           "Networking"
           "Rendering"
           "Scripting"
           "Timeline"
           "UI"
           "Video"
           "VR" */
                            break;
                                }
                    default:
                        {
                            // Misc.
                            break;
                        }
                }
            }
            if (categories[i].ReflectedType == typeof(Object))
            {
                Debug.Log(categories[i].ReflectedType.ToString());
                Debug.Log("Object");
            }
            GetComponentTypes();
        }
    }

	/*private void SynchronizeLayout()
	{
		this.m_HorizontalSplitter.realSizes[1] = (int)Mathf.Min(this.m_Position.width - (float)this.m_HorizontalSplitter.realSizes[0], (float)this.m_HorizontalSplitter.realSizes[1]);
		/*if (this.selectedItem != null && this.selectedItem.animationClip != null)
		{
			this.m_State.frameRate = this.selectedItem.animationClip.frameRate;
		}
		else
		{
			this.m_State.frameRate = 60f;
		}* /
}

private void Initialize()
	{
		//AnimationWindowStyles.Initialize();
		//this.InitializeHierarchy();
		this.m_Position = position;
		this.m_HorizontalSplitter.realSizes[0] = 300;
		this.m_HorizontalSplitter.realSizes[1] = (int)Mathf.Max(this.m_Position.width - 300f, 300f);
		//this.m_DopeSheet.rect = new Rect(0f, 0f, this.contentWidth, 100f);
		//this.m_Initialized = true;
	}

	private void InitializeHorizontalSplitter()
	{
		this.m_HorizontalSplitter = new SplitterState(new float[]
		{
				300f,
				900f
		}, new int[]
		{
				300,
				300
		}, null);
		this.m_HorizontalSplitter.realSizes[0] = 300;
		this.m_HorizontalSplitter.realSizes[1] = 300;
	}

	private void OverlayEventOnGUI()
	{
		if (!this.m_State.animatorIsOptimized)
		{
			if (!this.m_State.disabled)
			{
				Rect position = new Rect(this.hierarchyWidth - 1f, 0f, this.contentWidth - 15f, this.m_Position.height - 15f);
				GUI.BeginGroup(position);
				this.m_Overlay.HandleEvents();
				GUI.EndGroup();
			}
		}
	}

	private void OverlayOnGUI(Rect contentRect)
	{
		if (!this.m_State.animatorIsOptimized)
		{
			if (!this.m_State.disabled)
			{
				if (Event.current.type == EventType.Repaint)
				{
					Rect rect = new Rect(contentRect.xMin, contentRect.yMin, contentRect.width - 15f, contentRect.height - 15f);
					Rect position = new Rect(this.hierarchyWidth - 1f, 0f, this.contentWidth - 15f, this.m_Position.height - 15f);
					GUI.BeginGroup(position);
					Rect rect2 = new Rect(0f, 0f, position.width, position.height);
					Rect contentRect2 = rect;
					contentRect2.position -= position.min;
					this.m_Overlay.OnGUI(rect2, contentRect2);
					GUI.EndGroup();
				}
			}
		}
	}
	private void RenderEventTooltip()
	{
		this.m_Events.DrawInstantTooltip(this.m_Position);
	}*/
}