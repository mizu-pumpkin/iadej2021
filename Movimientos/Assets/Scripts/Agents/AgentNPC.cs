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
    
    public void NewTarget(SteeringBehaviour sb) {
        // TODO: añadir el nuevo target y arbitrarlo
        this.steeringBehaviours = new SteeringBehaviour[] { sb }; // HACK
    }

    public void resetSteerings() {
        this.steeringBehaviours = new SteeringBehaviour[] {}; // HACK
    }

    // Usa GetComponents<>() para cargar todas las componentes
    // SteeringBehavior que tenga el personaje
    public override void Awake()
    {
        base.Awake();
        this.steeringBehaviours = this.GetComponents<SteeringBehaviour>();
    }

    // Recorre la lista construida en Awake() y calcula cada uno
    // de los Steering que calcula cada SteeringBehaviour
    // TODO: En este punto puedes aplicar un árbitro o dejarlo para el método Update()
    void LateUpdate()
    {
        steerings = new List<Steering>();
        foreach (SteeringBehaviour sb in this.steeringBehaviours)
            steerings.Add(sb.GetSteering(this));
    }

    // En el método Update() se invocará, al menos, al método ApplySteering()
    void Update()
    {
        foreach (Steering steer in this.steerings)
            ApplySteering(steer);
    }

    // TODO: Modifica el método para que la aceleración lineal del Steering se
    // interprete como una fuerza. En esta caso la aceleración vendrá dada por
    // a = F/masa. Ejecuta el programa cambiando en tiempo de ejecución la masa.
    void ApplySteering(Steering steer)
    {
        //this.acceleration = steer.linear * Time.deltaTime;
        this.velocity += steer.linear * Time.deltaTime;
        //this.rotation += steer.angular * Time.deltaTime;
        this.rotation = steer.angular;

        if (this.velocity.magnitude > this.maxSpeed)
            this.velocity = this.velocity.normalized * this.maxSpeed;

        if (steer.linear.sqrMagnitude == 0 && steer.angular == 0) {
            //this.acceleration = Vector3.zero;
            this.velocity = Vector3.zero;
            this.rotation = 0;
        }

        // Fórmulas de Newton
        if (steer.linear.sqrMagnitude > 0)
            this.position += this.velocity * Time.deltaTime;
        if (steer.angular > 0)
            this.orientation += this.rotation * Time.deltaTime;

        // Limitar la orientación al rango 0~360
        if (this.orientation < 0)
            this.orientation += 360;
        else if (this.orientation > 360)
            this.orientation -= 360;

        // Pasar los valores position y orientation a Unity
        transform.rotation = new Quaternion(); // Quaternion.identity;
        transform.Rotate(Vector3.up, this.orientation);//transform.Rotate(0, this.orientation, 0);
    }

}