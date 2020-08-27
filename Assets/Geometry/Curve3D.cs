using System.Collections.Generic;
using UnityEngine;


namespace Geometry
{
    /// <summary>
    /// Attach this to a gameobject to generate a geometric curve
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Curve3D : MonoBehaviour
    {
        [Range(1, 1000)]
        public int segments = 1;

        [Range(0.01f, 4f)]
        public float length = 1f;

        /// <summary>
        /// A path object containing a list of vertices describing the "curve" of the geometry
        /// </summary>
        public ScriptableObject loftPath;
        public ScriptableObject profilePath;
        public bool hasCaps;
        public bool faceted;

        Mesh mMesh;

        void GenerateMesh()
        {
            mMesh.Clear();

            // Make the profile Loop
            Loop2D profileLoop = new Loop2D();

            profileLoop.SetVerts((PathInterface)profilePath);

            var stepSize = 1 / (float)(segments);

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();

            for (int i = 0; i < segments + 1; i++)
            {
                var t = i * stepSize;
                var posOri = ((PathInterface)loftPath).GetInterpPoint(t);
                verts.AddRange(profileLoop.GetVerts(posOri, faceted));
                normals.AddRange(profileLoop.GetNormals(posOri, faceted));
            }

            var tris = new List<int>();

            for (int i = 0; i < segments; i++)
            {
                int rootIndex = i * profileLoop.VertexCount;

                for (int loopVertIndex = 0; loopVertIndex < profileLoop.VertexCount; loopVertIndex++)
                {
                    int currentVert = rootIndex + loopVertIndex;
                    int nextVert = rootIndex + (loopVertIndex + 1) % profileLoop.VertexCount;
                    int nextLoopCurrentVert = currentVert + profileLoop.VertexCount;
                    int nextLoopNextVert = nextVert + profileLoop.VertexCount;

                    tris.Add(currentVert);
                    tris.Add(nextLoopNextVert);
                    tris.Add(nextLoopCurrentVert);

                    tris.Add(currentVert);
                    tris.Add(nextVert);
                    tris.Add(nextLoopNextVert);

                    /* Debug.Log("Current vert: " + currentVert);
                    Debug.Log("nextLoopCurrentVert vert: " + nextLoopCurrentVert);
                    Debug.Log("nextLoopNextVert vert: " + nextLoopNextVert);

                    Debug.Log("nextVert vert: " + nextVert);
                    Debug.Log("nextLoopCurrentVert vert: " + nextLoopCurrentVert);
                     Debug.Log("nextLoopNextVert vert: " + nextLoopNextVert);*/
                }
            }

            if (hasCaps)
            {
                var posOriCap1 = ((PathInterface)loftPath).GetInterpPoint(0);
                var posOriCap2 = ((PathInterface)loftPath).GetInterpPoint(1);

                verts.AddRange(profileLoop.GetVerts(posOriCap1, faceted));
                normals.AddRange(profileLoop.GetNormals(posOriCap1, faceted));

                verts.AddRange(profileLoop.GetVerts(posOriCap2, faceted));
                normals.AddRange(profileLoop.GetNormals(posOriCap2, faceted));

                verts.Add(posOriCap1.position);
                verts.Add(posOriCap2.position);

                //int cap1MiddlePtIndex = verts.Count - 2;
                //int cap2MiddlePtIndex = verts.Count - 1;

                var rootVertCap1 = verts.Count - 2 - segments * profileLoop.VertexCount;
                //int rootVertCap2 = rootVertCap1 + profileLoop.VertexCount;

                for (int i = 0; i < profileLoop.VertexCount; i++)
                {

                }
            }

            mMesh.SetVertices(verts);
            mMesh.SetNormals(normals);
            mMesh.SetTriangles(tris, 0);
            mMesh.RecalculateNormals();
        }
       
        void Start()
        {
            mMesh = new Mesh
            {
                name = "Curve3D"
            };
            var mFilter = GetComponent<MeshFilter>();
            mFilter.sharedMesh = mMesh;


            //loftPath = (Path3D)ScriptableObject.CreateInstance<EllipsePath>();
            //GenerateMesh();
        }

        // Update is called once per frame, generate a dynamic mesh.
        void Update()
        {
            GenerateMesh();
        }
    }
}