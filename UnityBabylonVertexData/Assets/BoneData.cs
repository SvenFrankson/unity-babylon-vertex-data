using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct BoneData
{
    public string name;
    public string parentName;
	public float[] matrix;
}

public struct SkeletonData
{
    public List<BoneData> skeleton;
}
