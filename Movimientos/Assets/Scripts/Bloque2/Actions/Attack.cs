using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    public AgentUnit enemyUnit;

    public override bool CanInterrupt()
    {
        return false;
    }

    public override bool CanDoBoth(Action other)
    {
        return false;
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override void Execute()
    {
        unit.SetTarget(enemyUnit.position, enemyUnit.orientation);

        float distance = (unit.position - enemyUnit.position).magnitude;

        if (distance <= unit.attackRange)
        {
            int damage = CombatSystem.Damage(unit, enemyUnit);
            enemyUnit.hp -= damage;

            if (enemyUnit.hp <= 0) 
                unit.InitializeSteerings();
        }
    }
}