using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Environment : MonoBehaviour
    {
        // Unity Stuff
        public GameObject plantPrefab;
        public GameObject villagerMalePrefab;
        public GameObject villagerFemalePrefab;
        public GameObject userInterface;
        public GameObject buildingController;
        
        // Properties
        public GameObject VillagerMalePrefab { get; set; }
        public GameObject VillagerFemalePrefab { get; set; }
        public List<GameObject> PlantFoodSources { get; set; }
        public List<GameObject> Villagers { get; set; }
        public int NumberOfPlantFoodSources { get; set; }
        public Transform[] WaterSources { get; set; }
        public WaterSource[] WaterSourcess { get; set; }
        public Vector3[] WaterSourceCoords { get; set; }
        public int NumWaterSources { get; set; }
        public int NumDays { get; set; }
        public int NumVillagersToSpawn { get; set; }
        public int NumDaysPassed { get; set; }
        public float TimeToDayIncrease { get; set; }
        public float DayProgressToNextDay { get; set; }
        public UserInterface UserInterface { get; set; }

        void Start()
        {
            UserInterface = userInterface.GetComponent<UserInterface>();
            
            // Get number of plants, villagers and days from customisation menu
            NumberOfPlantFoodSources = PlayerPrefs.GetInt("numPlants");
            NumVillagersToSpawn = PlayerPrefs.GetInt("numVillagers");
            NumDays = PlayerPrefs.GetInt("numDays");
            
            NumWaterSources = 10;
            TimeToDayIncrease = 10;
            
            VillagerMalePrefab = villagerMalePrefab;
            VillagerFemalePrefab = villagerFemalePrefab;
            
            // Create waypoints for villagers to navigate to when finding water
            WaterSourceCoords = new Vector3[NumWaterSources];
            GenerateWaterSourceCoords();
            // Create waypoints at coordinates defined above
            WaterSources = new Transform[NumWaterSources];
            WaterSourcess = new WaterSource[NumWaterSources];
            GenerateWaterSources();
            
            // Generate plants
            PlantFoodSources = new List<GameObject>();
            GeneratePlantFoodSources();
            
            // Spawn villagers
            Villagers = new List<GameObject>();
            SpawnVillagers();
        }

        private void Update()
        {
            DayProgressToNextDay += Time.deltaTime * 1 / TimeToDayIncrease;
            
            if (DayProgressToNextDay >= 1)
            {
                // Day has elapsed
                NumDaysPassed++;
                DayProgressToNextDay = 0;
            }

            if (GetNumVillagers() == 0 || NumDays == NumDaysPassed)
            {
                // All villagers have died
                // Or max number of days has elapsed
                UserInterface.GameEnd();
            }
            
            // Check to see if villagers are alive
            CheckVillagerStatus();
        }

        void GenerateWaterSourceCoords()
        {
            // Waypoint coordinates for villagers to drink at
            WaterSourceCoords[0] = new Vector3(38, 5, 58);
            WaterSourceCoords[1] = new Vector3(36, 4, 56);
            WaterSourceCoords[2] = new Vector3(34, 5, 55);
            WaterSourceCoords[3] = new Vector3(32, 5, 53);
            WaterSourceCoords[4] = new Vector3(44, 3, 23);
            WaterSourceCoords[5] = new Vector3(45, 3, 20);
            WaterSourceCoords[6] = new Vector3(49, 3, 17);
            WaterSourceCoords[7] = new Vector3(51, 3, 14);
            WaterSourceCoords[8] = new Vector3(40, 5, 57);
            WaterSourceCoords[9] = new Vector3(41, 5, 59);
        }

        void CheckVillagerStatus()
        {
            foreach (GameObject v in Villagers)
            {
                if (v == null)
                {
                    // Villager has died, remove from list of villagers
                    RemoveVillager(v);
                }
            }
        }
        
        void GenerateWaterSources()
        {
            for (int i = 0; i < NumWaterSources; i++)
            {
                // Generate waypoints and store in list
                GameObject waterSource = new GameObject();
                waterSource.transform.position = WaterSourceCoords[i];
                WaterSources[i] = waterSource.transform;
                WaterSource ws = new WaterSource(waterSource, true);
                WaterSourcess[i] = ws;
            }
        }

        public WaterSource GetWaterSource()
        {
            bool isEmpty = false;
            int num = 0;
            while (!isEmpty)
            {
                // Check if a villager is at the water source
                num = Random.Range(0, NumWaterSources-1);
                if (WaterSourcess[num].IsEmpty)
                {
                    isEmpty = true;
                }
            }
            // Return an empty water source
            return WaterSourcess[num];
        }

        // Iteratively generate a defined number of plants at the start of the game
        void GeneratePlantFoodSources()
        {
            // Create plants using plant prefab at a random location
            for (int i = 0; i < NumberOfPlantFoodSources; i++)
            {
                GameObject go = Instantiate(plantPrefab, GetRandomPoint(), Quaternion.identity);
                PlantFoodSources.Add(go);
            }
        }

        // Generate a single plant
        public GameObject GeneratePlantFoodSource(Vector3 position)
        {
            GameObject go = Instantiate(plantPrefab, position, Quaternion.identity);
            PlantFoodSources.Add(go);
            return go;
        }

        // Called when a villager decides to eat a defined plant
        public void EatPlantFoodSource(GameObject plantFoodSource)
        {
            foreach (GameObject pfs in PlantFoodSources.ToList())
            {
                if (pfs == plantFoodSource)
                {
                    // Destroy the plant
                    PlantFoodSources.Remove(plantFoodSource);
                    Destroy(plantFoodSource);
                }
            }
        }
        
        // Find a random plant on the terrain
        public GameObject FindPlantFoodSource()
        {
            int n = Random.Range(0, PlantFoodSources.Count - 1);
            return PlantFoodSources[n];
        }

        // Iteratively spawn a defined number of villagers at the start of the game
        public void SpawnVillagers()
        {
            for (int i = 0; i < NumVillagersToSpawn; i++)
            {
                // Use modulo to determine whether villager will spawn as male or female
                // 50/50 chance of spawning as either gender, therefore % 2 is used
                if (i % 2 == 0)
                {
                    // Spawn male villager
                    GameObject go = Instantiate(villagerMalePrefab, GetRandomPoint(), Quaternion.identity);
                    go.tag = "Male";
                    go.GetComponent<Villager>().AssignGender(true);
                    Villagers.Add(go);
                    go.GetComponent<Villager>().Name = "Villager " + GetNumVillagers();
                }
                else
                {
                    // Spawn female villager
                    GameObject go = Instantiate(villagerFemalePrefab, GetRandomPoint(), Quaternion.identity);
                    go.tag = "Female";
                    go.GetComponent<Villager>().AssignGender(false);
                    Villagers.Add(go);
                    go.GetComponent<Villager>().Name = "Villager " + GetNumVillagers();
                }
            }
        }

        // Spawn a single villager at a defined position
        public void SpawnVillager(Vector3 position)
        {
            // Generate a random number between 0 and 1 to determine if male or female villager
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                // Male villager
                GameObject go = Instantiate(VillagerMalePrefab, position, Quaternion.identity);
                go.tag = "Male";
                Villagers.Add(go);
                go.GetComponent<Villager>().Name = "Villager " + GetNumVillagers();
            }
            else
            {
                // Female villager
                GameObject go = Instantiate(VillagerFemalePrefab, position, Quaternion.identity);
                go.tag = "Female";
                Villagers.Add(go);
                go.GetComponent<Villager>().Name = "Villager " + GetNumVillagers();
            }
        }

        public void RemoveVillager(GameObject v)
        {
            Villagers.Remove(v);
        }

        public int GetNumVillagers()
        {
            return Villagers.Count;
        }
        
        // Return the villagers as GameObjects
        public List<GameObject> GetVillagersAsGameObjects()
        {
            return Villagers;
        }
        
        // Return the villagers as Villager objects
        public List<Villager> GetVillagersAsVillagers()
        {
            List<Villager> villagers = new List<Villager>();

            foreach (GameObject villager in Villagers)
            {
                Villager v = villager.GetComponent<Villager>();
                villagers.Add(v);
            }
            
            return villagers;
        }

        // Find average age of villagers
        public int GetAverageAgeOfVillagers()
        {
            List<Villager> villagers = new List<Villager>();
            villagers = GetVillagersAsVillagers();
            int totalAge = 0;
            int numVillagers = GetNumVillagers();
            int avgAge = 0;

            foreach (Villager v in villagers)
            {
                totalAge += v.Age;
            }

            avgAge = (totalAge / numVillagers);

            return avgAge;
        }
        
        // Find average wealth of villagers
        public int GetAverageWealthOfVillagers()
        {
            List<Villager> villagers = new List<Villager>();
            villagers = GetVillagersAsVillagers();
            int totalWealth = 0;
            int numVillagers = GetNumVillagers();
            int avgWealth = 0;

            foreach (Villager v in villagers)
            {
                totalWealth += v.Wealth;
            }

            avgWealth = (totalWealth / numVillagers);

            return avgWealth;
        }
        
        // Find average happiness of villagers
        public float GetAverageHappinessOfVillagers()
        {
            List<Villager> villagers = new List<Villager>();
            villagers = GetVillagersAsVillagers();
            float totalHappiness = 0;
            int numVillagers = GetNumVillagers();
            float avgHappiness = 0;

            foreach (Villager v in villagers)
            {
                totalHappiness += v.Happiness;
            }

            avgHappiness = (totalHappiness / numVillagers);

            return avgHappiness;
        }

        // Get Random Point on a Navmesh surface
        public Vector3 GetRandomPoint() {
            // Get Random Point inside Sphere which position is center, radius is maxDistance
            Vector3 center = new Vector3(50, 0, 50);
            float maxDistance = 75;
            Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

            NavMeshHit hit; // NavMesh Sampling Info Container

            // from randomPos find a nearest point on NavMesh surface in range of maxDistance
            NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

            return hit.position;
        }
    }