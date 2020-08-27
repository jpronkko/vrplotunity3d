using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    /// <summary>
    /// Mesh generating functions for different geometries, produces lists for verts, normals and tris.
    /// You add the geometry to an existing list of for geometries, therefore we use a baseIndex parameter
    /// to tell at which index we add the new geometry. The index is incremented and returned from
    /// these functions according to the generated geometries.
    /// </summary>
    public static class GeomUtil
    {
        /// <summary>
        /// Creates spheroid geometry
        /// </summary>
        /// <param name="baseIndex">The index indicating a position in the geomety lists.</param>
        /// <param name="xySegments"></param>
        /// <param name="zSegments"></param>
        /// <param name="dimx">Width of a sphereoid</param>
        /// <param name="dimy">Height of a spheroid</param>
        /// <param name="dimz">The depth of a spheroid</param>
        /// <param name="pos">Position offset</param>
        /// <param name="ori">Orientation offset</param>
        /// <param name="gVerts">Vertices list to add the new vertices to</param>
        /// <param name="gNormals">Normals list to add the new normals to</param>
        /// <param name="gTris">Triangle indices list to add new the triangle indices to</param>
        /// <returns>A new base index for new mesh geometry to be added later.</returns>
        public static int CreateSpheroid(
                                        int baseIndex,
                                        int xySegments,
                                        int zSegments,
                                        float dimx,
                                        float dimy,
                                        float dimz,
                                        Vector3 pos,
                                        Quaternion ori,
                                        List<Vector3> gVerts,
                                        List<Vector3> gNormals,
                                        List<int> gTris
        )
        {
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tris = new List<int>();

            var rx = dimx / 2f;
            var ry = dimy / 2f;
            var rz = dimz / 2f;

            //float stepZ = dimz / zSegments;

            float stepAngleXY = 2 * Mathf.PI / xySegments;
            float stepAngleZ = Mathf.PI / zSegments;

            // verts and normals
            for (var j = 0; j < zSegments + 1; j++)
            {
                for (var i = 0; i < xySegments + 1; i++)
                {
                    float zAng = j * stepAngleZ;
                    float xyAng = i * stepAngleXY;
                    float x = rx * Mathf.Sin(zAng) * Mathf.Cos(xyAng);// + rx;
                    float y = ry * Mathf.Sin(zAng) * Mathf.Sin(xyAng);// + ry;
                    float z = rz * Mathf.Cos(zAng);// + rz; //j * stepZ - dimz / 2f;

                    var posVec = new Vector3(x, y, z);
                    verts.Add(posVec);
                    normals.Add(posVec.normalized);
                }
            }

            int nextRow = xySegments + 1;
            // tris
            for (var row = 0; row < zSegments; row++)
            {
                for (var vertIndex = 0; vertIndex < nextRow; vertIndex++)
                {
                    int currentVert = row * nextRow + baseIndex + vertIndex;
                    int nextVert = row * nextRow + baseIndex + (vertIndex + 1) % nextRow;
                    int nextRowCurrentVert = currentVert + nextRow;
                    int nextRowNextVert = nextVert + nextRow;

                    tris.Add(currentVert);
                    tris.Add(nextRowCurrentVert);
                    tris.Add(nextRowNextVert);

                    tris.Add(currentVert);
                    tris.Add(nextRowNextVert);
                    tris.Add(nextVert);
                }
            }

            baseIndex += verts.Count;
            gVerts.AddRange(GetTFVerts(verts, pos, ori));
            gNormals.AddRange(GetTFNorms(normals, ori));
            gTris.AddRange(tris);
            return baseIndex;
        }

        /// <summary>
        /// Creates cubic/box geometry
        /// </summary>
        /// <param name="baseIndex">The index indicating a position in the geomety lists.</param>
        /// <param name="dimx">The width of a box</param>
        /// <param name="dimy">The height of a box</param>
        /// <param name="dimz">The depth of a box</param>
        /// <param name="pos">Position offset</param>
        /// <param name="ori">Orientation offset</param>
        /// <param name="gVerts">Vertices list to add the new vertices to</param>
        /// <param name="gNormals">Normals list to add the new normals to</param>
        /// <param name="gTris">Triangle indices list to add new the triangle indices to</param>
        /// <returns>A new base index for new mesh geometry to be added later.</returns>
        public static int CreateCubic(
                                        int baseIndex,
                                        float dimx,
                                        float dimy,
                                        float dimz,
                                        Vector3 pos,
                                        Quaternion ori,
                                        List<Vector3> gVerts,
                                        List<Vector3> gNormals,
                                        List<int> gTris
                                        )
        {
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tris = new List<int>();

            baseIndex = CreateFace(baseIndex,
                                    dimx, dimy, dimz / 2f * -Vector3.forward,
                                    Quaternion.identity,
                                    verts,
                                    normals,
                                    tris
                                    );

            baseIndex = CreateFace(baseIndex,
                                     dimx, dimy, dimz / 2f * Vector3.forward,
                                     Quaternion.Euler(0f, 180f, 0f),
                                     verts,
                                     normals,
                                     tris
                                     );

            baseIndex = CreateFace(baseIndex,
                                    dimz, dimy, dimx / 2f * -Vector3.right,
                                    Quaternion.Euler(0f, 90f, 0f),
                                    verts,
                                    normals,
                                    tris
                                    );

            baseIndex = CreateFace(baseIndex,
                                    dimz, dimy, dimx / 2f * Vector3.right,
                                    Quaternion.Euler(0f, -90f, 0f),
                                    verts,
                                    normals,
                                    tris
                                    );

            baseIndex = CreateFace(baseIndex,
                                    dimx, dimz, dimy / 2f * Vector3.up,
                                    Quaternion.Euler(90f, 0f, 0f),
                                    verts,
                                    normals,
                                    tris
                                    );

            baseIndex = CreateFace(baseIndex,
                                            dimx, dimz, dimy / 2f * Vector3.down,
                                            Quaternion.Euler(-90f, 0f, 0f),
                                            verts,
                                            normals,
                                            tris
                                            );

            gVerts.AddRange(GetTFVerts(verts, pos, ori));
            gNormals.AddRange(GetTFNorms(normals, ori));
            gTris.AddRange(tris);
            return baseIndex;
        }

        /// <summary>
        /// Creates a single face in the verts, normals and tris lists
        /// </summary>
        /// <param name="baseIndex"></param>
        /// <param name="w">The width of the face</param>
        /// <param name="h">The height of the face</param>
        /// <param name="pos">Position offset</param>
        /// <param name="ori">Orientation offset</param>
        /// <param name="gVerts">Vertices list to add the new vertices to</param>
        /// <param name="gNormals">Normals list to add the new normals to</param>
        /// <param name="gTris">Triangle indices list to add new the triangle indices to</param>
        /// <returns></returns>
        public static int CreateFace(
                        int baseIndex,
                        float w, float h, Vector3 pos,
                        Quaternion ori,
                        List<Vector3> gVerts,
                        List<Vector3> gNormals,
                        List<int> gTris)
        {
            var rdimx = w / 2f;
            var rdimy = h / 2f;

            var front = new List<Vector3> {
                new Vector3(-rdimx, rdimy, 0f),
                new Vector3(rdimx, rdimy, 0f),
                new Vector3(rdimx, -rdimy, 0f),
                new Vector3(-rdimx, -rdimy, 0f),
            };
        

            int[] gtris = new int[] {
                baseIndex + 0, baseIndex + 1, baseIndex + 3,
                baseIndex + 3, baseIndex + 1, baseIndex + 2
            };

            gVerts.AddRange(GetTFVerts(front, pos, ori));
            gNormals.AddRange(GetNorms(front, ori));
            gTris.AddRange(gtris);
            return baseIndex + 4;
        }

        public static List<Vector3> GetTFVerts(Vector2[] verts, Vector3 pos, Quaternion ori)
        {
            var tVerts = new List<Vector3>();
            foreach (var vert in verts)
            {
                tVerts.Add(pos + ori * vert);
            }

            return tVerts;
        }

        public static List<Vector3> GetTFVerts(List<Vector3> verts, Vector3 pos, Quaternion ori)
        {
            var tVerts = new List<Vector3>();
            foreach (var vert in verts)
            {
                tVerts.Add(pos + ori * vert);
            }

            return tVerts;
        }

        public static List<Vector3> GetTFNorms(List<Vector3> norms, Quaternion ori)
        {
            var tNorms = new List<Vector3>();
            foreach (var norm in norms)
            {
                tNorms.Add(ori * norm);
            }

            return tNorms;
        }

        public static List<Vector3> GetNorms(List<Vector3> verts, Quaternion ori)
        {
            var norms = new List<Vector3>();
            foreach (var vert in verts)
            {
                norms.Add(ori * -Vector3.forward);
            }

            return norms;
        }

        public static List<Vector3> GetTFNorms(Vector2[] verts, Quaternion ori)
        {
            var norms = new List<Vector3>();
            foreach (var vert in verts)
            {
                norms.Add(ori * -Vector3.forward);
            }

            return norms;
        }

        public static Quaternion GetQuatFromDir(Vector3 dir, Vector3 up)
        {
            var quat = Quaternion.LookRotation(dir, up);

            if (dir == Vector3.up)
            {
                quat = Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
            }

            if (dir == Vector3.down)
            {
                quat = Quaternion.AngleAxis(90, Vector3.up) *
                        Quaternion.AngleAxis(-90, Vector3.left) * Quaternion.AngleAxis(0, Vector3.forward);
            }

            return quat;
        }
    }
}
