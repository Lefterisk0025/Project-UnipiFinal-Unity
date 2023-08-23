using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MissionService
{
    private const string localFileName = "/mission.json";

    ILocalDataService _dataService = new JsonLocalDataService();

    List<Mission> missionsList = new List<Mission>() {
            new Mission("Diamond District Vault", "Nestled amidst a bustling city, this high-security vault holds billions in gems. Surveillance is tight, and the labyrinth layout confounds outsiders.", Difficulty.Medium, 80),
            new Mission("Sunken Spanish Galleon", "Resting deep beneath the sea lies a shipwreck, its cargo hold brimming with gold doubloons and ancient artifacts. Dark waters, predatory sea creatures, and limited oxygen pose threats.", Difficulty.Easy, 20),
            new Mission("The Forbidden Museum", "A secretive institution, rumored to house relics of untold power. Hidden away on a mountain peak, it's protected by both elite guards and treacherous terrain.", Difficulty.Hard, 200),
            new Mission("Mystic Island Casino", "An opulent casino on a private island, where the rich and infamous gamble away fortunes. Behind the glitzy facade, lies a vault of ill-gotten treasures.", Difficulty.VeryHard, 350),
            new Mission("Penthouse of the Oligarch", "Atop the city's tallest skyscraper, a tycoon's penthouse hides valuable art and encrypted data. Guarded by private security and advanced tech defenses.", Difficulty.Medium, 200),
            new Mission("Celestial Observatory Heist", "Perched on a remote mountaintop, the observatory holds astronomical secrets and rare cosmic gems. Guarded by a dedicated crew and natural isolation.", Difficulty.Easy, 100),
            new Mission("Royal Train Robbery", "A moving target, the luxury royal train transports a monarch's treasures. Security is tight, and the ever-changing landscape complicates the plot.", Difficulty.Hard, 200),
            new Mission("Opera House Caper", "An architectural marvel echoing with music, it's said to have a vault of old world wealth beneath its stage. Heisting during a live performance?", Difficulty.Medium, 300),
            new Mission("-Tech Tycoon's Tower", "A modern skyscraper fitted with the latest technology, owned by a billionaire tech magnate. Rumored to hold a digital treasure of immense value.", Difficulty.Medium, 100),
            new Mission("Underground Speakeasy Heist", "Hidden below the bustling city streets, a prohibition-era speakeasy, now a den for modern elite. Holds rare liquors and a secret stash of golden bars.", Difficulty.VeryHard, 340),
    };

    public List<Mission> GetNewRandomMissions(int count)
    {
        List<Mission> randomMissionsList = new List<Mission>(missionsList);
        int n = randomMissionsList.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Mission value = randomMissionsList[k];
            randomMissionsList[k] = randomMissionsList[n];
            randomMissionsList[n] = value;
        }

        return randomMissionsList.Take(count).ToList();
    }

    #region LOCAL DATA

    public bool SaveLocalMissionData(Mission mission)
    {
        return _dataService.SaveData(localFileName, mission, true);
    }

    public Mission GetLocalMissionData()
    {
        return _dataService.LoadData<Mission>(localFileName, true);
    }

    public bool DeleteLocalMission()
    {
        return _dataService.DeleteData(localFileName);
    }

    #endregion
}
