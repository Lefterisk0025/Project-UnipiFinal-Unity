using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLineRenderer))]
public class MapLineRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapLineRenderer mapLineRenderer = (MapLineRenderer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Draw Line"))
        {
            //mapLineRenderer.DrawLine();
        }
    }
}
