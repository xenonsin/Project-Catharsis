using strange.extensions.editor.impl;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Catharsis.InputEditor.Controller;
using strange.extensions.signal.impl;

namespace Catharsis.InputEditor.View
{
    public sealed class InputManagerEditorView : EditorView
    {
        #region Signals
        internal Signal<string> SendLoadPathSignal = new Signal<string>();
        internal Signal<string> SendSaveInputPathSignal = new Signal<string>();
        #endregion

        #region Menu Options
        public enum FileMenuOptions
        {
            NewInputConfiguration = 0, NewAxisConfiguration, Export, Import, OverriteInputSettings, CreateDefaultInputConfig,
        }
        #endregion


        #region Fields

        private InputManager _inputManager;
        [SerializeField]
        private List<int> _selectionIndex;
        //Note: Might change this later to be more hidden. The Data/ Folder should contain json, xml files that are visable to the player.
        private const string FILE_NAME = @"InputManager.asset";
        private const string FOLDER_NAME = @"Input";
        private const string INPUTMANAGER_PATH = @"Assets/Data/" + FOLDER_NAME + "/" + FILE_NAME;

        private const string INPUT_MANAGER_DEFAULT_CONFIG = "input_manager_default_config";

        [SerializeField]
        private Vector2 _hierarchyScrollPos = Vector2.zero;
        [SerializeField]
        private Vector2 _mainPanelScrollPos = Vector2.zero;
        [SerializeField]
        private float _hierarchyPanelWidth = _menuWidth * 2;
        [SerializeField]
        private Texture2D _highlightTexture;
        [SerializeField]
        private string _searchString = "";
        [SerializeField]
        private string _keyString = string.Empty;

        private GUIStyle _whiteLabel;
        private GUIStyle _whiteFoldout;
        private GUIStyle _warningLabel;

        private bool _isResizingHierarchy = false;
        private bool _editingPositiveKey = false;
        private bool _editingAltPositiveKey = false;
        private bool _editingNegativeKey = false;
        private bool _editingAltNegativeKey = false;

        private float _toolbarHeight = 20.0f;
        private float _minCursorRectWidth = 10.0f;
        private float _maxCursorRectWidth = 50.0f;
        private float _hierarchyItemHeight = 18.0f;

        private string[] _axisOptions = new string[] { "X", "Y", "3rd(Scrollwheel)", "4th", "5th", "6th", "7th", "8th", "9th", "10th" };
        private string[] _joystickOptions = new string[] { "Joystick 1", "Joystick 2", "Joystick 3", "Joystick 4" };

        //Window Dimensions
        private const float _minWindowRectWidth = 400.0f;
        private const float _minWindowRectHeight = 200.0f;

        private const float _menuWidth = 100.0f;
        private const float _minHierarchyPanelWidth = 150.0f;
        #endregion


        [MenuItem("Catharsis/InputManager/Input Editor", false, 0)]
        public static void OpenWindow()
        {
            InputManagerEditorView window = EditorWindow.GetWindow<InputManagerEditorView>("Input Manager");
            window.minSize = new Vector2(_minWindowRectWidth, _minWindowRectHeight);
            //window.title
            window.Show();
        }

        void OnEnable()
        {

            base.OnEnable();
            //Get the serializable input manager
            //_inputManager = AssetDatabase.LoadAssetAtPath(INPUTMANAGER_PATH, typeof (InputManager)) as InputManager;

            //if (_inputManager == null)
            //{
            //    if (!AssetDatabase.IsValidFolder("Assets/Data/" + FOLDER_NAME))
            //    {
            //        AssetDatabase.CreateFolder("Assets", "Data");
            //        AssetDatabase.CreateFolder("Assets/Data", FOLDER_NAME);
            //        Debug.Log("hi");
            //    }

            //    _inputManager = ScriptableObject.CreateInstance<InputManager>();
            //    AssetDatabase.CreateAsset(_inputManager, INPUTMANAGER_PATH);
            //    AssetDatabase.SaveAssets();
            //    AssetDatabase.Refresh();

           // }

            if (_selectionIndex == null)
                _selectionIndex = new List<int>();
            if (_highlightTexture == null)
                CreateHighlightTexture();
        }

