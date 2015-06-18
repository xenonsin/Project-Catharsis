using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.injector.api;
using strange.extensions.signal.impl;

namespace Catharsis.InputEditor
{

    /// <summary>
    /// Encapsulates a method that takes one parameter(the key) and returns 'true' if
    /// the key is accepted or 'false' if it isn't.
    /// The 'userData' parameter is used to send additional user data.
    /// </summary>
    public delegate bool KeyScanHandler(KeyCode key, object[] userData);

    /// <summary>
    /// Encapsulates a method that takes one parameter(the axis) and returns 'true' if
    /// the axis is accepted or 'false' if it isn't.
    /// The 'userData' parameter is used to send additional user data.
    /// </summary>
    public delegate bool AxisScanHandler(int axis, object[] userData);

    /// <summary>
    /// Encapsulates a method that takes one parameter(the scan result) and returns 'true' if
    /// the scan result is accepted or 'false' if it isn't.
    /// </summary>
    public delegate bool ScanHandler(ScanResult result);

    [Implements(typeof (IInputManager), InjectionBindingScope.CROSS_CONTEXT)]
    public class InputManager : IInputManager
    {


        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        //TODO: Inject this in ever first context, same with the Input Manager.

        [Inject]
        public IRoutineRunner RoutienRunner { get; set; }

        //TODO: Hook this up.
        [Inject]
        public InputManagerConfigurationChangedSignal ConfigurationChangedSignal { get; set; }
        [Inject]
        public InputManagerConfigurationDirtySignal ConfigurationDirtySignal { get; set; }
        [Inject]
        public InputManagerLoadedSignal LoadedSignal { get; set; }
        [Inject]
        public InputManagerSavedSignal SavedSignal { get; set; }
        [Inject]
        public InputManagerRemoteUpdateSignal RemoteUpdateSignal { get; set; }

            #region Fields

        //Bug: This doesn't allow for configs that are the same name. Doesn't disallow the player to create duplicates.
        [SerializeField]
        private string defaultConfiguration;

        [SerializeField] //Note: Might change this to it's own seperate editor script.
        private List<InputConfiguration> _inputConfigurations = new List<InputConfiguration>();

        private InputConfiguration _currentConfiguration;


        public bool IgnoreTimeScale { get; set; }
        public bool IsScanning { get { return _scanFlags != ScanFlags.None; } }
        private ScanFlags _scanFlags;
        private ScanResult _scanResult;
        private ScanHandler _scanHandler;
        private object _scanUserData;
        private string _cancelScanButton;
        private float _scanStartTime;
        private float _scanTimeout;
        private int _scanJoystick;

        //Cached data
        private string[] _rawMouseAxes;
        private string[] _rawJoystickAxes;
        private KeyCode[] _keys;
        private Dictionary<string, InputConfiguration> _configurationTable; //Holds all active configurations
        private Dictionary<string, Dictionary<string, AxisConfiguration>> _axesTable; //Holds current axes Table



        public void AddNewInputConfiguration()
        {
            _inputConfigurations.Add(new InputConfiguration());
        }

        public int GetInputConfigurationCount()
        {
            return _inputConfigurations.Count;
        }

        public InputConfiguration GetInputConfiguration(int index)
        {
            return _inputConfigurations[index];
        }

        public void SetInputConfiguration(List<InputConfiguration> config)
        {
            _inputConfigurations = config;
        }

        public InputConfiguration GetCurrentInputConfiguration()
        {
            return _currentConfiguration;

        }

        public string GetDefaultConfiguration()
        {
            return defaultConfiguration;
        }

        public void SetDefaultConfiguration(string name)
        {
            defaultConfiguration = name;
        }



        #endregion


        [PostConstruct]
        public void PostConstruct()
        {
            _keys = (KeyCode[]) Enum.GetValues(typeof (KeyCode));
            IgnoreTimeScale = true;
            _configurationTable = new Dictionary<string, InputConfiguration>();
            _axesTable = new Dictionary<string, Dictionary<string, AxisConfiguration>>();

            //SEND LOAD SIGNAL TO GET COMMAND..

            SetRawAxisNames();
            Init();

            RoutienRunner.StartCoroutine(update());
        }

