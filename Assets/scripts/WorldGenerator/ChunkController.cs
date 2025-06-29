using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.scripts.WorldGenerator
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ChunkController : MonoBehaviour
    {
        [Header("Chunk settings")]
        [SerializeField] private int width = 30;
        [SerializeField] private int height = 10;

        [SerializeField] float resolution = 1;
        [SerializeField] float noiseScale = 1;

        [SerializeField] private float heightTresshold = 0.5f;

        [SerializeField] bool visualizeNoise;
        [SerializeField] bool use3DNoise;

        [Header("Chunk color")]
        [SerializeField] Gradient terrainGradient;
        [SerializeField] private Material material;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private float[,,] heights;

        private Mesh mesh;
        private Texture2D gradientTexture;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private FastNoiseLite noise;
        private Vector3 chunkOffset;
        private bool isContainRunway;
        private FastNoiseLite biomeNoise;

        public void SetChunk(Vector3 offset, bool _isContainRunway)
        {
            meshFilter = GetComponent<MeshFilter>();
            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFrequency(0.03f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(4);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(0.5f);

            biomeNoise = new FastNoiseLite();
            biomeNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            biomeNoise.SetFrequency(0.005f);

            chunkOffset = offset;
            isContainRunway = _isContainRunway;
        }

        public void SetChunkSeed(int value)
        {
            noise.SetSeed(value);
            biomeNoise.SetSeed(value);
        }

        public void MakeMash()
        {
            SetHeights();
            MarchCubes();
            SetMesh();
            //paintTerrain();
        }

        private void SetMesh()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();

            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();

            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();

            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();

            mesh = new Mesh();

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;

            meshRenderer.material = material;
        }

        //private void paintTerrain()
        //{
        //    GradientToTexture();

        //    gradientTexture.wrapMode = TextureWrapMode.Clamp;

        //    float minTerrainHeight = mesh.bounds.min.y + transform.position.y - 0.1f;
        //    float maxTerrainHeight = mesh.bounds.max.y + transform.position.y + 0.1f;

        //    material.SetTexture("_TerrainGradient", gradientTexture);
        //    material.SetFloat("_MinTerrainHeight", minTerrainHeight);
        //    material.SetFloat("_MaxTerrainHeight", maxTerrainHeight);
        //}

        private void SetHeights()
        {
            heights = new float[width + 1, height + 1, width + 1];

            for (int x = 0; x < width + 1; x++)
            {
                for (int y = 0; y < height + 1; y++)
                {
                    for (int z = 0; z < width + 1; z++)
                    {
                        float worldX = x + chunkOffset.x;
                        float worldY = y + chunkOffset.y;
                        float worldZ = z + chunkOffset.z;

                        float biomeValue = (biomeNoise.GetNoise(worldX, worldZ) + 1f) * 0.5f;

                        float biomeHeightMultiplier = Mathf.Lerp(0.4f, 1.2f, biomeValue);
                        float biomeNoiseScale = Mathf.Lerp(0.45f, 1.4f, biomeValue);

                        float nx = worldX * biomeNoiseScale;
                        float ny = worldY * biomeNoiseScale;
                        float nz = worldZ * biomeNoiseScale;

                        float noiseValue = noise.GetNoise(nx, ny, nz);
                        float noiseHeight = height * biomeHeightMultiplier * ((noiseValue + 1f) * 0.5f);

                        float flatHeight = height * 0.55f;

                        float heightValue = noiseHeight;

                        if (isContainRunway)
                        {
                            if (x > 0.3 * width && x < 0.7 * width && z > 0.3 * width && z < 0.7 * width)
                            {
                                heightValue = flatHeight;
                            }
                            else if (x > 0.2 * width && x < 0.8 * width && z > 0.2 * width && z < 0.8 * width)
                            {
                                heightValue = (flatHeight + noiseHeight) / 2;
                            }
                        }

                        float distToSurface;

                        if (y <= heightValue - 0.5f)
                            distToSurface = 0f;
                        else if (y > heightValue + 0.5f)
                            distToSurface = 1f;
                        else if (y > heightValue)
                            distToSurface = y - heightValue;
                        else
                            distToSurface = heightValue - y;

                        heights[x, y, z] = distToSurface;
                    }
                }
            }
        }

        private int GetConfigIndex(float[] cubeCorners)
        {
            int configIndex = 0;

            for (int i = 0; i < 8; i++)
            {
                if (cubeCorners[i] > heightTresshold)
                {
                    configIndex |= 1 << i;
                }
            }

            return configIndex;
        }

        private void MarchCubes()
        {
            vertices.Clear();
            triangles.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        float[] cubeCorners = new float[8];

                        for (int i = 0; i < 8; i++)
                        {
                            Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                            cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                        }

                        MarchCube(new Vector3(x, y, z), cubeCorners);
                    }
                }
            }
        }

        private void MarchCube(Vector3 position, float[] cubeCorners)
        {
            int configIndex = GetConfigIndex(cubeCorners);

            if (configIndex == 0 || configIndex == 255)
            {
                return;
            }

            int edgeIndex = 0;
            for (int t = 0; t < 5; t++)
            {
                for (int v = 0; v < 3; v++)
                {
                    int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                    if (triTableValue == -1)
                    {
                        return;
                    }

                    Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                    Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                    Vector3 vertex = (edgeStart + edgeEnd) / 2;

                    vertices.Add(vertex);
                    triangles.Add(vertices.Count - 1);

                    edgeIndex++;
                }
            }
        }

        private void GradientToTexture()
        {
            gradientTexture = new Texture2D(100, 1, TextureFormat.RGBA32, false);
            Color[] pixelColors = new Color[100];

            for (int i = 0; i < 100; i++)
            {
                pixelColors[i] = terrainGradient.Evaluate(i / 100f);
            }

            gradientTexture.SetPixels(pixelColors);
            gradientTexture.Apply();
        }

        private void OnDrawGizmosSelected()
        {
            if (!visualizeNoise || !Application.isPlaying)
            {
                return;
            }

            for (int x = 0; x < width + 1; x++)
            {
                for (int y = 0; y < height + 1; y++)
                {
                    for (int z = 0; z < width + 1; z++)
                    {
                        Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                        Gizmos.DrawSphere(new Vector3(x * resolution, y * resolution, z * resolution), 0.2f * resolution);
                    }
                }
            }
        }
    }
}

[System.Serializable]
class Layer
{
    public Texture2D texture;
    [Range(0, 1)] public float startHeight;
}



