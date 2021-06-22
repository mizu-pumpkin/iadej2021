using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    // Tipos de unidades
    public enum UnitType { patrulla, explorador, pesada, ligera, arqueros };
    //public static int n_units = Enum.GetNames(typeof(UnitType)).Length;

    // Tipos de terrenos
    public enum TerrainType { carretera, bosque, llanura };
    //public static int n_terrains = Enum.GetNames(typeof(TerrainType)).Length;

    // Factor de tipo de atacante/defensor
    public static float[,] FAD = {
        // patrulla, explorador, pesada, ligera, arqueros
        { 1.00f, 1.25f, 0.50f, 1.50f, 1.75f }, // patrulla
        { 0.75f, 1.00f, 0.75f, 1.25f, 1.75f }, // explorador
        { 1.50f, 1.25f, 1.00f, 1.50f, 1.75f }, // pesada
        { 1.00f, 0.50f, 0.25f, 1.00f, 1.75f }, // ligera
        { 1.50f, 1.25f, 1.00f, 1.50f, 1.00f }  // arqueros
    };

    // Factor por terreno del atacante
    public static float[,] FTA = {
        // patrulla, explorador, pesada, ligera, arqueros
        { 1.00f, 1.00f, 1.00f, 1.00f, 2.00f }, // carretera
        { 0.75f, 2.00f, 0.25f, 0.75f, 0.10f }, // bosque
        { 1.00f, 0.75f, 1.00f, 1.00f, 2.00f }  // llanura
    };

    // Factor por terreno del defensor
    public static float[,] FTD = {
        // patrulla, explorador, pesada, ligera, arqueros
        { 1.00f, 0.50f, 1.00f, 0.75f, 0.75f }, // carretera
        { 1.25f, 2.00f, 0.50f, 1.25f, 1.25f }, // bosque
        { 1.00f, 0.50f, 1.00f, 1.00f, 0.75f }  // llanura
    };

    // Factor de calidad de las unidades
    public static float[] Calidad = { 50, 100, 200, 100, 70 };
    // Puntos de vida de las unidades
    public static float[] HP = { 1000, 1000, 4000, 2000, 1000 };

    // Constante de impacto. Depende de la vida que se les ponga a las unidades
    // y de lo rápido que se quiera que maten unas unidades a otras.
    public static float CteImpacto = 200;

    // Determina la vida que se pierde en cada round
    public static int Damage(AgentUnit attacker, AgentUnit defender)
    {
        // Indices de los factores de ataque/defensa
        int atk = (int) attacker.unitType;
        int def = (int) defender.unitType;
        int terrA = (int) attacker.terrain;
        int terrD = (int) defender.terrain;

        // Fuerza del atacante
        float FA = Calidad[atk] * FAD[atk, def] * FTA[terrA, atk];
        // Fuerza del defensor
        float FD = Calidad[def] * FTD[terrD, def];

        // Normal hit
        float damage = (FA / FD) * CteImpacto * (Random.Range(1,51)/100 + 0.5f);
        // Para hacer siempre algo de daño
        if (damage <= (CteImpacto/10) ) 
            damage = Random.Range(1, 1+CteImpacto/5);
        // Critical hit
        if (Random.Range(1,101) == 42)
            damage *= 2;

        return (int) damage;
    }
}

/*
Estas tablas se pueden usar para añadir elementos tácticos a las unidades.
Por ejemplo, si un arquero está siendo atacado, y recibe más daño del que produce, podría
buscar huir a un bosque. No obstante, en bosque apenas producen daño, con lo que si
tienen buena salud buscarían una llanura o una carretera que este en rango de combate
con el enemigo.
Es normal que, al introducir reglas de comportamiento, las unidades se puedan volver
"locas" de vez en cuando (no confundir con el modo “berserker”) y hagan cosas muy raras,
pasa hasta en juegos comerciales que se supone que emplean mucho tiempo depurando
sus sistemas tácticos y sus mecánicas de combate.
Introduce los comportamientos uno a uno y prueba a ver que tal funciona.
*/