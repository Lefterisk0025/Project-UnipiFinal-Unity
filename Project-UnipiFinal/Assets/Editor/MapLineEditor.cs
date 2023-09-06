using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLineRender))]
public class MapLineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapLineRender mapLineRender = (MapLineRender)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Draw Line"))
        {
            mapLineRender.DrawLine();
        }
    }
}
