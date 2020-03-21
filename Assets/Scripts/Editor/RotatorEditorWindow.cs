using com.technical.test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class RotatorEditorWindow : EditorWindow
{

    List<Rotator> _rotators = new List<Rotator>();
    Dictionary<Rotator, AnimBool> _togleDisplayedRotator = new Dictionary<Rotator, AnimBool>();
    Dictionary<Rotator, bool> _selectedRotators = new Dictionary<Rotator, bool>();
    Dictionary<string, bool> _selectedProterties = new Dictionary<string,bool>();

    Rotator _rotatorSample ;

    Vector2 scrollPosSideBar;
    Vector2 scrollPosBox;
    
    private void Awake()
    {
        _rotators = FindObjectsOfType<Rotator>().ToList();
        _rotatorSample = _rotators.FirstOrDefault();
        foreach (Rotator _rotator in _rotators)
        {
            _selectedRotators.Add(_rotator, false);
            _togleDisplayedRotator.Add(_rotator, new AnimBool(true));
            _togleDisplayedRotator.Last().Value.valueChanged.AddListener(Repaint);
        }


        _selectedProterties.Add("_timeBeforeStoppingInSeconds", false);
        _selectedProterties.Add("_shouldReverseRotation", false);
        _selectedProterties.Add("_rotationsSettings", false);


    }

    [MenuItem("Window/Custom/Rotators Value Setter")]
    public static void OpenWindow()
    {
        RotatorEditorWindow window = GetWindow<RotatorEditorWindow>("Rotator Setter Windows");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();    
        DrawSideBar();
        DrawBoxData();
        EditorGUILayout.EndHorizontal();
    }


    /// <summary>
    /// Display all proterties of one rotator
    /// </summary>
    /// <param name="rotator"></param>
    void DrawData(Rotator rotator)
    {
        EditorUtility.SetDirty(rotator);
        SerializedObject serializedObject = new SerializedObject(rotator);
        EditorGUILayout.BeginFadeGroup(_togleDisplayedRotator[rotator].faded);
        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_identifier"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_timeBeforeStoppingInSeconds"));
    
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_shouldReverseRotation"));
      

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_rotationsSettings"), true);
        serializedObject.ApplyModifiedProperties();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndFadeGroup();
    }

    /// <summary>
    /// Draw a sidebar with all rotator detected in scene
    /// </summary>
    void DrawSideBar()
    {
        scrollPosSideBar = EditorGUILayout.BeginScrollView(scrollPosSideBar);
        EditorGUILayout.BeginVertical("sideBar", GUILayout.MaxWidth(200), GUILayout.ExpandHeight(true));
        foreach (Rotator rotator in _rotators)
        {
            SerializedObject serializedObject = new SerializedObject(rotator);
            _selectedRotators[rotator] = EditorGUILayout.Toggle("Rotator - " + serializedObject.FindProperty("_identifier").stringValue, _selectedRotators[rotator]);

        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Manage all properties want to be modify
    /// </summary>
    void SelectProterties()
    {
        EditorUtility.SetDirty(_rotatorSample);
        SerializedObject serializedObject = new SerializedObject(_rotatorSample);
        
        serializedObject.Update();
   
        EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        
        
        #region Property Time Before Stopping In Seconds
        _selectedProterties["_timeBeforeStoppingInSeconds"] = EditorGUILayout.ToggleLeft("Time Before Stopping In Seconds", _selectedProterties["_timeBeforeStoppingInSeconds"]);
        EditorGUI.BeginDisabledGroup(_selectedProterties["_timeBeforeStoppingInSeconds"] == false);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_timeBeforeStoppingInSeconds"));
            EditorGUI.indentLevel--;
        EditorGUI.EndDisabledGroup();
        #endregion

        #region Property Should Reverse Rotation
        _selectedProterties["_shouldReverseRotation"] = EditorGUILayout.ToggleLeft("Should Reverse Rotation",_selectedProterties["_shouldReverseRotation"]);

        EditorGUI.BeginDisabledGroup(_selectedProterties["_shouldReverseRotation"] == false);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_shouldReverseRotation"));
            EditorGUI.indentLevel--;
        EditorGUI.EndDisabledGroup();
        #endregion

        #region Property Rotations Settings
        _selectedProterties["_rotationsSettings"] = EditorGUILayout.ToggleLeft("Rotations Settings",_selectedProterties["_rotationsSettings"]);
        EditorGUI.BeginDisabledGroup(_selectedProterties["_rotationsSettings"] == false);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rotationsSettings"),true);
            EditorGUI.indentLevel--;
        EditorGUI.EndDisabledGroup();
        #endregion
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Change"))
        {
            if (_selectedRotators.Where(sr => sr.Value == true).ToList().Count > 0 && _selectedProterties.Where(sp => sp.Value == true).ToList().Count > 0)
            {
                UpdateChange(serializedObject);
            }
            else if (_selectedRotators.Where(sr => sr.Value == true).ToList().Count <= 0)
            {
                EditorUtility.DisplayDialog("Warning", "Select at least one object to modify.", "Ok");
            }
            else if (_selectedProterties.Where(sp => sp.Value == true).ToList().Count <= 0)
            {
                EditorUtility.DisplayDialog("Warning", "Select at least one proprety to modify", "Ok");
            }
        }
    }

    /// <summary>
    /// Apply modification on all selected rotator
    /// </summary>
    /// <param name="serializedObjectSample"></param>
    void UpdateChange(SerializedObject serializedObjectSample)
    {
        foreach(KeyValuePair<Rotator,bool> rotator in _selectedRotators)
        {
            if(rotator.Value)
            {
                EditorUtility.SetDirty(rotator.Key);
                SerializedObject serializedObjectNeedTochange = new SerializedObject(rotator.Key);
                serializedObjectNeedTochange.Update();
                foreach (KeyValuePair<string,bool> properties in _selectedProterties)
                {
                    if(properties.Value)
                    {
                        if (properties.Key == "_rotationsSettings")
                        {
                            EditorUtility.SetDirty(serializedObjectSample.targetObject);
                            serializedObjectSample.Update();
                            serializedObjectSample.CopyFromSerializedProperty(serializedObjectNeedTochange.FindProperty("_rotationsSettings").FindPropertyRelative("ObjectToRotate"));
                            serializedObjectNeedTochange.ApplyModifiedProperties();
                        }
                           
                        serializedObjectNeedTochange.CopyFromSerializedProperty(serializedObjectSample.FindProperty(properties.Key));
                        serializedObjectNeedTochange.ApplyModifiedProperties();
                    }
                   
           
                }
                
            }
        }
    }
    
    /// <summary>
    /// Draw box properties modifier
    /// </summary>
    void DrawBoxData()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
        scrollPosBox = EditorGUILayout.BeginScrollView(scrollPosBox);
        EditorGUILayout.LabelField("Click on the Rotator you want to modify or select in the list below the field you want modify on all.", EditorStyles.boldLabel);
        SelectProterties();


        foreach (KeyValuePair<Rotator, bool> _selectedRotator in _selectedRotators)
        {
            if (_selectedRotator.Value)
            {
                SerializedObject serializedObject = new SerializedObject(_selectedRotator.Key);
                _togleDisplayedRotator[_selectedRotator.Key].target = EditorGUILayout.ToggleLeft("Rotator - " + serializedObject.FindProperty("_identifier").stringValue, _togleDisplayedRotator[_selectedRotator.Key].target, EditorStyles.boldLabel);

                if (_togleDisplayedRotator[_selectedRotator.Key].value)
                {
                    DrawData(_selectedRotator.Key);
                    EditorGUILayout.Separator();

                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

}
