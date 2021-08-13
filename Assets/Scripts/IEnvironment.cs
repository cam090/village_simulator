    using System.Collections.Generic;
    using DefaultNamespace;
    using UnityEngine;

    public interface IEnvironment
    {
        // Properties
        GameObject VillagerMalePrefab { get; set; }
        GameObject VillagerFemalePrefab { get; set; }
        List<GameObject> PlantFoodSources { get; set; }
        List<GameObject> Villagers { get; set; }
        int NumberOfPlantFoodSources { get; set; }
        Transform[] WaterSources { get; set; }
        WaterSource[] WaterSourcess { get; set; }
        Vector3[] WaterSourceCoords { get; set; }
        int NumWaterSources { get; set; }
        int NumVillagersToSpawn { get; set; }
    }
