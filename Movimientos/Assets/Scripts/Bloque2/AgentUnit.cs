using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor; // Handles

public class AgentUnit : AgentNPC
{
    public AStar controlador;

    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
    */
    
    // Estado en el que se encuentra la unidad
    public enum State { none, attacking, defending, fleeing, dying, healing, routed };
    [SerializeField] private State state;

    // Modo de estrategia del equipo
    public enum StrategyMode { NEUTRAL, ATTACK, DEFEND, TOTALWAR }; // FIXME: ATTACK == TOTALWAR ?
    [SerializeField] private StrategyMode strategyMode;

    // Posiciones de interés para PathFollowing
    [SerializeField] private PathFollowing pathFollowing;
    [SerializeField] private List<Vector3> patrolPath;
    public Vector3 teamBasePosition;
    [SerializeField] private Vector3 enemyBasePosition;
    [SerializeField] private Vector3 healingZonePosition;
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 deathPosition;

    // Atributos de la unidad
    public CombatSystem.UnitType unitType;
    [SerializeField] private int team;
    public float influence; // the intrinsic military power of the unit
    public float effectRadius; // the radius of effect of the influence
    public float attackRange;
    public float attackSpeed;
    public float healStrength;
    [SerializeField] private float maxHP;
    [SerializeField] private float hp;

    // La acción a ejecutar por la unidad
    [SerializeField] private Action action = null;
    // Indica si la unidad puede moverse (no puede cuando ataca/cura)
    public bool canMove = true;
    // El tipo de terreno en el que se encuentra la unidad
    public CombatSystem.TerrainType terrain;
    
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
    */
    
    public override void Awake()
    {
        base.Awake();
        InitializeUnit();
    }

    private void InitializeUnit()
    {
        maxSpeed = CombatSystem.MaxSpeed[(int)unitType];
        maxAcceleration = CombatSystem.MaxAcceleration[(int)unitType];

        attackRange = CombatSystem.AtkRange[(int)unitType];
        exteriorRadius = attackRange * 2;
        interiorRadius = gameObject.transform.localScale.x;

        attackSpeed = CombatSystem.AtkSpeed[(int)unitType];
        healStrength = CombatSystem.HealStrength[(int)unitType];

        maxHP = CombatSystem.MaxHP[(int)unitType];
        hp = maxHP;

        influence = CombatSystem.Influence[(int)unitType];
        effectRadius = CombatSystem.Radius[(int)unitType];
    }

    protected override void FindPath()
    {
        if (Input.GetKey(KeyCode.Keypad1))
            heuristic = 1;
        if (Input.GetKey(KeyCode.Keypad2))
            heuristic = 2;
        if (Input.GetKey(KeyCode.Keypad3))
            heuristic = 3;
        
        if (Input.GetMouseButtonDown(0))
            pathFinding.A_star(this);
    }

    public override void Update()
    {
        if (canMove)
            base.Update();
        ExecuteAction();
    }

    public void OnDrawGizmos()
    {
        string text = unitType.ToString()+" / "+terrain.ToString();
        text += "\nHP: "+hp.ToString();
        if (action != null)
            text += "\n"+action.ToString();
        
        Handles.Label(transform.position, text);

        // Draw a yellow sphere at the transform's position
        // if (disparoLejano) {
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawSphere(transform.position, 40);
        // }
    }

    /*
        ▄▀█ █▀▀ ▀█▀ █ █▀█ █▄░█ █▀
        █▀█ █▄▄ ░█░ █ █▄█ █░▀█ ▄█
    */
    public void ExecuteAction()
    {
        if (action != null)
        {
            action.Execute();
            if (action.IsComplete()) {
                action = null;
                canMove = true;
                InitializeSteerings(); // HACK
                if (state == State.dying) SetTarget(deathPosition, 0); // HACK
                state = State.none;
            }
        }
    }

    public void Attack(AgentUnit enemyUnit)
    {
        action = new Attack(this, enemyUnit);
        state = State.attacking;

        SetTarget(enemyUnit); // HACK
    }
    
    public void Defend()
    {
        action = new Defend(this, teamBasePosition);
        state = State.defending;

        List<Vector3> path = controlador.FindPath(this, teamBasePosition);
        FollowPathAndStop(path);
    }

    public void Die()
    {
        action = new Die(this);
        state = State.dying;

        // Respawn
        hp = maxHP;//InitializeUnit();
        deathPosition = this.position;
        this.position = teamBasePosition;
    }
    
