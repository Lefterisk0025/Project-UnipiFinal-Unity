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

public class PlayerService
{
    private const string localFileName = "/player.json";

    ILocalDataService _dataService;
    FirebaseFirestore _fsDB;

    public PlayerService()
    {
        _fsDB = FirebaseFirestore.DefaultInstance;
        _dataService = new JsonLocalDataService();
    }

    #region LOCAL

    public async Task<bool> SaveLocalPlayerDataAsync(Player player)
    {
        return await _dataService.SaveData(localFileName, player, true);
    }

    public async Task<Player> GetLocalPlayerDataAsync()
    {
        return await _dataService.LoadData<Player>(localFileName, true);
    }

    public async Task<bool> DeleteLocalPlayerDataAsync()
    {
        return await _dataService.DeleteData(localFileName);
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

    #endregion
}