        private void OnDisable()
        {
            base.OnDisable();
            Texture2D.DestroyImmediate(_highlightTexture);
            _highlightTexture = null;

        }

        private void CreateHighlightTexture()
        {
            _highlightTexture = new Texture2D(1, 1);
            _highlightTexture.SetPixel(0, 0, new Color32(50, 125, 255, 255));
            _highlightTexture.Apply();
        }

        void OnGUI()
        {
            //ValidateGUIStyles();
            //DisplayHierarchyPanel();
            //if (_selectionIndex.Count >= 1)
            //    DisplayMainPanel();
            DisplayToolbar();
        }

        private void ValidateGUIStyles()
        {
            if (_whiteLabel == null)
            {
                _whiteLabel = new GUIStyle(EditorStyles.label);
                _whiteLabel.normal.textColor = Color.white;
            }
            if (_whiteFoldout == null)
            {
                _whiteFoldout = new GUIStyle(EditorStyles.foldout);
                _whiteFoldout.normal.textColor = Color.white;
                _whiteFoldout.onNormal.textColor = Color.white;
                _whiteFoldout.active.textColor = Color.white;
                _whiteFoldout.onActive.textColor = Color.white;
                _whiteFoldout.focused.textColor = Color.white;
                _whiteFoldout.onFocused.textColor = Color.white;
            }
            if (_warningLabel == null)
            {
                _warningLabel = new GUIStyle(EditorStyles.largeLabel);
                _warningLabel.alignment = TextAnchor.MiddleCenter;
                _warningLabel.fontStyle = FontStyle.Bold;
                _warningLabel.fontSize = 14;
            }
        }

        private void DisplayHierarchyPanel()
        {
            Rect screenRect = new Rect(0.0f, _toolbarHeight - 5.0f, _hierarchyPanelWidth, position.height - _toolbarHeight + 10.0f);
            Rect scrollView = new Rect(screenRect.x, screenRect.y, screenRect.width, position.height - screenRect.y);

            GUI.Box(screenRect, "");
            GUILayout.BeginArea(scrollView);
            _hierarchyScrollPos = EditorGUILayout.BeginScrollView(_hierarchyScrollPos);
            //GUILayout.Space(5.0f);

            for (int i = 0; i < _inputManager.GetInputConfigurationCount(); i++)
            {
                DisplayHierarchyInputConfigItem(screenRect, i, _inputManager.GetInputConfiguration(i).name);
                if (_inputManager.GetInputConfiguration(i).isExpanded)
                {
                    for (int j = 0; j < _inputManager.GetInputConfiguration(i).axes.Count; j++)
                    {
                        DisplayHierarchiAxisConfigItem(screenRect, i, j, _inputManager.GetInputConfiguration(i).axes[j].name);
                    }
                }
            }

            GUILayout.Space(5.0f);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        // TODO: Add right clicking contextual menu so that they can quickly add new axes etc.
        void DisplayHierarchyInputConfigItem(Rect rect, int index, string name)
        {
            Rect configPos = GUILayoutUtility.GetRect(new GUIContent(name), EditorStyles.foldout, GUILayout.Height(_hierarchyItemHeight));

            //This is where we handle the selection of the configs.
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (configPos.Contains(Event.current.mousePosition))
                {
                    _selectionIndex.Clear();
                    _selectionIndex.Add(index);
                    Repaint();
                }
                else if (rect.Contains(Event.current.mousePosition))
                {
                    _selectionIndex.Clear();
                    Repaint();
                }
            }

            if (_selectionIndex.Count == 1 && _selectionIndex[0] == index)
            {
                if (_highlightTexture == null)
                {
                    CreateHighlightTexture();
                }
                GUI.DrawTexture(configPos, _highlightTexture, ScaleMode.StretchToFill);
                _inputManager.GetInputConfiguration(index).isExpanded = EditorGUI.Foldout(configPos, _inputManager.GetInputConfiguration(index).isExpanded, name, _whiteFoldout);
            }
            else
            {
                _inputManager.GetInputConfiguration(index).isExpanded = EditorGUI.Foldout(configPos, _inputManager.GetInputConfiguration(index).isExpanded, name);
            }
        }

