using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(ProgressManager))]
public class ProgressManagerEditor : Editor
{
    Scene scene;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset"))
            (target as ProgressManager)?.Reset();
    }
}
