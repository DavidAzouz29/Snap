
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow
{
    Rect window1;
    Rect window2;
    float panX = 0;
    float panY = 0;

    [MenuItem("Window/Node editor %&s")]
    static void ShowEditor()
    {
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
        editor.Init();
    }

    public void Init()
    {
        window1 = new Rect(100, 100, 100, 100);
        window2 = new Rect(260, 260, 100, 100);
    }

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(panX, panY, 100000, 100000));
        DrawNodeCurve(window1, window2); // Here the curve is drawn under the windows              

        BeginWindows();
        window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged       
        window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
        EndWindows();

        GUI.EndGroup();

        if (GUI.RepeatButton(new Rect(15, 5, 20, 20), "^"))
        {
            panY -= 1;
            Repaint();
        }

        if (GUI.RepeatButton(new Rect(5, 25, 20, 20), "<"))
        {
            panX -= 1;
            Repaint();
        }

        if (GUI.RepeatButton(new Rect(25, 25, 20, 20), ">"))
        {
            panX += 1;
            Repaint();
        }

        if (GUI.RepeatButton(new Rect(15, 45, 20, 20), "v"))
        {
            panY += 1;
            Repaint();
        }
    }

    void DrawNodeWindow(int id)
    {
        GUI.DragWindow();
    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++) // Draw a shadow           
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }
}