using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Environment : MonoBehaviour
    {
        // Unity Stuff
        public GameObject plantPrefab;
        
        // Properties
        private static List<GameObject> _plantFoodSources;
        private int _numberOfPlantFoodSources = 25;
        private static Transform[] _waterSources;
        private int _numWaterSources = 2;
        void Start()
        {
            _waterSources = new Transform[_numWaterSources];
            GenerateWaterSources();
            _plantFoodSources = new List<GameObject>();
            GeneratePlantFoodSources();
        }

        void GenerateWaterSources()
        {
            for (int i = 0; i < _numWaterSources; i++)
            {
                GameObject waterSource = new GameObject();
                waterSource.transform.position = new Vector3(38, 0, 58);
                _waterSources[i] = waterSource.transform;
            }
        }

        public static Transform GetWaterSourcePosition()
        {
            return _waterSources[0];
        }

        void GeneratePlantFoodSources()
        {
            for (int i = 0; i < _numberOfPlantFoodSources; i++)
            {
                GameObject go = Instantiate(plantPrefab, GetRandomPoint(), Quaternion.identity);
                _plantFoodSources.Add(go);
            }
        }

        public static void EatPlantFoodSource(GameObject plantFoodSource)
        {
            foreach (GameObject pfs in _plantFoodSources)
            {
                if (pfs == plantFoodSource)
                {
                    _plantFoodSources.Remove(plantFoodSource);
                    Destroy(plantFoodSource);
                }
            }
        }
        
        public static GameObject FindPlantFoodSource()
        {
            return _plantFoodSources.First();
        }

        // Get Random Point on a Navmesh surface
        public static Vector3 GetRandomPoint() {
            // Get Random Point inside Sphere which position is center, radius is maxDistance
            Vector3 center = new Vector3(50, 0, 50);
            float maxDistance = 25;
            Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

            NavMeshHit hit; // NavMesh Sampling Info Container

            // from randomPos find a nearest point on NavMesh surface in range of maxDistance
            NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

            return hit.position;
        }
    }