        private void SetRawAxisNames()
        {
            _rawMouseAxes = new string[AxisConfiguration.MaxMouseAxes];
            for (int i = 0; i < _rawMouseAxes.Length; i++)
            {
                _rawMouseAxes[i] = string.Concat("mouse_axis_", i);
            }

            _rawJoystickAxes = new string[AxisConfiguration.MaxJoysticks * AxisConfiguration.MaxJoystickAxes];
            for (int i = 0; i < AxisConfiguration.MaxJoysticks; i++)
            {
                for (int j = 0; j < AxisConfiguration.MaxJoystickAxes; j++)
                {
                    _rawJoystickAxes[i * AxisConfiguration.MaxJoystickAxes + j] = string.Concat("joy_", i, "_axis_", j);
                }
            }
        }

        private void Init()
        {
            if (_inputConfigurations.Count == 0)
                return;

            PopulateLookupTables();
            if (string.IsNullOrEmpty(defaultConfiguration) || !_configurationTable.ContainsKey(defaultConfiguration))
                _currentConfiguration = _inputConfigurations[0];
            else
                _currentConfiguration = _configurationTable[defaultConfiguration];

            foreach (InputConfiguration inputConfig in _inputConfigurations)
            {
                foreach (AxisConfiguration axisConfig in inputConfig.axes)
                {
                    axisConfig.Initialize();
                }
            }
            ResetInputAxes();
        }

        private void PopulateLookupTables()
        {
            _configurationTable.Clear();
            foreach (InputConfiguration inputConfig in _inputConfigurations)
            {
                if (!_configurationTable.ContainsKey(inputConfig.name))
                {
                    _configurationTable.Add(inputConfig.name, inputConfig);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("An input configuration named \'" + inputConfig.name + "\' already exists in the lookup table");
                }
#endif
            }

            _axesTable.Clear();
            foreach (InputConfiguration inputConfig in _inputConfigurations)
            {
                Dictionary<string, AxisConfiguration> table = new Dictionary<string, AxisConfiguration>();
                foreach (AxisConfiguration axisConfig in inputConfig.axes)
                {
                    if (!table.ContainsKey(axisConfig.name))
                    {
                        table.Add(axisConfig.name, axisConfig);
                    }
#if UNITY_EDITOR
                    else
                    {
                        Debug.LogWarning(string.Format("Input configuration \'{0}\' already contains an axis named \'{1}\'", inputConfig.name, axisConfig.name));
                    }
#endif
                }

                _axesTable.Add(inputConfig.name, table);
            }
        }

        protected IEnumerator update()
        {
            while (true)
            {
                if (_currentConfiguration != null)
                {
                    int count = _currentConfiguration.axes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        _currentConfiguration.axes[i].Update();
                    }

                    RemoteUpdateSignal.Dispatch();

                    if (_scanFlags != ScanFlags.None)
                        ScanInput();
                }
                else
                {
                    if (_scanFlags != ScanFlags.None)
                        StopInputScan();
                }

                yield return null;
            }
        }

        private void RaiseInputConfigurationChangedEvent(string configName)
        {
            ConfigurationChangedSignal.Dispatch(configName);
        }

        private void RaiseConfigurationDirtyEvent(string configName)
        {
            ConfigurationDirtySignal.Dispatch(configName);
        }

        private void RaiseLoadedEvent()
        {
            LoadedSignal.Dispatch();
        }

        private void RaiseSavedEvent()
        {
            SavedSignal.Dispatch();
        }

