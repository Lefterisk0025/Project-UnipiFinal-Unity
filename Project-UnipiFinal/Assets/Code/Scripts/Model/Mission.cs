using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

[FirestoreData]
public class Mission
{
    [FirestoreProperty]
    public string Title { get; set; }
    [FirestoreProperty]
    public string Description { get; set; }
    [FirestoreProperty]
    public string Difficulty { get; set; }

    public int Id { get; set; }
    public bool IsCompleted { get; set; }
    public MapGraph MapGraph { get; set; }
}