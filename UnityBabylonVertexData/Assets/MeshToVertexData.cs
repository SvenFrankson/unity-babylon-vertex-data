using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshToVertexData : MonoBehaviour {

    public Mesh Target;

    public void GameObjectToJSON()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = skinnedMeshRenderer.sharedMesh;

        FileStream meshOutput = new FileStream(Application.dataPath + "\\mesh.json", FileMode.Create, FileAccess.Write);
        StreamWriter meshDataStream = new StreamWriter(meshOutput);

        meshDataStream.Write(JsonUtility.ToJson(MeshToVertexData.GetVertexData(mesh)));

        meshDataStream.Close();
        meshOutput.Close();

        Transform rootBone = skinnedMeshRenderer.rootBone;

        FileStream bonesOutput = new FileStream(Application.dataPath + "\\bones.json", FileMode.Create, FileAccess.Write);
        StreamWriter bonesDataStream = new StreamWriter(bonesOutput);

        bonesDataStream.Write(JsonUtility.ToJson(MeshToVertexData.GetBonesData(rootBone)));

        bonesDataStream.Close();
        bonesOutput.Close();
    }

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

	public static SkeletonData GetBonesData(Transform rootBone)
    {
        List<BoneData> bonesData = new List<BoneData>();

        BoneData rootData = new BoneData();
        rootData.name = rootBone.name;
		rootData.parentName = "_null";
		rootData.matrix = Matrix4x4.TRS (rootBone.localPosition, rootBone.localRotation, rootBone.localScale);

        bonesData.Add(rootData);

        foreach (Transform child in rootBone.GetComponentsInChildren<Transform>())
        {
			if (child != rootBone) {
				BoneData childData = new BoneData();

				childData.name = child.name;
				childData.parentName = child.parent.name;
				childData.matrix = Matrix4x4.TRS (child.localPosition, child.localRotation, child.localScale);

				bonesData.Add(childData);
			}
        }
        Debug.Log("Bones Length = " + bonesData.Count);

        SkeletonData skeletonData = new SkeletonData();
		skeletonData.skeleton = new List<BoneData>(bonesData);

		return skeletonData;
    }
}