        private void ScanInput()
        {
            float timeout = IgnoreTimeScale ? (Time.realtimeSinceStartup - _scanStartTime) : (Time.time - _scanStartTime);
            if (!string.IsNullOrEmpty(_cancelScanButton) && GetButtonDown(_cancelScanButton) || timeout >= _scanTimeout)
            {
                StopInputScan();
                return;
            }

            bool scanSuccess = false;
            if (((int)_scanFlags & (int)ScanFlags.Key) == (int)ScanFlags.Key)
            {
                scanSuccess = ScanKey();
            }
            if (!scanSuccess && (((int)_scanFlags & (int)ScanFlags.JoystickButton) == (int)ScanFlags.JoystickButton))
            {
                scanSuccess = ScanJoystickButton();
            }
            if (!scanSuccess && (((int)_scanFlags & (int)ScanFlags.JoystickAxis) == (int)ScanFlags.JoystickAxis))
            {
                scanSuccess = ScanJoystickAxis();
            }
            if (!scanSuccess && (((int)_scanFlags & (int)ScanFlags.MouseAxis) == (int)ScanFlags.MouseAxis))
            {
                ScanMouseAxis();
            }
        }

        private bool ScanKey()
        {
            int length = _keys.Length;
            for (int i = 0; i < length; i++)
            {
                if ((int)_keys[i] >= (int)KeyCode.JoystickButton0)
                    break;

                if (Input.GetKeyDown(_keys[i]))
                {
                    _scanResult.scanFlags = ScanFlags.Key;
                    _scanResult.key = _keys[i];
                    _scanResult.joystick = -1;
                    _scanResult.joystickAxis = -1;
                    _scanResult.mouseAxis = -1;
                    _scanResult.userData = _scanUserData;
                    if (_scanHandler(_scanResult))
                    {
                        _scanHandler = null;
                        _scanResult.userData = null;
                        _scanFlags = ScanFlags.None;
                        return true;
                    }
                }
            }

            return false;
        }


