using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum StrategyMode { NEUTRAL, ATTACK, DEFEND, TOTALWAR };
    [SerializeField] private GameObject teamA, teamB;
    public static List<AgentUnit> teamAUnits, teamBUnits;

    void Awake()
    {
        teamAUnits = new List<AgentUnit>();
        foreach(Transform child in teamA.transform)
        {
            AgentUnit npc = child.GetComponent<AgentUnit>();
            teamAUnits.Add(npc);
        }
        teamBUnits = new List<AgentUnit>();
        foreach(Transform child in teamB.transform)
        {
            AgentUnit npc = child.GetComponent<AgentUnit>();
            teamBUnits.Add(npc);
        }
    }

    // FIXME: de momento todo esto es tmp
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F1)) {
            Neutral(0);
            Neutral(1);
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            Attack(0);
            Attack(1);
        }
        
        if (Input.GetKeyDown(KeyCode.F3)) {
            Defend(0);
            Defend(1);
        }

        if (Input.GetKeyDown(KeyCode.F12))
            TotalWar();

    }

    public void TotalWar()
    {
        ChangeMode(0, StrategyMode.TOTALWAR);
        ChangeMode(1, StrategyMode.TOTALWAR);
    }

    public void Attack(int team)
    {
        ChangeMode(team, StrategyMode.ATTACK);
    }

    public void Defend(int team)
    {
        ChangeMode(team, StrategyMode.DEFEND);
    }

    public void Neutral(int team)
    {
        ChangeMode(team, StrategyMode.NEUTRAL);
    }

    public void ChangeMode(int team, StrategyMode mode)
    {
        switch (team)
        {
            case 0:
                foreach (AgentUnit npc in teamAUnits)
                    npc.strategyMode = mode;
                break;
            case 1:
                foreach (AgentUnit npc in teamBUnits)
                    npc.strategyMode = mode;
                break;
        }
    }

    public static bool CheckVictory()
    {
        // TODO
        return false;
    }

    public static List<AgentUnit> GetTeamA()
    {
        return teamAUnits;
    }

    public static List<AgentUnit> GetTeamB()
    {
        return teamBUnits;
    }

    public static List<AgentUnit> GetAllies(int team)
    {
        switch (team)
        {
            case 0: return teamAUnits;
            case 1: return teamBUnits;
            default: return null;
        }
    }

    public static List<AgentUnit> GetEnemies(int team)
    {
        switch (team)
        {
            case 0: return teamBUnits;
            case 1: return teamAUnits;
            default: return null;
        }
    }

}