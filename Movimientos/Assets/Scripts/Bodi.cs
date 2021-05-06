using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Bodi Una componente física que consta de las características físicas del
    agente(variables y propiedades) junto con, si así se quiere, una serie de
    métodos básicos sobre dichos atributos.
 */

// Esta componente representa la física de cualquier personaje móvil.
public class Bodi : MonoBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    //bool canMove = true; // FIXME: me sirve para algo?

    // Escalares
    public float mass;
    public float orientation;
    public float rotation;
    public float maxSpeed;
    public float maxAcceleration;
    public float maxRotation;
    //public float maxAngular; // FIXME: me sirve para algo?

    // Vectores
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 position
    { // Para lo posición se usara la información contenida en la componente Transform
        get { return transform.position; }
        set { transform.position = value; }
    }

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Convertir la posición del personaje en un angulo
    float PositionToAngle()
    {
        return Mathf.Atan2(this.position.z, this.position.x) * Mathf.Rad2Deg;
    }

    // Convertir la orientación del personaje en un vector
    Vector3 OrientationToVector()
    {
        float rad = this.orientation * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
    }

    // Dada la posición de otro personaje (un Vector3) determinar cuál es
    // el ángulo más pequeño para que el personaje se rote hacia él
    public float Heading(Vector3 positionNPC)
    {
        // TODO: controlar que esté bien
        float hipotenusa = Mathf.Sqrt(Mathf.Pow(positionNPC.x, 2) + Mathf.Pow(positionNPC.z, 2));
        float anguloRotacion = Mathf.Asin(positionNPC.z / hipotenusa);

        if (positionNPC.z < 0 && this.position.x < 0)
            anguloRotacion = anguloRotacion + 180;

        if (positionNPC.z > 0 && this.position.x < 0)
            anguloRotacion = anguloRotacion + 90;

        if (positionNPC.z < 0 && this.position.x > 0)
            anguloRotacion = anguloRotacion + 270;

        float rotacion1 = anguloRotacion * (180 / Mathf.PI);
        float rotacion2 = (360 - anguloRotacion) * (180 / Mathf.PI);

        if (rotacion1 > rotacion2)
            return rotacion1;

        return rotacion2;
    }

    // Añade cuantos métodos te sean necesarios, y relacionado solo
    // con las características físicas, conforme vayas añadiendo
    // comportamiento a los NPC.
    // TODO

    // Start is called before the first frame update
    void Start()
    {
        // TODO: esto es temporaneo
        mass = 60;
        maxSpeed = 8;
        orientation = PositionToAngle();
    }
}
