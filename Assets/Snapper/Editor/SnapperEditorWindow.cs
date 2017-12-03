///<summary>
/// Author: David Azouz
/// Date: 02/12/17
/// viewed: http://wiki.unity3d.com/index.php/FindGameObjects
/// https://docs.unity3d.com/Manual/RunningEditorCodeOnLaunch.html 
/// 
/// </summary>

using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SnapperEditorWindow : EditorWindow
{
    // GUI
    const float fPadding = 4f;
    const float fButtonWidth  = 128f; // Multiple of 8.
    const float fButtonHeight = fButtonWidth * 0.5f; // Multiple of 8.
    const float fWinMinHeight = fButtonHeight + (fPadding * 5f);
    GUILayoutOption buttonSmallWidth = GUILayout.Width(fButtonWidth * 0.5f);

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
        window.minSize = new Vector2(300f,  fWinMinHeight);
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

        if (GUILayout.Button(EditorGUIUtility.IconContent("GameObject Icon", "|This button doesn't do anything.")))
        {
            Debug.Log("This button doesn't do anything.");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        UISystemProfilerApi.EndSample(UISystemProfilerApi.SampleType.Render);
        UnityEngine.Profiling.Profiler.EndSample();
    }

    #endregion


    void OnInspectorUpdate()
    {
        Repaint();
    }
}