using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    /// <summary>
    /// An interface for objects returing a vertices list describing a path
    /// or an interpolated point on the path.
    /// </summary>
    public interface PathInterface
    {
        PosOri GetInterpPoint(float t);
        List<Vector3> GetVerts();
    }
}