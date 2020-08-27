using System.Collections.Generic;
using UnityEngine;


namespace Geometry
{
    /// <summary>
    /// Generates a closed vertex loop that can be faceted or not.
    /// </summary>
    public class Loop2D
    {
        readonly List<Vector2> mVerts = new List<Vector2>();

        public int VertexCount => mVerts.Count;
        public void SetVerts(PathInterface path)
        {
            mVerts.Clear();

            foreach (var vert in path.GetVerts())
            {
                mVerts.Add(vert);
            }
        }
        public Vector3[] GetVerts(PosOri posOri, bool faceted)
        {
            if (faceted)
            {
                var facetedVerts = new Vector3[2 * mVerts.Count];
                for (int i = 0, j = 0; i < mVerts.Count; i++, j += 2)
                {
                    facetedVerts[j] = posOri.position + posOri.orientation * mVerts[i];
                    facetedVerts[j + 1] = posOri.position + posOri.orientation * mVerts[i];
                }

                return facetedVerts;
            }

            var verts = new Vector3[mVerts.Count];
            for (var i = 0; i < mVerts.Count; i++)
            {
                verts[i] = posOri.position + posOri.orientation * mVerts[i];
            }

            return verts;
        }

        virtual public Vector3[] GetNormals(PosOri posOri, bool faceted)
        {
            if (faceted)
            {
                var facetedNormals = new Vector3[2 * mVerts.Count];
                for (int currentIndex = 0; currentIndex < 2 * mVerts.Count; currentIndex += 2)
                {
                    var nextIndex = (currentIndex + 1) % mVerts.Count;
                    facetedNormals[currentIndex] = GetNormal(currentIndex - 1, currentIndex, posOri.orientation);
                    facetedNormals[currentIndex + 1] = GetNormal(currentIndex, nextIndex, posOri.orientation);
                }
            }

            var normals = new Vector3[mVerts.Count];
            for (int currentIndex = 0; currentIndex < mVerts.Count; currentIndex++)
            {
                var nextIndex = (currentIndex + 1) % mVerts.Count;
                normals[currentIndex] = GetNormal(currentIndex, nextIndex, posOri.orientation);
            }

            return normals;
        }

        Vector3 GetNormal(int fIndex, int sIndex, Quaternion rot)
        {
            if (fIndex < 0)
                fIndex = mVerts.Count - 1;
            if (sIndex > mVerts.Count - 1)
                sIndex = 1;

            Vector3 diff = rot * mVerts[fIndex] - rot * mVerts[sIndex];
            return Vector3.Cross(diff, Vector3.forward).normalized;
        }
    }
}