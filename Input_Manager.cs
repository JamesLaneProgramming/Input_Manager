using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public enum PRESSTYPES
{
    Pressed,
    Held,
    Released
};

[System.Serializable] //Needed to initialize the struct and allow other scripts to access the Keys.
public struct InputKeysValues
{
    public string keyName;
    public KeyCode key;
    [HideInInspector]
    public bool isPressed;
    public PRESSTYPES pressType;
}

[ExecuteInEditMode]
public class Input_Manager : MonoBehaviour
{
    public static Input_Manager instance;
    public List<InputKeysValues> keysList = new List<InputKeysValues>(1);
    [HideInInspector]
    public InputKeysValues[] keys;
    private int keyIteration;

    #region UnityFunctions
    private void Awake()
    {
        if (Exists() == false)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        keys = keysList.ToArray();
    }
    private void Update()
    {
        checkInputs();
    }
    #endregion

    private void checkInputs()
    {
        for (keyIteration = 0; keyIteration < keys.Length; keyIteration++)
        {
            if (keys[keyIteration].pressType == PRESSTYPES.Held)
            {
                keys[keyIteration].isPressed = Input.GetKey(keys[keyIteration].key);
            }
            else if (keys[keyIteration].pressType == PRESSTYPES.Pressed)
            {
                keys[keyIteration].isPressed = Input.GetKeyDown(keys[keyIteration].key);
            }
            else if (keys[keyIteration].pressType == PRESSTYPES.Released)
            {
                keys[keyIteration].isPressed = Input.GetKeyUp(keys[keyIteration].key);
            }
        }
    }
    ///<summary>
    ///Determines whether the Input_Manager class exists within the current project
    ///</summary>
    public static bool Exists()
    {
        if (instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// Checks whether the Input_Manager is active, Input_Manager.Exists() should be called before this method.
    /// </summary>
    /// <returns></returns>
    public static bool isActive()
    {
        if (instance.isActiveAndEnabled == false || instance.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning(instance.name + " is not enabled or the gameobject it is attatched to is inactive, no inputs will be tracked.");
            return false;
        }
        else
        {
            return true;
        }
    }
    public int getNumberOfKeys()
    {
        return keys.Length;
    }
    public string[] getKeyNames()
    {
        keys = keysList.ToArray();
        string[] keyNames = new string[keys.Length];
        for (int i = 0; i < keyNames.Length; i++)
        {
            keyNames[i] = keys[i].keyName;
        }
        return keyNames;
    }
    public int getKeyIndexFromName(string _name)
    {
        int result = keysList.FindIndex(x => x.keyName == _name);
        return result;
    }
}

#region CustomInspector
[CustomEditor(typeof(Input_Manager))]
public class ModifyKeyList : Editor
{
    public override void OnInspectorGUI()
    {
        if (Input_Manager.Exists())
        {
            if (GUILayout.Button("Add Key"))
            {
                Input_Manager.instance.keysList.Add(new InputKeysValues());
            }

            if (GUILayout.Button("Remove Key"))
            {
                if (Input_Manager.instance.keysList.Count != 0)
                {
                    Input_Manager.instance.keysList.RemoveAt(Input_Manager.instance.keysList.Count - 1);
                }
            }
            DrawDefaultInspector();
        }
    }
}
#endregion
#region Custom_Key_DataType
[System.Serializable, InitializeOnLoad]
public class Custom_Key_Property : System.Object
{
    public int currentKeyIndex = 0;
}

[CustomPropertyDrawer(typeof(Custom_Key_Property))]
public class Custom_Key_Property_drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Input_Manager.Exists())
        {
            SerializedProperty currentKeyIndex = property.FindPropertyRelative("currentKeyIndex");
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            currentKeyIndex.intValue = EditorGUI.Popup(position, currentKeyIndex.intValue, Input_Manager.instance.getKeyNames());
            EditorGUI.EndProperty();
        }
    }
}
#endregion
