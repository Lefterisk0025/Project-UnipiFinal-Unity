using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackTarget
{
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Difficulty _difficulty;
    [SerializeField] private int _minimumReputationNeeded;
    [SerializeField] private bool _isCompleted;

    public string Title { get => _title; set => _title = value; }
    public string Description { get => _description; set => _description = value; }
    public Difficulty Difficulty { get => _difficulty; private set => _difficulty = value; }
    public int MinimumReputationNeeded { get => _minimumReputationNeeded; private set => _minimumReputationNeeded = value; }

    public bool IsCompleted { get => _isCompleted; private set => _isCompleted = value; }

    public AttackTarget(string title, string description, Difficulty difficulty, int minimumReputationNeeded)
    {
        _title = title;
        _description = description;
        _difficulty = difficulty;
        _minimumReputationNeeded = minimumReputationNeeded;
        _isCompleted = false;
    }

    public void CompleteAttack() => _isCompleted = true;
}