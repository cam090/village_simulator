using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public InputField numVillagers;
    public InputField numPlants;
    public InputField numDays;
    private const int NUM_VILLAGERS = 3;
    private const int NUM_PLANTS = 25;
    private const int NUM_DAYS = 100;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }

    public void SaveChanges()
    {
        PlayerPrefs.SetInt("numVillagers", Int32.Parse(numVillagers.text));
        PlayerPrefs.SetInt("numPlants", Int32.Parse(numPlants.text));
        PlayerPrefs.SetInt("numDays", Int32.Parse(numDays.text));
    }

    public void DiscardChanges()
    {
        numVillagers.text = NUM_VILLAGERS.ToString();
        numPlants.text = NUM_PLANTS.ToString();
        numDays.text = NUM_DAYS.ToString();
        
        PlayerPrefs.SetInt("numVillagers", Int32.Parse(numVillagers.text));
        PlayerPrefs.SetInt("numPlants", Int32.Parse(numPlants.text));
        PlayerPrefs.SetInt("numDays", Int32.Parse(numDays.text));
    }
}
