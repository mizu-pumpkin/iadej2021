using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    SteeringBehavior Representa al comportamiento en una dirección. Indica qué
    cálculos se deben de realizar para que el agente tenga el
    comportamiento deseado.
 */

// Calcula una direccion y una rotacion
// Esta componente establece el comportamiento en la dirección de un personaje
public abstract class SteeringBehaviour : MonoBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    // Representa a cualquier target sobre el que se calculará el comportamiento
    public Agent target;
    // El movimiento que deberá de realizar el personaje según el comportamiento que se desee
    Steering steer;
    // ???: weight/priority

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Agent>(); // FIXME
    }

    // Calcular el Steering para el agente dado en función del comportamiento deseado
    // Supongamos que el agente A se mueve en la dirección del agente B.
    // Entonces el SteeringBehaviour tendrá como target al agente B y el método
    // GetSteering serán invocado desde el agente A como:
    // Steering steer = steeringConcreto.GetSteering(this)
    public abstract /*virtual*/ Steering GetSteering(AgentNPC agent);
}
