using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;

[FirestoreData]
public class Player
{
    [FirestoreProperty]
    public bool IsRegistered { get; set; }

    [FirestoreProperty]
    public string DisplayName { get; set; }

    [FirestoreProperty]
    public int Reputation { get; set; }

    [FirestoreProperty]
    public string UserId { get; set; }

    [FirestoreProperty]
    public DateTime LastTimeMissionsFetched { get; set; }
}
