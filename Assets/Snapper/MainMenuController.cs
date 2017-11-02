///<summary>
///
/// viewed: http://www.alanzucconi.com/2016/03/30/loading-bar-in-unity/
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

//These are here to prevent errors and are in no way functional
public class GameManager
{
    public static GameManager Instance;
    public GameSettings m_ActiveGameSettings;
};
public class PlayerInfoController { public void Refresh() { } };
public class MenuScript { public int GetLevelSelection() { return 1; } };

public class MainMenuController : MonoBehaviour
{
	public GameSettings GameSettingsTemplate;

	public Color[] AvailableColors;

    public Text[] LoadingBarTexts = new Text[3];
    public Slider c_LoadingBarSlider;

    // hack used for joystick selecting level twice
    private bool isFirstTimePlayed = false;

    //public UnityEngine.UI.Button PanelSwitcher;
    //public GameObject PlayersPanel;
    //public GameObject SettingsPanel;

    public string SavedSettingsPathEditor
    {
		get {
			return System.IO.Path.Combine(Application.persistentDataPath, "snacks-settings-editor.json");
		}
	}

    public string SavedSettingsPath
    {
        get
        {
            return System.IO.Path.Combine(Application.persistentDataPath, "snacks-settings-build.json");
        }
    }

    void Start ()
    {
        isFirstTimePlayed = false;
        if (GameSettingsTemplate == null)
        {
            GameSettingsTemplate = GameManager.Instance.m_ActiveGameSettings;// GameSettings.Instance;
        }

#if UNITY_EDITOR
        if (System.IO.File.Exists(SavedSettingsPathEditor))
            GameSettings.LoadFromJSON(SavedSettingsPathEditor);
            //System.IO.File.Delete(SavedSettingsPathEditor);

#else
        if (System.IO.File.Exists(SavedSettingsPath))
			GameSettings.LoadFromJSON(SavedSettingsPath);
            //System.IO.File.Delete(SavedSettingsPath);
#endif
        else //
            GameSettings.InitializeFromDefault(GameSettingsTemplate);

		foreach(var info in GetComponentsInChildren<PlayerInfoController>())
			info.Refresh();

		//NumberOfRoundsSlider.value = GameSettings.Instance.NumberOfRounds;
	}

    IEnumerator AsynchronousLoad(int a_scene)
    {
        yield return null;

        AsyncOperation ao = SceneManager.LoadSceneAsync(a_scene);
        ao.allowSceneActivation = false;
        // TODO: clean up one day? 17/10/2016
        string sTextSlot = null;
        /*if(a_scene == Scene.Level1Kitchen)
        {
            sTextSlot = "Kitchen";
        }
        else if (a_scene == Scene.Level2Banquet)
        {
            sTextSlot = "Banquet";
        }*/
        LoadingBarTexts[1].text = sTextSlot; //SceneManager.GetSceneAt(a_scene).name.TrimStart("L1_".ToCharArray());

        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            //Debug.Log("Loading progress: " + (progress * 100) + "%");
            c_LoadingBarSlider.value = progress;
            LoadingBarTexts[2].text = (progress * 100).ToString("F0") + "% Complete";

            // Loading completed
            if (ao.progress == 0.9f)
            {
                LoadingBarTexts[2].text = "Press Any Key."; //Debug.Log("Press a key to start");
                if (Input.anyKey && !Input.GetKeyDown(KeyCode.JoystickButton15))
                {
                    ao.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public void Play()
    {
        if (!isFirstTimePlayed)
        {
            string sPath = "";
#if UNITY_EDITOR
            sPath = SavedSettingsPathEditor;
#else
                sPath = SavedSettingsPath;
#endif
            GameSettings.Instance.SaveToJSON(sPath);
            GameState.CreateFromSettings(GameSettings.Instance);
            //SceneManager.LoadScene(this.GetComponent<MenuScript>().GetLevelSelection(), LoadSceneMode.Single);
            StartCoroutine(AsynchronousLoad(this.GetComponent<MenuScript>().GetLevelSelection()));
            isFirstTimePlayed = true;
        }
    }

	public Color GetNextColor(Color color)
	{
		int existingColor = Array.FindIndex(AvailableColors, c => c == color);
		existingColor = (existingColor + 1)%AvailableColors.Length;
		return AvailableColors[existingColor];
	}

    //public UnityEngine.UI.Text NumberOfRoundsLabel;
    //public UnityEngine.UI.Slider NumberOfRoundsSlider;

    public void OnChangeNumberOfRounds(float value)
	{
		//GameSettings.Instance.NumberOfRounds = (int) value;

		//NumberOfRoundsLabel.text = GameSettings.Instance.NumberOfRounds.ToString();
	}

	/*public void OnSwitchPanels()
	{
		PlayersPanel.SetActive(!PlayersPanel.activeSelf);
		SettingsPanel.SetActive(!SettingsPanel.activeSelf);
		PanelSwitcher.GetComponentInChildren<UnityEngine.UI.Text>().text = PlayersPanel.activeSelf ? "Settings" : "Players";
	} */
}
