using System.Collections.Generic;
using UnityEngine;
using Geometry;

namespace Plotter
{
    /// <summary>
    /// Attach this plotter to a game object to generate the geometry and  the plottin commands received from the TCPServer
    /// </summary>
    public class PlotCmd : MonoBehaviour
    {
        public bool dbgPlotOn = false;
        [Range(3, 100f)]
        public int xySegments = 10;

        [Range(1, 100f)]
        public int zSegments = 10;

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

        const float pointScale = 1 / 15f;
        
        // A mapping between the interial material name and material in the Resources folder
        readonly Dictionary<string, string> colMatNames = new Dictionary<string, string>
        {
            { "red", "RedMat" },
            { "green", "GreenMat" },
            { "blue", "BlueMat" },
            { "orange", "OrangeMat" },
            { "turquoise", "TurquoiseMat"},
            { "pink", "PinkMat" },
            { "yellow", "YellowMat" },
            { "lightblue", "LightBlueMat" },
            { "violet", "VioletMat" },
            { "brown", "BrownMat" }
            };

        // A mapping between a color index and a color 
        readonly Dictionary<int, string> colIndices = new Dictionary<int, string>
        {
            {0, "red" },
            {1, "green" },
            {2, "blue" },
            {3, "orange" },
            {4, "turquoise" },
            {5, "pink" },
            {6, "yellow" },
            {7, "lightblue" },
            {8, "violet" },
            {9, "brown" }
        };

        private Dictionary<string, Material> matNameMaterials = new Dictionary<string, Material>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void HandleCommand(Command cmd)
        {
            if (cmd.commandType == Command.CommandType.Dbg)
            {
                cmd = DbgPlot();
            }

            var countOfVerts = cmd.getFloatVectorLength("x");
            var countOfColorIndices = cmd.getIntVectorLength("colors");
            var colorIndices = cmd.getIntVector("colors");
            Dbg.Log("");
            Dbg.Log("New plot");
            Dbg.Log("Handling a plotting command. " + countOfVerts + " data points.");
            Dbg.Log("Color indices: " + countOfColorIndices);

            var xVector = cmd.getFloatVector("x");
            var yVector = cmd.getFloatVector("y");
            var zVector = cmd.getFloatVector("z");

            var colorIndInModel = new Dictionary<int, List<Vector3>>();
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;


            for (var i = 0; i < countOfColorIndices; i++)
            {
                var currentIndex = colorIndices[i];
                if (!colIndices.ContainsKey(currentIndex))
                {
                    currentIndex = 0;
                }

                if (!colorIndInModel.ContainsKey(currentIndex))
                {
                    colorIndInModel.Add(currentIndex, new List<Vector3>());
                }

                var xVal = xVector[i];
                var yVal = yVector[i];
                var zVal = zVector[i];

                if (xVal < minX)
                    minX = xVal;
                if (yVal < minY)
                    minY = yVal;
                if (zVal < minZ)
                    minZ = zVal;
                if (xVal > maxX)
                    maxX = xVal;
                if (yVal > maxY)
                    maxY = yVal;
                if (zVal > maxZ)
                    maxZ = zVal;

                var pt = new Vector3(xVector[i], yVector[i], zVector[i]);
                colorIndInModel[currentIndex].Add(pt);
            }

            var aX = Mathf.Max(Mathf.Abs(maxX), Mathf.Abs(minX));
            var aY = Mathf.Max(Mathf.Abs(maxY), Mathf.Abs(minY));
            var aZ = Mathf.Max(Mathf.Abs(maxZ), Mathf.Abs(minZ));

            var type = cmd.getString("type");
            var ptSize = cmd.getFloatVector("size");

            Dbg.Log("Type: " + type + " pt: " + ptSize[0]);
            Dbg.Log("Colors in model: " + colorIndInModel.Count);

            float modelScale = 0.5f / (2f * Mathf.Max(Mathf.Max(aX, aY), aZ) + pointScale * ptSize[0]);

            Dbg.Log("minx " + minX + " maxx " + maxX);
            Dbg.Log("miny " + minY + " maxy " + maxY);
            Dbg.Log("minz " + minZ + " maxz " + maxZ);

            Dbg.Log("Scaling value: " + modelScale);

            foreach (var kvp in colorIndInModel)
            {
                //Dbg.Log("Wft happens: " + kvp.Key);
                CreateDataPoints(kvp.Value, kvp.Key, type, pointScale * ptSize[0], modelScale);
            }
        }


