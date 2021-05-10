using System.Collections;
using System.Collections.Generic;
using Datatypes;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Villager : LivingEntity
{
    // Unity Stuff
    private Rigidbody _rb;
    private Animator _animator;
    private NavMeshAgent _agent;
    public Image thirstBar;
    public Image hungerBar;
    public TextMeshProUGUI textMeshProUGUI;

    public GameObject foodTarget;
    
    // Properties
    private float _hunger;
    private float _thirst;
    private float _age;
    private VillagerAction _currentAction;
    private bool _cr_running = false;
    
    // Settings
    private float _timeToDeathByHunger = 200;
    private float _timeToDeathByThirst = 200;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("IsRunning", false);
        _agent = GetComponent<NavMeshAgent>(); 
        _currentAction = VillagerAction.None;
    }

    // Update is called once per frame
    void Update()
    {
        thirstBar.fillAmount = 1 - _thirst;
        hungerBar.fillAmount = 1 - _hunger;

        // Increase hunger and thirst over time
        _hunger += Time.deltaTime * 1 / _timeToDeathByHunger;
        _thirst += Time.deltaTime * 1 / _timeToDeathByThirst;
        
        // Target destination has been reached
        // aka movement action has been completed
        if (!_agent.pathPending && _currentAction != VillagerAction.None)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                Debug.Log("Destination reached.");
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    if (_currentAction == VillagerAction.GatheringFood)
                    {
                        // if the current action is to gather food
                        // then food has been gathered
                        // time to eat
                        _animator.SetBool("IsRunning", false);
                        EatFood();
                    }
                    else if (_currentAction == VillagerAction.GoingToWater)
                    {
                        // if the current action is to find water
                        // then water has been found
                        // time to drink
                        _animator.SetBool("IsRunning", false);
                        DrinkWater();
                    }
                    else
                    {
                        Debug.Log("Idle.");
                        textMeshProUGUI.text = "idle";
                        _animator.SetBool("IsRunning", false);
                        _currentAction = VillagerAction.None;
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
        
        
        if (_hunger >= 1) {
            Die (CauseOfDeath.Hunger);
        } else if (_thirst >= 1) {
            Die (CauseOfDeath.Thirst);
        }

        if (_currentAction == VillagerAction.None)
        {
            ChooseNextAction();
        }
    }

    public void ChooseNextAction()
    {
        if (_hunger >= _thirst) {
            GatherFood();
        }
        // More thirsty than hungry
        else {
            FindWater();
        }
    }

    public void EatFood()
    {
        if (!_cr_running)
        {
            StartCoroutine(WaitForTime(3));
            Debug.Log("Eating food.");
            _currentAction = VillagerAction.Eating;
            textMeshProUGUI.text = "eating food";
            _cr_running = true;
        }
        
        if (!waiting)
        {
            float eatAmount = Mathf.Min(_hunger, 0.25f);
            Debug.Log("Eat amount: " + eatAmount);
            _hunger -= eatAmount;
            Environment.EatPlantFoodSource(foodTarget);
            _currentAction = VillagerAction.None;
            _cr_running = false;
        }
    }
    
    public void GatherFood()
    {
        Debug.Log("Gathering food.");
        textMeshProUGUI.text = "gathering food";
        foodTarget = Environment.FindPlantFoodSource();
        _agent.SetDestination(foodTarget.transform.position);
        _currentAction = VillagerAction.GatheringFood;
    }
    
    public void DrinkWater()
    {
        if (!_cr_running)
        {
            StartCoroutine(WaitForTime(3));
            Debug.Log("Drinking water.");
            _currentAction = VillagerAction.Drinking;
            textMeshProUGUI.text = "drinking water";
            _cr_running = true;
        }

        if (!waiting)
        {
            float drinkAmount = Mathf.Min(_thirst, 0.25f);
            Debug.Log("Drink amount: " + drinkAmount);
            _thirst -= drinkAmount;
            _currentAction = VillagerAction.None;
            _cr_running = false;
        }
    }

    public void FindWater()
    {
        Debug.Log("Finding water.");
        textMeshProUGUI.text = "finding water";
        _agent.SetDestination(Environment.GetWaterSourcePosition().position);
        _currentAction = VillagerAction.GoingToWater;
    }
}
