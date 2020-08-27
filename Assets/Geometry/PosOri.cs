using UnityEngine;

namespace Geometry
{
    public struct PosOri
    {
        public PosOri(Vector3 pos, Quaternion ori)
        {
            position = pos;
            orientation = ori;
        }
        public Vector3 position;
        public Quaternion orientation;

    }
}