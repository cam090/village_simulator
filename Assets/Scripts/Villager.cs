using System;
using System.Collections;
using System.Collections.Generic;
using Datatypes;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public class Villager : LivingEntity, IVillager
{
    // Unity Stuff
    private Rigidbody _rb;
    private Animator _animator;
    private NavMeshAgent _agent;
    public Image thirstBar;
    public Image hungerBar;
    public TextMeshProUGUI textMeshProUGUI;
    public GameObject environment;
    public GameObject userInterface;
    public GameObject target;
    
    // Properties
    public Environment Environment { get; set; }
    public UserInterface UserInterface { get; set; }
    public string Name { get; set; }
    public float Hunger { get; set; }
    public float Thirst { get; set; }
    public float Tiredness { get; set; }
    public float ReproductiveUrge { get; set; }
    public float AgeProgressToNextAge { get; set; }
    public float Happiness { get; set; }
    public float TradingCoolDown { get; set; }
    public bool RecentlyTraded { get; set; }
    public float PlantingCoolDown { get; set; }
    public bool RecentlyPlanted { get; set; }
    
    public int Age { get; set; }
    public int AgeLimit { get; set; }
    public int Wealth { get; set; }
    public VillagerAction CurrentAction { get; set; }
    public Gender Gender { get; set; }
    public bool CrRunning { get; set; }
    public Villager VillagerMate { get; set; }
    
    // Settings
    public float TimeToDeathByHunger { get; set; }
    public float TimeToDeathByThirst { get; set; }
    public float TimeToDeathByTiredness { get; set; }
    public float TimeToAgeIncrease { get; set; }
    public float TimeToReproductiveDebuff { get; set; }
    public float TimeToUnhappinessDebuff { get; set; }
    public float TimeForTradingCooldown { get; set; }
    public float TimeForPlantingCooldown { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Happiness = 1;
        RecentlyTraded = false;
        Age = 0;
        AgeLimit = 25;
        Wealth = 0;
        CrRunning = false;
        TimeToDeathByHunger = 200;
        TimeToDeathByThirst = 200;
        TimeToDeathByTiredness = 200;
        TimeToAgeIncrease = 10;
        TimeToReproductiveDebuff = 200;
        TimeToUnhappinessDebuff = 30;
        TimeForTradingCooldown = 25;
        TimeForPlantingCooldown = 400;
        
        environment = GameObject.Find("Environment");
        Environment = environment.GetComponent<Environment>();
        userInterface = GameObject.Find("Canvas");
        UserInterface = userInterface.GetComponent<UserInterface>();
        
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("IsRunning", false);
        _agent = GetComponent<NavMeshAgent>(); 
        CurrentAction = VillagerAction.None;
        UserInterface.AddGameText(Name + " was born.");
        
    }

    // Update is called once per frame
    void Update()
    {
        Environment = environment.GetComponent<Environment>();
        UserInterface = userInterface.GetComponent<UserInterface>();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            LayerMask layerMask = LayerMask.GetMask("Default");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                GameObject go = hit.collider.gameObject;
                if (go.CompareTag("Male") || go.CompareTag("Female"))
                {
                    UserInterface.ShowVillagerProfile(go);
                }
            }
        }
        
        thirstBar.fillAmount = 1 - Thirst;
        hungerBar.fillAmount = 1 - Hunger;

        // Increase hunger and thirst over time
        Hunger += Time.deltaTime * 1 / TimeToDeathByHunger;
        Thirst += Time.deltaTime * 1 / TimeToDeathByThirst;
        AgeProgressToNextAge += Time.deltaTime * 1 / TimeToAgeIncrease;
        ReproductiveUrge += Time.deltaTime * 1 / TimeToReproductiveDebuff;
        
        UpdateHappiness();

        if (RecentlyTraded)
        {
            TradingCoolDown += Time.deltaTime * 1 / TimeForTradingCooldown;
        }
        
        if (RecentlyPlanted)
        {
            PlantingCoolDown += Time.deltaTime * 1 / TimeForPlantingCooldown;
        }
        
        if (CurrentAction == VillagerAction.FindingMate)
        {
            if (target != null)
            {
                _agent.SetDestination(target.transform.position);
            
                if (Vector3.Distance(_agent.transform.position, _agent.destination) <= 1f)
                {
                    // I'm there!
                    _agent.isStopped = true;
                    _agent.ResetPath();
                    Mate();
                }
            }
            else
            {
                FindMate();
            }
        } 
        else if (CurrentAction == VillagerAction.Resting)
        {
            Tiredness -= Time.deltaTime * 1 / TimeToDeathByTiredness;
        }
        else if (CurrentAction == VillagerAction.FindingTrade)
        {
            if (target != null)
            {
                _agent.SetDestination(target.transform.position);
            
                if (Vector3.Distance(_agent.transform.position, _agent.destination) <= 1f)
                {
                    // I'm there!
                    _agent.isStopped = true;
                    _agent.ResetPath();
                    Trade();
                }
            }
            else
            {
                FindTrade();
            }
            
        }

        // Target destination has been reached
        // aka movement action has been completed
        if (!_agent.pathPending && CurrentAction != VillagerAction.None)
        {
            if (_agent.remainingDistance <= 0.0001)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    if (CurrentAction == VillagerAction.GatheringFood)
                    {
                        // if the current action is to gather food
                        // then food has been gathered
                        // time to eat
                        _animator.SetBool("IsRunning", false);
                        EatFood();
                    }
                    else if (CurrentAction == VillagerAction.GoingToWater)
                    {
                        // if the current action is to find water
                        // then water has been found
                        // time to drink
                        _animator.SetBool("IsRunning", false);
                        DrinkWater();
                    }
                    else if (CurrentAction == VillagerAction.PlantingFood)
                    {
                        _animator.SetBool("IsRunning", false);
                        PlantFood();
                    }
                    else if (CurrentAction == VillagerAction.Eating)
                    {
                        EatFood();
                    }
                    else if (CurrentAction == VillagerAction.Drinking)
                    {
                        DrinkWater();
                    }
                    else if (CurrentAction == VillagerAction.FindingMate)
                    {
                        Mate();
                    }
                    else if (CurrentAction == VillagerAction.FindingTrade)
                    {
                        Trade();
                    }
                    else if (CurrentAction == VillagerAction.Working)
                    {
                        _animator.SetBool("IsRunning", false);
                        Work();
                    }
                    else if (CurrentAction == VillagerAction.Resting)
                    {
                        _animator.SetBool("IsRunning", false);
                        Rest();
                    }
                    else
                    {
                        textMeshProUGUI.text = "idle";
                        _animator.SetBool("IsRunning", false);
                        CurrentAction = VillagerAction.None;
                    }
                }
            }
            else
            {
                // target destination has not been reached
                // keep moving
                _animator.SetBool("IsRunning", true);
            }
        }

        if (AgeProgressToNextAge >= 1)
        {
            Age++;
            UserInterface.AddGameText(Name + " is now " + Age + " days old.");
            AgeProgressToNextAge = 0;
        }
        
        if (Hunger >= 1) {
            UserInterface.AddGameText(Name + " died of hunger.");
            Environment.RemoveVillager(gameObject);
            Die (CauseOfDeath.Hunger);
        } 
        else if (Thirst >= 1) 
        {
            UserInterface.AddGameText(Name + " died of thirst.");
            Die (CauseOfDeath.Thirst);
        } 
        else if (Age >= AgeLimit) 
        {
            UserInterface.AddGameText(Name + " died of old age.");
            Die(CauseOfDeath.Age);
        } 
        else if (Tiredness >= 1)
        {
            UserInterface.AddGameText(Name + " died of tiredness.");
            Die(CauseOfDeath.Tiredness);
        } 
        else if (TradingCoolDown >= 1)
        {
            RecentlyTraded = false;
        }

        if (CurrentAction == VillagerAction.None)
        {
            ChooseNextAction();
        }
    }

    // Happiness is updated based on the villager's current situation
    public void UpdateHappiness()
    {
        
        if (Hunger >= 0.75)
        {
            // Villager is hungry, so happiness is reduced
            Happiness -= Time.deltaTime * 1 / 500;
        }
        else if (Hunger >= 0.5)
        {
            // Villager is hungry, so happiness is reduced
            Happiness -= Time.deltaTime * 1 / 200;
        }
        else if (Hunger <= 0.25)
        {
            // Villager is not hungry, so happiness is increased
            Happiness += Time.deltaTime * 1 / 500;
        }
        
        if (Thirst >= 0.75)
        {
            // Villager is thirsty, so happiness is reduced
            Happiness -= Time.deltaTime * 1 / 500;
        }
        else if (Thirst >= 0.5)
        {
            // Villager is thirsty, so happiness is reduced
            Happiness -= Time.deltaTime * 1 / 200;
        }
        else if (Thirst <= 0.25)
        {
            // Villager is not thirsty, so happiness is increased
            Happiness += Time.deltaTime * 1 / 500;
        }
        
        if (ReproductiveUrge >= 0.75)
        {
            // Villager has an urge to reproduce, so happiness is reduced
            Happiness -= Time.deltaTime * 1 / 500;
        }
        else if (ReproductiveUrge >= 0.5)
        {
            // Villager has an urge to reproduce, so happiness is reduced
            Happiness -= Time.deltaTime * 1 / 200;
        }
        else if (ReproductiveUrge <= 0.25)
        {
            // Villager has no urge to reproduce, so happiness is increased
            Happiness += Time.deltaTime * 1 / 500;
        }

        // Make sure happiness doesn't exceed bounds
        if (Happiness > 1)
        {
            Happiness = 1;
        } 
        else if (Happiness < 0)
        {
            Happiness = 0;
        }
    }

    // Assign the villager a gender at birth
    public void AssignGender(bool male)
    {
        if (male == true)
        {
            Gender = Gender.Male;
        }
        else
        {
            Gender = Gender.Female;
        }
    }

    // Determine what next action to perform for a villager
    public void ChooseNextAction()
    {
        // More hungry than thirsty
        if (Hunger >= Thirst && Hunger >= 0.25f) {
            GatherFood();
        }
        // More thirsty than hungry and reproductive urge is smaller
        else if (Thirst >= ReproductiveUrge && Thirst >= 0.25f) {
            FindWater();
        }
        // Reproductive urge is priority
        else if (ReproductiveUrge >= 0.25f)
        {
            FindMate();
        }
        // Villager is tired
        else if (Tiredness >= 0.25f)
        {
            Rest();
        }
        // Villager hasn't traded recently and has some money to trade
        else if (!RecentlyTraded && Wealth > 0)
        {
            FindTrade();
        }
        // Villager hasn't recently planted some food
        else if (!RecentlyPlanted)
        {
            FindPlaceToPlantFood();
        }
        else
        {
            Work();
        }
}

    // Villager will find someone to trade with
    public void FindTrade()
    {
        UserInterface.AddGameText(Name + " is finding a trade.");
        GameObject trade = null;
        textMeshProUGUI.text = "finding trade";
        CurrentAction = VillagerAction.FindingTrade;

        List<GameObject> villagers = Environment.GetVillagersAsGameObjects();
        Random rand = new Random();
        int num = rand.Next(0, villagers.Count);
        trade = villagers[num];
        if (trade == gameObject && num < villagers.Count-1)
        {
            trade = villagers[num+1];
        }
        else if (trade == gameObject && num == villagers.Count-1)
        {
            trade = villagers[num-1];
        }
        target = trade;
        if (target != null)
        {
            _agent.SetDestination(target.transform.position);
        }
    }
    
    // Villager has found someone to trade with, so trade can commence
    public void Trade()
    {
        if (target != null)
        {
            UserInterface.AddGameText(Name + " is trading with " + target.GetComponent<Villager>().Name + ".");
            textMeshProUGUI.text = "trading";
            CurrentAction = VillagerAction.Trading;

            int traderWealth = target.GetComponent<Villager>().Wealth;

            Random rand = new Random();
            int amountToGive = rand.Next(0, Wealth);
            int amountToGain = rand.Next(0, traderWealth);
            Wealth += amountToGain;
            target.GetComponent<Villager>().Wealth += amountToGive;
        
            CurrentAction = VillagerAction.None;
            RecentlyTraded = true;
        }
        else
        {
            FindTrade();
        }
    }

    // Villager begins resting
    public void Rest()
    {
        if (!CrRunning)
        {
            UserInterface.AddGameText(Name + " is resting.");
            CurrentAction = VillagerAction.Resting;
            textMeshProUGUI.text = "resting";
            StartCoroutine(WaitForTime(5));
            CrRunning = true;
        }
        
        if (!Waiting)
        {
            CurrentAction = VillagerAction.None;
            CrRunning = false;
        }
    }

    // Villager begins working
    public void Work()
    {
        Wealth += 1;
        Tiredness += Time.deltaTime * 1 / TimeToDeathByTiredness;
        if (!CrRunning)
        {
            _animator.SetBool("IsWorking", true);
            UserInterface.AddGameText(Name + " is working.");
            CurrentAction = VillagerAction.Working;
            textMeshProUGUI.text = "working";
            StartCoroutine(WaitForTime(10));
            CrRunning = true;
        }
        
        if (!Waiting)
        {
            _animator.SetBool("IsWorking", false);
            CurrentAction = VillagerAction.None;
            CrRunning = false;
        }
    }

    // Villager will find someone to mate with
    public void FindMate()
    {
        
        UserInterface.AddGameText(Name + " is finding a mate.");
        GameObject mate = null;
        textMeshProUGUI.text = "finding mate";
        CurrentAction = VillagerAction.FindingMate;

        List<GameObject> villagers = Environment.GetVillagersAsGameObjects();

        GameObject[] mates;

        if (gameObject.CompareTag("Male"))
        {
            mates = GameObject.FindGameObjectsWithTag("Female");
        }
        else
        {
            mates = GameObject.FindGameObjectsWithTag("Male");
        }
        
        Random rand = new Random();
        int num = rand.Next(0, mates.Length);
        mate = villagers[num];

        target = mate;
        if (target != null)
        {
            _agent.SetDestination(target.transform.position);
        }
    }

    // Villager has found someone to mate with, so mating can commence
    public void Mate()
    {
        if (target != null)
        {
            UserInterface.AddGameText(Name + " is mating with " + target.GetComponent<Villager>().Name + ".");
            textMeshProUGUI.text = "mating";
            CurrentAction = VillagerAction.Mating;

            ReproductiveUrge = 0;
            CurrentAction = VillagerAction.None;
            Environment.SpawnVillager(gameObject.transform.position);
        }
    }

    // Villager will find somewhere to plant some food
    public void FindPlaceToPlantFood()
    {
        Vector3 position = Environment.GetRandomPoint();
        GameObject plant = Environment.GeneratePlantFoodSource(position);
        plant.SetActive(false);
        target = plant;
        textMeshProUGUI.text = "planting food";
        _agent.SetDestination(target.transform.position);
        CurrentAction = VillagerAction.PlantingFood;
    }
    
    // Villager has found somewhere to plant food, so can now plant food
    public void PlantFood()
    {
        UserInterface.AddGameText(Name + " planted some food.");
        target.SetActive(true);
        CurrentAction = VillagerAction.None;
        RecentlyPlanted = true;
    }

    // Villager has found some food to eat, so can eat the food
    public void EatFood()
    {
        if (!CrRunning)
        {
            _animator.SetBool("IsEating", true);
            UserInterface.AddGameText(Name + " is eating food.");
            StartCoroutine(WaitForTime(3));
            CurrentAction = VillagerAction.Eating;
            textMeshProUGUI.text = "eating food";
            CrRunning = true;
        }
        
        if (!Waiting)
        {
            _animator.SetBool("IsEating", false);
            float eatAmount = Mathf.Min(Hunger, 0.25f);
            Hunger -= eatAmount;
            Environment.EatPlantFoodSource(target);
            CurrentAction = VillagerAction.None;
            CrRunning = false;
        }
    }
    
    // Villager will look for some food
    public void GatherFood()
    {
        UserInterface.AddGameText(Name + " is gathering food.");
        textMeshProUGUI.text = "gathering food";
        target = Environment.FindPlantFoodSource();
        _agent.SetDestination(target.transform.position);
        CurrentAction = VillagerAction.GatheringFood;
    }
    
    // Villager has found a water source, so can drink
    public void DrinkWater()
    {
        if (!CrRunning)
        {
            _animator.SetBool("IsDrinking", true);
            UserInterface.AddGameText(Name + " is drinking water.");
            StartCoroutine(WaitForTime(3));
            CurrentAction = VillagerAction.Drinking;
            textMeshProUGUI.text = "drinking water";
            CrRunning = true;
        }

        if (!Waiting)
        {
            _animator.SetBool("IsDrinking", false);
            float drinkAmount = Mathf.Min(Thirst, 0.25f);
            Thirst -= drinkAmount;
            CurrentAction = VillagerAction.None;
            CrRunning = false;
        }
    }

    // Villager will find a water source to drink from
    public void FindWater()
    {
        UserInterface.AddGameText(Name + " is finding water.");
        textMeshProUGUI.text = "finding water";
        WaterSource waterSource = Environment.GetWaterSource();
        if (waterSource.IsEmpty)
        {
            _agent.SetDestination(waterSource.go.transform.position);
            CurrentAction = VillagerAction.GoingToWater;
        }
    }
}
