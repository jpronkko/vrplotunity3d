using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    /// <summary>
    /// Generates and displays a set of data points as spheres or cubes.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PointCloud : MonoBehaviour
    {
        [Range(3, 100f)]
        public int xySegments = 3;

        [Range(1, 100f)]
        public int zSegments = 1;

        [Range(0.01f, 4f)]
        public float dimx = 1f;

        [Range(0.01f, 4f)]
        public float dimy = 1f;

        [Range(0.01f, 4f)]
        public float dimz = 1f;

        [Range(0.0f, 3.5f)]
        public float posx = 1f;

        [Range(0.0f, 3.5f)]
        public float posy = 1f;

        [Range(0.0f, 3.5f)]
        public float posz = 1f;

        /// <summary>
        /// A list of data points to be rendered.
        /// </summary>
        public List<Vector3> dataPoints = new List<Vector3>();

        const float scale = 1 / 25f;
        void Generate(Mesh mesh, List<Vector3> dataPoints, bool pointSphere, float pointSize)
        {
            //mesh.Clear();
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tris = new List<int>();
            var baseIndex = 0;

            var dimx = pointSize * scale;
            var dimy = pointSize * scale;
            var dimz = pointSize * scale;

            for (int i = 0; i < dataPoints.Count; i++)
            {
                var pos = dataPoints[i];
                if (pointSphere)
                {
                    baseIndex = GeomUtil.CreateSpheroid(
                                                        baseIndex,
                                                        xySegments,
                                                        zSegments,
                                                        dimx, dimz, dimy,
                                                        pos,
                                                        Quaternion.identity,
                                                        verts,
                                                        normals,
                                                        tris);
                }
                else
                {
                    baseIndex = GeomUtil.CreateCubic(
                                                        baseIndex,
                                                        dimx,
                                                        dimy,
                                                        dimz,
                                                        pos,
                                                        Quaternion.identity,
                                                        verts,
                                                        normals,
                                                        tris
                                                        );
                }
            }
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            mesh.SetTriangles(tris, 0);
        }

        public void CreateDataPoints(List<Vector3> dataPoints, string color, string pointType = "sphere", float pointSize = 1f)
        {
            var colorNames = new Dictionary<string, string>
            {
                { "red", "RedMat" },
                { "green", "greenMat" },
                { "blue", "blueMat" },
                { "orange", "orangeMat" },
                { "turquoise", "turquoiseMat"}
            };

            var colorName = colorNames["red"];

            if (colorNames.TryGetValue(color, out string colName))
            {
                colorName = colName;
            }

            bool pointSphere = true;
            if (pointType == "cube")
            {
                pointSphere = false;
            }

            var go = new GameObject(pointType, typeof(MeshRenderer), typeof(MeshFilter));
            var mesh = new Mesh();
            var mFilter = go.GetComponent<MeshFilter>();
            mFilter.sharedMesh = mesh;
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3();
            var material = Resources.Load(colorName, typeof(Material)) as Material;
            go.GetComponent<Renderer>().material = material;

            Generate(mesh, dataPoints, pointSphere, pointSize);
        }

        public void Clear()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        void Start()
        {
            //CreateDataPoints(dataPoints, "red", "cube");
            CreateDataPoints(dataPoints, "green", "sphere", 1f);
        }
    }
}