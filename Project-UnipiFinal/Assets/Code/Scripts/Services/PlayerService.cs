using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class PlayerService
{
    private const string localPlayerFileName = "/player.json";
    private const string localMissionPerfromanceFileName = "/perfomance.json";

    ILocalDataService _dataService;
    FirebaseFirestore _fsDB;

    public PlayerService()
    {
        _fsDB = FirebaseFirestore.DefaultInstance;
        _dataService = new JsonLocalDataService();
    }

    #region LOCAL

    // PLAYER
    public async Task<bool> SaveLocalPlayerDataAsync(Player player)
    {
        return await _dataService.SaveData(localPlayerFileName, player, true);
    }

    public async Task<Player> LoadLocalPlayerDataAsync()
    {
        return await _dataService.LoadData<Player>(localPlayerFileName, true);
    }

    public async Task<bool> DeleteLocalPlayerDataAsync()
    {
        return await _dataService.DeleteData(localPlayerFileName);
    }

    // PLAYER PERFORMANCE
    public async Task<bool> SaveLocalPlayerMissionPerformanceDataAsync(MissionPerformance missionPerformance)
    {
        return await _dataService.SaveData(localMissionPerfromanceFileName, missionPerformance, true);
    }

    public async Task<MissionPerformance> LoadLocalPlayerMissionPerformanceDataAsync()
    {
        return await _dataService.LoadData<MissionPerformance>(localMissionPerfromanceFileName, true);
    }

    public async Task<bool> DeleteLocalPlayerMissionPerformanceDataAsync()
    {
        return await _dataService.DeleteData(localMissionPerfromanceFileName);
    }

    #endregion

    #region REMOTE

    public string GetRemoteAuthUserId()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
            return user.UserId;
        else
            return null;
    }

    public async Task<bool> CreateRemotePlayer(string userId, string displayName)
    {
        try
        {
            Player player = new Player
            {
                IsRegistered = true,
                DisplayName = displayName,
                Reputation = 0,
                UserId = userId,
                LastTimeMissionsFetched = DateTime.Now,
            };

            DocumentReference playerRef = _fsDB.Collection("players").Document();
            await playerRef.SetAsync(player).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Player created");
            });

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public async Task<Player> GetRemotePlayerByUserId(string userId)
    {
        try
        {
            Query playerQuery = _fsDB.Collection("players").WhereEqualTo("UserId", userId);
            QuerySnapshot playerSnapshot = await playerQuery.GetSnapshotAsync();

            if (playerSnapshot.Count > 0)
            {
                DocumentSnapshot document = playerSnapshot.Documents.FirstOrDefault();
                Player player = document.ConvertTo<Player>();
                player.Uid = document.Id;

                return player;
            }

            return null;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public async Task<bool> UpdateRemoteProgressionStatsOfPlayer(MatchResults matchResults, string userId)
    {
        try
        {
            // Get player
            Player player = await GetRemotePlayerByUserId(userId);
            player.Reputation += matchResults.ReputationEarned;

            // Update reputation
            DocumentReference playerRef = _fsDB.Collection("players").Document(player.Uid);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Reputation", player.Reputation }
            };

            await playerRef.UpdateAsync(updates);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    #endregion
}