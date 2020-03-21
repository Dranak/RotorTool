using com.technical.test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rotator))]
public class RotatorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Open Window"))
        {
            RotatorEditorWindow.OpenWindow();
        }
    }

}
