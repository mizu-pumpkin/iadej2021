using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor; // Handles

/*
    Agent::Bodi Una componente sensorial que determina los márgenes de
    reacción del agente
 */

// Esta componente establece el campo sensorial del personaje, que depende de sus cualidades físicas.
public class Agent : Bodi
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    public float interiorRadius = 0; // radio de colision
    public float exteriorRadius = 0; // radio de llegada
    public float interiorAngle = 45; // angulo de colision / vision normal
    public float exteriorAngle = 90; // angulo de llegada / limite de vision

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Construye las Propiedades para que los valores interiores siempre sean
    // inferiores a los exteriores
    public override void Awake()
    {
        base.Awake();
        if (this.exteriorRadius < 0)
            this.exteriorRadius = 0;
        
        if (this.interiorRadius < 0)
            this.interiorRadius = 0;
        
        if (this.interiorRadius > this.exteriorRadius)
            this.interiorRadius = this.exteriorRadius;
    }

    /*
        █▀▄ █▀▀ █▄▄ █░█ █▀▀
        █▄▀ ██▄ █▄█ █▄█ █▄█
     */

    // Para la depuración. Dispón de un variable booleana para que,
    // según su valor, muestre como Gizmos los 4 valores anteriores.
    public bool debug = false;

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
            // TODO: cambiar lo de arriba por lo de abajo cuando se implemente
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
