using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    private AgentUnit enemyUnit;
    private Vector3 targetPosition;
    private float time = -1;

    public Attack(AgentUnit unit, AgentUnit enemyUnit) : base(unit)
    {
        this.enemyUnit = enemyUnit;
        this.targetPosition = enemyUnit.position;
    }

    public override void Execute()
    {
        if (time == -1)
            time = Time.time;
        
        float d1 = (unit.position - enemyUnit.position).magnitude;
        float d2 = (unit.position - targetPosition).magnitude;

        // If it's in range, attack
        if (d1 <= unit.attackRange)
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
        // The enemy moved away, find it again
        else if (d2 <= unit.attackRange) {
            unit.canMove = true;
            unit.color = unit.colorOriginal;
            unit.Attack(enemyUnit);
        }
        // If it's not in range, move closer
        else {
            unit.canMove = true;
            unit.color = unit.colorOriginal;
        }
    }

    public override string ToString() => "ATTACKING "+enemyUnit.gameObject.name;
}