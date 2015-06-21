

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Catharsis.InputEditor
{
    public interface IInputManager
    {
        bool IsScanning { get; }
        void StartScan(ScanSettings settings, ScanHandler scanHandler);
        List<InputConfiguration> GetAllInputConfigurations();
        void RemoveAxisConfig(int inputIndex, int axisIndex);
        void RemoveInputConfig(int inputIndex);
        void RemoveAll();

        void AddInputConfiguration(InputConfiguration config);
        void InsertInputConfiguration(int index, InputConfiguration config);
        
        string GetDefaultConfiguration();
        void Load(IInputLoader inputLoader);

        void Save(IInputSaver inputSaver);

        void StartAfterConfigLoaded();

        InputConfiguration GetCurrentInputConfiguration();
        AxisConfiguration GetAxisConfiguration(string inputConfigName, string axisName);

        bool IgnoreTimeScale { get; set; }
        Vector3 acceleration { get; }
        int accelerationEventCount { get; }
        AccelerationEvent[] accelerationEvents { get; }
        bool anyKey { get; }
        bool anyKeyDown { get; }
        Compass compass { get; }
        string compositionString { get; }
        DeviceOrientation deviceOrientation { get; }
        Gyroscope gyro { get; }
        bool imeIsSelected { get; }
        string inputString { get; }
        LocationService location { get; }
        Vector2 mousePosition { get; }
        bool mousePresent { get; }
        int touchCount { get; }
        Touch[] touches { get; }

        bool compensateSensors { get; set; }

        Vector2 compositionCursorPos { get; set; }

        IMECompositionMode imeCompositionMode { get; set; }

        bool multiTouchEnabled { get; set; }

        AccelerationEvent GetAccelerationEvent(int index);

        float GetAxis(string name);

        float GetAxisRaw(string name);

        bool GetButton(string name);

        bool GetButtonDown(string name);

        bool GetButtonUp(string name);

        bool GetKey(KeyCode key);

        bool GetKeyDown(KeyCode key);

        bool GetKeyUp(KeyCode key);

        bool GetMouseButton(int index);

        bool GetMouseButtonDown(int index);

        bool GetMouseButtonUp(int index);

        Touch GetTouch(int index);

        string[] GetJoystickNames();

        void ResetInputAxes();

    }
}