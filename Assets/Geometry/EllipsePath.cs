using System.Collections.Generic;
using UnityEngine;
using Geometry;

/// <summary>
/// Generates a test geometry of an ellipse. 
/// </summary>

[CreateAssetMenu(fileName = "EllipsePath", menuName = "ScriptableObjects/Ellipse", order = 2)]
public class EllipsePath : ScriptableObject, PathInterface
{
    public float rx = 1.0f;
    public float ry = 0.5f;
    public Vector3 position = new Vector3();

    public float angle = 0;
    public Vector3 axis = new Vector3();

    public int segments = 10;


    public float Length()
    {
        return 2f * Mathf.PI * Mathf.Sqrt((rx * rx + ry * ry) / 2.0f);
    }

    public List<Vector3> GetVerts()
    {
        return Generate(rx, ry, position, Quaternion.AngleAxis(angle, axis), segments);
    }
    public List<Vector3> Generate(float rx, float ry, Vector3 pos, Quaternion rot, int segments)
    {
        var verts = new List<Vector2>();

        float stepSize = 2 * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float x = rx * Mathf.Cos(i * stepSize);
            float y = ry * Mathf.Sin(i * stepSize);

            verts.Add(new Vector2(x, y));
        }
        return GeomUtil.GetTFVerts(verts.ToArray(), pos, rot);
    }

    public PosOri GetInterpPoint(float t)
    {
        var point = GetPoint(rx, ry, t);
        var dir = GetTangent(rx, ry, t);
        var upVector = -Vector3.Cross(dir, Vector3.forward);
        var quat = GeomUtil.GetQuatFromDir(dir, upVector);
        //Quaternion.LookRotation(dir, Vector3.up);
        //if (t < 0.5f)
        //    quat = Quaternion.LookRotation(dir, Vector3.down);

        return new PosOri(point, quat);
    }

    Vector3 GetPoint(float rx, float ry, float t)
    {
        return new Vector3(
                            rx * Mathf.Cos(2 * t * Mathf.PI),
                            ry * Mathf.Sin(2 * t * Mathf.PI)
                            );
    }

    Vector3 GetTangent(float rx, float ry, float t)
    {
        var tgt = new Vector3(
            -2 * Mathf.PI * rx * Mathf.Sin(2 * t * Mathf.PI),
            2 * Mathf.PI * ry * Mathf.Cos(2 * t * Mathf.PI)
        ).normalized;

        //if (t > 0.5f)
        //    return -tgt;
        return tgt;

    }
}