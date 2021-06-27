using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum StrategyMode { NEUTRAL, ATTACK, DEFEND, TOTALWAR };
    [SerializeField] private GameObject teamA, teamB;
    public static List<AgentUnit> teamAUnits, teamBUnits;

    public static StrategyMode modeA, modeB;

    public static int hpBaseA = 10000;
    public static int hpBaseB = 10000;
    public static int killCountA = 0;
    public static int killCountB = 0;

    [SerializeField] private Text hpAText;
    [SerializeField] private Text hpBText;
    [SerializeField] private Text modeAText;
    [SerializeField] private Text modeBText;
    [SerializeField] private Text victoryText;

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

    void Update()
    {
        if (!CheckVictory()) {
            BaseAttacks();
            modeAText.text = modeA.ToString();
            modeBText.text = modeB.ToString();
        }
        else
            EndGame();
    }

    public static void TotalWar()
    {
        ChangeMode(0, StrategyMode.TOTALWAR);
        ChangeMode(1, StrategyMode.TOTALWAR);
    }

    public static void Attack(int team)
    {
        ChangeMode(team, StrategyMode.ATTACK);
    }

    public static void Defend(int team)
    {
        ChangeMode(team, StrategyMode.DEFEND);
    }

    public static void Neutral(int team)
    {
        ChangeMode(team, StrategyMode.NEUTRAL);
    }

    public static void ChangeMode(int team, StrategyMode mode)
    {
        switch (team)
        {
            case 0:
                modeA = mode;
                foreach (AgentUnit npc in teamAUnits)
                    npc.ChangeMode(mode);
                break;
            case 1:
                modeB = mode;
                foreach (AgentUnit npc in teamBUnits)
                    npc.ChangeMode(mode);
                break;
        }
    }

    public void EndGame()
    {
        if (hpBaseA <= 0) {
            hpBaseA = 0;
            victoryText.text = "BLACK WINS";
            victoryText.enabled = true;
        }
        if (hpBaseB <= 0) {
            hpBaseB = 0;
            victoryText.text = "WHITE WINS";
            victoryText.enabled = true;
        }
    }

    public static bool CheckVictory()
    {
        return hpBaseA <= 0 || hpBaseB <= 0;
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

    public static bool IsBaseUnderAttack(int team)
    {
        switch (team)
        {
            case 0:
                foreach (AgentUnit npc in teamAUnits)
                    if (npc != null && npc.inEnemyBase)
                        return true;
                break;
            case 1:
                foreach (AgentUnit npc in teamBUnits)
                    if (npc != null && npc.inEnemyBase)
                        return true;
                break;
        }
        return false;
    }

    private void BaseAttacks()
    {
        int a = 0;
        foreach (AgentUnit npc in teamAUnits)
            if (npc != null && npc.inEnemyBase)
                a++;

        int b = 0;
        foreach (AgentUnit npc in teamBUnits)
            if (npc != null && npc.inEnemyBase)
                b++;
        
        hpBaseA -= b * 200;
        hpBaseB -= a * 200;
        hpAText.text = hpBaseA.ToString();
        hpBText.text = hpBaseB.ToString();
    }

    public static void CountKilled(AgentUnit attacker, AgentUnit killed)
    {
        Debug.Log(attacker.gameObject.name+" killed "+killed.gameObject.name);
        switch(attacker.team)
        {
            case 0: killCountA++; break;
            case 1: killCountB++; break;
        }
    }

}