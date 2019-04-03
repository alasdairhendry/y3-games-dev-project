using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        PropRotateIncrementAntiClockwise,

        Save,
        Exit,

        ToggleResources,
        ToggleJobs,
        ToggleProfessions,

        TogglePreferences,
        ToggleTaxBands
    }

    public enum Category
    {
        Camera,
        BuildMode,
        Modes,
        Misc,
        System,
        UI
    }

    public static Dictionary<Function, HotkeyData> hotkeys { get; protected set; } = new Dictionary<Function, HotkeyData> ();
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

        AddHotkey ( new HotkeyData ( Function.PropRotateClockwise, Category.BuildMode, KeyCode.R, KeyCode.LeftShift, "Rotate Props Clockwise" ) );
        AddHotkey ( new HotkeyData ( Function.PropRotateAntiClockwise, Category.BuildMode, KeyCode.None, "Rotate Props Anti-Clockwise" ) );

        AddHotkey ( new HotkeyData ( Function.PropRotateIncrementClockwise, Category.BuildMode, KeyCode.R, "Rotate Props Clockwise (Increment)" ) );
        AddHotkey ( new HotkeyData ( Function.PropRotateIncrementAntiClockwise, Category.BuildMode, KeyCode.None, "Rotate Props Anti-Clockwise (Increment)" ) );

        AddHotkey ( new HotkeyData ( Function.HaltUI, Category.Misc, KeyCode.LeftShift, "Halt UI" ) );

        AddHotkey ( new HotkeyData ( Function.Save, Category.System, KeyCode.S, KeyCode.LeftShift, "Save Current Game" ) );
        AddHotkey ( new HotkeyData ( Function.Exit, Category.System, KeyCode.Escape, KeyCode.LeftAlt, "Exit The Game" ) );

        AddHotkey ( new HotkeyData ( Function.ToggleResources, Category.UI, KeyCode.I, KeyCode.None, "Toggle The Resources Panel" ) );
        AddHotkey ( new HotkeyData ( Function.ToggleJobs, Category.UI, KeyCode.J, KeyCode.None, "Toggle The Jobs Panel" ) );
        AddHotkey ( new HotkeyData ( Function.ToggleProfessions, Category.UI, KeyCode.P, KeyCode.None, "Toggle The Professions Panel" ) );
        AddHotkey ( new HotkeyData ( Function.ToggleTaxBands, Category.UI, KeyCode.T, KeyCode.None, "Toggle The Professions Panel" ) );

        AddHotkey ( new HotkeyData ( Function.TogglePreferences, Category.UI, KeyCode.P, KeyCode.LeftShift, "Toggle The Preferences Panel" ) );
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

    public static bool InputFieldIsActive ()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            return false;
        }

        InputField iField = EventSystem.current.currentSelectedGameObject.GetComponent<InputField> ();
        TMP_InputField tmp_iField = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField> ();

        if (iField == null && tmp_iField == null)
        {
            return false;
        }

        return true;
    }

    public static bool GetKeyDown (Function function)
    {
        if (InputFieldIsActive ()) return false;

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
        if (InputFieldIsActive ()) return false;

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
        if (InputFieldIsActive ()) return false;

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

    public static HotkeyData GetData(Function function)
    {
        if (hotkeys.ContainsKey ( function ))
        {
            return hotkeys[function];
        }

        return null;
    }

    public class HotkeyData
    {
        public Function Function { get; private set; }
        public Category Category { get; private set; }
        public KeyCode KeyCode { get; private set; }
        public KeyCode Modifier { get; private set; }
        public string Description { get; private set; }

        public string GetCommandString ()
        {
            string s = "";

            if(Modifier != KeyCode.None)
            {
                switch (Modifier)
                {
                    case KeyCode.LeftControl:
                        s += "CTRL + ";
                        break;

                    case KeyCode.LeftShift:
                        s += "SHIFT + ";
                        break;

                    case KeyCode.LeftAlt:
                        s += "ALT + ";
                        break;
                }
            }

            s += KeyCode.ToString ().ToUpper ();

            return "[ " + s + " ]";
        }

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