        void DisplayHierarchiAxisConfigItem(Rect rect, int inputConfigIndex, int index, string name)
        {
            Rect configPos = GUILayoutUtility.GetRect(new GUIContent(name), EditorStyles.label, GUILayout.Height(_hierarchyItemHeight));
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (configPos.Contains(Event.current.mousePosition))
                {
                    _editingPositiveKey = false;
                    _editingPositiveKey = false;
                    _editingAltPositiveKey = false;
                    _editingAltNegativeKey = false;
                    _keyString = string.Empty;
                    _selectionIndex.Clear();
                    _selectionIndex.Add(inputConfigIndex);
                    _selectionIndex.Add(index);
                    Event.current.Use();
                    Repaint();
                }
                else if (rect.Contains(Event.current.mousePosition))
                {
                    _selectionIndex.Clear();
                    Repaint();
                }
            }

            if (_selectionIndex.Count == 2 && _selectionIndex[0] == inputConfigIndex &&
                _selectionIndex[1] == index)
            {
                if (_highlightTexture == null)
                {
                    CreateHighlightTexture();
                }
                GUI.DrawTexture(configPos, _highlightTexture, ScaleMode.StretchToFill);

                configPos.x += 20.0f;
                EditorGUI.LabelField(configPos, name, _whiteLabel);
            }
            else
            {
                configPos.x += 20.0f;
                EditorGUI.LabelField(configPos, name);
            } 
        }

        void DisplayMainPanel()
        {
            Rect screenRect = new Rect(_hierarchyPanelWidth + 5.0f, _toolbarHeight + 5,
                                        position.width - (_hierarchyPanelWidth + 5.0f),
                                        position.height - _toolbarHeight - 5.0f);
            InputConfiguration inputConfig = _inputManager.GetInputConfiguration(_selectionIndex[0]);

            if (_selectionIndex.Count < 2)
            {
                DisplayInputConfigurationFields(inputConfig, screenRect);
            }
            else
            {
                AxisConfiguration axisConfig = inputConfig.axes[_selectionIndex[1]];
                DisplayAxisConfigurationFields(inputConfig, axisConfig, screenRect);
            }
        }

