using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem
{
    // Tipos de terrenos
    public enum TerrainType { Street, Forest, Plains, Heal, BaseA, BaseB, Water, Unknown };

    // Constante de impacto. Depende de la vida que se les ponga a las unidades
    // y de lo rápido que se quiera que maten unas unidades a otras.
    public static float CteImpacto = 200;
    public static float HealSpeed = 1;
    public static float HealPower = CteImpacto;

    // Tipos de unidades
    public enum UnitType { Tanker, Infantry, Ranged };
    public static float[] MaxHP = { 4000, 2000, 1000 };
    public static float[] MaxSpeed = { 30, 50, 40 };
    public static float[] MaxAcceleration = { 60, 100, 80 };
    public static float[] Strength = { 200, 100, 50 };
    public static float[] AtkRange = { 15, 20, 40 };
    public static float[] AtkSpeed = { 2, 1, 0.5f };
    public static float[] Influence = { 4, 2, 1 };
    public static float[] Radius = { 4, 2, 1 };

/*
    Estas tablas se pueden usar para añadir elementos tácticos a las unidades.
    Por ejemplo, si un ranged está siendo atacado, y recibe más daño del que produce, podría
    buscar huir a un bosque. No obstante, en bosque apenas producen daño, con lo que si
    tienen buena salud buscarían una llanura o una carretera que este en rango de combate
    con el enemigo.
    Es normal que, al introducir reglas de comportamiento, las unidades se puedan volver
    "locas" de vez en cuando (no confundir con el modo “berserker”) y hagan cosas muy raras,
    pasa hasta en juegos comerciales que se supone que emplean mucho tiempo depurando
    sus sistemas tácticos y sus mecánicas de combate.
    Introduce los comportamientos uno a uno y prueba a ver que tal funciona.
*/

    // Factor de tipo de atacante/defensor
    public static float[,] FAD = {
        // Tanker, Infantry, Ranged
        { 1.00f, 0.50f, 0.50f }, // Tanker
        { 1.50f, 1.00f, 1.50f }, // Infantry
        { 0.50f, 1.50f, 1.00f }  // Ranged
    };

    // Factor por terreno del atacante
    public static float[,] FTA = {
        // Tanker, Infantry, Ranged
        { 1.50f, 1.00f, 1.00f }, // Street
        { 0.50f, 1.00f, 0.50f }, // Forest
        { 1.00f, 1.00f, 1.50f }, // Plains
        { 1, 1, 1 }, // Heal
        { 1, 1, 1 }, // BaseA
        { 1, 1, 1 }, // BaseB
        { 1, 1, 1 }, // Water
        { 1, 1, 1 }, // Unknown
    };

    // Factor por terreno del defensor
    public static float[,] FTD = {
        // Tanker, Infantry, Ranged
        { 1.50f, 1.00f, 1.00f }, // Street
        { 0.50f, 1.00f, 1.50f }, // Forest
        { 1.00f, 1.00f, 0.50f }, // Plains
        { 1, 1, 1 }, // Heal
        { 1, 1, 1 }, // BaseA
        { 1, 1, 1 }, // BaseB
        { 1, 1, 1 }, // Water
        { 1, 1, 1 }, // Unknown
    };

    // Determina la vida que se pierde en cada round
    public static int Damage(AgentUnit attacker, AgentUnit defender)
    {
        // Indices de los factores de ataque/defensa
        int atk = (int) attacker.unitType;
        int def = (int) defender.unitType;
        int terrA = (int) attacker.terrain;
        int terrD = (int) defender.terrain;

        // Fuerza del atacante
        float FA = Strength[atk] * FAD[atk, def] * FTA[terrA, atk];
        // Fuerza del defensor
        float FD = Strength[def] * FTD[terrD, def];

        // Normal hit
        float damage = (FA / FD) * CteImpacto;// * (Random.Range(1,51)/100 + 0.5f);
        // Para hacer siempre algo de daño
        if (damage <= (CteImpacto/10) ) 
            damage = Random.Range(1, 1+CteImpacto/5);
        // Critical hit
        if (Random.Range(1,101) == 42)
            damage *= 2;

        return (int) damage;
    }

}