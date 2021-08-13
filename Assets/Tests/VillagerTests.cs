using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using NSubstitute;
using System.Collections;
using System.Collections.Generic;
using Datatypes;
using DefaultNamespace;

public class VillagerTests
{
    [UnityTest]
    public IEnumerator VillagerHasName()
    {
        IVillager villager = Substitute.For<IVillager>();
        string newName = "Villager 1";
        
        villager.Name = newName;
        
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(newName, villager.Name);
    }
    
    [UnityTest]
    public IEnumerator VillagerHasGender()
    {
        IVillager villager = Substitute.For<IVillager>();

        villager.Gender = Gender.Female;
        
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(Gender.Female, villager.Gender);
    }
    
    [UnityTest]
    public IEnumerator VillagerCanEat()
    {
        IVillager villager = Substitute.For<IVillager>();
        float initialHunger = 1f;

        villager.Waiting = false;
        villager.Hunger = initialHunger;
        villager.CurrentAction = VillagerAction.Eating;
        villager.CrRunning = true;
        
        villager.EatFood();
        
        yield return new WaitForSeconds(0.1f);

        Assert.AreNotEqual(villager.Hunger, initialHunger);
        Assert.AreNotEqual(villager.CurrentAction, VillagerAction.Eating);
        Assert.AreNotEqual(villager.CrRunning, true);
    }
    
    [UnityTest]
    public IEnumerator VillagerCanDrink()
    {
        IVillager villager = Substitute.For<IVillager>();
        float initialThirst = 1f;

        villager.Waiting = false;
        villager.Thirst = initialThirst;
        villager.CurrentAction = VillagerAction.Drinking;
        villager.CrRunning = true;
        
        villager.DrinkWater();
        
        yield return new WaitForSeconds(0.1f);

        Assert.AreNotEqual(villager.Thirst, initialThirst);
        Assert.AreNotEqual(villager.CurrentAction, VillagerAction.Drinking);
        Assert.AreNotEqual(villager.CrRunning, true);
    }
}