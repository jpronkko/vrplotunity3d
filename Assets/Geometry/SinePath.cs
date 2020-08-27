using System.Collections.Generic;
using UnityEngine;
using Geometry;

/// <summary>
/// A test object that creates vertices for a sine path.
/// </summary>
[CreateAssetMenu(fileName = "SinePath", menuName = "ScriptableObjects/Sine", order = 3)]
public class SinePath : ScriptableObject, PathInterface
{
    public float amplitude = 1.0f;
    public float multiplier = 1.0f;
    public float length = 2.5f;
    public Vector3 position = new Vector3();

    public float angle = 0;
    public Vector3 axis = new Vector3();

    public int segments = 10;

    // Start is called before the first frame update
    public List<Vector3> GetVerts()
    {
        return Generate(multiplier, length, position, Quaternion.AngleAxis(angle, axis), segments);
    }

    public List<Vector3> Generate(float multiplier, float length, Vector3 pos, Quaternion rot, int segments)
    {
        var verts = new List<Vector2>();

        float stepSize = length / segments;
        for (int i = 0; i < segments; i++)
        {
            float x = i * stepSize;
            float y = amplitude * Mathf.Sin(2 * multiplier * i * stepSize * Mathf.PI);

            verts.Add(new Vector2(x, y));
        }
        return GeomUtil.GetTFVerts(verts.ToArray(), pos, rot);
    }

    public PosOri GetInterpPoint(float t)
    {
        var point = GetPoint(multiplier, length, t);
        var dir = GetTangent(multiplier, length, t);
        var upVector = -Vector3.Cross(dir, Vector3.forward);
        upVector = Vector3.up;
        var quat = GeomUtil.GetQuatFromDir(dir, upVector);
        
        return new PosOri(point, quat);
    }

    Vector3 GetPoint(float rx, float ry, float t)
    {
        return new Vector3(
                            t * length,
                            amplitude * Mathf.Sin(2 * multiplier * t * Mathf.PI)
                            );
    }

    Vector3 GetTangent(float multiplier, float length, float t)
    {
        var tgt = new Vector3(
            1,
            2 * Mathf.PI * amplitude * multiplier * Mathf.Cos(2 * multiplier * t * Mathf.PI)
        ).normalized;

        return -Vector3.left;
    }
}
