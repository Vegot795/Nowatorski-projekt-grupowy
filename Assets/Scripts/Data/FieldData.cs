using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FieldData
{
    public Vector3 position;
}

[System.Serializable]
public class FieldDataList
{
    public List<FieldData> fields = new List<FieldData>();
}
