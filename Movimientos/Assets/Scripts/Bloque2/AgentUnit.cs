using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentUnit : AgentNPC
{
    public float influence; // the intrinsic military power of the unit
    public float effectRadius; // the radius of effect of the influence
    public int team;

    public float atk; // offensive strength
    public float def; // defensive strength

    public CombatSystem.UnitType unitType;
    public CombatSystem.TerrainType terrain;
    public float hp;
    
    public enum AttackType { melee, ranged }
    public AttackType attackType = AttackType.melee;
    public float attackRange = 1;
    public float attackSpeed = 1;

    // El control táctico de toda unidad tiene que tener modo antisuicida
    public enum Mode { attack, defend, flee, panic, routed };
}
