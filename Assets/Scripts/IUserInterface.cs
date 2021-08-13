    using UnityEngine.UI;

    public interface IUserInterface
    {
        bool GameIsPaused { get; set; }
        string GameTextString { get; set; }
        bool NewTextToAdd { get; set; }
        Text NameText { get; set; }
        Text CurrentActionText { get; set; }
        Text AgeText { get; set; }
        Text GenderText { get; set; }
        Text WealthText { get; set; }
        Text HappinessText { get; set; }
        Text DaysPassedText { get; set; }
        Text PopulationText { get; set; }
        Text AvgAgeText { get; set; }
        Text AvgWealthText { get; set; }
        Text AvgHappinessText { get; set; }
        Text NumDeathsText { get; set; }
        int GameTextCount { get; set; }
    }