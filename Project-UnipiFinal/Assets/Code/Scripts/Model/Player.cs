using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;

[FirestoreData]
public class Player
{
    public string Uid { get; set; }


    [FirestoreProperty]
    public string DisplayName { get; set; }
    [FirestoreProperty]
    public string UserId { get; set; }
    [FirestoreProperty]
    public int Reputation { get; set; }
    [FirestoreProperty]
    public int NetCoins { get; set; }
    [FirestoreProperty]
    public int Gender { get; set; }
}
