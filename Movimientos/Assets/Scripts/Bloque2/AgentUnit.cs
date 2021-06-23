using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor; // Handles

public class AgentUnit : AgentNPC
{
    // El control táctico de toda unidad tiene que tener modo antisuicida
    public enum State { none, attacking, defending, fleeing, dying, healing, routed };
    private State state;

    public int team;
    public Vector3 respawnPos = Vector3.zero;
    public Vector3 deathPos = Vector3.zero;
    public CombatSystem.UnitType unitType;
    public CombatSystem.TerrainType terrain;
    public float influence; // the intrinsic military power of the unit
    public float effectRadius; // the radius of effect of the influence

    public float attackRange;
    [SerializeField] public float attackSpeed;
    [SerializeField] private float maxHP;
    [SerializeField] private float hp;
    public float healStrength;

    private Action action = null;
    public bool canMove = true;
    public bool isNPC = false;

    public override void Awake()
    {
        base.Awake();
        InitializeUnit();
    }

    private void InitializeUnit()
    {
        maxSpeed = CombatSystem.MaxSpeed[(int)unitType];
        maxAcceleration = CombatSystem.MaxAcceleration[(int)unitType];

        attackRange = CombatSystem.AtkRange[(int)unitType];
        exteriorRadius = attackRange * 2;

        attackSpeed = CombatSystem.AtkSpeed[(int)unitType];
        healStrength = CombatSystem.HealStrength[(int)unitType];

        maxHP = CombatSystem.MaxHP[(int)unitType];
        hp = maxHP;

        influence = CombatSystem.Influence[(int)unitType];
        effectRadius = CombatSystem.Radius[(int)unitType];
    }

    public override void Update()
    {
        if (canMove)
            base.Update();
        ExecuteAction();
    }

    public void OnDrawGizmos()
    {
        string text = unitType.ToString() +
        "\nHP: "+hp.ToString();
        if (action != null)
            text += "\n"+action.ToString();
        
        Handles.Label(transform.position, text);
    }

    public void ExecuteAction()
    {
        if (action != null)
        {
            action.Execute();
            if (action.IsComplete()) {
                action = null;
                canMove = true;
                InitializeSteerings(); // HACK
                if (state == State.dying) SetTarget(deathPos, 0); // HACK
                state = State.none;
            }
        }
    }

    public void Attack(AgentUnit otherUnit)
    {
        action = new Attack(this, otherUnit);
        state = State.attacking;

        SetTarget(otherUnit); // HACK
    }
    
    public void Defend()
    {
        action = new Defend(this);
        state = State.defending;

        SetTarget(respawnPos); // HACK
    }

    public void Die()
    {
        action = new Die(this);
        state = State.dying;

        // Respawn
        hp = maxHP;//InitializeUnit();
        deathPos = position;
        position = respawnPos;
    }
    
    public void Heal(AgentUnit otherUnit)
    {
        if (healStrength > 0) {
            action = new Heal(this, otherUnit);
            state = State.healing;

            SetTarget(otherUnit); // HACK
        }
    }
    
    public void Hide()
    {
        // TODO: buscar un Waypoint escondido
    }
    
    public void Patrol()
    {
        action = new Patrol(this);
        state = State.routed;
        
        // FIXME: mejor PathFollowing que Wander
        SteeringBehaviour sb = new GameObject("Wander").AddComponent<Wander>();
        NewTarget(sb);
    }
    
    public void RunAway(AgentUnit otherUnit)
    {
        action = new RunAway(this, otherUnit);
        state = State.fleeing;

        SteeringBehaviour sb = new GameObject("Flee").AddComponent<Flee>();
        sb.target = otherUnit;
        NewTarget(sb);
    }

    public bool TakeDamage(int damage, AgentUnit attacker)
    {
        hp -= damage;
        // If health is 0, die
        if (hp <= 0) {
            Die();
            return true;
        }
        // If attacked, respond to attack
        else if (state != State.attacking) {
            Attack(attacker);
        }
        // If health is low and speed is high, run away
        else if (hp < (maxHP / 2) && hp < attacker.hp && maxSpeed >= attacker.maxSpeed && state != State.fleeing) {
            RunAway(attacker);
        }
        return false;
    }

    public bool TakeHeal(float heal)
    {
        hp += heal;
        if (hp >= maxHP) {
            hp = maxHP;
            return true;
        }
        return false;
    }

}