using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public void Attack(int team) => GameManager.Attack(team);
    public void Defend(int team) => GameManager.Defend(team);
    public void Neutral(int team) => GameManager.Neutral(team);
    public void TotalWar() => GameManager.TotalWar(); 
}
