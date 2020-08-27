using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    /// <summary>
    /// A list of 3D path vertices for geometry generation.
    /// </summary>
    [CreateAssetMenu(fileName = "Path3D", menuName = "ScriptableObjects/Path3D", order = 1)]
    public class Path3D : ScriptableObject, PathInterface
    {
        public List<Vector3> mVerts = new List<Vector3>();

        public int MaxIndex { get { return mVerts.Count - 1; } }
        public virtual List<Vector3> GetVerts()
        {
            return mVerts;
        }

        public virtual float Length()
        {
            var len = 0f;
            for (int i = 0; i < MaxIndex; i++)
            {
                len += Vector3.Distance(mVerts[i + 1], mVerts[i]);
            }
            return len;
        }

        public void IndexOf(float tgtPos, out int index, out float distance)
        {
            distance = 0f;
            for (index = 0; index < MaxIndex; index++)
            {
                float toAdd = Vector3.Distance(mVerts[index + 1], mVerts[index]);
                if (distance + toAdd > tgtPos)
                    return;
                distance += toAdd;
            }
            return;
        }

        public virtual PosOri GetInterpPoint(float t)
        {
            var length = Length();
            var tgtPos = t * length;
            IndexOf(tgtPos, out int index, out float distance);

            Vector3 point = Vector3.zero;
            Vector3 dir = Vector3.up;

            if (index < MaxIndex)
            {
                point = mVerts[index];
                dir = mVerts[index + 1] - point;
            }

            if (index == MaxIndex)
            {
                point = mVerts[MaxIndex];
                dir = point - mVerts[MaxIndex - 1];
            }

            dir = dir.normalized;
            var overShoot = tgtPos - distance;

            var quat = GetDir(index);
            var pos = point + overShoot * dir;
            PosOri posOri = new PosOri(pos, quat);

            return posOri;
        }

        // This requires more work
        private Quaternion GetDir(int index)
        {
            var dir = -Vector3.left;
            return GeomUtil.GetQuatFromDir(dir, Vector3.up);

            /*if (index == MaxIndex)
                index--;

            var dir = mVerts[index + 1] - mVerts[index];
            
            var quat = Quaternion.LookRotation(dir, Vector3.up);
            if (dir == Vector3.up)
            {
                quat = Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
                //quat = Quaternion.AngleAxis(-80, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
                Debug.Log("kdkd");
            }

            if (dir == Vector3.down)
            {
                quat = Quaternion.AngleAxis(90, Vector3.up) *
                        Quaternion.AngleAxis(-90, Vector3.left) * Quaternion.AngleAxis(0, Vector3.forward);
            }
            return quat;*/
        }
    }
}