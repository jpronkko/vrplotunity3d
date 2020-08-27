using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    /// <summary>
    /// Generates a grid geometry with bars dimx x dimy x dimz of width, height and depth of
    /// gridWidth, gridHeight and gridDepth
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Grid : MonoBehaviour
    {
        [Range(0.001f, 4f)]
        public float dimx = 0.01f;

        [Range(0.001f, 4f)]
        public float dimy = 0.01f;

        [Range(0.001f, 4f)]
        public float dimz = 0.01f;

        [Range(0.01f, 4f)]
        public float gridWidth = 1f;

        [Range(0.01f, 4f)]
        public float gridHeight = 1f;

        [Range(0.01f, 4f)]
        public float gridDepth = 1f;

        // Offset position of the geometry
        [Range(0.0f, 3.5f)]
        public float posx = 1f;

        [Range(0.0f, 3.5f)]
        public float posy = 1f;

        [Range(0.0f, 3.5f)]
        public float posz = 1f;

        Mesh mMesh;

        void Generate()
        {
            mMesh.Clear();
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tris = new List<int>();

            int baseIndex = 0;
            
            float hGridW = gridWidth / 2f;
            float hGridH = gridHeight / 2f;
            float hGridD = gridDepth / 2f;

            var verticals = new List<Vector3>{
                            new Vector3(-hGridW + dimx/2, 0, -hGridD + dimz/2f),
                            new Vector3(-hGridW + dimx/2f, 0, hGridD - dimz/2f),
                            new Vector3( hGridW - dimy/2f, 0, hGridD - dimz/2f),
                            new Vector3( hGridW - dimy/2f, 0, -hGridD + dimz/2f) };

            foreach (var pt in verticals)
            {
                baseIndex = GeomUtil.CreateCubic(baseIndex,
                                        dimx, gridHeight - 2 * dimy, dimz,
                                        new Vector3(posx, posy, posz) + pt,
                                        Quaternion.identity,
                                        verts,
                                        normals,
                                        tris);
            }

            var horizontals = new List<Vector3>{
                            new Vector3(0, -hGridH + dimy/2f, -hGridD + dimz/2f),
                            new Vector3(0, -hGridH + dimy/2f, hGridD - dimz/2f),
                            new Vector3(0, hGridH - dimy/2f, hGridD - dimz/2f),
                            new Vector3(0, hGridH - dimy/2f, -hGridD + dimz/2f) };

            foreach (var pt in horizontals)
            {
                baseIndex = GeomUtil.CreateCubic(baseIndex,
                                        gridWidth, dimy, dimz,
                                        new Vector3(posx, posy, posz) + pt,
                                        Quaternion.identity,
                                        verts,
                                        normals,
                                        tris);
            }

            var depthinals = new List<Vector3>{
                            new Vector3(-hGridW + dimx/2f, -hGridH + dimy/2f, 0),
                            new Vector3(-hGridW + dimx/2f, hGridH - dimy/2f, 0),
                            new Vector3(hGridW - dimx/2f, hGridH - dimy/2f, 0),
                            new Vector3(hGridW - dimx/2f, -hGridH + dimy/2f, 0) };

            foreach (var pt in depthinals)
            {
                baseIndex = GeomUtil.CreateCubic(baseIndex,
                                        dimx, dimy, gridDepth - 2 * dimz,
                                        new Vector3(posx, posy, posz) + pt,
                                        Quaternion.identity,
                                        verts,
                                        normals,
                                        tris);
            }
           
            mMesh.SetVertices(verts);
            mMesh.SetNormals(normals);
            mMesh.SetTriangles(tris, 0);
        }

        void Start()
        {
            mMesh = new Mesh();
            var mFilter = GetComponent<MeshFilter>();
            mFilter.sharedMesh = mMesh;
        }

        void Update()
        {
            Generate();
        }
    }
}