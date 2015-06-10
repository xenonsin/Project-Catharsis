using UnityEngine;
using System;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.injector.api;

namespace Catharsis.InputEditor
{
    [Implements(typeof(IInputManager), InjectionBindingScope.CROSS_CONTEXT)]
    public class InputManager : IInputManager
    {
         [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        #region Fields
        private InputConfiguration currentConfiguration;
        public bool ignoreTimeScale { get; set; }


        private string[] rawMouseAxes;
        private string[] rawJoystickAxes;
        private KeyCode[] keys;
        private Dictionary<string, InputConfiguration> configurationTable; //Holds all active configurations
        private Dictionary<string, Dictionary<string, AxisConfiguration>> axesTable; //Holds current axes Table
        #endregion


         [PostConstruct]
        public void PostConstruct()
         {
             keys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
             ignoreTimeScale = true;
             SetRawAxisNames();
             Initialize();
         }

        void SetRawAxisNames()
        {
            
        }

        void Initialize()
        {
            
        }

        
        public AxisConfiguration GetAxisConfiguration(string inputConfigName, string axisName)
        {
            Dictionary<string, AxisConfiguration> table;
            if (axesTable.TryGetValue(inputConfigName, out table)) //Returns true if it contains an element with the specified key, and when it returns, it contains the value associated with the key.
            {
                AxisConfiguration axisConfig;
                if (table.TryGetValue(axisName, out axisConfig))
                    return axisConfig;
            }

            return null;
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
            AxisConfiguration axisConfig = GetAxisConfiguration(currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An axis named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetAxis();
        }

        public  float GetAxisRaw(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An axis named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetAxisRaw();
        }

        public  bool GetButton(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An button named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetButton();
        }

        public  bool GetButtonDown(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(currentConfiguration.name, name);
            if (axisConfig == null)
                throw new ArgumentException(string.Format("An button named \'{0}\' does not exist in the active input configuration", name));

            return axisConfig.GetButtonDown();
        }

        public  bool GetButtonUp(string name)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(currentConfiguration.name, name);
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
            InputConfiguration inputConfig = currentConfiguration;
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