        private void DisplayInputConfigurationFields(InputConfiguration inputConfig, Rect screenRect)
        {
            GUILayout.BeginArea(screenRect);
            _mainPanelScrollPos = EditorGUILayout.BeginScrollView(_mainPanelScrollPos);
            inputConfig.name = EditorGUILayout.TextField("Name", inputConfig.name);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = (!EditorApplication.isPlaying && _inputManager.GetDefaultConfiguration() != inputConfig.name);
            if (GUILayout.Button("Make Default", GUILayout.Width(135.0f), GUILayout.Height(20.0f)))
            {
                _inputManager.SetDefaultConfiguration(inputConfig.name);
            }
            //ToDo: When editor is playing, allow on the fly switching of input configs.
            //GUI.enabled = (EditorApplication.isPlaying && InputManager.CurrentConfiguration.name != inputConfig.name);
            //if (GUILayout.Button("Switch To", GUILayout.Width(135.0f), GUILayout.Height(20.0f)))
            //{
            //    InputManager.SetInputConfiguration(inputConfig.name);
            //}
            GUI.enabled = true;
//            EditorUtility.SetDirty(_inputManager);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DisplayAxisConfigurationFields(InputConfiguration inputConfigx, AxisConfiguration axisConfig, Rect screenRect)
        {
            GUIContent gravityInfo = new GUIContent("Gravity", "The speed(in units/sec) at which a digital axis falls towards neutral.");
            GUIContent sensitivityInfo = new GUIContent("Sensitivity", "The speed(in units/sec) at which an axis moves towards the target value.");
            GUIContent snapInfo = new GUIContent("Snap", "If input switches direction, do we snap to neutral and continue from there? For digital axes only.");
            GUIContent deadZoneInfo = new GUIContent("Dead Zone", "Size of analog dead zone. Values within this range map to neutral.");

            GUILayout.BeginArea(screenRect);
            _mainPanelScrollPos = GUILayout.BeginScrollView(_mainPanelScrollPos);
            axisConfig.name = EditorGUILayout.TextField("Name", axisConfig.name);
            axisConfig.description = EditorGUILayout.TextField("Description", axisConfig.description);

            //	Positive Key
            InputEditorTools.KeyCodeField(ref _keyString, ref _editingPositiveKey, "Positive",
                                       "editor_positive_key", axisConfig.positive);
            ProcessKeyString(ref axisConfig.positive, ref _editingPositiveKey);
            //	Negative Key
            InputEditorTools.KeyCodeField(ref _keyString, ref _editingNegativeKey, "Negative",
                                       "editor_negative_key", axisConfig.negative);
            ProcessKeyString(ref axisConfig.negative, ref _editingNegativeKey);
            //	Alt Positive Key
            InputEditorTools.KeyCodeField(ref _keyString, ref _editingAltPositiveKey, "Alt Positive",
                                       "editor_alt_positive_key", axisConfig.altPositive);
            ProcessKeyString(ref axisConfig.altPositive, ref _editingAltPositiveKey);
            //	Alt Negative key
            InputEditorTools.KeyCodeField(ref _keyString, ref _editingAltNegativeKey, "Alt Negative",
                                       "editor_alt_negative_key", axisConfig.altNegative);
            ProcessKeyString(ref axisConfig.altNegative, ref _editingAltNegativeKey);

            axisConfig.gravity = EditorGUILayout.FloatField(gravityInfo, axisConfig.gravity);
            axisConfig.deadZone = EditorGUILayout.FloatField(deadZoneInfo, axisConfig.deadZone);
            axisConfig.sensitivity = EditorGUILayout.FloatField(sensitivityInfo, axisConfig.sensitivity);
            axisConfig.snap = EditorGUILayout.Toggle(snapInfo, axisConfig.snap);
            axisConfig.invert = EditorGUILayout.Toggle("Invert", axisConfig.invert);
            axisConfig.type = (InputType)EditorGUILayout.EnumPopup("Type", axisConfig.type);
            axisConfig.axis = EditorGUILayout.Popup("Axis", axisConfig.axis, _axisOptions);
            axisConfig.joystick = EditorGUILayout.Popup("Joystick", axisConfig.joystick, _joystickOptions);

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.Space();
                GUI.enabled = false;
                EditorGUILayout.FloatField("Raw Axis", axisConfig.GetAxisRaw());
                EditorGUILayout.FloatField("Axis", axisConfig.GetAxis());
                EditorGUILayout.Toggle("Button", axisConfig.GetButton());
                GUI.enabled = true;
            }

            //EditorUtility.SetDirty(_inputManager);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void ProcessKeyString(ref KeyCode key, ref bool isEditing)
        {
            if (isEditing && Event.current.type == EventType.KeyUp)
            {
                key = AxisConfiguration.StringToKey(_keyString);
                if (key == KeyCode.None)
                {
                    _keyString = string.Empty;
                }
                else
                {
                    _keyString = key.ToString();
                }
                isEditing = false;
            }
        }
        private void DisplayToolbar()
        {
            //The rects constructed here determine the look of all the components hence forth.
            Rect screenRect = new Rect(0.0f,0.0f, position.width, _toolbarHeight); //This rect encompasses the toolbars
            Rect fileMenuRect = new Rect(0.0f, 0.0f, _menuWidth, screenRect.height); //This rect is the dimensions of the FileMenu drop down
            Rect editMenuRect = new Rect(fileMenuRect.xMax, 0.0f, _menuWidth, screenRect.height); //this rect is the dimensiosn of the EditMenu drop down
            Rect paddingLabelRect = new Rect(editMenuRect.xMax, 0.0f, screenRect.width - _menuWidth * 2, screenRect.height); //This rect is the dimensions of the padding after the drop down menus

            GUI.BeginGroup(screenRect);
            DisplayFileMenu(fileMenuRect);
            DisplayEditMenu(editMenuRect);
            EditorGUI.LabelField(paddingLabelRect, "", EditorStyles.toolbarButton); //This displays the padding after the menus.
            GUI.EndGroup();

        }

        private void DisplayFileMenu(Rect rect)
        {
            EditorGUI.LabelField(rect, "File", EditorStyles.toolbarDropDown);

            //If someone clicks the dropdown box..
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                rect.Contains(Event.current.mousePosition))
            {
                CreateFileMenu(new Rect(rect.x, rect.yMax, 0.0f,0.0f));
            }
        }

