using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    AgentNPC::Agent La componente reactiva que se encarga de calcular los
    steering, arbirarlos y aplicar un actuador.
 */

// Esta componente establece el comportamiento reactivo del personaje
// Reacciona en función de sus características físicas y sensoriales
// Un NPC tiene que tener en cuenta todos los movimientos que tiene que
// realizar, SteeringBehavior, y arbitrarlos para considerar al final un
// movimiento final, Steering.
public class AgentNPC : Agent
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    // Dispone de una lista de referencias a todas las componentes SteeringBehavior que tiene el personaje
    SteeringBehaviour[] listSteerings;
    List<Steering> kinetics = new List<Steering>();
    // ???: blendWeight/blendPriority

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Usa GetComponents<>() para cargar todas las componentes
    // SteeringBehavior que tenga el personaje
    void Awake()
    {
        this.listSteerings = this.GetComponents<SteeringBehaviour>();
    }

    // Recorre la lista construida en Awake() y calcula cada uno
    // de los Steering que calcula cada SteeringBehaviour
    // TODO: En este punto puedes aplicar un árbitro o dejarlo para el método Update()
    void LateUpdate()
    {
        kinetics = new List<Steering>();
        foreach (SteeringBehaviour str in this.listSteerings)
            kinetics.Add(str.GetSteering(this));
            //ApplySteering(str.GetSteering(this));
    }

    // En el método Update() se invocará, al menos, al método ApplySteering()
    void Update()
    {
        BuildProperties();
        // ???: preguntar por qué se recorre dos veces
        // y no se hace directamente en LateUpdate()
        foreach (Steering kinetic in this.kinetics)
            ApplySteering(kinetic);
    }

    void ApplySteering(Steering steer) // applySteering(kinetic*)
    { // Newton > Arbitro > Actuador
        this.acceleration = Vector3.zero;
        this.velocity = steer.linear;
        this.rotation = steer.angular;

        this.position += this.velocity * Time.deltaTime; // Fórmulas de Newton
        this.orientation += this.rotation * Time.deltaTime;

        // Pasar los valores position y orientation a Unity. Por ejemplo
        transform.rotation = new Quaternion(); // Quaternion.identity;
        transform.Rotate(Vector3.up, orientation);
    }

}
