/// <summary>
/// 
/// 
/// viewed: https://bitbucket.org/richardfine/scriptableobjectdemo 
/// https://bitbucket.org/richardfine/scriptableobjectdemo/src/9a60686609a42fea4d00f5d20ffeb7ae9bc56eb9/Assets/ScriptableObject/GameSession/GameSettings.cs?at=default&fileviewer=file-view-default
/// </summary>

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//These are here to prevent errors and are in no way functional
public class PlayerBuild 
{
    public enum E_BASE_CLASS_STATE { };
};
public class PlayerController
{
    public enum E_CLASS_STATE { };
};
public class SnackBrain : ScriptableObject
{
    public void GetClassName() {; }
    public void GetBaseState() {; }
    public void GetClassState() {; }
};
public class SubClassBrain : SnackBrain
{
    public int _iBrainID;
};
public class SnackThinker { };
public class GameState { public static void CreateFromSettings(GameSettings a_GameSettings) { } };

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    /*void OnEnable()
    {
        for (int i = 0; i < PlayerManager.MAX_PLAYERS; i++)
        {
            players[i].isReady = false;
        }
    } */

	[Serializable]
	public class PlayerInfo
	{
        // Unique per class varient
        public int playerID;
		public string ClassName; // name to display e.g. "Rocky Road"
		public string sPlayerTag; // name to display on scoreboard e.g. "Rocky Road"
        public Color Color;
        public PlayerBuild.E_BASE_CLASS_STATE eBaseClassState;
        public PlayerController.E_CLASS_STATE eClassState;
		public int iKills; // Kills for scoreboard and podium position
		public int iDeaths; // Deaths for scoreboard
		public bool isReady = false; // Used for Player Select

        // Serializing an object reference directly to JSON doesn't do what we want - we just get an InstanceID
        // which is not stable between sessions. So instead we serialize the string name of the object, and
        // look it back up again after deserialization
        private SnackBrain _cachedBrain;
		public SnackBrain Brain
		{
			get
			{
                if (!_cachedBrain && !String.IsNullOrEmpty(BrainName))
                {
                    SubClassBrain[] availableBrains;
#if UNITY_EDITOR
                    // When working in the Editor and launching the game directly from the play scenes, rather than the
                    // main menu, the brains may not be loaded and so Resources.FindObjectsOfTypeAll will not find them.
                    // Instead, use the AssetDatabase to find them. At runtime, all available brains get loaded by the
                    // MainMenuController so it's not a problem outside the editor.

                    availableBrains = UnityEditor.AssetDatabase.FindAssets("t:SubClassBrain")
                                .Select(guid => UnityEditor.AssetDatabase.GUIDToAssetPath(guid))
                                .Select(path => UnityEditor.AssetDatabase.LoadAssetAtPath<SubClassBrain>(path))
                                .OrderBy(b => b._iBrainID).ToArray();

#else
					availableBrains = Resources.FindObjectsOfTypeAll<SubClassBrain>().OrderBy(b => b._iBrainID).ToArray();
#endif
                    Instance.SetAvailableBrains(availableBrains);
                    _cachedBrain = availableBrains.FirstOrDefault(b => b.name == BrainName);
                }
                return _cachedBrain; 
            }
            set
			{
				_cachedBrain = value;
				BrainName = value ? value.name : String.Empty;
                //ClassName = _cachedBrain.GetClassName();
                //sPlayerTag = _cachedBrain.GetClassName();
                //eBaseClassState = _cachedBrain.GetBaseState();
                //eClassState = _cachedBrain.GetClassState();
            }
		}

		[SerializeField] private string BrainName;

		public string GetColoredName()
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(Color) + ">" + ClassName + "</color>";
        }

        public string GetPlayerTag()
        {
            return sPlayerTag;
        }

        public void SetPlayerTag(string a_tag)
        {
            sPlayerTag = a_tag;
        }

        public void SetPlayerKills(int a_kills)
        {
            iKills = a_kills;
        }
        public void SetPlayerDeaths(int a_deaths)
        {
            iDeaths = a_deaths;
        }
    }

	public List<PlayerInfo> players;

    public SubClassBrain[] availableBrains; //SnackBrain[]
    public void SetAvailableBrains(SubClassBrain[] a_availableBrains) { availableBrains = a_availableBrains; }

    private static GameSettings _instance;
	public static GameSettings Instance
    {
        get
        {
#if UNITY_EDITOR
            if (!_instance)
                InitializeFromDefault(UnityEditor.AssetDatabase.LoadAssetAtPath<GameSettings>("Assets/Resources/Default game settings.asset"));
#else
            if (!_instance)
                _instance = Resources.Load("Default game settings") as GameSettings; //FindObjectsOfTypeAll<GameSettings>().FirstOrDefault(); //GameManager.Instance.m_ActiveGameSettings;
#endif
            return _instance;
        }
    }

    // Public due to SO
    public int NumberOfRounds;
	public int iRoundTimerChoice;

    public SnackBrain CycleNextSelection(SnackBrain brain, bool isRight)
    {
        if (brain == null)
            return availableBrains[0];

        // Where you are now
        int index = Array.FindIndex(availableBrains, b => b == brain);
        if (isRight)
        {
            index++; //TODO: for when computer is selected: availableBrains[1]
            return (index <= (availableBrains.Length - 1)) ? availableBrains[index] : availableBrains[0];
        }
        // Player chose left
        else
        {
            index--;
            if(index < 0) //TODO: correct? < 1
            {
                index = availableBrains.Length - 1;
            }
            return (index >= 0) ? availableBrains[index] : null;
        }
    }

    public void SetRoundTimer(int a_time)
    {
        iRoundTimerChoice = a_time;
    }

	public static void LoadFromJSON(string path)
	{
		if (_instance != null) DestroyImmediate(_instance);
		_instance = ScriptableObject.CreateInstance<GameSettings>();
		JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), _instance);
		_instance.hideFlags = HideFlags.HideAndDontSave;
	}

	public void SaveToJSON(string path)
	{
		Debug.LogFormat("Saving game settings to {0}", path);
		System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
	}

    public static void InitializeFromDefault(GameSettings settings)
    {
        // If we have a settings to replace our null instance with
        if (settings != null && _instance == null)
        {
            //if (_instance != null)
            DestroyImmediate(_instance);
            _instance = Instantiate(settings);
        }
        _instance.hideFlags = HideFlags.HideAndDontSave;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Window/Game Settings")]
	public static void ShowGameSettings()
	{
		UnityEditor.Selection.activeObject = Instance;
	}
#endif
    /*
	public bool ShouldFinishGame()
	{
		return GameState.Instance.RoundNumber >= NumberOfRounds;
    }

    public void OnBeginRound()
	{
		++GameState.Instance.RoundNumber;
	}

	public SnackThinker OnEndRound()
	{
		// Return the winner of the round, if there is one
		var winner = GameState.Instance.players.FirstOrDefault(t => t.IsAlive);

		if (winner != null)
			winner.TotalWins++;

		return winner != null ? winner.Snack : null;
	}

	public bool ShouldFinishRound()
	{
        //TODO: set another condition for winning
		return GameState.Instance.players.Count(p => p.IsAlive) <= 1;
	}

	public GameState.PlayerState GetGameWinner()
	{
		return GameState.Instance.GetPlayerWithMostWins();
	}*/
}
