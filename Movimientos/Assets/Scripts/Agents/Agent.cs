using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor; // Handles

/*
    Agent::Bodi Una componente sensorial que determina los márgenes de
    reacción del agente
 */

// Esta componente establece el campo sensorial del personaje, que depende de sus cualidades físicas.
public abstract class Agent : Bodi
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    public float interiorRadius; // radio de colision
    public float exteriorRadius; // radio de llegada
    public float interiorAngle; // angulo de colision
    public float exteriorAngle; // angulo de llegada, limite

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Construye las Propiedades para que los valores interiores siempre sean
    // inferiores a los exteriores
    public void BuildProperties()
    {
        float [] localScale = new float[]{
            transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        };
        
        this.interiorRadius = Mathf.Min(localScale);
        // TODO:???: preguntar qué se supone que deberían representar estos valores
        this.exteriorRadius = Mathf.Max(localScale);
        this.interiorAngle = 0;
        this.exteriorAngle = 0;
    }

    /*
        █▀▄ █▀▀ █▄▄ █░█ █▀▀
        █▄▀ ██▄ █▄█ █▄█ █▄█
     */

    // Para la depuración. Dispón de un variable booleana para que,
    // según su valor, muestre como Gizmos los 4 valores anteriores.
    public bool debug = true;

    void OnDrawGizmos()
    {
        if (debug == true)
        {
            string text =
                "\nRadio interior: " + this.interiorRadius +
                "\nRadio exterior: " + this.exteriorRadius +
                "\nAngulo interior: " + this.interiorAngle +
                "\nAngulo exterior: " + this.exteriorAngle;
            Handles.Label(transform.position, text);
            // TODO: cambiar lo de arriba por lo de abajo
            //DrawGizmoInteriorRadius();
            //DrawGizmoExteriorRadius();
            //DrawGizmoInteriorAngle();
            //DrawGizmoExteriorAngle();
        }
    }

    void DrawGizmoInteriorRadius() { }
    void DrawGizmoExteriorRadius() { }
    void DrawGizmoInteriorAngle() { }
    void DrawGizmoExteriorAngle() { }

}
