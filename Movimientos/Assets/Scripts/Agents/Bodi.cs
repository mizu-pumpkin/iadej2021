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
    public float maxSpeed = 8;
    public float maxAcceleration = 4;
    public float maxRotation = 360;
    public float maxAngularAcceleration = 60;

    // Vectores
    public Vector3 velocity = Vector3.zero;
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
    public float PositionToAngle()
    {
        return Mathf.Atan2(this.position.x, this.position.z) * Mathf.Rad2Deg; // ???: (z,x) o (x,z)
    }
    
    float PositionToAngle(Vector3 position)
    {
        return Mathf.Atan2(position.x, position.z) * Mathf.Rad2Deg; // ???: (z,x) o (x,z)
    }

    // Convertir la orientación del personaje en un vector
    Vector3 OrientationToVector()
    {
        float rad = this.orientation * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
    }
    
    Vector3 OrientationToVector(float orientation)
    {
        float rad = orientation * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
    }

    // Dada la posición de otro personaje (un Vector3) determinar cuál es
    // el ángulo más pequeño para que el personaje se rote hacia él
    public float Heading2(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - position;
        return PositionToAngle(direction);
    }

    public float Heading(Vector3 positionNPC) // FIXME: no funciona como debería
    {
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
}
