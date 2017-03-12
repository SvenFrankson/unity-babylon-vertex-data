using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshToVertexData : MonoBehaviour {

    public Mesh Target;

    public void TargetToJSON()
    {
        FileStream output = new FileStream(Application.dataPath + "\\out.json", FileMode.Create, FileAccess.Write);
        StreamWriter dataStream = new StreamWriter(output);

        dataStream.Write(JsonUtility.ToJson(MeshToVertexData.GetVertexData(Target)));

        dataStream.Close();
        output.Close();
    }

    public static VertexData GetVertexData(Mesh mesh)
    {
        VertexData data = new VertexData();

        List<int> verticesAsArray = new List<int>();
        foreach (Vector3 vertex in mesh.vertices)
        {
            // Vertices are stored as int with a factor 100, reducing the JSON file size (no decimals).
            // As a consequence, Mesh is a 100 times bigger once deserialized.
            verticesAsArray.Add(Mathf.RoundToInt(vertex.x * 100));
            verticesAsArray.Add(Mathf.RoundToInt(vertex.y * 100));
            verticesAsArray.Add(Mathf.RoundToInt(vertex.z * 100));
        }
        data.positions = verticesAsArray.ToArray();
        Debug.Log("Positions Length = " + data.positions.Length);

        List<int> normalsAsArray = new List<int>();
        foreach (Vector3 normal in mesh.normals)
        {
            normalsAsArray.Add(Mathf.RoundToInt(normal.x * 100));
            normalsAsArray.Add(Mathf.RoundToInt(normal.y * 100));
            normalsAsArray.Add(Mathf.RoundToInt(normal.z * 100));
        }
        data.normals = normalsAsArray.ToArray();
        Debug.Log("Normals Length = " + data.normals.Length);

        data.indices = new int[mesh.triangles.Length];
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            data.indices[i] = mesh.triangles[mesh.triangles.Length - 1 - i];
        }
        Debug.Log("Indices Length = " + data.indices.Length);

        return data;
    }

    public static BoneData[] GetBonesData(Transform rootBone)
    {
        List<BoneData> bonesData = new List<BoneData>();

        BoneData rootData = new BoneData();
        rootData.name = rootBone.name;
        rootData.parentName = "_null";
        rootData.matrix = Matrix4x4.TRS(rootBone.localPosition, rootBone.localRotation, rootBone.localScale);

        bonesData.Add(rootData);

        foreach (Transform child in rootBone.GetComponentsInChildren<Transform>())
        {
            BoneData childData = new BoneData();

            childData.name = rootBone.name;
            childData.parentName = child.parent.name;
            childData.matrix = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale);

            bonesData.Add(rootData);
        }

        return bonesData.ToArray();
    }
}
