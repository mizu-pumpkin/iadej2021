using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        if (this.exteriorAngle < 0)
            this.exteriorAngle = 0;
        
        if (this.interiorAngle < 0)
            this.interiorAngle = 0;
        
        if (this.interiorAngle > this.exteriorAngle)
            this.interiorAngle = this.exteriorAngle;
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
            DrawRadius(this.interiorRadius, Color.red);
            DrawRadius(this.exteriorRadius, Color.green);
            DrawAngle(this.interiorAngle, Color.blue);
            DrawAngle(this.exteriorAngle, Color.yellow);
        }
    }

    void DrawRadius(float radius, Color color) {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);
    }

    void DrawAngle(float angle, Color color) {
        Gizmos.color = color;
        float degree_angle = angle*Mathf.Rad2Deg; // ???: está bien así?
        Vector3 fwd = transform.forward; // ???: puedo usarlo aquí?
        Gizmos.DrawLine(position, position + Quaternion.Euler(0, degree_angle, 0) * fwd);
        Gizmos.DrawLine(position, position + Quaternion.Euler(0, -degree_angle, 0) * fwd);
    }

}