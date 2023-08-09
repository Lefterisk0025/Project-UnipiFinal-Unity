using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackTargetView : MonoBehaviour
{
    AttackTargetPresenter _attackTargetPresenter;

    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private TextMeshProUGUI _difficultyTMP;
    [SerializeField] private TextMeshProUGUI _minimumReputationTMP;

    private void Awake()
    {
        _attackTargetPresenter = new AttackTargetPresenter(this);
    }

    private void Start()
    {
        AttackTarget attackTarget = _attackTargetPresenter.GetRandomAttackTarget();

        _titleTMP.text = attackTarget.Title;
        _descriptionTMP.text = attackTarget.Description;
        _difficultyTMP.text = attackTarget.Difficulty.ToString();
        _minimumReputationTMP.text = attackTarget.MinimumReputationNeeded.ToString();
    }
}
