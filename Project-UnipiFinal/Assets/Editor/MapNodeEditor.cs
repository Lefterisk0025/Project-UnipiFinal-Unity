using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapNodeView))]
public class MapNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapNodeView mapNodeView = (MapNodeView)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Print Position"))
        {
            mapNodeView.PrintPosition();
        }
    }
}
