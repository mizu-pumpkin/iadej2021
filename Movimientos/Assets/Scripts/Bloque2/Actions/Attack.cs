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

        // if (enemyUnit.position != unit.pathFollowing.path[unit.pathFollowing.path.Count -1])
        // {
        //     unit.Attack(enemyUnit);
        // }
        
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
                unit.color = unit.colorAttack;
            }
            else unit.color = unit.colorOriginal;
        }
        // If it's not in range, move closer
        else {
            unit.canMove = true;
            unit.color = unit.colorOriginal;
        }
    }

    public override string ToString() => "ATTACKING "+enemyUnit.gameObject.name;
}