    public void Heal(AgentUnit allyUnit)
    {
        if (healStrength > 0) {
            action = new Heal(this, allyUnit);
            state = State.healing;

            SetTarget(allyUnit); // HACK
        }
    }
    
    public void Hide(Vector3 targetPoint)
    {
        action = new TakeCover(this, targetPoint);
        state = State.fleeing;

        List<Vector3> path = controlador.FindPath(this, targetPoint);
        FollowPathAndStop(path);
    }

    public void Move(Vector3 targetPoint)
    {
        action = new Move(this, targetPoint);
        state = State.routed;

        List<Vector3> path = controlador.FindPath(this, targetPoint);
        FollowPathAndStop(path);
    }
    
    public void Patrol()
    {
        action = new Patrol(this);
        state = State.none;
        
        pathFollowing = new GameObject("PathFollowing").AddComponent<PathFollowing>();
        pathFollowing.path = patrolPath;
        pathFollowing.mode = PathFollowing.Mode.patrol;
        NewTarget(pathFollowing);
    }
    
    public void RunAway()
    {
        action = new RunAway(this, healingZonePosition);
        state = State.fleeing;

        List<Vector3> path = controlador.FindPath(this, healingZonePosition);
        FollowPathAndStop(path);
    }

    public void FollowPathAndStop(List<Vector3> route)
    {
        // TODO: borrar del Agent el PathFollowing anterior
        pathFollowing = new GameObject("PathFollowing").AddComponent<PathFollowing>();
        pathFollowing.path = route;
        pathFollowing.mode = PathFollowing.Mode.stop;
        NewTarget(pathFollowing);
    }

    public bool TakeDamage(int damage, AgentUnit attacker)
    {
        hp -= damage;
        // If health is 0, die
        if (hp <= 0) {
            Die();
            return true;
        }
        // If attacked, respond to attack
        else if (state != State.attacking) {
            Attack(attacker);
        }
        // If health is low and speed is high, run away
        else if (hp < (maxHP / 2) && hp < attacker.hp && maxSpeed >= attacker.maxSpeed && state != State.fleeing) {
            RunAway();
        }
        return false;
    }

    public bool TakeHeal(float heal)
    {
        hp += heal;
        if (hp >= maxHP) {
            hp = maxHP;
            return true;
        }
        return false;
    }

    /*
        █▀ ▀█▀ █▀█ ▄▀█ ▀█▀ █▀▀ █▀▀ █▄█
        ▄█ ░█░ █▀▄ █▀█ ░█░ ██▄ █▄█ ░█░
    */

    void Strategy()
    {
        if (state == State.none)
        {
            AgentUnit enemy = controlador.FindEnemy(this);
            // If there is an enemy in sight, attack it
            if (enemy != null) {
                Attack(enemy);
            }
            // If there is no enemy near, do what the strategy demands
            else {
                switch (strategyMode)
                {
                    case StrategyMode.ATTACK:
                    case StrategyMode.TOTALWAR:
                        // Find path to enemy base
                        Move(enemyBasePosition);
                        break;
                    case StrategyMode.DEFEND:
                        Defend();
                        break;
                    default:
                        Patrol();
                        break;
                }
            }
        }
    }



    // public Material materialOriginal;
    // public Material materialElegido;
    // //Renderer renderer;

    // public LayerMask hitLayers;
    // public Transform target;
    // Vector3 targetPosicion; //Variable para si el objetivo se ha movido, cambiar el pathfinding

    // bool llego; //Variable para que se quede quieto
    // bool camino;
    // bool seleccionado;
    // bool quieto; //Si el NPC está atacando, se queda quieto
    // bool finJuego;
    // bool esperando;
    // bool muerto;  //Esta variable nos serivirá para si estamos muertos y nos habian detectados antes de morir, no quite vida
    // bool curando; //Esta variable nos servirá para saber cuando nos hemos curado y tenemos que espera un tiempo para volver a curarnos
    // public bool cambio;  //Esta variable me servirá para saber si se ha producido cambio de objetivo/recorrido, cambiar el camino (path)
    // public bool patrulla;
    // bool pausarPatrulla;
    
    // List<Node> path;
    // Node nodoActual; //Nodo actual del camino al que sigo
    // string tagZona; //Esta variable la usaremos cuando estemos quietos para saber donde estamos

