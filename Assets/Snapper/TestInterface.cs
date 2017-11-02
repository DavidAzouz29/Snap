using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInterface : MonoBehaviour {


    GUISkin guiSkin;
    GameObject redBoxPrefab;
    GameObject blueBoxPrefab;
    private WebMediator webMediator;
    private string note;

    // Show the web view (with margins) and load the index page.
    private void ActivateWebView()
    {
        WebMediator.LoadUrl("http://keijiro.github.com/unity-webview-integration/index.html");
        WebMediator.SetMargin(12, Screen.height / 2 + 12, 12, 12);
        WebMediator.Show();
    }

    // Hide the web view.
    private void DeactivateWebView()
    {
        WebMediator.Hide();
        // Clear the state of the web view (by loading a blank page).
        WebMediator.LoadUrl("about:blank");
    }

    // Process messages coming from the web view.
    private void ProcessMessages()
    {
        while (true)
        {
            // Poll a message or break.
            var message = WebMediator.PollMessage();
            if (message == null) break;

            if (message.path == "/spawn")
            {
                GameObject prefab = null;
                // "spawn" message.
                if (message.args.ContainsKey("color"))
                {
                    prefab = ((string)message.args["color"] == "red") ? redBoxPrefab : blueBoxPrefab;
                }
                else
                {
                    prefab = Random.value < 0.5 ? redBoxPrefab : blueBoxPrefab;
                }
                var box = Instantiate(prefab, redBoxPrefab.transform.position, Random.rotation) as GameObject;
                if (message.args.ContainsKey("scale"))
                {
                    box.transform.localScale = Vector3.one * float.Parse(message.args["scale"] as string);
                }
            }
            else if (message.path == "/note")
            {
                // "note" message.
                note = message.args["text"] as string;
            }
            else if (message.path == "/print")
            {
                // "print" message.
                var text = message.args["line1"] as string;
                if (message.args.ContainsKey("line2"))
                {
                    text += "\n" + message.args["line2"] as string;
                }
                Debug.Log(text);
                Debug.Log("(" + text.Length + " chars)");
            }
            else if (message.path == "/close")
            {
                // "close" message.
                DeactivateWebView();
            }
        }
    }

    void Start()
    {
        //WebMediator instance = WebMediator.Instance;
        WebMediator.Install();
    }

    void Update()
    {
        if (WebMediator.IsVisible())
        {
            ProcessMessages();
        }
        else if (Input.GetButtonDown("Fire1") && Input.mousePosition.y < Screen.height / 2)
        {
            ActivateWebView();
        }
    }

    void OnGUI()
    {
        var sw = Screen.width;
        var sh = Screen.height;
        GUI.skin = guiSkin;
        Rect rectNote = new Rect(0, 0, sw, 0.5f * sh);
        if (note != null) GUI.Label(rectNote, note);
        Rect rectCentre = new Rect(0, 0.5f * sh, sw, 0.5f * sh);
        GUI.Label(rectCentre, "TAP HERE", "center");
    }
}
