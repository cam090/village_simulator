using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject villagerProfile;
    public GameObject villageStats;
    public GameObject scrollArea;
    public GameObject villageStatsButton;
    public GameObject pauseGameButton;
    public GameObject playGameButton;
    public GameObject oneSpeedButton;
    public GameObject twoSpeedButton;
    public GameObject threeSpeedButton;
    public GameObject fourSpeedButton;
    public GameObject gameEnd;

    public Text gameText;
    
    public bool GameIsPaused { get; set; }
    public string GameTextString { get; set; }
    public bool NewTextToAdd { get; set; }
    public Text NameText { get; set; }
    public Text CurrentActionText { get; set; }
    public Text AgeText { get; set; }
    public Text GenderText { get; set; }
    public Text WealthText { get; set; }
    public Text HappinessText { get; set; }
    public Text DaysPassedText { get; set; }
    public Text PopulationText { get; set; }
    public Text AvgAgeText { get; set; }
    public Text AvgWealthText { get; set; }
    public Text AvgHappinessText { get; set; }
    public Text NumDeathsText { get; set; }
    public int GameTextCount { get; set; }

    public void AddGameText(string text)
    {
        GameTextCount++;
        GameTextString += "\n" + GameTextCount + ")  " + text;
        NewTextToAdd = true;
    }

    public void RemoveGameText()
    {
        string oldText = gameText.text;
        int middle = oldText.Length / 2;
        int length = oldText.Length - middle;
        string newText = oldText.Substring(middle, length);
        gameText.text = newText;
    }

    private void Start()
    {
        GameIsPaused = false;
        NewTextToAdd = false;
        GameTextCount = 0;
        
        gameText.text = ("Simulation started.");
    }

    // Update is called once per frame
    void Update()
    {
        if (NewTextToAdd)
        {
            gameText.text += GameTextString;
            NewTextToAdd = false;
            GameTextString = "";
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (gameText.text.Length > 2000)
        {
            RemoveGameText();
        }
    }

    public void ShowVillagerProfile(GameObject go)
    {
        scrollArea.SetActive(false);
        villageStatsButton.SetActive(false);
        pauseGameButton.SetActive(false);
        playGameButton.SetActive(false);
        oneSpeedButton.SetActive(false);
        twoSpeedButton.SetActive(false);
        threeSpeedButton.SetActive(false);
        fourSpeedButton.SetActive(false);
        villagerProfile.SetActive(true);

        villagerProfile.SetActive(true);
        Time.timeScale = 0;

        IVillager v = go.GetComponent<IVillager>();
        
        NameText = GameObject.Find("NameText").GetComponent<Text>();
        CurrentActionText = GameObject.Find("CurrentActionText").GetComponent<Text>();
        AgeText = GameObject.Find("AgeText").GetComponent<Text>();
        GenderText = GameObject.Find("GenderText").GetComponent<Text>();
        WealthText = GameObject.Find("WealthText").GetComponent<Text>();
        HappinessText = GameObject.Find("HappinessText").GetComponent<Text>();

        NameText.text = v.Name;
        CurrentActionText.text = "Current action: " + v.CurrentAction;
        AgeText.text = "Age: " + v.Age + " days old";
        GenderText.text = "Gender: " + v.Gender;
        WealthText.text = "Wealth: " + v.Wealth.ToString();
        int happiness = (int)Math.Round(v.Happiness * 100);
        HappinessText.text = "Happiness: " + happiness.ToString() + "/100";
    }
    
    public void ShowVillageStats(GameObject go)
    {
        scrollArea.SetActive(false);
        villageStatsButton.SetActive(false);
        pauseGameButton.SetActive(false);
        playGameButton.SetActive(false);
        oneSpeedButton.SetActive(false);
        twoSpeedButton.SetActive(false);
        threeSpeedButton.SetActive(false);
        fourSpeedButton.SetActive(false);
        villageStats.SetActive(true);
        
        Time.timeScale = 0;

        Environment env = go.GetComponent<Environment>();
        
        DaysPassedText = GameObject.Find("DaysPassedText").GetComponent<Text>();
        PopulationText = GameObject.Find("PopulationText").GetComponent<Text>();
        AvgAgeText = GameObject.Find("AvgAgeText").GetComponent<Text>();
        AvgWealthText = GameObject.Find("AvgWealthText").GetComponent<Text>();
        AvgHappinessText = GameObject.Find("AvgHappinessText").GetComponent<Text>();

        DaysPassedText.text = "Days passed: " + env.NumDaysPassed.ToString();
        PopulationText.text = "Population: " + env.GetNumVillagers();
        AvgAgeText.text = "Average age: " + env.GetAverageAgeOfVillagers() + " days old";
        AvgWealthText.text = "Average wealth: " + env.GetAverageWealthOfVillagers();
        int avgHappiness = (int)Math.Round(env.GetAverageHappinessOfVillagers() * 100);
        AvgHappinessText.text = "Average happiness: " + avgHappiness.ToString() + "/100";
    }

    public void GameEnd()
    {
        scrollArea.SetActive(false);
        villageStatsButton.SetActive(false);
        pauseGameButton.SetActive(false);
        playGameButton.SetActive(false);
        oneSpeedButton.SetActive(false);
        twoSpeedButton.SetActive(false);
        threeSpeedButton.SetActive(false);
        fourSpeedButton.SetActive(false);
        gameEnd.SetActive(true);
        
        Time.timeScale = 0;
    }

    public void ExitVillagerProfile()
    {
        scrollArea.SetActive(true);
        villageStatsButton.SetActive(true);
        pauseGameButton.SetActive(true);
        playGameButton.SetActive(true);
        oneSpeedButton.SetActive(true);
        twoSpeedButton.SetActive(true);
        threeSpeedButton.SetActive(true);
        fourSpeedButton.SetActive(true);
        villagerProfile.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void ExitVillageStats()
    {
        scrollArea.SetActive(true);
        villageStatsButton.SetActive(true);
        pauseGameButton.SetActive(true);
        playGameButton.SetActive(true);
        oneSpeedButton.SetActive(true);
        twoSpeedButton.SetActive(true);
        threeSpeedButton.SetActive(true);
        fourSpeedButton.SetActive(true);
        villageStats.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        scrollArea.SetActive(true);
        villageStatsButton.SetActive(true);
        pauseGameButton.SetActive(true);
        playGameButton.SetActive(true);
        oneSpeedButton.SetActive(true);
        twoSpeedButton.SetActive(true);
        threeSpeedButton.SetActive(true);
        fourSpeedButton.SetActive(true);
    }

    public void Pause()
    {
        scrollArea.SetActive(false);
        villageStatsButton.SetActive(false);
        pauseGameButton.SetActive(false);
        playGameButton.SetActive(false);
        oneSpeedButton.SetActive(false);
        twoSpeedButton.SetActive(false);
        threeSpeedButton.SetActive(false);
        fourSpeedButton.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void PauseGameWithoutMenu()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Loading Menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void OneSpeed()
    {
        Time.timeScale = 1f;
    }
    
    public void TwoSpeed()
    {
        Time.timeScale = 2f;
    }
    
    public void ThreeSpeed()
    {
        Time.timeScale = 3f;
    }
    
    public void FourSpeed()
    {
        Time.timeScale = 4f;
    }
}