    // float[,] mapaCostes;

    // void Update2() // Update
    // {
    //     if ((!esperando) && (!finJuego))
    //     {
    //         comprobarVictoria();
    //         StartCoroutine(comprobarVida());
    //         if (seleccionado) buscarNuevoTarget();
    //         StartCoroutine(detectarObjetivo());
            
    //         if (!quieto)
    //         {
    //             if (!cambio)
    //             {
    //                 if (!llego && patrulla)
    //                     AccionPatrulla();
    //             }
    //             else
    //             {
    //                 nodoActual = null;
    //                 Node n = controlador.GridReference.NodeFromWorldPoint(transform.position);
    //                 tagZona = controlador.getTagNode(n);
    //                 cambio = false;
    //                 llego = false;
    //                 camino = false; 
    //             }
    //         }
    //     }
    // }

    // void comprobarVictoria()
    // {
    //     string tagBase = (nodoActual == null) ? tagZona : controlador.getTagNode(nodoActual);
    //     int baseTeam = (tagBase == "BaseRoja") ? 0 : 1;

    //     if (team != baseTeam)
    //         controlador.ganar(team.ToString());
    // }

    // private void OnMouseOver()
    // {
    //     if ((!controlador.ocupadoSeleccionando) && (!finJuego))//Con esto evitamos que más de un NPC esté seleccionado
    //     {
    //         if (!esperando)
    //         {
    //             if (!patrulla)  //Si es una patrulla, no podrá ser seleccionado
    //             {
    //                 if (Input.GetMouseButtonDown(0))
    //                 {
    //                     //renderer.sharedMaterial = materialElegido;
    //                     controlador.ocupadoSeleccionando = true;
    //                     seleccionado = true;
    //                 }
    //             }
    //         }
    //     }
    // }
    
    // void buscarNuevoTarget()
    // {
    //     if (Input.GetMouseButtonDown(0)) // Left click
    //     {
    //         Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
    //         RaycastHit hit;
    //         if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers)) //Si el raycast no golpea en una pared o en el agua
    //         {
    //             if (hit.transform != this.transform) //Para evitar ser autoseleccionado
    //             {
    //                 target = hit.transform;
    //                 cambio = true;
    //                 // renderer.sharedMaterial = materialOriginal;
    //                 controlador.ocupadoSeleccionando = false;
    //                 seleccionado = false;
    //             }
    //         }
    //     }
    //     if (Input.GetMouseButtonDown(1)) // Right click
    //     {
    //         // renderer.sharedMaterial = materialOriginal;
    //         controlador.ocupadoSeleccionando = false;
    //         seleccionado = false;
    //     }
    // }

    // IEnumerator detectarObjetivo()
    // {
    //     // Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
    //     // int i = 0;
    //     // bool detectado = false;
    //     // foreach (Collider collider in hitColliders)
    //     // {
    //     //     if (team != collider.team)
    //     //     {
    //     //         NPC enemigo = controlador.devolverEnemigo(collider.transform);
    //     //         if (enemigo.vida > 0)
    //     //         {
    //     //             quieto = true;
    //     //             atacar(enemigo);
    //     //             esperando = true;
    //     //             detectado = true;
    //     //             if (seleccionado)
    //     //             {
    //     //                 renderer.sharedMaterial = materialOriginal;
    //     //                 controlador.ocupadoSeleccionando = false;
    //     //                 seleccionado = false;
    //     //             }
    //     //             renderer.sharedMaterial = materialElegido;
    //     //             yield return new WaitForSeconds(2);
    //     //             renderer.sharedMaterial = materialOriginal;
    //     //             esperando = false;
    //     //         }
    //     //     }
    //     // }
    //     // if (!detectado) quieto = false; //Si no se ha detectado a nadie, entonces por si se estaba quieto, que ya no lo este
    // }

    // void detectarEnemigoPatrulla()
    // {
    //     // Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
    //     // int i = 0;
    //     // bool detectado = false;
    //     // foreach (Collider collider in hitColliders)
    //     // {
    //     //     if (team != collider.team)
    //     //     {
    //     //         detectado = true;
    //     //         NPC enemigo = controlador.devolverEnemigo(collider.transform);
    //     //         if (target != enemigo.transform)
    //     //         {
    //     //             target = enemigo.transform;
    //     //             targetPosicion = enemigo.transform.position;
    //     //             nodoActual = null;
    //     //         }
    //     //         pausarPatrulla = true;
    //     //         break;    //El primero que encuentre será su objetivo
    //     //     }
    //     // }
    //     // if (!detectado && pausarPatrulla)
    //     // {
    //     //     pausarPatrulla = false; //Si no se ha detectado a nadie, entonces por si se estaba quieto, que ya no lo este
    //     //     nodoActual = null;
    //     //     numCaminoPatrulla = 0;
    //     // }
    // }

