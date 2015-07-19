/*
 * The Purpose of this Editor is to edit the contents of the Default Input Config. This is the main configurations that will ship
 * with the game, and would represent the configs that would be loaded when the user presses the "Defaults" Button in the control
 * options menu.
 * 
 * Work in Progress Features:
 * Right Click (Context) Menus that make it more intuitive to delete, copy, and add new configs directly to the Hierarchy Menu.
 */


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
        internal Signal LoadInputConfigSignal = new Signal();
        internal Signal<List<InputConfiguration>, string> SaveInputConfigSignal = new Signal<List<InputConfiguration>, string>();
        #endregion

        #region Menu Options
        public enum FileMenuOptions
        {
            NewInputConfiguration = 0, NewAxisConfiguration, Export, Import, OverriteInputSettings, CreateDefaultInputConfig, Save,
        }

        public enum EditMenuOptions
        {
            Duplicate, Delete, DeleteAll, Copy, Paste
        }

        #endregion


        #region Fields

        [SerializeField]
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

        private AxisConfiguration _copySource;
        private GUIStyle _whiteLabel;
        private GUIStyle _whiteFoldout;
        private GUIStyle _warningLabel;

        private bool Loaded = false;

        private bool _isResizingHierarchy = false;
        private bool _editingPositiveKey = false;
        private bool _editingAltPositiveKey = false;
        private bool _editingNegativeKey = false;
        private bool _editingAltNegativeKey = false;

        private float _toolbarHeight = 18.0f;
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

        private bool clickingWithMouse = false;

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
            if (_selectionIndex == null)
                _selectionIndex = new List<int>();
            if (_highlightTexture == null)
                CreateHighlightTexture();
            if (_inputManager == null)
            {
                _inputManager = new InputManager();
                LoadInputConfiguration();
            }

        }

        private void OnDisable()
        {
            base.OnDisable();
            Texture2D.DestroyImmediate(_highlightTexture);
            _highlightTexture = null;

        }

        private void LoadInputConfiguration()
        {
            LoadInputConfigSignal.Dispatch();
        }

        public void SaveInputConfiguration()
        {
            SaveInputConfigSignal.Dispatch(_inputManager.GetAllInputConfigurations(),
                _inputManager.GetDefaultConfiguration());
        }

        private void CreateHighlightTexture()
        {
            _highlightTexture = new Texture2D(1, 1);
            _highlightTexture.SetPixel(0, 0, new Color32(50, 125, 255, 255));
            _highlightTexture.Apply();
        }

        void OnGUI()
        {
            if (Loaded)
            {
                ValidateGUIStyles();
                DisplayHierarchyPanel();
                if (_selectionIndex.Count >= 1)
                     DisplayMainPanel();
            }
            UpdateHierarchyPanelWidth();
            
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
            GUILayout.Space(5.0f);

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

        void Update()
        { }

        // TODO: Add right clicking contextual menu so that they can quickly add new axes etc.
        void DisplayHierarchyInputConfigItem(Rect rect, int index, string name)
        {
            Rect configPos = GUILayoutUtility.GetRect(new GUIContent(name), EditorStyles.foldout, GUILayout.Height(_hierarchyItemHeight));

            //This is where we handle the selection of the configs.
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
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

        void CreateInputConfigContextMenu()
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("New Axis Configuration"), false, HandleFileMenuOption,
                FileMenuOptions.NewAxisConfiguration);
            contextMenu.AddItem(new GUIContent("Delete                Del"), false, HandleEditMenuOption,
                EditMenuOptions.Delete);
            contextMenu.ShowAsContext();

        }

        void CreateAxisConfigContextMenu()
        {
            GenericMenu contextMenu = new GenericMenu();
            //contextMenu.AddItem(new GUIContent("New Axis Configuration"), false, HandleFileMenuOption, FileMenuOptions.NewAxisConfiguration);
            contextMenu.AddItem(new GUIContent("Delete                Del"), false, HandleEditMenuOption, EditMenuOptions.Delete);
            contextMenu.ShowAsContext();
           
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
                if (Event.current.type == EventType.Repaint)

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
            Rect saveFieldRect = new Rect(editMenuRect.xMax, 0.0f, _menuWidth, screenRect.height);

            Rect paddingLabelRect = new Rect(saveFieldRect.xMax, 0.0f, screenRect.width - _menuWidth * 2, screenRect.height); //This rect is the dimensions of the padding after the drop down menus



            GUI.BeginGroup(screenRect);
            DisplayFileMenu(fileMenuRect);
            DisplayEditMenu(editMenuRect);
            DisplaySaveButton(saveFieldRect);
            EditorGUI.LabelField(paddingLabelRect, "", EditorStyles.toolbarButton); //This displays the padding after the menus.
            GUI.EndGroup();

        }

        private void DisplaySaveButton(Rect rect)
        {
            if (GUI.Button(rect, "Save", EditorStyles.toolbarButton))
                Save();
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
            if (_selectionIndex.Count >= 1)
                fileMenu.AddItem(new GUIContent("New Axis Configuration"), false, HandleFileMenuOption, FileMenuOptions.NewAxisConfiguration);
            else
                fileMenu.AddDisabledItem(new GUIContent("New Axis"));

            fileMenu.AddSeparator("");
            fileMenu.AddItem(new GUIContent("Import"), false, HandleFileMenuOption, FileMenuOptions.Import);
            fileMenu.AddItem(new GUIContent("Export"), false, HandleFileMenuOption, FileMenuOptions.Export);
            fileMenu.AddSeparator("");
            fileMenu.AddItem(new GUIContent("Save"), false, HandleFileMenuOption, FileMenuOptions.Save);


            fileMenu.DropDown(rect);
        }



        private void DisplayEditMenu(Rect rect)
        {
            EditorGUI.LabelField(rect, "Edit", EditorStyles.toolbarDropDown);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                rect.Contains(Event.current.mousePosition))
            {
                CreateEditMenu(new Rect(rect.x, rect.yMax, 0.0f, 0.0f));
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.D && Event.current.shift)
                {
                    Duplicate();
                    Event.current.Use();
                }
                if (Event.current.keyCode == KeyCode.Delete)
                {
                    Delete();
                    Event.current.Use();
                }
            }
        }

        private void CreateEditMenu(Rect rect)
        {
            GenericMenu editMenu = new GenericMenu();
            if (_selectionIndex.Count > 0)
                editMenu.AddItem(new GUIContent("Duplicate          Shift+D"), false, HandleEditMenuOption, EditMenuOptions.Duplicate);
            else
                editMenu.AddDisabledItem(new GUIContent("Duplicate          Shift+D"));

            if (_selectionIndex.Count > 0)
                editMenu.AddItem(new GUIContent("Delete                Del"), false, HandleEditMenuOption, EditMenuOptions.Delete);
            else
                editMenu.AddDisabledItem(new GUIContent("Delete                Del"));

            if (_inputManager.GetInputConfigurationCount() > 0)
                editMenu.AddItem(new GUIContent("Delete All"), false, HandleEditMenuOption, EditMenuOptions.DeleteAll);
            else
                editMenu.AddDisabledItem(new GUIContent("Delete All"));

            if (_selectionIndex.Count >= 2)
                editMenu.AddItem(new GUIContent("Copy"), false, HandleEditMenuOption, EditMenuOptions.Copy);
            else
                editMenu.AddDisabledItem(new GUIContent("Copy"));

            if (_copySource != null && _selectionIndex.Count >= 2)
                editMenu.AddItem(new GUIContent("Paste"), false, HandleEditMenuOption, EditMenuOptions.Paste);
            else
                editMenu.AddDisabledItem(new GUIContent("Paste"));

            //editMenu.AddSeparator("");
            editMenu.DropDown(rect);

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
                    case FileMenuOptions.Save:
                        Save();
                        break;
            }
        }

        private void HandleEditMenuOption(object arg)
        {
            EditMenuOptions option = (EditMenuOptions)arg;
            switch (option)
            {
                case EditMenuOptions.Duplicate:
                    Duplicate();
                    break;
                case EditMenuOptions.Delete:
                    Delete();
                    break;
                case EditMenuOptions.DeleteAll:
                    DeleteAll();
                    break;
                case EditMenuOptions.Copy:
                    CopySelectedAxisConfig();
                    break;
                case EditMenuOptions.Paste:
                    PasteAxisConfig();
                    break;
            }
        }
        private void Save()
        {
            bool cont = EditorUtility.DisplayDialog("Save?", "This operation will save all modifications to the default input file!\nDo you want to continue?", "Yes", "No");
            if (!cont) return;
            Debug.Log("hi");
            SaveInputConfigSignal.Dispatch(_inputManager.GetAllInputConfigurations(), _inputManager.GetDefaultConfiguration());
            
        }

        private void Duplicate()
        {
            if (_selectionIndex.Count == 1)
            {
                DuplicateInputConfiguration();
            }
            else if (_selectionIndex.Count == 2)
            {
                DuplicateAxisConfiguration();
            }
        }

        private void DuplicateAxisConfiguration()
        {
            InputConfiguration inputConfig = _inputManager.GetInputConfiguration(_selectionIndex[0]);
            AxisConfiguration source = inputConfig.axes[_selectionIndex[1]];
            AxisConfiguration axisConfig = AxisConfiguration.Duplicate(source);
            if (_selectionIndex[1] < inputConfig.axes.Count - 1)
            {
                inputConfig.axes.Insert(_selectionIndex[1], axisConfig);
                _selectionIndex[1]++;
            }
            else
            {
                inputConfig.axes.Add(axisConfig);
                _selectionIndex[1] = inputConfig.axes.Count - 1;
            }
            //if (_searchString.Length > 0)
            //{
            //    UpdateSearchResults();
            //}
            Repaint();
        }

        private void DuplicateInputConfiguration()
        {
            InputConfiguration source = _inputManager.GetInputConfiguration(_selectionIndex[0]);
            InputConfiguration inputConfig = InputConfiguration.Duplicate(source);
            if (_selectionIndex[0] < _inputManager.GetInputConfigurationCount() - 1)
            {
                _inputManager.InsertInputConfiguration(_selectionIndex[0] + 1, inputConfig);

                _selectionIndex[0]++;
            }
            else
            {
                _inputManager.AddInputConfiguration(inputConfig);
                _selectionIndex[0] = _inputManager.GetInputConfigurationCount() - 1;
            }
            //if (_searchString.Length > 0)
            //{
            //    UpdateSearchResults();
            //}
            Repaint();
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
            string file = EditorUtility.SaveFilePanel("Export input profile", "", "profile.xml", "xml");
            if (string.IsNullOrEmpty(file))
                return;

            InputSaverXML inputSaver = new InputSaverXML(file);
            inputSaver.Save(_inputManager.GetAllInputConfigurations(), _inputManager.GetDefaultConfiguration());
            if (file.StartsWith(Application.dataPath))
                AssetDatabase.Refresh();
        }

        private void CopySelectedAxisConfig()
        {
            if (_copySource == null)
                _copySource = new AxisConfiguration();

            InputConfiguration inputConfig = _inputManager.GetInputConfiguration(_selectionIndex[0]);
            AxisConfiguration axisConfig = inputConfig.axes[_selectionIndex[1]];
            _copySource.Copy(axisConfig);
        }

        private void PasteAxisConfig()
        {
            InputConfiguration inputConfig = _inputManager.GetInputConfiguration(_selectionIndex[0]);
            AxisConfiguration axisConfig = inputConfig.axes[_selectionIndex[1]];
            axisConfig.Copy(_copySource);
        }


        public void ImportInputConfigurations()
        {
            string file = EditorUtility.OpenFilePanel("Import input profile", "", "xml");
            if (string.IsNullOrEmpty(file))
                return;

            bool replace = EditorUtility.DisplayDialog("Replace or Append", "Do you want to replace the current input configrations?", "Replace", "Append");
            if (replace)
            {
                string defaultConfig = "";
                List<InputConfiguration> configs = new List<InputConfiguration>();
                InputLoaderXML inputLoader = new InputLoaderXML(file);
                inputLoader.Load(out configs, out defaultConfig);
                SetCurrentConfigurations(configs,defaultConfig);
                _selectionIndex.Clear();
            }
            else
            {
                List<InputConfiguration> configurations;
                string defaultConfig;

                InputLoaderXML inputLoader = new InputLoaderXML(file);
                inputLoader.Load(out configurations, out defaultConfig);
                if (configurations != null && configurations.Count > 0)
                {
                    foreach (var config in configurations)
                    {
                        _inputManager.AddInputConfiguration(config);
                    }

                }
            }

            //if (_searchString.Length > 0)
            //{
            //    UpdateSearchResults();
            //}
        }

        private void LoadInputConfigurationsFromResource()
        {
            //TODO: Currently this does nothing.... Would like it if there was a backup input config just in case everything got deleted.
            if (_inputManager.GetInputConfigurationCount() > 0)
            {
                bool cont = EditorUtility.DisplayDialog("Warning", "This operation will replace the current input configrations!\nDo you want to continue?", "Yes", "No");
                if (!cont) return;
            }

            //TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
            //if (textAsset != null)
            //{
            //    using (System.IO.StringReader reader = new System.IO.StringReader(textAsset.text))
            //    {
            //        InputLoaderXML inputLoader = new InputLoaderXML(reader);
            //        inputLoader.Load(out _inputManager.inputConfigurations, out _inputManager.defaultConfiguration);
            //        _selectionPath.Clear();
            //    }
            //}
            //else
            //{
            //    EditorUtility.DisplayDialog("Error", "Failed to load input configurations. The resource file might have been deleted or renamed.", "OK");
            //}
        }

        public void SetCurrentConfigurations(List<InputConfiguration> config, string defaultConfig)
        {
            _inputManager.SetDefaultConfiguration(defaultConfig);
            _inputManager.SetInputConfiguration(config);
            _selectionIndex.Clear();
            Loaded = true;
        }




        private void Delete()
        {
            if (_selectionIndex.Count == 1)
            {
                _inputManager.RemoveInputConfig(_selectionIndex[0]);
                Repaint();
            }
            else if (_selectionIndex.Count == 2)
            {
                _inputManager.RemoveAxisConfig(_selectionIndex[0],_selectionIndex[1]);
                Repaint();
            }
            if (_inputManager.GetInputConfigurationCount() == 0)
            {
                _inputManager.SetDefaultConfiguration(string.Empty);
            }
            _selectionIndex.Clear();
            //if (_searchString.Length > 0)
            //{
            //    UpdateSearchResults();
            //}
        }

        private void DeleteAll()
        {
            _inputManager.RemoveAll();
            _selectionIndex.Clear();
            //if (_searchString.Length > 0)
            //{
            //    UpdateSearchResults();
            //}
            Repaint();
        }

        private void UpdateHierarchyPanelWidth()
        {
            float cursorRectWidth = _isResizingHierarchy ? _maxCursorRectWidth : _minCursorRectWidth;
            Rect cursorRect = new Rect(_hierarchyPanelWidth - cursorRectWidth / 2, _toolbarHeight, cursorRectWidth,
                                        position.height - _toolbarHeight);
            Rect resizeRect = new Rect(_hierarchyPanelWidth - _minCursorRectWidth / 2, 0.0f,
                                        _minCursorRectWidth, position.height);

            EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.ResizeHorizontal);
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && resizeRect.Contains(Event.current.mousePosition))
                    {
                        _isResizingHierarchy = true;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (Event.current.button == 0 && _isResizingHierarchy)
                    {
                        _isResizingHierarchy = false;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (_isResizingHierarchy)
                    {
                        _hierarchyPanelWidth = Mathf.Clamp(_hierarchyPanelWidth + Event.current.delta.x,
                                                         _minHierarchyPanelWidth, position.width / 2);
                        Event.current.Use();
                        Repaint();
                    }
                    break;
                default:
                    break;
            }
        }

    }
}