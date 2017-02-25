using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshToVertexData))]
public class MeshToVertexDataInspector : Editor {

    private MeshToVertexData _target;
    private MeshToVertexData Target
    {
        get
        {
            if (_target == null)
            {
                _target = (MeshToVertexData)target;
            }
            return _target;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("VertexData"))
        {
            Target.TargetToJSON();
        }
    }
}
