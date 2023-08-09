using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetPresenter
{
    AttackTargetView _attackTargetView;
    AttackTarget _attackTarget;

    public AttackTargetPresenter(AttackTargetView attackTargetView)
    {
        _attackTargetView = attackTargetView;
    }

    public AttackTarget GetRandomAttackTarget()
    {
        // FIND A WAY TO GET RANDOM DATA

        AttackTarget attackTarget = null;

        return attackTarget;
    }
}
