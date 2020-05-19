using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class CreateValueWordEditor : EditorWindow
{
    public Sprite source;
    string wordLabel;
    string wordDescription;
    Sprite image;

    [MenuItem("Tools/Value Word Editor")]
    public static void Open()
    {
        GetWindow<CreateValueWordEditor>();
    }

    private void OnGUI()
    {
        wordLabel = EditorGUILayout.TextField("Value Word: ", wordLabel);
        wordDescription = EditorGUILayout.TextField("Value Word Description: ", wordDescription);
        image = (Sprite)EditorGUILayout.ObjectField(source, typeof(Sprite), false);

        if (GUILayout.Button("Create Scriptable Object"))
        {
            CreateValueWord(wordLabel, wordDescription, image);
        }
    }

    private void CreateValueWord(string name, string description, Sprite image)
    {
        ValueWord newAsset = ScriptableObject.CreateInstance<ValueWord>();
        newAsset.word = name;
        newAsset.description = description;
        newAsset.wordArtwork = image;

        AssetDatabase.CreateAsset(newAsset, "Assets/ScriptableObjects/Value Words" + "/" + name + ".asset");
        AssetDatabase.SaveAssets();
    }
}
