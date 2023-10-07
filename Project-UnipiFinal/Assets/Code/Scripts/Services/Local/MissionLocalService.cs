using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MissionLocalService
{
    private const string localMissionFileName = "/mission.json";
    private const string localMissionsCacheFileName = "/missions_cache.json";

    List<Mission> missionsList = new List<Mission>() {
            new Mission() {Id=0, Title="Diamond District Vault", Description="Nestled amidst a bustling city, this high-security vault holds billions in gems. Surveillance is tight, and the labyrinth layout confounds outsiders.", Difficulty = "Medium", IsCompleted = false},
            new Mission() {Id=1,Title="Sunken Spanish Galleon", Description="Resting deep beneath the sea lies a shipwreck, its cargo hold brimming with gold doubloons and ancient artifacts. Dark waters, predatory sea creatures, and limited oxygen pose threats.", Difficulty="Easy", IsCompleted = false},
            new Mission() {Id=2,Title="The Forbidden Museum", Description="A secretive institution, rumored to house relics of untold power. Hidden away on a mountain peak, it's protected by both elite guards and treacherous terrain.", Difficulty="Hard", IsCompleted = false},
            new Mission() {Id=3,Title="Mystic Island Casino", Description="An opulent casino on a private island, where the rich and infamous gamble away fortunes. Behind the glitzy facade, lies a vault of ill-gotten treasures.", Difficulty="Very Hard", IsCompleted = false},
            new Mission() {Id=4,Title="Penthouse of the Oligarch", Description="Atop the city's tallest skyscraper, a tycoon's penthouse hides valuable art and encrypted data. Guarded by private security and advanced tech defenses.", Difficulty="Medium", IsCompleted = false},
            new Mission() {Id=5,Title="Celestial Observatory Heist", Description="Perched on a remote mountaintop, the observatory holds astronomical secrets and rare cosmic gems. Guarded by a dedicated crew and natural isolation.", Difficulty="Easy", IsCompleted = false},
            new Mission() {Id=6,Title="Royal Train Robbery", Description="A moving target, the luxury royal train transports a monarch's treasures. Security is tight, and the ever-changing landscape complicates the plot.", Difficulty="Hard", IsCompleted = false},
            new Mission() {Id=7,Title="Opera House Caper", Description="An architectural marvel echoing with music, it's said to have a vault of old world wealth beneath its stage. Heisting during a live performance?", Difficulty="Medium", IsCompleted = false},
            new Mission() {Id=8,Title="Tech Tycoon's Tower", Description="A modern skyscraper fitted with the latest technology, owned by a billionaire tech magnate. Rumored to hold a digital treasure of immense value.", Difficulty="Medium", IsCompleted = false},
            new Mission() {Id=10,Title="Underground Speakeasy Heist", Description="Hidden below the bustling city streets, a prohibition-era speakeasy, now a den for modern elite. Holds rare liquors and a secret stash of golden bars.", Difficulty="Very Hard", IsCompleted = false},
    };

    ILocalDataService _dataService;

    public MissionLocalService()
    {
        _dataService = new JsonLocalDataService();
    }

    public async Task<bool> SaveMission(Mission mission)
    {
        return await _dataService.SaveData(localMissionFileName, mission, true);
    }

    public async Task<Mission> LoadMission()
    {
        return await _dataService.LoadData<Mission>(localMissionFileName, true);
    }

    public async Task<bool> DeleteMission()
    {
        return await _dataService.DeleteData(localMissionFileName);
    }

    public async Task<bool> SaveAllMissions(List<Mission> missionsList)
    {
        return await _dataService.SaveData(localMissionsCacheFileName, missionsList, true);
    }

    public async Task<List<Mission>> LoadAllMissions()
    {
        return await _dataService.LoadData<List<Mission>>(localMissionsCacheFileName, true);
    }

    public async Task<bool> DeleteAllMissions()
    {
        return await _dataService.DeleteData(localMissionsCacheFileName);
    }

    public List<Mission> GetRandomMissions(int count)
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
}
