using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    private AgentUnit enemyUnit;
    private float time = -1;

    public Attack(AgentUnit unit, AgentUnit enemyUnit) : base(unit)
    {
        this.enemyUnit = enemyUnit;
    }

    public override void Execute()
    {
        if (time == -1)
            time = Time.time;
        
        float distance = (unit.position - enemyUnit.position).magnitude;

        // If it's in range, attack
        if (distance <= unit.attackRange)
        {
            unit.canMove = false;
            if (Time.time - time >= unit.attackSpeed) {
                time = -1;
                int damage = CombatSystem.Damage(unit, enemyUnit);
                bool dead = enemyUnit.TakeDamage(damage, unit);
                if (dead) isComplete = true;
            }
        }
        // If it's not in range, move closer
        else {
            unit.canMove = true;
        }
    }

    public override string ToString() => "ATTACK";
}