        private void CreateFileMenu(Rect rect)
        {
            GenericMenu fileMenu = new GenericMenu();
            fileMenu.AddItem(new GUIContent("Overwrite Input Settings"), false, HandleFileMenuOption, FileMenuOptions.OverriteInputSettings);
            fileMenu.AddItem(new GUIContent("Default Input Configuration"), false, HandleFileMenuOption, FileMenuOptions.CreateDefaultInputConfig);
            fileMenu.AddItem(new GUIContent("New Input Configuration") , false, HandleFileMenuOption, FileMenuOptions.NewInputConfiguration);
            fileMenu.AddItem(new GUIContent("New Axis Configuration"), false, HandleFileMenuOption, FileMenuOptions.NewAxisConfiguration);

            fileMenu.AddSeparator("");
            fileMenu.AddItem(new GUIContent("Import"), false, HandleFileMenuOption, FileMenuOptions.Import);
            fileMenu.AddItem(new GUIContent("Export"), false, HandleFileMenuOption, FileMenuOptions.Export);

            fileMenu.DropDown(rect);
        }

        private void HandleFileMenuOption(object arg)
        {
            FileMenuOptions option = (FileMenuOptions) arg;
            switch (option)
            {
                    case FileMenuOptions.OverriteInputSettings:
                        InputEditorTools.OverwriteInputSettings();
                        break;
                    case FileMenuOptions.CreateDefaultInputConfig:
                        LoadInputConfigurationsFromResource();                   
                        break;
                    case FileMenuOptions.NewInputConfiguration:
                        CreateNewInputConfiguration();
                        break;
                    case FileMenuOptions.NewAxisConfiguration:
                        CreateNewAxisConfiguration();
                        break;
                    case FileMenuOptions.Import:
                        ImportInputConfigurations();
                        break;
                    case FileMenuOptions.Export:
                        ExportInputConfigurations();
                        break;
            }
        }

        public void CreateNewInputConfiguration()
        {
            _inputManager.AddNewInputConfiguration();
            _selectionIndex.Clear();
            _selectionIndex.Add(_inputManager.GetInputConfigurationCount() -1);
            Repaint();
        }

        public void CreateNewAxisConfiguration()
        {
            if (_selectionIndex.Count >= 1)
            {
                InputConfiguration inputConfig = _inputManager.GetInputConfiguration(_selectionIndex[0]);
                inputConfig.AddNewAxisConfiguration();

                if (_selectionIndex.Count == 2)
                {
                    _selectionIndex[1] = inputConfig.axes.Count - 1;
                }
                else
                {
                    _selectionIndex.Add(inputConfig.axes.Count - 1);
                }
                Repaint();
            }
        }

        public void ExportInputConfigurations()
        {
            
        }

        public void ImportInputConfigurations()
        {
            
        }

        private void LoadInputConfigurationsFromResource()
        {
            //TODO: Work on json serialization
            if (_inputManager.GetInputConfigurationCount() > 0)
            {
                bool cont = EditorUtility.DisplayDialog("Warning", "This operation will replace the current input configrations!\nDo you want to continue?", "Yes", "No");
                if (!cont) return;
            }

            SendLoadPathSignal.Dispatch(INPUT_MANAGER_DEFAULT_CONFIG);
        }

        public void ReplaceCurrentConfigurations(string defaultConfig, List<InputConfiguration> config)
        {
            _inputManager.SetDefaultConfiguration(defaultConfig);
            _inputManager.SetInputConfiguration(config);
            _selectionIndex.Clear();
        }


        private void DisplayEditMenu(Rect rect)
        {
            EditorGUI.LabelField(rect, "Edit", EditorStyles.toolbarDropDown);
        }


    }
}