    // public void AccionPatrulla()
    // {
    //     // detectarEnemigoPatrulla();
    //     // if (pausarPatrulla)  //Ha detectado a alguien, va a por él
    //     // {
    //     //     if ((nodoActual == null) || (target.position != targetPosicion))
    //     //     {
    //     //         path = controlador.FindPath(transform.position, target.position, mapaCostes);//Find a path to the target
    //     //         if (path.Count > 0)
    //     //         {
    //     //             nodoActual = path[0];
    //     //             numeroCamino = 0;
    //     //             camino = true;
    //     //             targetPosicion = target.position;
    //     //         }
    //     //         else
    //     //         {
    //     //             camino = false;
    //     //         }
    //     //     }
    //     //     if (camino)
    //     //     {
    //     //         //Ahora nos movemos por el camino
    //     //         Node actual = controlador.GridReference.NodeFromWorldPoint(transform.position);
    //     //         if (actual == nodoActual) //Avanzo al siguiente nodo del camino
    //     //         {
    //     //             numeroCamino++;
    //     //             if (numeroCamino == path.Count)
    //     //             {
    //     //                 //Quedate quieto porque has llegado al final del camino
    //     //                 transform.position = transform.position;
    //     //                 llego = true;
    //     //                 //Debug.Log(actitud);
    //     //                 StartCoroutine(buscarNuevoObjetivo());
    //     //             }
    //     //             else
    //     //             {
    //     //                 nodoActual = path[numeroCamino];
    //     //             }
    //     //         }
    //     //         if (numeroCamino < path.Count)
    //     //         {
    //     //             //Muevete al siguiente nodo
    //     //             Vector3 direccion = nodoActual.vPosition - transform.position;
    //     //             direccion.Normalize();
    //     //             //Debug.Log(transform.position + " " + direccion + " " + mapaCostes[nodoActual.iGridX, nodoActual.iGridY]);
    //     //             transform.position = transform.position + direccion * vel * Time.deltaTime / mapaCostes[nodoActual.iGridX, nodoActual.iGridY];
    //     //         }
    //     //     }
    //     // }
    //     // else  //No ha detectado a nadie, patrulla
    //     // {
    //     //     if (nodoActual == null) //Inicio de la patrulla
    //     //     {
    //     //         //Debug.Log("kwd " + name);
    //     //         path = controlador.FindPath(transform.position, caminoPatrulla[numCaminoPatrulla].vPosition, mapaCostes);//Find a path to the target
    //     //         if (path.Count > 0)
    //     //         {
    //     //             nodoActual = path[0];
    //     //             numeroCamino = 0;
    //     //             camino = true;
    //     //             targetPosicion = caminoPatrulla[numCaminoPatrulla].vPosition;
    //     //         }
    //     //         else
    //     //         {
    //     //             camino = false;
    //     //         }
    //     //     }
    //     //     if (camino)
    //     //     {
    //     //         //Ahora nos movemos por el camino
    //     //         Node actual = controlador.GridReference.NodeFromWorldPoint(transform.position);
    //     //         if (actual == nodoActual) //Avanzo al siguiente nodo del camino
    //     //         {
    //     //             numeroCamino++;
    //     //             if (numeroCamino == path.Count)
    //     //             {
    //     //                 //Quedate quieto porque has llegado al final del camino
    //     //                 transform.position = transform.position;
    //     //                 nodoActual = null;
    //     //                 numCaminoPatrulla++;
    //     //                 if (numCaminoPatrulla == caminoPatrulla.Length) numCaminoPatrulla = 0; //Si he llegado al final, repetimos
    //     //             }
    //     //             else
    //     //             {
    //     //                 nodoActual = path[numeroCamino];
    //     //             }
    //     //         }
    //     //         if (numeroCamino < path.Count)
    //     //         {
    //     //             //Muevete al siguiente nodo
    //     //             Vector3 direccion = nodoActual.vPosition - transform.position;
    //     //             direccion.Normalize();
    //     //             //Debug.Log(transform.position + " " + direccion + " " + mapaCostes[nodoActual.iGridX, nodoActual.iGridY]);
    //     //             transform.position = transform.position + direccion * vel * Time.deltaTime / mapaCostes[nodoActual.iGridX, nodoActual.iGridY];
    //     //         }
    //     //     }
    //     // }
    // }

