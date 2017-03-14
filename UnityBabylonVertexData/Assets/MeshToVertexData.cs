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

		bonesDataStream.Write(JsonUtility.ToJson(MeshToVertexData.GetBonesData(skinnedMeshRenderer)));

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

		List<float> verticesAsArray = new List<float>();
        foreach (Vector3 vertex in mesh.vertices)
        {
            // Vertices are stored as int with a factor 100, reducing the JSON file size (no decimals).
            // As a consequence, Mesh is a 100 times bigger once deserialized.
            verticesAsArray.Add(vertex.x * 100);
            verticesAsArray.Add(vertex.y * 100);
            verticesAsArray.Add(vertex.z * 100);
        }
        data.positions = verticesAsArray.ToArray();
        Debug.Log("Positions Length = " + data.positions.Length);

		List<float> normalsAsArray = new List<float>();
        foreach (Vector3 normal in mesh.normals)
        {
            normalsAsArray.Add(normal.x * 100);
            normalsAsArray.Add(normal.y * 100);
            normalsAsArray.Add(normal.z * 100);
        }
        data.normals = normalsAsArray.ToArray();
        Debug.Log("Normals Length = " + data.normals.Length);

        data.indices = new int[mesh.triangles.Length];
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            data.indices[i] = mesh.triangles[mesh.triangles.Length - 1 - i];
        }
        Debug.Log("Indices Length = " + data.indices.Length);

		data.matricesIndices = new int[mesh.vertices.Length * 4];
		data.matricesWeights = new float[mesh.vertices.Length * 4];
		for (int i = 0; i < mesh.boneWeights.Length; i++)
		{
			data.matricesIndices [4 * i] = mesh.boneWeights [i].boneIndex0;
			data.matricesWeights [4 * i] = mesh.boneWeights [i].weight0;
			data.matricesIndices [4 * i + 1] = mesh.boneWeights [i].boneIndex1;
			data.matricesWeights [4 * i + 1] = mesh.boneWeights [i].weight1;
			data.matricesIndices [4 * i + 2] = mesh.boneWeights [i].boneIndex2;
			data.matricesWeights [4 * i + 2] = mesh.boneWeights [i].weight2;
			data.matricesIndices [4 * i + 3] = mesh.boneWeights [i].boneIndex3;
			data.matricesWeights [4 * i + 3] = mesh.boneWeights [i].weight3;
		}

        return data;
    }

	public static SkeletonData GetBonesData(SkinnedMeshRenderer skinnedMeshRenderer)
    {
        List<BoneData> bonesData = new List<BoneData>();

        foreach (Transform bone in skinnedMeshRenderer.bones)
        {
			BoneData childData = new BoneData();

			childData.name = bone.name;
			childData.parentName = bone.parent.name;
			childData.matrix = Matrix4x4ToFloatArray(Matrix4x4.TRS (bone.localPosition, bone.localRotation, bone.localScale));

			if (bone == skinnedMeshRenderer.rootBone) {
				childData.parentName = "__root";
			}
 
			bonesData.Add(childData);
        }
        Debug.Log("Bones Length = " + bonesData.Count);

        SkeletonData skeletonData = new SkeletonData();
		skeletonData.skeleton = new List<BoneData>(bonesData);

		return skeletonData;
    }

	public static float[] Matrix4x4ToFloatArray(Matrix4x4 matrix) {
		float[] floatArray = new float[16];
		floatArray [0] = matrix.m00;
		floatArray [1] = matrix.m01;
		floatArray [2] = matrix.m02;
		floatArray [3] = matrix.m03;
		floatArray [4] = matrix.m10;
		floatArray [5] = matrix.m11;
		floatArray [6] = matrix.m12;
		floatArray [7] = matrix.m13;
		floatArray [8] = matrix.m20;
		floatArray [9] = matrix.m21;
		floatArray [10] = matrix.m22;
		floatArray [11] = matrix.m23;
		floatArray [12] = matrix.m30;
		floatArray [13] = matrix.m31;
		floatArray [14] = matrix.m32;
		floatArray [15] = matrix.m33;

		return floatArray;
	}
}
