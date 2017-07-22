///<summary>
/// Author: NuneShelping
/// viewed: http://wiki.unity3d.com/index.php/FindGameObjects
///</summary>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ToolGameObjectFinder : EditorWindow
{
    // static
    private static string _staticFilterString = "";

    [MenuItem("Tools/GameObject/Finder", priority = 100)]
    public static void ShowWindow()
    {
        var window = (ToolGameObjectFinder)EditorWindow.GetWindow(typeof(ToolGameObjectFinder), false, "Finder", true);
        window.Initialize();
    }

    // enum

    // properties

    // public variables

    // public methods
    public void Initialize()
    {
        if (_initialized) return;

        _filterString = _staticFilterString;

        EditorApplication.hierarchyWindowChanged += GetSceneGameObjects;
        EditorApplication.hierarchyWindowChanged += TryAutoSearch;
        EditorApplication.projectWindowChanged += GetProjectGameObjects;
        EditorApplication.projectWindowChanged += TryAutoSearch;

        GetComponentTypes();
        GetSceneGameObjects();
        GetProjectGameObjects();
        TryAutoSearch();

        _initialized = true;
    }

    // private variables
    private float _INVERT_WIDTH = 120;
    private float _EXTRAS_WIDTH = 135;
    private bool _initialized = false;

    // private methods
    private void OnGUI()
    {
        Initialize();

        if (Event.current.type == EventType.KeyDown) ProcessKeyEvent(Event.current);

        EditorGUI.BeginChangeCheck();

        DrawIncludedObjects();

        DrawFilters();

        if (EditorGUI.EndChangeCheck()) TryAutoSearch();

        DrawResults();

        DrawSettings();
    }

    private void ProcessKeyEvent(Event current)
    {
        int index = _results.IndexOf(_selectedListing);
        switch (current.keyCode)
        {
            case KeyCode.UpArrow:
                index -= 1;
                break;

            case KeyCode.DownArrow:
                index += 1;
                break;

            default:
                return;
        }

        index = Wrap(index, _results.Count);
        _selectedListing = _results[index];

        Selection.activeGameObject = _selectedListing.GameObject;
        EditorGUIUtility.PingObject(_selectedListing.GameObject);

        float listingHeight = EditorStyles.toolbarButton.CalcHeight(new GUIContent(" "), 100);
        float listingPosition = index * listingHeight;

        // max full listings scroll height
        float completeListingHeight = Mathf.Floor(_scrollHeight / listingHeight) * listingHeight;
        float heightDifference = _scrollHeight - completeListingHeight;

        // past view
        if (listingPosition > _resultScrollbarPosition.y + completeListingHeight - heightDifference) _resultScrollbarPosition.y = listingPosition - completeListingHeight - heightDifference + listingHeight;

        // before view
        if (listingPosition < _resultScrollbarPosition.y) _resultScrollbarPosition.y = listingPosition;

        Repaint();
    }

    private void DrawSearchButton()
    {
        if (!_autoSearch)
        {
            var style = EditorStyles.miniButton;
            FontStyle oldFontStyle = style.fontStyle;
            if (!_autoSearch && _wantSearch)
            {
                style.fontStyle = FontStyle.Bold;
            }
            if (GUILayout.Button("Search", style)) UpdateSearch();
            style.fontStyle = oldFontStyle;
        }
    }

    private bool _foldoutIncludedObjects = true;
    private void DrawIncludedObjects()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Included GameObjects"), EditorStyles.toolbarButton)) _foldoutIncludedObjects = !_foldoutIncludedObjects;

        if (_foldoutIncludedObjects)
        {
            EditorGUI.indentLevel++;
            DrawIncludePrefabObjects();
            DrawIncludeSceneObjects();
            EditorGUI.indentLevel--;
        }
    }

    private bool _foldoutSettings = false;
    private bool _autoSearch = true;
    private bool _wantSearch = false;
    private void DrawSettings()
    {
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Settings"), EditorStyles.toolbarButton)) _foldoutSettings = !_foldoutSettings;

        if (_foldoutSettings)
        {
            _autoSearch = EditorGUILayout.ToggleLeft(new GUIContent("Auto-Search", "On-the-fly search updating. Use the \"Search\" button to manually update the results."), _autoSearch);
        }
    }

    private GameObjectListing _selectedListing;
    private Vector2 _resultScrollbarPosition;
    private bool _foldoutResults = true;
    private float _scrollHeight;
    private void DrawResults()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent(string.Format("Results ({0})", _results.Count)), EditorStyles.toolbarButton)) _foldoutResults = !_foldoutResults;

        if (_foldoutResults)
        {
            DrawSearchButton();

            _resultScrollbarPosition = GUILayout.BeginScrollView(_resultScrollbarPosition);

            EditorGUI.indentLevel++;

            var style = EditorStyles.toolbarButton;
            Color oldTextColor = new Color(style.normal.textColor.r, style.normal.textColor.g, style.normal.textColor.b);
            TextAnchor oldAlignment = style.alignment;
            style.alignment = TextAnchor.MiddleLeft;
            foreach (var listing in _results)
            {
                style.normal.textColor = listing.Color;
                if (_selectedListing != null && listing == _selectedListing) style.normal.textColor = Color.white;
                if (GUILayout.Button(listing.GameObject.name, style))
                {
                    _selectedListing = listing;

                    GameObject target = Event.current.button == 1 ? listing.GetRootGameObject() : listing.GameObject;
                    Selection.activeGameObject = target;
                    EditorGUIUtility.PingObject(target);
                }
            }
            style.normal.textColor = oldTextColor;
            style.alignment = oldAlignment;

            EditorGUI.indentLevel--;

            GUILayout.EndScrollView();

            if (Event.current.type == EventType.Repaint) _scrollHeight = GUILayoutUtility.GetLastRect().height;
        }
    }

    private bool _foldoutFilters = true;
    private void DrawFilters()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Filters"), EditorStyles.toolbarButton)) _foldoutFilters = !_foldoutFilters;

        if (_foldoutFilters)
        {
            EditorGUI.indentLevel++;

            DrawFilterString();
            DrawFilterUnassignedScript();
            DrawFilterLayer();
            DrawFilterTag();
            DrawFilterComponent();
            DrawFilterScript();
            DrawFilterStatic();

            EditorGUI.indentLevel--;
        }
    }

    private bool _filterStatic;
    private bool _invertStaticFilter;
    private StaticEditorFlags _filteredStaticFlag = StaticEditorFlags.LightmapStatic;
    private void DrawFilterStatic()
    {
        // static
        EditorGUILayout.BeginHorizontal();
        _filterStatic = EditorGUILayout.ToggleLeft(new GUIContent("Static", "Filter objects by static flag."), _filterStatic);
        if (_filterStatic)
        {
            _filteredStaticFlag = (StaticEditorFlags)EditorGUILayout.EnumPopup(_filteredStaticFlag);
            if (GUILayout.Button(_invertStaticFilter ? new GUIContent("Not Set", "Objects without this static flag set.")
                                                     : new GUIContent("Is Set", "Objects with this static flag set."),
                                 EditorStyles.miniButton, GUILayout.Width(_INVERT_WIDTH))) _invertStaticFilter = !_invertStaticFilter;
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool _filterComponent;
    private bool _invertComponentFilter;
    private int _filteredComponentIndex = 0;
    private void DrawFilterComponent()
    {
        // component
        EditorGUILayout.BeginHorizontal();

        _filterComponent = EditorGUILayout.ToggleLeft(new GUIContent("Component", "Filter objects by component."), _filterComponent);
        if (_filterComponent)
        {
            _filteredComponentIndex = EditorGUILayout.Popup(_filteredComponentIndex, _componentNames);
            if (GUILayout.Button(_invertComponentFilter ? new GUIContent("Missing Component", "Objects without this component.")
                                                        : new GUIContent("Has Component", "Objects with this component."),
                                 EditorStyles.miniButton, GUILayout.Width(_INVERT_WIDTH))) _invertComponentFilter = !_invertComponentFilter;
        }

        EditorGUILayout.EndHorizontal();

        if (!_invertComponentFilter)
        {
            DrawColliderFilterExtras();
        }
    }

    private enum ColliderFilterExtras { All, IsTrigger, IsNotTrigger }
    private ColliderFilterExtras _colliderFilterExtra = ColliderFilterExtras.All;
    private bool _filterComponentIsCollider;
    private void DrawColliderFilterExtras()
    {
        if (_filterComponent)
        {
            System.Type type = _componentTypes[_filteredComponentIndex];
            if (type == null) return;

            _filterComponentIsCollider = type == typeof(Collider) || type.IsSubclassOf(typeof(Collider)) || type == typeof(Collider2D) || type.IsSubclassOf(typeof(Collider2D));

            if (_filterComponentIsCollider)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                _colliderFilterExtra = (ColliderFilterExtras)EditorGUILayout.EnumPopup(_colliderFilterExtra, GUILayout.Width(_EXTRAS_WIDTH));
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private bool FilterColliderComponent(Component component)
    {
        if (_filterComponentIsCollider)
        {
            if (_colliderFilterExtra == ColliderFilterExtras.All) return false;

            if (component is Collider)
            {
                switch (_colliderFilterExtra)
                {
                    case ColliderFilterExtras.IsTrigger:
                        return !((Collider)component).isTrigger;

                    case ColliderFilterExtras.IsNotTrigger:
                        return ((Collider)component).isTrigger;
                }
            }
            else if (component is Collider2D)
            {
                switch (_colliderFilterExtra)
                {
                    case ColliderFilterExtras.IsTrigger:
                        return !((Collider2D)component).isTrigger;

                    case ColliderFilterExtras.IsNotTrigger:
                        return ((Collider2D)component).isTrigger;
                }
            }
        }

        return true;
    }

    private string[] _componentNames;
    private List<string> _categories;
	private System.Type[] _componentTypes;
    private void GetComponentTypes()
    {
        // get types
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(Component));
        _componentTypes = assembly.GetTypes().Where(t => typeof(Component).IsAssignableFrom(t) && t.IsPublic && t != typeof(Component)).ToArray();

        // get names
        _componentNames = new string[_componentTypes.Length];
        for (int i = 0; i < _componentTypes.Length; i++) _componentNames[i] = GetComponentName(_componentTypes[i]);

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
				category = _componentTypes[i].Namespace.Substring(12) + "/";
				if(category.Contains("."))
				{
					category = category.Split('.').First() + "/";
				}
			}
			else
			{
				//string contains = _componentNames[i];
				#region Categories
				_categories = new List<string>();
				List<string> subStrings = new List<string>
				{ "TextMesh", "MeshRenderer", "Sprite",
					"Particle", "Trail", "Line", "Lens", "Halo", "Projector", // Effects
					"Audio", "2D" , "Canvas", "TextMeshPro", // Custom.
					"Anim", "Terrain", "Wind", "Network", "Mono", "Transform", "Tree"}; // Misc.
				switch (subStrings.FirstOrDefault(_componentNames[i].Contains))
				{
					case "TextMesh":
					case "MeshRenderer":
						{
							category = "Mesh/";
							break;
						}
					case "Particle":
					case "Trail":
					case "Line":
					case "Lens":
					case "Halo":
					case "Projector":
						{
							category = "Effects/";
							break;
						}

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
					case "Canvas":
					case "TextMeshPro":
						{
							category = "UI/";
							break;
						}
					case "Anim":
					case "Terrain":
					case "Wind":
					case "Mono":
					case "Transform":
					case "Tree":
						{
							category = "Misc./";
							break;
						}
					case "Network":
						{
							category = "Networking/"; //TODO: Network?
							break;
						}
					case "Sprite":
						{
							category = "Rendering/";
							break;
						}

					default:
						{
							// TODO: RECODE: HACK HACK HACK
							if (i > 0 && i <= 21 || i == 24 || i == 92)
							{
								category = "Rendering/";
							}
							else if (i > 31 && i <= 74)
							{
								category = "Physics/";
							}
							else
							{
								category = "";
							}
							break;
						}
				}
				#endregion
			}

            // add it to its own path
            if (contained)
				_componentNames[i] = name + '/' + name.Split('/').Last();
			if (category == "AI/")
				category = "Navigation/";
			else if (category == "Renderer/")
				category = string.Concat("Rendering/", category);
			if (category != "")
			{
				_componentNames[i] = string.Concat(category, _componentNames[i]);
				_categories.Add(category);
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

    private bool _filterScript;
    private bool _invertScriptFilter;
    private MonoScript _filteredScript = null;
    private void DrawFilterScript()
    {
        // script
        EditorGUILayout.BeginHorizontal();
        _filterScript = EditorGUILayout.ToggleLeft(new GUIContent("Script", "Filter objects by script."), _filterScript);
        if (_filterScript)
        {
            _filteredScript = (MonoScript)EditorGUILayout.ObjectField(_filteredScript, typeof(MonoScript), true);
            if (_filteredScript != null)
            {
                var type = _filteredScript.GetClass();
                if (type.IsAbstract) _filteredScript = null;
                if (!type.IsSubclassOf(typeof(MonoBehaviour))) _filteredScript = null;
            }

            if (GUILayout.Button(_invertScriptFilter ? new GUIContent("Missing Script", "Objects without this script.")
                                                     : new GUIContent("Has Script", "Objects with this script."),
                                 EditorStyles.miniButton, GUILayout.Width(_INVERT_WIDTH))) _invertScriptFilter = !_invertScriptFilter;
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool _filterTag;
    private bool _invertTagFilter;
    private string _filteredTag = "Untagged";
    private void DrawFilterTag()
    {
        // tag
        EditorGUILayout.BeginHorizontal();
        _filterTag = EditorGUILayout.ToggleLeft(new GUIContent("Tag", "Filter objects by tag."), _filterTag);

        if (_filterTag)
        {
            _filteredTag = EditorGUILayout.TagField(_filteredTag);
            if (GUILayout.Button(_invertTagFilter ? new GUIContent("Not Tagged", "Objects without this tag.")
                                                  : new GUIContent("Is Tagged", "Objects with this tag."),
                                 EditorStyles.miniButton, GUILayout.Width(_INVERT_WIDTH))) _invertTagFilter = !_invertTagFilter;
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool _filterLayer;
    private bool _invertLayerFilter;
    private LayerMask _filteredLayer;
    private void DrawFilterLayer()
    {
        // layer
        EditorGUILayout.BeginHorizontal();
        _filterLayer = EditorGUILayout.ToggleLeft(new GUIContent("Layer", "Filter objects by layer."), _filterLayer);

        if (_filterLayer)
        {
            _filteredLayer = EditorGUILayout.LayerField(_filteredLayer);
            if (GUILayout.Button(_invertLayerFilter ? new GUIContent("Not In Layer", "Objects not in this layer.")
                                                    : new GUIContent("In Layer", "Objects in this layer."),
                                 EditorStyles.miniButton, GUILayout.Width(_INVERT_WIDTH))) _invertLayerFilter = !_invertLayerFilter;
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool _filterUnassignedScript;
    private void DrawFilterUnassignedScript()
    {
        // unassigned script
        _filterUnassignedScript = EditorGUILayout.ToggleLeft(new GUIContent("Unassigned Script", "Objects with unassigned script components."), _filterUnassignedScript);
    }

    private string _filterString = "";
    private bool _caseSensitive = false;
    private void DrawFilterString()
    {
        // filter by string
        EditorGUILayout.BeginHorizontal();
        string newFilterString = EditorGUILayout.TextField(_filterString);
        if (_filterString != newFilterString) // catch immediate changes
        {
            _filterString = newFilterString;
            _staticFilterString = _filterString;
        }

        if (GUILayout.Button(_caseSensitive ? new GUIContent("Match Case", "Use case sensitive name searching.")
                                            : new GUIContent("Ignore Case", "Use any-case name searching."),
                             EditorStyles.miniButton, GUILayout.Width(_INVERT_WIDTH))) _caseSensitive = !_caseSensitive;
        EditorGUILayout.EndHorizontal();
    }

    private bool _includeSceneObjects = false;
    private void DrawIncludeSceneObjects()
    {
        _includeSceneObjects = EditorGUILayout.ToggleLeft(new GUIContent("Include Scene", "Include scene gameobjects in the search."), _includeSceneObjects);
    }

    private bool _includePrefabObjects = true;
    private bool _includePrefabChildren = true;
    private void DrawIncludePrefabObjects()
    {
        EditorGUILayout.BeginHorizontal();
        _includePrefabObjects = EditorGUILayout.ToggleLeft(new GUIContent("Include Prefabs", "Include prefab gameobjects in the search."), _includePrefabObjects);
        if (_includePrefabObjects)
        {
            if (GUILayout.Button(_includePrefabChildren ? new GUIContent("With Children", "Include chilcren in the seasrch.")
                                                        : new GUIContent("No Children", "Exclude children in the search."), EditorStyles.miniButton, GUILayout.Width(_INVERT_WIDTH)))
            {
                _includePrefabChildren = !_includePrefabChildren;
                GetProjectGameObjects();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool _focused;
    private void OnFocus() { _focused = true; }
    private void OnLostFocus() { _focused = false; }

    private int _MAX_AUTO_SEARCHABLE = 10000;
    private void TryAutoSearch()
    {

        if (_autoSearch)
        {
            int numObjectsToSearch = 0;
            if (_includePrefabObjects) numObjectsToSearch += _projectListings.Length;
            if (_includeSceneObjects) numObjectsToSearch += _sceneListings.Length;
            if (numObjectsToSearch > _MAX_AUTO_SEARCHABLE)
            {
                _autoSearch = false;
                Debug.LogWarning(string.Format("Auto-Searching disabled! Attempted to search {0} (>{1}) objects without express permission from user. Searches of this magnitude must be made manually.", numObjectsToSearch, _MAX_AUTO_SEARCHABLE));
                _wantSearch = true;
                return;
            }
            UpdateSearch();
        }
        else
        {
            _wantSearch = true;
        }
    }

    private void UpdateSearch()
    {
        Search();
        _wantSearch = false;
        _resultScrollbarPosition = Vector2.zero;
        _selectedListing = null;
    }

    private List<GameObjectListing> _results = new List<GameObjectListing>();
    private void Search()
    {
        _results = new List<GameObjectListing>();

        // include prefab objects
        if (_includePrefabObjects)
        {
            _results.AddRange(Filter(_projectListings));
        }

        // include scene objects
        if (_includeSceneObjects)
        {
            _results.AddRange(Filter(_sceneListings));
        }

        if (!_focused) Repaint();
    }

    private List<GameObjectListing> Filter(GameObjectListing[] gameObjectListings)
    {
        string filterString = _caseSensitive ? _filterString : _filterString.ToLower();
        string[] filterSubstrings = filterString.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        var filtered = new List<GameObjectListing>();
        if (gameObjectListings == null) return filtered;
        foreach (var listing in gameObjectListings)
        {
            GameObject go = listing.GameObject;

            // check substrings
            bool passSubstringFilter = true;
            foreach (var substring in filterSubstrings)
            {
                if (_caseSensitive ? !go.name.Contains(substring) : !go.name.ToLower().Contains(substring))
                {
                    passSubstringFilter = false;
                    break;
                }
            }
            if (!passSubstringFilter) continue;

            // check missing script
            if (_filterUnassignedScript)
            {
                bool passMissingScriptFilter = false;
                var components = go.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component == null)
                    {
                        passMissingScriptFilter = true;
                        break;
                    }
                }
                if (!passMissingScriptFilter) continue;
            }

            // check layer
            if (_filterLayer && _invertLayerFilter != (_filteredLayer != go.layer)) continue;

            // check tag
            if (_filterTag && _invertTagFilter != (!go.CompareTag(_filteredTag))) continue;

            // check component
            if (_filterComponent)
            {
                System.Type componentType = _componentTypes[_filteredComponentIndex];
                if (componentType != null)
                {
                    var component = go.GetComponent(componentType);
                    if (_invertComponentFilter && component != null || !_invertComponentFilter && component == null) continue;

                    if (!_invertComponentFilter && _filterComponentIsCollider && FilterColliderComponent(component)) continue;
                }
            }

            // check script
            if (_filterScript && _filteredScript != null && _invertScriptFilter != (go.GetComponent(_filteredScript.GetClass()) == null)) continue;

            // check static
            if (_filterStatic && _invertStaticFilter != !GameObjectUtility.AreStaticEditorFlagsSet(go, _filteredStaticFlag)) continue;

            // add gameobject to filtered set
            filtered.Add(listing);
        }

        return filtered;
    }

    private GameObjectListing[] _sceneListings = new GameObjectListing[0];
    private void GetSceneGameObjects()
    {
        GameObject[] sceneGameObjects = FindObjectsOfType<GameObject>();

        _sceneListings = new GameObjectListing[sceneGameObjects.Length];
        for (int i = 0; i < sceneGameObjects.Length; i++) _sceneListings[i] = new GameObjectListing(sceneGameObjects[i]);
    }

    private GameObjectListing[] _projectListings = new GameObjectListing[0];
    private void GetProjectGameObjects()
    {
        string[] files = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        int dataPathLength = Application.dataPath.Length - "Assets".Length;

        List<GameObjectListing> prefabs = new List<GameObjectListing>();
        foreach (string fullPath in files)
        {
            string path = fullPath.Remove(0, dataPathLength);
            GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                prefabs.Add(new GameObjectListing(go));

                if (_includePrefabChildren) prefabs.AddRange(GetChildGameObjects(go.transform));
            }
        }

        _projectListings = prefabs.ToArray();
    }

    private List<GameObjectListing> GetChildGameObjects(Transform parent)
    {
        var children = new List<GameObjectListing>();

        int childCount = parent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            children.Add(new GameObjectListing(child.gameObject, parent.gameObject));
            children.AddRange(GetChildGameObjects(child));
        }

        return children;
    }

    private int Wrap(int number, int max)
    {
        return ((number % max) + max) % max;
    }

    private class GameObjectListing
    {
        public static Color PrefabColor = new Color(77f / 255, 128f / 255, 217f / 255);
        public static Color PrefabChildColor = new Color(126f / 255, 158f / 255, 215f / 255);
        public static Color DefaultColor = new Color(180f / 255, 180f / 255, 180f / 255);

        public Color Color
        {
            get
            {
                if (_prefabType == PrefabType.Prefab || _prefabType == PrefabType.PrefabInstance)
                {
                    return _isChild ? GameObjectListing.PrefabChildColor : GameObjectListing.PrefabColor;
                }
                else
                {
                    return GameObjectListing.DefaultColor;
                }
            }
        }

        public readonly GameObject GameObject;
        public readonly GameObject Parent;
        private PrefabType _prefabType;
        private bool _isChild;
        public GameObjectListing(GameObject gameObject, GameObject parent = null)
        {
            GameObject = gameObject;
            Parent = parent;
            _prefabType = PrefabUtility.GetPrefabType(gameObject);
            _isChild = parent != null || PrefabUtility.FindRootGameObjectWithSameParentPrefab(gameObject) != gameObject;
        }

        public GameObject GetRootGameObject()
        {
            return GameObject.transform.root.gameObject;
        }
    }
}