    // IEnumerator buscarNuevoObjetivo()
    // {
    //     yield return new WaitForSeconds(4);
    //     //Buscar nuevo objetivo
    //     target = controlador.buscarNuevoObjetivo(this, mapaCostes);
    //     cambio = true;
    // }
    
    // public void cambioActitud() // TODO: lo llama el A*
    // {
    //     target = controlador.buscarNuevoObjetivo(this, mapaCostes);
    //     cambio = true;
    // }

    // public void quedarseQuietoFinJuego()
    // {
    //     quieto = true;
    //     finJuego = true;
    // }

    // IEnumerator comprobarVida()
    // {
    //     // if (vida <= 0)
    //     // {
    //     //     Transform m = new GameObject().transform;
    //     //     m.position = transform.position;
    //     //     target = m; //Su objetivo será ir a donde a muerto
    //     //     controlador.moverHaciaSpawn(this);
    //     //     cambio = false;
    //     //     llego = false;
    //     //     camino = true;
    //     //     seleccionado = false;
    //     //     quieto = false;
    //     //     muerto = true;
    //     //     esperando = true;
    //     //     vida = vidaInicial;
    //     //     healthBar.fillAmount = 1;
    //     //     yield return new WaitForSeconds(4);
    //     //     esperando = false;
    //     //     muerto = false;
    //     // }
    //     // else
    //     // {
    //     //     if (vida / vidaInicial <= 0.15f) //Que su objetivo sea una zona segura para curarse
    //     //     {
    //     //         if ((actitud == 2) && (!quieto) && ((target.tag != "ZonaSeguraAzul") || (target.tag != "ZonaSeguraRojo")))
    //     //         {
    //     //             Debug.Log("Soy " + name + " y voy a una zona segura");
    //     //             target = controlador.buscarZonaSegura(this, mapaCostes);
    //     //         }
    //     //     }
    //     //     if (!curando)
    //     //     {
    //     //         if (team == "ROJO")
    //     //         {
    //     //             if (nodoActual != null)
    //     //             {
    //     //                 if (controlador.getTagNode(nodoActual) == "ZonaSeguraRojo")
    //     //                 {
    //     //                     vida += 20;
    //     //                     if (vida > vidaInicial) vida = vidaInicial;
    //     //                     healthBar.fillAmount = vida / vidaInicial;
    //     //                     curando = true;
    //     //                     yield return new WaitForSeconds(2);
    //     //                     curando = false;
    //     //                 }
    //     //             }
    //     //             else
    //     //             {
    //     //                 if (tagZona == "ZonaSeguraRojo")
    //     //                 {
    //     //                     vida += 20;
    //     //                     if (vida > vidaInicial) vida = vidaInicial;
    //     //                     healthBar.fillAmount = vida / vidaInicial;
    //     //                     curando = true;
    //     //                     yield return new WaitForSeconds(2);
    //     //                     curando = false;
    //     //                 }
    //     //             }

    //     //         }
    //     //         else
    //     //         {
    //     //             if (nodoActual != null)
    //     //             {
    //     //                 if (controlador.getTagNode(nodoActual) == "ZonaSeguraAzul")
    //     //                 {
    //     //                     vida += 20;
    //     //                     if (vida > vidaInicial) vida = vidaInicial;
    //     //                     healthBar.fillAmount = vida / vidaInicial;
    //     //                     curando = true;
    //     //                     yield return new WaitForSeconds(2);
    //     //                     curando = false;
    //     //                 }
    //     //             }
    //     //             else
    //     //             {
    //     //                 if (tagZona == "ZonaSeguraAzul")
    //     //                 {
    //     //                     vida += 20;
    //     //                     if (vida > vidaInicial) vida = vidaInicial;
    //     //                     healthBar.fillAmount = vida / vidaInicial;
    //     //                     curando = true;
    //     //                     yield return new WaitForSeconds(2);
    //     //                     curando = false;
    //     //                 }
    //     //             }
    //     //         }
    //     //     }
    //     // }
    // }

}