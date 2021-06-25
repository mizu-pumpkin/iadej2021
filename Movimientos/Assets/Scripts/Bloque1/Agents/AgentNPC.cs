﻿using System.Collections;
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
    public List<SteeringBehaviour> steeringBehaviours;
    public List<Steering> steerings = new List<Steering>();
    
    public PathFinding pathFinding;
    public int heuristic = 1;
    //public List<Vector3> path = new List<Vector3>();
    
    // ???: blendWeight/blendPriority

    Agent pointedTarget, target;
    Arrive arrive;
    Align align;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public void CreatePointedTarget()
    {
        pointedTarget = new GameObject("PointedTarget").AddComponent<Agent>();
        pointedTarget.interiorRadius = 0.1f;
        pointedTarget.exteriorRadius = 2;
        pointedTarget.debug = true;
    }

    public void ChangeTarget(Agent npc)
    {
        target = npc;

        arrive.target = target;
        arrive.targetRadius = target.interiorRadius;
        arrive.slowRadius = target.exteriorRadius;

        align.target = target;
    }

    public void SetTarget(object[] args)
    { // Para poder usar SendMessage() desde UnitsController
        SetTarget((Vector3) args[0], (float) args[1]);
    }

    public virtual void SetTarget(Vector3 position)
    {
        SetTarget(position, Utils.PositionToAngle(position));
    }
    
    public virtual void SetTarget(Vector3 position, float orientation)
    {   
        if (pointedTarget == null)
            CreatePointedTarget();

        pointedTarget.position = position;
        pointedTarget.Orientation = orientation;
        SetTarget(pointedTarget);
    }

    public void SetTarget(Agent npc)
    {
        ChangeTarget(npc);
        if (!steeringBehaviours.Contains(arrive)) {
            NewTarget(arrive);
            AddTarget(align);
        }
    }
    
    // Vacía la lista de SteeringBehaviour del NPC, que ahora solo tendrá sb
    public virtual void NewTarget(SteeringBehaviour sb) {
        this.steeringBehaviours = new List<SteeringBehaviour>();
        this.steeringBehaviours.Add(sb);
    }

    // Si no quiero vaciar la lista pero quiero añadirle un nuevo sb
    public virtual void AddTarget(SteeringBehaviour sb) {
        if (this.steeringBehaviours == null)
            this.steeringBehaviours = new List<SteeringBehaviour>();
        this.steeringBehaviours.Add(sb);
    }

    // Para quitar un sb de la lista del NPC
    public virtual void RemoveTarget(SteeringBehaviour sb) {
        if (this.steeringBehaviours.Contains(sb))
            this.steeringBehaviours.Remove(sb);
    }

    // Si quiero resetear los sb para que vuelvan a ser los del principio
    public virtual void InitializeSteerings() {
        this.steeringBehaviours = new List<SteeringBehaviour>(this.GetComponents<SteeringBehaviour>());
    }

    // Usa GetComponents<>() para cargar todas las componentes
    // SteeringBehavior que tenga el personaje
    public override void Awake()
    {
        base.Awake();
        InitializeSteerings();
        arrive = new GameObject("TargetArrive").AddComponent<Arrive>();
        align = new GameObject("TargetAlign").AddComponent<Align>();
    }

    protected virtual void OnDestroy()
    {
        if (arrive != null)
            Destroy(arrive.gameObject);
        if (align != null)
            Destroy(align.gameObject);
    }

    protected virtual void FindPath()
    {
        if (Input.GetKey(KeyCode.Keypad1))
            heuristic = 1;
        if (Input.GetKey(KeyCode.Keypad2))
            heuristic = 2;
        if (Input.GetKey(KeyCode.Keypad3))
            heuristic = 3;
        
        if (Input.GetMouseButtonDown(0))
            pathFinding.LRTA_star(this);
    }

    // Recorre la lista construida en Awake() y calcula cada uno
    // de los Steering que calcula cada SteeringBehaviour
    // TODO: En este punto puedes aplicar un árbitro o dejarlo para el método Update()
    public virtual void LateUpdate()
    {
        if (pathFinding != null)
            FindPath();

        steerings = new List<Steering>();
        foreach (SteeringBehaviour sb in this.steeringBehaviours) {
            Steering steer = sb.GetSteering(this);
            if (steer == null) continue;

            // HACK: para que no salga de las paredes
            // Funciona el 90% del tiempo :D
            if (sb is WallAvoid) {
                steerings = new List<Steering>();
                steerings.Add(steer);
                break;
            }

            steerings.Add(steer);
        }
    }

    // En el método Update() se invocará, al menos, al método ApplySteering()
    public virtual void Update()
    {
        foreach (Steering steer in this.steerings)
            ApplySteering(steer);
        
        if (steeringBehaviours.Contains(arrive) && arrive.target == null) {
            ChangeTarget(pointedTarget);
        }
    }

    // TODO: Modifica el método para que la aceleración lineal del Steering se
    // interprete como una fuerza. En esta caso la aceleración vendrá dada por
    // a = F/masa. Ejecuta el programa cambiando en tiempo de ejecución la masa.
    public virtual void ApplySteering(Steering steer)
    {
        // FIXME: this.acceleration = steer.linear / this.mass;
        // FIXME: this.velocity += this.acceleration * Time.deltaTime;
        this.velocity += steer.linear * Time.deltaTime;
        //this.rotation += steer.angular;
        this.rotation += steer.angular * Time.deltaTime;
        
        // ???: necesito esto?
        if (this.rotation > this.maxRotation)
            this.rotation = this.maxRotation;

        if (this.velocity.magnitude > this.maxSpeed)
            this.velocity = this.velocity.normalized * this.maxSpeed;

        if (steer.linear.sqrMagnitude == 0 && steer.angular == 0) {
            //this.acceleration = Vector3.zero;
            this.velocity = Vector3.zero;
            this.rotation = 0;
        }

        this.velocity.y = 0; // HACK

        // Fórmulas de Newton
        if (steer.linear.sqrMagnitude != 0)
            this.position += this.velocity * Time.deltaTime;
        if (steer.angular != 0)
            this.orientation += this.rotation * Time.deltaTime;

        // Limitar la orientación al rango 0~360
        this.orientation = Utils.rango360(this.orientation);

        // Pasar los valores position y orientation a Unity
        transform.rotation = new Quaternion(); // Quaternion.identity;
        transform.Rotate(Vector3.up, this.orientation);
    }

}