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
    SteeringBehaviour[] steeringBehaviours;
    List<Steering> steerings = new List<Steering>();
    // ???: blendWeight/blendPriority

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Usa GetComponents<>() para cargar todas las componentes
    // SteeringBehavior que tenga el personaje
    void Awake()
    {
        this.steeringBehaviours = this.GetComponents<SteeringBehaviour>();
    }

    // Recorre la lista construida en Awake() y calcula cada uno
    // de los Steering que calcula cada SteeringBehaviour
    // TODO: En este punto puedes aplicar un árbitro o dejarlo para el método Update()
    void LateUpdate()
    {
        steerings = new List<Steering>();
        foreach (SteeringBehaviour str in this.steeringBehaviours)
            steerings.Add(str.GetSteering(this));
    }

    // En el método Update() se invocará, al menos, al método ApplySteering()
    void Update()
    {
        BuildProperties(); // HACK
        foreach (Steering kinetic in this.steerings) {
            ApplySteering(kinetic);
            Move();
        }
    }

    void ApplySteering(Steering steer)
    {
        //this.acceleration = Vector3.zero;
        this.velocity += steer.linear * Time.deltaTime;
        //this.rotation += steer.angular * Time.deltaTime;
        this.rotation = steer.angular;

        if (this.velocity.magnitude > this.maxSpeed)
        {
            this.velocity.Normalize();
            this.velocity = this.velocity * this.maxSpeed;
        }

        if (steer.angular == 0.0f)
            this.rotation = 0.0f;
        
        if (steer.linear.sqrMagnitude == 0.0f)
            this.velocity = Vector3.zero;
    }

    void Move()
    {
        // Fórmulas de Newton
        this.position += this.velocity * Time.deltaTime;
        this.orientation += this.rotation * Time.deltaTime;

        if (this.orientation < 0.0f)
            this.orientation += 360.0f;
        else if (this.orientation > 360.0f)
            this.orientation -= 360.0f;

        // Pasar los valores position y orientation a Unity. Por ejemplo
        transform.rotation = new Quaternion(); // Quaternion.identity;
        transform.Rotate(Vector3.up, this.orientation);
    }

}