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
        BuildMode,
        PropRotateClockwise,
        PropRotateAntiClockwise,
        PropRotateIncrementClockwise,
        PropRotateIncrementAntiClockwise
    }

    public enum Category
    {
        Camera,
        BuildMode,
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

        AddHotkey ( new HotkeyData ( Function.PropRotateClockwise, Category.BuildMode, KeyCode.R, KeyCode.LeftShift, "Rotate Props" ) );
        AddHotkey ( new HotkeyData ( Function.PropRotateAntiClockwise, Category.BuildMode, KeyCode.None, "Rotate Props" ) );

        AddHotkey ( new HotkeyData ( Function.PropRotateIncrementClockwise, Category.BuildMode, KeyCode.R, "Rotate Props By 45 Degrees" ) );
        AddHotkey ( new HotkeyData ( Function.PropRotateIncrementAntiClockwise, Category.BuildMode, KeyCode.None, "Rotate Props By 45 Degrees" ) );

        AddHotkey ( new HotkeyData ( Function.HaltUI, Category.Misc, KeyCode.LeftShift, "Halt UI" ) );
    }

    private void AddHotkey(HotkeyData hotkey)
    {
        if(IsModifierKey(hotkey.KeyCode) && hotkey.Modifier != KeyCode.None)
        {
            Debug.LogError ( "Hotkey has two modifier values" );
        }

        if(hotkey.Modifier != KeyCode.None && !IsModifierKey ( hotkey.Modifier ))
        {
            Debug.LogError ( "Hotkey has modifier that is not a modifier" );
        }

        hotkeys.Add ( hotkey.Function, hotkey );        
    }

    public static bool GetKeyDown (Function function)
    {
        if (!hotkeys.ContainsKey ( function )) { Debug.LogError ( "Function does not exist, please assin in CreateHotkeys()" ); return false; }

        if (hotkeys[function].Modifier == KeyCode.None)
        {
            if ((Input.GetKey ( KeyCode.LeftControl ) || Input.GetKey ( KeyCode.LeftShift ) || Input.GetKey ( KeyCode.LeftAlt )) && !IsModifierKey ( hotkeys[function].KeyCode )) return false;
            return Input.GetKeyDown ( hotkeys[function].KeyCode );
        }
        else
        {
            if (Input.GetKey ( hotkeys[function].Modifier ))
            {
                return Input.GetKeyDown ( hotkeys[function].KeyCode );
            }
            else { return false; }
        }
    }

    public static bool GetKeyUp (Function function, bool includeModifier = false)
    {
        if (!hotkeys.ContainsKey ( function )) { Debug.LogError ( "Function does not exist, please assin in CreateHotkeys()" ); return false; }

        if (includeModifier)
        {
            if (hotkeys[function].Modifier == KeyCode.None)
            {
                if ((Input.GetKey ( KeyCode.LeftControl ) || Input.GetKey ( KeyCode.LeftShift ) || Input.GetKey ( KeyCode.LeftAlt )) && !IsModifierKey ( hotkeys[function].KeyCode )) return false;
                return Input.GetKeyUp ( hotkeys[function].KeyCode );
            }
            else
            {
                if (Input.GetKey ( hotkeys[function].Modifier ))
                {
                    return Input.GetKeyUp ( hotkeys[function].KeyCode );
                }
                else { return false; }
            }
        }
        else return Input.GetKeyUp ( hotkeys[function].KeyCode );
    }

    public static bool GetKey (Function function)
    {
        if (!hotkeys.ContainsKey ( function )) { Debug.LogError ( "Function does not exist, please assin in CreateHotkeys()" ); return false; }

        if (hotkeys[function].Modifier == KeyCode.None)
        {
            if ((Input.GetKey ( KeyCode.LeftControl ) || Input.GetKey ( KeyCode.LeftShift ) || Input.GetKey ( KeyCode.LeftAlt )) && !IsModifierKey(hotkeys[function].KeyCode)) return false;
            return Input.GetKey ( hotkeys[function].KeyCode );
        }
        else
        {
            if (Input.GetKey ( hotkeys[function].Modifier ))
            {
                return Input.GetKey ( hotkeys[function].KeyCode );
            }
            else { return false; }
        }
    }

    private static bool IsModifierKey (KeyCode code)
    {
        if (code == KeyCode.LeftControl) return true;
        if (code == KeyCode.LeftShift) return true;
        if (code == KeyCode.LeftAlt) return true;
        return false;
    }

    private class HotkeyData
    {
        public Function Function { get; private set; }
        public Category Category { get; private set; }
        public KeyCode KeyCode { get; private set; }
        public KeyCode Modifier { get; private set; }
        public string Description { get; private set; }

        public HotkeyData (Function function, Category category, KeyCode keyCode, string description)
        {
            Function = function;
            Category = category;
            KeyCode = keyCode;
            Modifier = KeyCode.None;
            Description = description;
        }

        public HotkeyData (Function function, Category category, KeyCode keyCode, KeyCode modifier, string description)
        {
            Function = function;
            Category = category;
            KeyCode = keyCode;
            Modifier = modifier;
            Description = description;
        }
    }
}
