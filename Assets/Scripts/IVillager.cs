using Datatypes;
    public interface IVillager : ILivingEntity
    {
        // Properties
        Environment Environment { get; set; }
        UserInterface UserInterface { get; set; }
        string Name { get; set; }
        float Hunger { get; set; }
        float Thirst { get; set; }
        float Tiredness { get; set; }
        float ReproductiveUrge { get; set; }
        float AgeProgressToNextAge { get; set; }
        float Happiness { get; set; }
        float TradingCoolDown { get; set; }
        bool RecentlyTraded { get; set; }
        float PlantingCoolDown { get; set; }
        bool RecentlyPlanted { get; set; }
        int Age { get; set; }
        int AgeLimit { get; set; }
        int Wealth { get; set; }
        VillagerAction CurrentAction{  get; set; }
        Gender Gender { get; set; }
        bool CrRunning { get; set; }
        Villager VillagerMate { get; set; }
    
        // Settings
        float TimeToDeathByHunger { get; set; }
        float TimeToDeathByThirst { get; set; }
        float TimeToDeathByTiredness { get; set; }
        float TimeToAgeIncrease { get; set; }
        float TimeToReproductiveDebuff { get; set; }
        float TimeToUnhappinessDebuff { get; set; }
        float TimeForTradingCooldown { get; set; }
        float TimeForPlantingCooldown { get; set; }

        void EatFood();
        void DrinkWater();
    }