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
using System.Collections.Generic;

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
    static string lastSnapperComponent = "";

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
        lastSnapperComponent = BlocklyPlayground.LastSnapperComponent;
    }

	#region GUI

	void OnGUI()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Snapper Quick Settings");
        UISystemProfilerApi.BeginSample(UISystemProfilerApi.SampleType.Render);

        EditorGUILayout.BeginHorizontal("Box", GUILayout.Height(fButtonHeight));
        #region Small Buttons
        EditorGUILayout.BeginVertical(buttonSmallWidth);
        if (GUILayout.Button((Texture)EditorGUIUtility.Load("Icons/Snapper.png")))//, "|Open Snapper Editor."))
        {
            BlocklyPlayground.OpenSnapperEditor();
        }
        // Two Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("Icon.Activation").normal.background))
        {
            AssessmentWizard.CreateWizard();
        }

        if (GUILayout.Button(EditorGUIUtility.IconContent("LookDevSideBySide", "|Publish.")))
        {
            BlocklyPlayground.PublishTo();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button(EditorGUIUtility.IconContent("renderdoc", "|Me.")))
        {
            Debug.Log(Application.companyName);
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Big Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.IconContent("cs Script Icon", "|Save as C# Script.")))
        {
            BlocklyPlayground.SaveAsCSharp();
        }

        /*if (GUILayout.Button(EditorGUIUtility.IconContent("js Script Icon", "|Save as JS.")))
        {
            BlocklyPlayground.SaveAsJavaScript();
        }*/

        // Create primitive with our most recent Snapper script.
        EditorGUILayout.BeginVertical();
        op = (E_PRIMITIVE_STATE)EditorGUILayout.EnumPopup("Primitive to create:", op);
        EditorGUILayout.LabelField(string.Format("Last Snapper Script: \"{0}\"", BlocklyPlayground.LastSnapperComponent));

        // If we're not compiling...
        if (!EditorApplication.isCompiling)
        {
            // ... allow us to create a primitive.
            if (GUILayout.Button(EditorGUIUtility.IconContent("GameObject Icon",
            "|Adds the latest Snapper script to a primitive."),
            GUILayout.Height(fButtonHeight - fPadding * 8f)))
            {
                InstantiatePrimitive(op);
            }
        }
        // ... We don't want to create a primitive if we're compiling.
        else
        {
            GUIStyle style = new GUIStyle();
            style.richText = true;
            EditorGUILayout.LabelField("<color=red>Compiling!</color> Please Wait.", style);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.EndHorizontal();

        UISystemProfilerApi.EndSample(UISystemProfilerApi.SampleType.Render);
        UnityEngine.Profiling.Profiler.EndSample();
    }

    void InstantiatePrimitive(E_PRIMITIVE_STATE op)
    {
        lastSnapperComponent = BlocklyPlayground.LastSnapperComponent;

        switch (op)
        {
            case E_PRIMITIVE_STATE.CUBE:
                CreatePrimitive(GameObject.CreatePrimitive(PrimitiveType.Cube), lastSnapperComponent);
                break;
            case E_PRIMITIVE_STATE.SPHERE:
                CreatePrimitive(GameObject.CreatePrimitive(PrimitiveType.Sphere), lastSnapperComponent);
                break;
            case E_PRIMITIVE_STATE.CAPSULE:
                CreatePrimitive(GameObject.CreatePrimitive(PrimitiveType.Capsule), lastSnapperComponent);
                break;
            case E_PRIMITIVE_STATE.CYLINDER:
                CreatePrimitive(GameObject.CreatePrimitive(PrimitiveType.Cylinder), lastSnapperComponent);
                break;
            case E_PRIMITIVE_STATE.PLANE:
                CreatePrimitive(GameObject.CreatePrimitive(PrimitiveType.Plane), lastSnapperComponent);
                break;
            case E_PRIMITIVE_STATE.QUAD:
                CreatePrimitive(GameObject.CreatePrimitive(PrimitiveType.Quad), lastSnapperComponent);
                break;
            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
    }

    GameObject CreatePrimitive(GameObject a_go, string a_component)
    {
        a_go.transform.position = Vector3.zero;
        a_go.name = a_component;
        a_go.AddComponent(GetAssemblyType(a_component));
        Selection.activeGameObject = a_go;
        Undo.RegisterCreatedObjectUndo(a_go, "Creating " + a_go.name);
        return a_go;
    }

    // https://forum.unity.com/threads/generating-a-script-then-using-addcomponent-to-attach-it-to-a-gameobject.299685/#post-1974265
    // https://blogs.unity3d.com/2015/01/21/addcomponentstring-api-removal-in-unity-5-0/#comment-210882
    // AddComponent(Type.GetType(scriptName + “, ” + assemblyName));
    public static System.Type GetAssemblyType(string typeName)
    {
        var type = System.Type.GetType(typeName);
        if (type != null) return type;
        foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            type = a.GetType(typeName);
            if (type != null) return type;
        }
        return null;
    }
    #endregion


    void OnInspectorUpdate()
    {
        Repaint();
    }
}