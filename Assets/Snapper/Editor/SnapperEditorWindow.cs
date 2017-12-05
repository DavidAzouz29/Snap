///<summary>
/// Author: David Azouz
/// Date: 02/12/17
/// viewed: http://wiki.unity3d.com/index.php/FindGameObjects
/// https://docs.unity3d.com/Manual/RunningEditorCodeOnLaunch.html 
/// https://docs.unity3d.com/ScriptReference/EditorGUILayout.EnumPopup.html
/// 
/// </summary>

using UnityEngine;
using UnityEditor;

public enum E_PRIMITIVE_STATE
{
    CUBE = 0,
    SPHERE = 1,
    CAPSULE = 2,
    CYLINDER = 3,
    PLANE = 4,
    QUAD = 5

    //E_PRIMITIVE_STATE_COUNT
}

[InitializeOnLoad]
public class SnapperEditorWindow : EditorWindow
{
    // GUI
    const float fPadding = 4f;
    const float fButtonWidth  = 128f; // Multiple of 8.
    const float fButtonHeight = fButtonWidth * 0.5f; // Multiple of 8.
    const float fWinMinHeight = fButtonHeight + (fPadding * 5f);
    GUILayoutOption buttonSmallWidth = GUILayout.Width(fButtonWidth * 0.5f);

    public E_PRIMITIVE_STATE op;
    /*static SnapperEditorWindow()
    {
        EditorApplication.update += UpdateContents;
    }

    static void UpdateContents()
    {
        //Debug.Log("Updating");
        //GenerateHeadings();
    }*/

    [MenuItem(BlocklyPlayground.snapperPath + "Snapper Quick Tools #q", priority = 1)]
    public static void ShowWindow()
    {
        // Shows an instance of custom window, SnapperEditorWindow
        SnapperEditorWindow window = GetWindow<SnapperEditorWindow>();
        // Loads an icon from an image stored at the specified path
        Texture icon = (Texture)EditorGUIUtility.Load("Icons/Snapper.png");
        // Create the instance of GUIContent to assign to the window. Gives the title "SnapperEditorWindow" and the icon
        GUIContent titleContent = new GUIContent("Snapper QT", icon);
        window.minSize = new Vector2(350f,  fWinMinHeight);
        window.maxSize = new Vector2(4004f, fWinMinHeight);
        window.titleContent = titleContent;
    }

	#region GUI

	void OnGUI()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Snapper Quick Settings");
        UISystemProfilerApi.BeginSample(UISystemProfilerApi.SampleType.Render);

        EditorGUILayout.BeginHorizontal("Box", GUILayout.Height(fButtonHeight));
        // Small Buttons
        EditorGUILayout.BeginVertical(buttonSmallWidth);
        if (GUILayout.Button((Texture)EditorGUIUtility.Load("Icons/Snapper.png")))//, "|Open Snapper Editor."))
        {
            BlocklyPlayground.OpenSnapperEditor();
        }

        if (GUILayout.Button(EditorGUIUtility.IconContent("LookDevSideBySide", "|Publish.")))
        {
            BlocklyPlayground.PublishTo();
        }

        if (GUILayout.Button(EditorGUIUtility.IconContent("renderdoc", "|Me.")))
        {
            Debug.Log(Application.companyName);
        }
        EditorGUILayout.EndVertical();

        // Big Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.IconContent("cs Script Icon", "|Save as C# Script.")))
        {
            BlocklyPlayground.SaveAsCSharp();
        }

        if (GUILayout.Button(EditorGUIUtility.IconContent("js Script Icon", "|Save as JS.")))
        {
            BlocklyPlayground.SaveAsJavaScript();
        }

        // Create primitive with our most recent Snapper script.
        EditorGUILayout.BeginVertical();
        op = (E_PRIMITIVE_STATE)EditorGUILayout.EnumPopup("Primitive to create:", op);

        if (GUILayout.Button(EditorGUIUtility.IconContent("GameObject Icon", "|Adds the latest Snapper script to a primitive."), GUILayout.Height(fButtonHeight - fPadding * 4f)))
        {
            InstantiatePrimitive(op);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        UISystemProfilerApi.EndSample(UISystemProfilerApi.SampleType.Render);
        UnityEngine.Profiling.Profiler.EndSample();
    }

    void InstantiatePrimitive(E_PRIMITIVE_STATE op)
    {
        string component = BlocklyPlayground.LastSnapperScript != null ? BlocklyPlayground.LastSnapperScript : "PlayerMovement";
        
        switch (op)
        {
            case E_PRIMITIVE_STATE.CUBE:
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = Vector3.zero;
                cube.name = "PlayerMovement"; //TODO: RECODE: HACK HACK OMG HACK. Can't add component via string anymore :/
                cube.AddComponent<PlayerMovement>();// (System.Type.GetType(component)); https://blogs.unity3d.com/2015/01/21/addcomponentstring-api-removal-in-unity-5-0/#comment-210882
                break;
            case E_PRIMITIVE_STATE.SPHERE:
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = Vector3.zero;
                sphere.name = component;
                sphere.AddComponent(System.Type.GetType(component)); //AddComponent(Type.GetType(scriptName + “, ” + assemblyName));
                break;
            case E_PRIMITIVE_STATE.CAPSULE:
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.transform.position = Vector3.zero;
                capsule.name = component;
                capsule.AddComponent(System.Type.GetType(component));
                break;
            case E_PRIMITIVE_STATE.CYLINDER:
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.transform.position = Vector3.zero;
                cylinder.name = component;
                cylinder.AddComponent(System.Type.GetType(component));
                break;
            case E_PRIMITIVE_STATE.PLANE:
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.position = Vector3.zero;
                plane.name = component;
                plane.AddComponent(System.Type.GetType(component));
                break;
            case E_PRIMITIVE_STATE.QUAD:
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Vector3.zero;
                quad.name = component;
                quad.AddComponent(System.Type.GetType(component));
                break;
            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
    }
    #endregion


    void OnInspectorUpdate()
    {
        Repaint();
    }
}