using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct BoneData
{
    public string name;
    public string parentName;
	public Matrix4x4 matrix;
}

public struct SkeletonData
{
    public List<BoneData> skeleton;
}
