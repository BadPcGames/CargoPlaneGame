using HeneGames.Airplane;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Assets.scripts.WorldGenerator
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject ChunkPrefabs;
        [SerializeField] private GameObject WatterPrefab;
        [SerializeField] private GameObject Cam;
        [SerializeField] private int range = 1;

        [SerializeField] private float sizeChunk;
        [SerializeField] private float chunkScale;

        [SerializeField] private int mainSeed = 2;
        [SerializeField] private bool manualSeed;

        [SerializeField] private GameObject StaionPrefab;
        [SerializeField] private GameObject HomeRunwayPrefab;
        [SerializeField] private int numberOfStations = 10;
        [SerializeField] private float stationHeight = 117f;
        [SerializeField] private float stationMinDistance = 5f;


        private Dictionary<Vector2Int, int> Stations = new Dictionary<Vector2Int, int>(); 
        private Vector2Int camChunk = new Vector2Int();
        private IEnumerator coroutine;

        private void Awake()
        {
            Cam = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {
            Debug.Log(Cam);
            if (!manualSeed)
            {
                mainSeed = System.DateTime.Now.Millisecond;
            }
            Random.seed = mainSeed;

            GenerateStationChunkPositions();

            camChunk = new Vector2Int(0, 0);
            for (int i = -range; i <= range; i++)
            {
                for (int j = -range; j <= range; j++)
                {
                    generateChunk(i, j);
                }
            }
        }
        
        public void setSeed(string value)
        {
            mainSeed = int.Parse(value);
        }

        public void setIsManualSeed(bool value)
        {
            manualSeed = value;
        }

        private void GenerateStationChunkPositions()
        {
            Stations.Clear();
            int id = 0;
            Stations.Add(new Vector2Int(0,0), id);
            AddStation(HomeRunwayPrefab,new Vector3(0,0,0));
            id++;
            while (Stations.Count < numberOfStations - 1)
            {
                Vector2Int candidate = new Vector2Int(
                    Random.Range(-100, 100),
                    Random.Range(-100, 100)
                );

                bool isFarEnough = true;
                foreach (var pos in Stations.Keys)
                {
                    if (Vector2.Distance(candidate, pos) < stationMinDistance)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                {
                    Stations.Add(candidate, id);
                    AddStation(StaionPrefab, new Vector3(candidate.x, 0, candidate.y));
                    Debug.Log(candidate);
                    id++;
                }
            }
        }


        private void AddStation(GameObject staionPrefab, Vector3 pos)
        {
            Vector3 stationPosition = pos*sizeChunk + new Vector3(sizeChunk * 0.5f, stationHeight, sizeChunk * 0.5f);
            GameObject station = Instantiate(staionPrefab, stationPosition, Quaternion.identity);
            station.transform.SetParent(transform);
            if (station.GetComponent<StationStats>() != null)
            {
                station.GetComponent<StationStats>().setId(Stations.Count);
            }
        }


        private void generateChunk(int i, int j)
        {
            Vector2Int chunkCoord = new Vector2Int(i, j);
            Vector3 pos = new Vector3(i * sizeChunk, 0, j * sizeChunk);

            ChunkPrefabs.GetComponent<ChunkController>().SetChunk(pos / chunkScale, Stations.ContainsKey(chunkCoord));
            ChunkPrefabs.GetComponent<ChunkController>().SetChunkSeed(mainSeed);
            ChunkPrefabs.GetComponent<ChunkController>().MakeMash();

            GameObject chunk = Instantiate(ChunkPrefabs, pos, Quaternion.identity);
            chunk.transform.localScale = Vector3.one * chunkScale;
            chunk.transform.SetParent(transform);
        }


        private void FixedUpdate()
        {
            if (camIsChangeChunk())
            {
                int changeByX = Mathf.FloorToInt(Cam.transform.position.x / sizeChunk) - camChunk.x;
                int changeByZ = Mathf.FloorToInt(Cam.transform.position.z / sizeChunk) - camChunk.y;

                camChunk = new Vector2Int(Mathf.FloorToInt(Cam.transform.position.x / sizeChunk),
                                            Mathf.FloorToInt(Cam.transform.position.z / sizeChunk));

                GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("Ground");
                foreach (GameObject obj in objectsToDelete)
                {
                    if ((obj.transform.position.x < (camChunk.x - range) * sizeChunk) || (obj.transform.position.x > (camChunk.x + range) * sizeChunk)
                    || (obj.transform.position.z < (camChunk.y - range) * sizeChunk) || (obj.transform.position.z > (camChunk.y + range) * sizeChunk))
                        Destroy(obj);
                }
 
                coroutine = addChunks(changeByX, changeByZ, true);
                StartCoroutine(coroutine);
                WatterPrefab.transform.position = new Vector3(camChunk.x*sizeChunk, transform.position.y, camChunk.y * sizeChunk);
            }
        }


        private IEnumerator addChunks(int changeByX, int changeByZ, bool wait)
        {
            if (changeByX != 0)
            {
                int i = camChunk.x + (range * changeByX);
                for (int j = camChunk.y - range; j <= camChunk.y + range; j++)
                {
                    generateChunk(i, j);
                    if (wait)
                    {
                        yield return new WaitForSecondsRealtime(0.01f);
                    }
                }
            }
            if (changeByZ != 0)
            {
                int j = camChunk.y + (range * changeByZ);
                for (int i = camChunk.x - range; i <= camChunk.x + range; i++)
                {
                    generateChunk(i, j);
                    if (wait)
                    {
                        yield return new WaitForSecondsRealtime(0.01f);
                    }
                }
            }
        }

        private bool camIsChangeChunk()
        {
            int x, z;
            x = Mathf.FloorToInt(Cam.transform.position.x / sizeChunk);
            z = Mathf.FloorToInt(Cam.transform.position.z / sizeChunk);

            return camChunk.x != x || camChunk.y != z;
        }
    }


}