using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Firebase.Firestore;
using UnityEngine;
using Random = UnityEngine.Random;

public class MissionRemoteService
{
    FirebaseFirestore _fsDB;

    public MissionRemoteService()
    {
        _fsDB = FirebaseFirestore.DefaultInstance;
    }

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

}