        public void Clear()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void Awake()
        {
            // Load marker materials in the very start
            LoadMaterials();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (dbgPlotOn)
            {
                var cmd = DbgPlot();
                HandleCommand(cmd);
            }
        }


        void LoadMaterials()
        {
            foreach (var matName in colMatNames.Values)
            {
                var material = Resources.Load(matName, typeof(Material)) as Material;
                matNameMaterials.Add(matName, material);
                //Dbg.Log("Adding material: " + matName);
            }
        }

        void CreateDataPoints(List<Vector3> dataPoints, int colorIndex, string pointType = "sphere", float pointSize = 1f, float modelScale = 1.0f)
        {
            var color = colIndices[0];
            var colorName = "";

            if (colIndices.TryGetValue(colorIndex, out colorName))
            {
                color = colorName;
            }

            CreateDataPoints(dataPoints, color, pointType, pointSize, modelScale);
        }

        void CreateDataPoints(List<Vector3> dataPoints, string color, string pointType = "sphere", float pointSize = 1f, float modelScale = 1.0f)
        {
            var materialName = colMatNames["red"];
            var matName = "";

            if (colMatNames.TryGetValue(color, out matName))
            {
                materialName = matName;
            }

            bool pointSphere = true;
            if (pointType == "cube")
            {
                pointSphere = false;
            }

            var go = new GameObject(pointType, typeof(MeshRenderer), typeof(MeshFilter));
            go.name = "Datapoints " + dataPoints.Count;
            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            var mFilter = go.GetComponent<MeshFilter>();
            mFilter.sharedMesh = mesh;
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3();

            var material = matNameMaterials[materialName];
            go.GetComponent<Renderer>().material = material;

            Generate(mesh, dataPoints, pointSphere, pointSize, modelScale);
        }

        void Generate(Mesh mesh, List<Vector3> dataPoints, bool pointSphere, float pointSize, float modelScale)
        {
            //mesh.Clear();
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tris = new List<int>();
            var baseIndex = 0;

            var dimx = pointSize * pointScale;
            var dimy = pointSize * pointScale;
            var dimz = pointSize * pointScale;

            for (int i = 0; i < dataPoints.Count; i++)
            {
                var pos = modelScale * dataPoints[i];
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

        Command DbgPlot()
        {
            const int n = 50000;
            float[] xVec = new float[n];
            float[] yVec = new float[n];
            float[] zVec = new float[n];
            int[] colors = new int[n];

            const double step = 6 * Mathf.PI / n;
            const double rStep = 1.0 / n;

            for (int i = 0; i < n; i++)
            {
                float angle = (float)(i * step);

                xVec[i] = (float)(i * rStep * Mathf.Cos(angle)) + Random.Range(-0.04f, 0.04f);
                yVec[i] = (float)(i * rStep + UnityEngine.Random.Range(-0.2f, 0.2f)) - 0.5f;
                zVec[i] = (float)(i * rStep * Mathf.Sin(angle)) + Random.Range(-0.04f, 0.04f);

                colors[i] = i % matNameMaterials.Count;
            }

            var cmd = new Command();
            cmd.commandType = Command.CommandType.Points;
            cmd.setFloatVector("x", xVec);
            cmd.setFloatVector("y", yVec);
            cmd.setFloatVector("z", zVec);
            cmd.setFloatVector("size", new float[] { 1.0f });

            cmd.setIntVector("colors", colors);
            cmd.setString("type", "cube");

            return cmd;
        }
    }
}