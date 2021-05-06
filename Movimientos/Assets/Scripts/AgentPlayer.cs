using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Un agente especial es el controlado por el jugador
public class AgentPlayer : Agent
{
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    void Update()
    {
        ApplySteering();
    }

    void ApplySteering()
    {
        // Un AgentPlayer es un Agent que se mueve a máxima velocidad en la dirección dada por el jugador
        velocity = (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * maxSpeed;
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // y que rota mediante un Steering
        transform.LookAt(position + velocity); // solo para este agente podemos usar LookAt()
        orientation = transform.rotation.eulerAngles.y;
    }
}
