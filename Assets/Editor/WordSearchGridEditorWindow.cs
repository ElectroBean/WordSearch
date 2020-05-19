using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class WordSearchGridEditorWindow : EditorWindow
{
    [MenuItem("Tools/Word Search Grid Editor")]
    public static void Open()
    {
        GetWindow<WordSearchGridEditorWindow>();
    }

    public WordSearchGrid wordSearchGrid;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("wordSearchGrid"));

        if (wordSearchGrid == null)
        {
            EditorGUILayout.HelpBox("Must have wordsearch grid selected", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        if(GUILayout.Button("Reset Grid"))
        {
            ResetGrid();
        }
    }

    void ResetGrid()
    {
        wordSearchGrid.ResetGrid();
    }
}
