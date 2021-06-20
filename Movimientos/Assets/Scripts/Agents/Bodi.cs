using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Bodi Una componente física que consta de las características físicas del
    agente(variables y propiedades) junto con, si así se quiere, una serie de
    métodos básicos sobre dichos atributos.
 */

// Esta componente representa la física de cualquier personaje móvil.
public abstract class Bodi : MonoBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    // ???: public bool canMove = true;

    // Escalares
    public float mass = 60;
    public float orientation;
    public float rotation;
    public float maxSpeed = 4;
    public float maxAcceleration = 8;
    public float maxRotation = 120; // TODO: investigar esto
    public float maxAngularAcceleration = 60; // TODO: investigar esto

    // Vectores
    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 position
    { // Para lo posición se usara la información contenida en la componente Transform
        get { return transform.position; }
        set { transform.position = value; }
    }

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public virtual void Awake() { }

    // Convertir la posición del personaje en un angulo
    float PositionToAngle()
    {
        return Utils.PositionToAngle(this.position);
    }

    // Convertir la orientación del personaje en un vector
    public Vector3 OrientationToVector()
    {
        return Utils.OrientationToVector(this.orientation);
    }

    // Dada la posición de otro personaje (un Vector3) determinar cuál es
    // el ángulo más pequeño para que el personaje se rote hacia él
    public float LookAt(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - position;
        return Utils.PositionToAngle(direction);
    }

    // Añade cuantos métodos te sean necesarios, y relacionado solo
    // con las características físicas, conforme vayas añadiendo
    // comportamiento a los NPC.

    public Vector3 Heading()
    {
        return OrientationToVector();
    }

}