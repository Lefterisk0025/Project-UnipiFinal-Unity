using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;

public class PlayerRemoteService
{
    FirebaseFirestore _fsDB;

    public PlayerRemoteService()
    {
        _fsDB = FirebaseFirestore.DefaultInstance;
    }

    public string GetUserIdOfAuthUser()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
            return user.UserId;
        else
            return null;
    }

    public async Task<Player> GetPlayerByUserId(string userId)
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

    public async Task<bool> CreatePlayer(string userId, string displayName, int gender)
    {
        try
        {
            Player player = new Player
            {
                DisplayName = displayName,
                Reputation = 0,
                UserId = userId,
                Gender = gender,
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
    public async Task<bool> UpdatePlayer(Player player)
    {
        try
        {
            // Update reputation
            DocumentReference playerRef = _fsDB.Collection("players").Document(player.Uid);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                {"DisplayName", player.DisplayName},
                {"Gender", player.Gender},
                { "NetCoins", player.NetCoins },
                { "Reputation", player.Reputation },
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

    public async Task<bool> SyncLocalAndRemotePlayer(Player localPlayer)
    {
        try
        {
            Player remotePlayer = await GetPlayerByUserId(GetUserIdOfAuthUser());

            remotePlayer.DisplayName = localPlayer.DisplayName;
            remotePlayer.Gender = localPlayer.Gender;
            remotePlayer.Reputation = localPlayer.Reputation;
            remotePlayer.NetCoins = localPlayer.NetCoins;

            await UpdatePlayer(remotePlayer);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }
}
