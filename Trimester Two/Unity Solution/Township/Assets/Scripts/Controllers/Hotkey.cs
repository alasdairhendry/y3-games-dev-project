using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotkey : MonoBehaviour {
    
    public enum Function
    {
        CameraRotateLeft,
        CameraRotateRight,
        CameraPan,
        HaltUI,
        BuildMode
    }

    public enum Category
    {
        Camera,
        Modes,
        Misc
    }

    private static Dictionary<Function, HotkeyData> hotkeys = new Dictionary<Function, HotkeyData> ();
    public static bool MouseMoved { get; protected set; }
    private Vector3 previousMousePosition = new Vector3 ();


    private void Awake ()
    {
        CreateHotkeys ();
    }

    private void Update ()
    {
        if (previousMousePosition != Input.mousePosition)
            MouseMoved = true;
        else MouseMoved = false;

        previousMousePosition = Input.mousePosition;
    }

    private void CreateHotkeys ()
    {
        AddHotkey ( new HotkeyData ( Function.CameraRotateLeft, Category.Camera, KeyCode.Q, "Rotate Left" ) );
        AddHotkey ( new HotkeyData ( Function.CameraRotateRight, Category.Camera, KeyCode.E, "Rotate Right" ) );
        AddHotkey ( new HotkeyData ( Function.CameraPan, Category.Camera, KeyCode.Mouse2, "Pan" ) );

        AddHotkey ( new HotkeyData ( Function.BuildMode, Category.Modes, KeyCode.B, "Build Mode" ) );

        AddHotkey ( new HotkeyData ( Function.HaltUI, Category.Misc, KeyCode.LeftShift, "Halt UI" ) );
    }

    private void AddHotkey(HotkeyData hotkey)
    {
        hotkeys.Add ( hotkey.Function, hotkey );        
    }

    public static bool GetKeyDown (Function function)
    {
        if (!hotkeys.ContainsKey ( function )) { Debug.LogError ( "Function does not exist, please assin in CreateHotkeys()" ); return false; }
        return Input.GetKeyDown ( hotkeys[function].KeyCode );
    }

    public static bool GetKeyUp (Function function)
    {
        if (!hotkeys.ContainsKey ( function )) { Debug.LogError ( "Function does not exist, please assin in CreateHotkeys()" ); return false; }
        return Input.GetKeyUp ( hotkeys[function].KeyCode );
    }

    public static bool GetKey (Function function)
    {
        if (!hotkeys.ContainsKey ( function )) { Debug.LogError ( "Function does not exist, please assin in CreateHotkeys()" ); return false; }
        return Input.GetKey ( hotkeys[function].KeyCode );
    }

    private class HotkeyData
    {
        public Function Function { get; private set; }
        public Category Category { get; private set; }
        public KeyCode KeyCode { get; private set; }
        public string Description { get; private set; }

        public HotkeyData (Function function, Category category, KeyCode keyCode, string description)
        {
            Function = function;
            Category = category;
            KeyCode = keyCode;
            Description = description;
        }
    }
}
