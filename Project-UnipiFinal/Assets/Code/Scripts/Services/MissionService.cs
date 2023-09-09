using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class MissionService
{
    private const string localMissionFileName = "/mission.json";
    private const string localMissionsCacheFileName = "/missions-chache.json";
    private const string localObjectivesFileName = "/objectives.json";

    ILocalDataService _dataService;
    FirebaseFirestore _fsDB;

    public MissionService()
    {
        _dataService = new JsonLocalDataService();
        _fsDB = FirebaseFirestore.DefaultInstance;
    }

    // FOR TESTING
    List<Mission> missionsList = new List<Mission>() {
            new Mission() {Title="Diamond District Vault", Description="Nestled amidst a bustling city, this high-security vault holds billions in gems. Surveillance is tight, and the labyrinth layout confounds outsiders.", Difficulty = "Medium"},
            new Mission() {Title="Sunken Spanish Galleon", Description="Resting deep beneath the sea lies a shipwreck, its cargo hold brimming with gold doubloons and ancient artifacts. Dark waters, predatory sea creatures, and limited oxygen pose threats.", Difficulty="Easy"},
            new Mission() {Title="The Forbidden Museum", Description="A secretive institution, rumored to house relics of untold power. Hidden away on a mountain peak, it's protected by both elite guards and treacherous terrain.", Difficulty="Hard"},
            new Mission() {Title="Mystic Island Casino", Description="An opulent casino on a private island, where the rich and infamous gamble away fortunes. Behind the glitzy facade, lies a vault of ill-gotten treasures.", Difficulty="Very Hard"},
            new Mission() {Title="Penthouse of the Oligarch", Description="Atop the city's tallest skyscraper, a tycoon's penthouse hides valuable art and encrypted data. Guarded by private security and advanced tech defenses.", Difficulty="Medium"},
            new Mission() {Title="Celestial Observatory Heist", Description="Perched on a remote mountaintop, the observatory holds astronomical secrets and rare cosmic gems. Guarded by a dedicated crew and natural isolation.", Difficulty="Easy"},
            new Mission() {Title="Royal Train Robbery", Description="A moving target, the luxury royal train transports a monarch's treasures. Security is tight, and the ever-changing landscape complicates the plot.", Difficulty="Hard"},
            new Mission() {Title="Opera House Caper", Description="An architectural marvel echoing with music, it's said to have a vault of old world wealth beneath its stage. Heisting during a live performance?", Difficulty="Medium"},
            new Mission() {Title="Tech Tycoon's Tower", Description="A modern skyscraper fitted with the latest technology, owned by a billionaire tech magnate. Rumored to hold a digital treasure of immense value.", Difficulty="Medium"},
            new Mission() {Title="Underground Speakeasy Heist", Description="Hidden below the bustling city streets, a prohibition-era speakeasy, now a den for modern elite. Holds rare liquors and a secret stash of golden bars.", Difficulty="Very Hard"},
    };

    // FOR TESTING
    public List<Mission> GetRandomLocalMissions(int count)
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

    // ----------------- MISSION -----------------

    public async Task<bool> SaveLocalMissionData(Mission mission)
    {
        return await _dataService.SaveData(localMissionFileName, mission, true);
    }

    public async Task<Mission> GetLocalMissionData()
    {
        return await _dataService.LoadData<Mission>(localMissionFileName, true);
    }

    public async Task<bool> DeleteLocalMission()
    {
        return await _dataService.DeleteData(localMissionFileName);
    }

    // ----------------- CURRECT OBJECTIVES -----------------
    public async Task<bool> SaveLocalMapNodesListData(List<MapNode> mapNodesList)
    {
        if (await _dataService.SaveData(localObjectivesFileName, mapNodesList, true))
            return true;

        return false;
    }

    public async Task<List<MapNode>> LoadLocalMapNodesList()
    {
        return await _dataService.LoadData<List<MapNode>>(localObjectivesFileName, true);
    }

    public async Task<bool> DeleteMapNodesList()
    {
        return await _dataService.DeleteData(localObjectivesFileName);
    }

    // ----------------- MISSIONS CACHE -----------------

    public async Task<bool> SaveLocalMissionsCacheData(List<Mission> missionsChache)
    {
        return await _dataService.SaveData(localMissionsCacheFileName, missionsChache, true);
    }

    public async Task<List<Mission>> GetLocalMissionsCacheData()
    {
        return await _dataService.LoadData<List<Mission>>(localMissionsCacheFileName, true);
    }

    public async Task<bool> DeleteLocalMissionCacheData()
    {
        return await _dataService.DeleteData(localMissionsCacheFileName);
    }

    #endregion

    #region REMOTE DATA

    public async Task<List<Mission>> GetRandomRemoteMissions(int count)
    {
        try
        {
            List<Mission> missionsList = new List<Mission>();

            Query missionsQuery = _fsDB.Collection("missions");
            QuerySnapshot missionsSnapshot = await missionsQuery.GetSnapshotAsync();

            if (missionsSnapshot.Count > 0)
            {
                foreach (var doc in missionsSnapshot.Documents)
                {
                    Mission mission = doc.ConvertTo<Mission>();
                    missionsList.Add(mission);
                }

                // Get x (=count) random missions from missionsList
                List<Mission> randomMissionsList = new List<Mission>(missionsList);
                int n = missionsList.Count;
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

            return null;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    #endregion
}