        private bool ScanJoystickButton()
        {
            for (int key = (int)KeyCode.JoystickButton0; key < (int)KeyCode.Joystick4Button19; key++)
            {
                if (Input.GetKeyDown((KeyCode)key))
                {
                    _scanResult.scanFlags = ScanFlags.JoystickButton;
                    _scanResult.key = (KeyCode)key;
                    _scanResult.joystick = -1;
                    _scanResult.joystickAxis = -1;
                    _scanResult.mouseAxis = -1;
                    _scanResult.userData = _scanUserData;
                    if (_scanHandler(_scanResult))
                    {
                        _scanHandler = null;
                        _scanResult.userData = null;
                        _scanFlags = ScanFlags.None;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ScanJoystickAxis()
        {
            int scanStart = _scanJoystick * AxisConfiguration.MaxJoystickAxes;
            for (int i = 0; i < AxisConfiguration.MaxJoystickAxes; i++)
            {
                if (Mathf.Abs(Input.GetAxisRaw(_rawJoystickAxes[scanStart + i])) >= 1.0f)
                {
                    _scanResult.scanFlags = ScanFlags.JoystickAxis;
                    _scanResult.key = KeyCode.None;
                    _scanResult.joystick = _scanJoystick;
                    _scanResult.joystickAxis = i;
                    _scanResult.mouseAxis = -1;
                    _scanResult.userData = _scanUserData;
                    if (_scanHandler(_scanResult))
                    {
                        _scanHandler = null;
                        _scanResult.userData = null;
                        _scanFlags = ScanFlags.None;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ScanMouseAxis()
        {
            for (int i = 0; i < _rawMouseAxes.Length; i++)
            {
                if (Mathf.Abs(Input.GetAxis(_rawMouseAxes[i])) > 0.0f)
                {
                    _scanResult.scanFlags = ScanFlags.MouseAxis;
                    _scanResult.key = KeyCode.None;
                    _scanResult.joystick = -1;
                    _scanResult.joystickAxis = -1;
                    _scanResult.mouseAxis = i;
                    _scanResult.userData = _scanUserData;
                    if (_scanHandler(_scanResult))
                    {
                        _scanHandler = null;
                        _scanResult.userData = null;
                        _scanFlags = ScanFlags.None;
                        return true;
                    }
                }
            }

            return false;
        }

        private void StopInputScan()
        {
            _scanResult.scanFlags = ScanFlags.None;
            _scanResult.key = KeyCode.None;
            _scanResult.joystick = -1;
            _scanResult.joystickAxis = -1;
            _scanResult.mouseAxis = -1;
            _scanResult.userData = _scanUserData;

            _scanHandler(_scanResult);

            _scanHandler = null;
            _scanResult.userData = null;
            _scanFlags = ScanFlags.None;
        }

    /// <summary>
    /// Returns true if any axis of the active input configuration is receiving input.
    /// </summary>
    public  bool AnyInput()
    {
        InputConfiguration inputConfig = _currentConfiguration;
        if (inputConfig != null)
        {
            int count = inputConfig.axes.Count;
            for (int i = 0; i < count; i++)
            {
                if (inputConfig.axes[i].AnyInput)
                    return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Returns true if any axis of the specified input configuration is receiving input.
    /// If the specified input configuration is not active and the axis is of type
    /// DigialAxis, RemoteAxis, RemoteButton or AnalogButton this method will return false.
    /// </summary>
    public  bool AnyInput(string inputConfigName)
    {
        InputConfiguration inputConfig;
        if (_configurationTable.TryGetValue(inputConfigName, out inputConfig))
        {
            int count = inputConfig.axes.Count;
            for (int i = 0; i < count; i++)
            {
                if (inputConfig.axes[i].AnyInput)
                    return true;
            }
        }

        return false;
    }


    /// <summary>
    /// If an axis with the requested name exists, and it is of type 'RemoteAxis', the axis' value will be changed.
    /// </summary>
    public  void SetRemoteAxisValue(string axisName, float value)
    {
        SetRemoteAxisValue(_currentConfiguration.name, axisName, value);
    }

    /// <summary>
    /// If an axis with the requested name exists, and it is of type 'RemoteAxis', the axis' value will be changed.
    /// </summary>
    public  void SetRemoteAxisValue(string inputConfigName, string axisName, float value)
    {
        AxisConfiguration axisConfig = GetAxisConfiguration(inputConfigName, axisName);
        if (axisConfig == null)
            throw new ArgumentException(string.Format("An axis named \'{0}\' does not exist in the input configuration named \'{1}\'", axisName, inputConfigName));

        axisConfig.SetRemoteAxisValue(value);
    }

    /// <summary>
    /// If an button with the requested name exists, and it is of type 'RemoteButton', the button's state will be changed.
    /// </summary>
    public  void SetRemoteButtonValue(string buttonName, bool down, bool justChanged)
    {
        SetRemoteButtonValue(_currentConfiguration.name, buttonName, down, justChanged);
    }

    /// <summary>
    /// If an button with the requested name exists, and it is of type 'RemoteButton', the button's state will be changed.
    /// </summary>
    public  void SetRemoteButtonValue(string inputConfigName, string buttonName, bool down, bool justChanged)
    {
        AxisConfiguration axisConfig = GetAxisConfiguration(inputConfigName, buttonName);
        if (axisConfig == null)
            throw new ArgumentException(string.Format("A remote button named \'{0}\' does not exist in the input configuration named \'{1}\'", buttonName, inputConfigName));

        axisConfig.SetRemoteButtonValue(down, justChanged);
    }

    /// <summary>
    /// Resets the internal state of the input manager.
    /// </summary>
    public  void Reinitialize()
    {
        Init();
    }

    /// <summary>
    /// Changes the active input configuration.
    /// </summary>

    
    public  void SetInputConfiguration(string name)
    {
        if (_currentConfiguration != null && name == _currentConfiguration.name)
            return;

        if (_configurationTable.TryGetValue(name, out _currentConfiguration))
        {
            ResetInputAxes();
           RaiseInputConfigurationChangedEvent(name);
        }
        else
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", name));
        }
    }

    public  InputConfiguration GetInputConfiguration(string name)
    {
        InputConfiguration inputConfig = null;
        if (_configurationTable.TryGetValue(name, out inputConfig))
            return inputConfig;

        return null;
    }

    public  AxisConfiguration GetAxisConfiguration(string inputConfigName, string axisName)
    {
        Dictionary<string, AxisConfiguration> table;
        if (_axesTable.TryGetValue(inputConfigName, out table))
        {
            AxisConfiguration axisConfig;
            if (table.TryGetValue(axisName, out axisConfig))
                return axisConfig;
        }

        return null;
    }

    public  InputConfiguration CreateInputConfiguration(string name)
    {
        if (_configurationTable.ContainsKey(name))
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' already exists", name));

        InputConfiguration inputConfig = new InputConfiguration(name);
        _inputConfigurations.Add(inputConfig);
        _configurationTable.Add(name, inputConfig);
        _axesTable.Add(name, new Dictionary<string, AxisConfiguration>());

        return inputConfig;
    }

    /// <summary>
    /// Deletes the specified input configuration. If the speficied input configuration is
    /// active the input manager will try to switch to the default input configuration.
    /// If a default input configuration has not been set in the inspector, the active
    /// input configuration will become null and the input manager will stop working.
    /// </summary>
    public  bool DeleteInputConfiguration(string name)
    {
        InputConfiguration inputConfig = GetInputConfiguration(name);
        if (inputConfig == null)
            return false;

        _axesTable.Remove(name);
        _configurationTable.Remove(name);
        _inputConfigurations.Remove(inputConfig);
        if (_currentConfiguration.name == inputConfig.name)
        {
            if (_inputConfigurations.Count == 0 || string.IsNullOrEmpty(defaultConfiguration) ||
               !_configurationTable.ContainsKey(defaultConfiguration))
            {
                _currentConfiguration = null;
            }
            else
            {
                _currentConfiguration = _configurationTable[defaultConfiguration];
            }
        }

        return true;
    }

    public  AxisConfiguration CreateButton(string inputConfigName, string buttonName, KeyCode primaryKey)
    {
        return CreateButton(inputConfigName, buttonName, primaryKey, KeyCode.None);
    }

    public  AxisConfiguration CreateButton(string inputConfigName, string buttonName, KeyCode primaryKey, KeyCode secondaryKey)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(buttonName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, buttonName);
            throw new ArgumentException(error);
        }

        AxisConfiguration axisConfig = new AxisConfiguration(buttonName);
        axisConfig.type = InputType.Button;
        axisConfig.positive = primaryKey;
        axisConfig.altPositive = secondaryKey;
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(buttonName, axisConfig);

        return axisConfig;
    }

    public  AxisConfiguration CreateDigitalAxis(string inputConfigName, string axisName, KeyCode positive, KeyCode negative, float gravity, float sensitivity)
    {
        return CreateDigitalAxis(inputConfigName, axisName, positive, negative, KeyCode.None, KeyCode.None, gravity, sensitivity);
    }

    public  AxisConfiguration CreateDigitalAxis(string inputConfigName, string axisName, KeyCode positive, KeyCode negative,
                                                      KeyCode altPositive, KeyCode altNegative, float gravity, float sensitivity)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(axisName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, axisName);
            throw new ArgumentException(error);
        }

        AxisConfiguration axisConfig = new AxisConfiguration(axisName);
        axisConfig.type = InputType.DigitalAxis;
        axisConfig.positive = positive;
        axisConfig.negative = negative;
        axisConfig.altPositive = altPositive;
        axisConfig.altNegative = altNegative;
        axisConfig.gravity = gravity;
        axisConfig.sensitivity = sensitivity;
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(axisName, axisConfig);

        return axisConfig;
    }

    public  AxisConfiguration CreateMouseAxis(string inputConfigName, string axisName, int axis, float sensitivity)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(axisName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, axisName);
            throw new ArgumentException(error);
        }
        if (axis < 0 || axis > 2)
            throw new ArgumentOutOfRangeException("axis");

        AxisConfiguration axisConfig = new AxisConfiguration(axisName);
        axisConfig.type = InputType.MouseAxis;
        axisConfig.axis = axis;
        axisConfig.sensitivity = sensitivity;
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(axisName, axisConfig);

        return axisConfig;
    }

    public  AxisConfiguration CreateAnalogAxis(string inputConfigName, string axisName, int joystick, int axis, float sensitivity, float deadZone)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(axisName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, axisName);
            throw new ArgumentException(error);
        }
        if (axis < 0 || axis >= AxisConfiguration.MaxJoystickAxes)
            throw new ArgumentOutOfRangeException("axis");
        if (joystick < 0 || joystick >= AxisConfiguration.MaxJoysticks)
            throw new ArgumentOutOfRangeException("joystick");

        AxisConfiguration axisConfig = new AxisConfiguration(axisName);
        axisConfig.type = InputType.AnalogAxis;
        axisConfig.axis = axis;
        axisConfig.joystick = joystick;
        axisConfig.deadZone = deadZone;
        axisConfig.sensitivity = sensitivity;
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(axisName, axisConfig);

        return axisConfig;
    }

    public  AxisConfiguration CreateRemoteAxis(string inputConfigName, string axisName)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(axisName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, axisName);
            throw new ArgumentException(error);
        }

        AxisConfiguration axisConfig = new AxisConfiguration(axisName);
        axisConfig.type = InputType.RemoteAxis;
        axisConfig.positive = KeyCode.None;
        axisConfig.negative = KeyCode.None;
        axisConfig.altPositive = KeyCode.None;
        axisConfig.altNegative = KeyCode.None;
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(axisName, axisConfig);

        return axisConfig;
    }

