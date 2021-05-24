using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekVelocity : SteeringBehaviour
{
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Si el agente está cerca del objetivo no se moverá; pero si aún no
    // está cerca, entonces irá al objetivo a máxima velocidad
    override public Steering GetSteering(AgentNPC agent)
    {
        // Establece a valores nulos el valor del steering que se tiene que retornar
        Steering steer = new Steering();

        // Calcula la distancia que hay entre el target y el agente
        Vector3 direction = this.target.position - agent.position;
        float distance = direction.magnitude;

        // Si la distancia es mayor que el radio interior del target establece la
        // magnitud vectorial del steering como el vector cuya magnitud es la
        // velocidad máxima del agente y cuya dirección va del agente hacia el target
        if (distance > this.target.interiorRadius)
        {
            steer.linear = direction;
            steer.linear.Normalize();
            steer.linear *= agent.maxSpeed;
            steer.angular = 0;
        }

        return steer;
    }

}