    public  AxisConfiguration CreateRemoteButton(string inputConfigName, string buttonName)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(buttonName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, buttonName);
            throw new ArgumentException(error);
        }

        AxisConfiguration axisConfig = new AxisConfiguration(buttonName);
        axisConfig.type = InputType.RemoteButton;
        axisConfig.positive = KeyCode.None;
        axisConfig.negative = KeyCode.None;
        axisConfig.altPositive = KeyCode.None;
        axisConfig.altNegative = KeyCode.None;
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(buttonName, axisConfig);

        return axisConfig;
    }

    public  AxisConfiguration CreateAnalogButton(string inputConfigName, string buttonName, int joystick, int axis)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(buttonName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, buttonName);
            throw new ArgumentException(error);
        }
        if (axis < 0 || axis >= AxisConfiguration.MaxJoystickAxes)
            throw new ArgumentOutOfRangeException("axis");
        if (joystick < 0 || joystick >= AxisConfiguration.MaxJoysticks)
            throw new ArgumentOutOfRangeException("joystick");

        AxisConfiguration axisConfig = new AxisConfiguration(buttonName);
        axisConfig.type = InputType.AnalogButton;
        axisConfig.joystick = joystick;
        axisConfig.axis = axis;
        axisConfig.positive = KeyCode.None;
        axisConfig.negative = KeyCode.None;
        axisConfig.altPositive = KeyCode.None;
        axisConfig.altNegative = KeyCode.None;
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(buttonName, axisConfig);

        return axisConfig;
    }

    public  AxisConfiguration CreateEmptyAxis(string inputConfigName, string axisName)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        if (inputConfig == null)
        {
            throw new ArgumentException(string.Format("An input configuration named \'{0}\' does not exist", inputConfigName));
        }
        if (_axesTable[inputConfigName].ContainsKey(axisName))
        {
            string error = string.Format("The input configuration named {0} already contains an axis configuration named {1}", inputConfigName, axisName);
            throw new ArgumentException(error);
        }

        AxisConfiguration axisConfig = new AxisConfiguration(axisName);
        axisConfig.Initialize();
        inputConfig.axes.Add(axisConfig);

        var table = _axesTable[inputConfigName];
        table.Add(axisName, axisConfig);

        return axisConfig;
    }

    public  bool DeleteAxisConfiguration(string inputConfigName, string axisName)
    {
        InputConfiguration inputConfig = GetInputConfiguration(inputConfigName);
        AxisConfiguration axisConfig = GetAxisConfiguration(inputConfigName, axisName);
        if (inputConfig != null && axisConfig != null)
        {
            _axesTable[inputConfig.name].Remove(axisConfig.name);
            inputConfig.axes.Remove(axisConfig);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Scans for keyboard input and calls the handler with the result.
    /// Returns KeyCode.None if timeout is reached or the scan is canceled.
    /// </summary>
    public  void StartKeyScan(KeyScanHandler scanHandler, float timeout, string cancelScanButton, params object[] userData)
    {
        if (_scanFlags != ScanFlags.None)
            StopInputScan();

        _scanTimeout = timeout;
        _scanFlags = ScanFlags.Key | ScanFlags.JoystickButton;
        _scanStartTime = IgnoreTimeScale ? Time.realtimeSinceStartup : Time.time;
        _cancelScanButton = cancelScanButton;
        _scanUserData = userData;
        _scanHandler = (result) =>
        {
            return scanHandler(result.key, (object[])result.userData);
        };
    }


    /// <summary>
    /// Scans for mouse input and calls the handler with the result.
    /// Returns -1 if timeout is reached or the scan is canceled.
    /// </summary>
    public  void StartMouseAxisScan(AxisScanHandler scanHandler, float timeout, string cancelScanButton, params object[] userData)
    {
        if (_scanFlags != ScanFlags.None)
            StopInputScan();

        _scanTimeout = timeout;
        _scanFlags = ScanFlags.MouseAxis;
        _scanStartTime = IgnoreTimeScale ? Time.realtimeSinceStartup : Time.time;
        _cancelScanButton = cancelScanButton;
        _scanUserData = userData;
        _scanHandler = (result) =>
        {
            return scanHandler(result.mouseAxis, (object[])result.userData);
        };
    }

    /// <summary>
    /// Scans for joystick input and calls the handler with the result.
    /// Returns -1 if timeout is reached or the scan is canceled.
    /// </summary>
    public  void StartJoystickAxisScan(AxisScanHandler scanHandler, int joystick, float timeout, string cancelScanButton, params object[] userData)
    {
        if (joystick < 0 || joystick >= AxisConfiguration.MaxJoystickAxes)
            throw new ArgumentOutOfRangeException("joystick");

        if (_scanFlags != ScanFlags.None)
            StopInputScan();

        _scanTimeout = timeout;
        _scanFlags = ScanFlags.JoystickAxis;
        _scanStartTime = IgnoreTimeScale ? Time.realtimeSinceStartup : Time.time;
        _cancelScanButton = cancelScanButton;
        _scanJoystick = joystick;
        _scanUserData = userData;
        _scanHandler = (result) =>
        {
            return scanHandler(result.mouseAxis, (object[])result.userData);
        };
    }

    public  void StartScan(ScanSettings settings, ScanHandler scanHandler)
    {
        if (settings.joystick < 0 || settings.joystick >= AxisConfiguration.MaxJoystickAxes)
            throw new ArgumentException("The joystick id you want to scan for is out of range");

        if (_scanFlags != ScanFlags.None)
            StopInputScan();

        _scanTimeout = settings.timeout;
        _scanFlags = settings.scanFlags;
        _scanStartTime = IgnoreTimeScale ? Time.realtimeSinceStartup : Time.time;
        _cancelScanButton = settings.cancelScanButton;
        _scanJoystick = settings.joystick;
        _scanUserData = settings.userData;
        _scanHandler = scanHandler;
    }

    public  void CancelScan()
    {
        if (_scanFlags != ScanFlags.None)
            StopInputScan();
    }

    /// <summary>
    /// Triggers the ConfigurationDirty event.
    /// </summary>
    public  void SetConfigurationDirty(string inputConfigName)
    {
        RaiseConfigurationDirtyEvent(inputConfigName);
    }

    /// <summary>
    /// Saves the input configurations in the XML format, in Application.persistentDataPath.
    /// </summary>
    public void Save()
    {
#if UNITY_WINRT && !UNITY_EDITOR
            string filename = Application.persistentDataPath + "/input_config.xml";
#else
        string filename = System.IO.Path.Combine(Application.persistentDataPath, "input_config.xml");
#endif
        Save(new InputSaverXML(filename));
    }

    /// <summary>
    /// Saves the input configurations in the XML format, at the specified location.
    /// </summary>
    public void Save(string filename)
    {
        Save(new InputSaverXML(filename));
    }

    public void Save(IInputSaver inputSaver)
    {
        if (inputSaver == null)
            throw new ArgumentNullException("inputSaver");

        inputSaver.Save(_inputConfigurations, defaultConfiguration);
        RaiseSavedEvent();
    }

    /// <summary>
    /// Loads the input configurations saved in the XML format, from Application.persistentDataPath.
    /// </summary>
    public void Load()
    {
#if UNITY_WINRT && !UNITY_EDITOR
            string filename = Application.persistentDataPath + "/input_config.xml";
            if(UnityEngine.Windows.File.Exists(filename))
            {
                Load(new InputLoaderXML(filename));
            }
#else
        string filename = System.IO.Path.Combine(Application.persistentDataPath, "input_config.xml");
        if (System.IO.File.Exists(filename))
        {
            Load(new InputLoaderXML(filename));
        }
#endif
    }

    /// <summary>
    /// Loads the input configurations saved in the XML format, from the specified location.
    /// </summary>
    public void Load(string filename)
    {
        Load(new InputLoaderXML(filename));
    }

    public void Load(IInputLoader inputLoader)
    {
        if (inputLoader == null)
            throw new ArgumentNullException("inputLoader");

        inputLoader.Load(out _inputConfigurations, out defaultConfiguration);
        Init();
        RaiseLoadedEvent();
    }

        #region [UNITY Interface]
        public Vector3 acceleration { get { return Input.acceleration; } }
        public  int accelerationEventCount { get { return Input.accelerationEventCount; } }
        public  AccelerationEvent[] accelerationEvents { get { return Input.accelerationEvents; } }
        public  bool anyKey { get { return Input.anyKey; } }
        public  bool anyKeyDown { get { return Input.anyKeyDown; } }
        public  Compass compass { get { return Input.compass; } }
        public  string compositionString { get { return Input.compositionString; } }
        public  DeviceOrientation deviceOrientation { get { return Input.deviceOrientation; } }
        public  Gyroscope gyro { get { return Input.gyro; } }
        public  bool imeIsSelected { get { return Input.imeIsSelected; } }
        public  string inputString { get { return Input.inputString; } }
        public  LocationService location { get { return Input.location; } }
        public  Vector2 mousePosition { get { return Input.mousePosition; } }
        public  bool mousePresent { get { return Input.mousePresent; } }
        public  int touchCount { get { return Input.touchCount; } }
        public  Touch[] touches { get { return Input.touches; } }

        public  bool compensateSensors
        {
            get { return Input.compensateSensors; }
            set { Input.compensateSensors = value; }
        }

        public  Vector2 compositionCursorPos
        {
            get { return Input.compositionCursorPos; }
            set { Input.compositionCursorPos = value; }
        }

        public  IMECompositionMode imeCompositionMode
        {
            get { return Input.imeCompositionMode; }
            set { Input.imeCompositionMode = value; }
        }

        public  bool multiTouchEnabled
        {
            get { return Input.multiTouchEnabled; }
            set { Input.multiTouchEnabled = value; }
        }

        public  AccelerationEvent GetAccelerationEvent(int index)
        {
            return Input.GetAccelerationEvent(index);
        }

        public  float GetAxis(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(_currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An axis named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetAxis();
        }

        public  float GetAxisRaw(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(_currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An axis named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetAxisRaw();
        }

        public  bool GetButton(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(_currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An button named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetButton();
        }

        public  bool GetButtonDown(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(_currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An button named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetButtonDown();
        }

        public  bool GetButtonUp(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(_currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An button named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetButtonUp();
        }

        public  bool GetKey(KeyCode key)
        {
            return Input.GetKey(key);
        }

        public  bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        public  bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public  bool GetMouseButton(int index)
        {
            return Input.GetMouseButton(index);
        }

        public  bool GetMouseButtonDown(int index)
        {
            return Input.GetMouseButtonDown(index);
        }

        public  bool GetMouseButtonUp(int index)
        {
            return Input.GetMouseButtonUp(index);
        }

        public  Touch GetTouch(int index)
        {
            return Input.GetTouch(index);
        }

        public  string[] GetJoystickNames()
        {
            return Input.GetJoystickNames();
        }

        public  void ResetInputAxes()
        {
            InputConfiguration inputConfig = _currentConfiguration;
            int count = inputConfig.axes.Count;
            for (int i = 0; i < count; i++)
            {
                inputConfig.axes[i].Reset();
            }
            Input.ResetInputAxes();
        }
        #endregion

    }
}