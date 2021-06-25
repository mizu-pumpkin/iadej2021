using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor; // Handles

public class AgentUnit : AgentNPC
{
    [SerializeField] private PathFindingAStar pathFinder;

    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
    */
    
    // Estado en el que se encuentra la unidad
    public enum State { none, patrolling, attacking, defending, routed, dying, healing };
    [SerializeField] private State state;

    // Modo de estrategia del team
    public GameManager.StrategyMode strategyMode;

    // Posiciones de interés para PathFollowing
    public PathFollowing pathFollowing;
    [SerializeField] private List<Vector3> patrolPath;
    public Vector3 teamBasePosition;
    [SerializeField] private Vector3 enemyBasePosition;
    public Vector3 healingZonePosition;
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 deathPosition;

    // Atributos de la unidad
    public CombatSystem.UnitType unitType;
    public int team;
    public float influence; // the intrinsic military power of the unit
    public float effectRadius; // the radius of effect of the influence
    public float attackRange;
    public float attackSpeed;
    public float healSpeed;
    public float healPower;
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
        initialPosition = this.position;

        maxSpeed = CombatSystem.MaxSpeed[(int)unitType];
        maxAcceleration = CombatSystem.MaxAcceleration[(int)unitType];

        attackSpeed = CombatSystem.AtkSpeed[(int)unitType];
        attackRange = CombatSystem.AtkRange[(int)unitType];
        exteriorRadius = attackRange * 2;
        interiorRadius = gameObject.transform.localScale.x;

        maxHP = CombatSystem.MaxHP[(int)unitType];
        hp = maxHP;
        healSpeed = CombatSystem.HealSpeed;
        healPower = CombatSystem.HealPower;

        influence = CombatSystem.Influence[(int)unitType];
        effectRadius = CombatSystem.Radius[(int)unitType];

        // HACK
        patrolPath = new List<Vector3>();
        patrolPath.Add(initialPosition);
        patrolPath.Add(teamBasePosition);
        patrolPath.Add(healingZonePosition);
    }

    public override void Update()
    {
        if (!GameManager.CheckVictory()) {
            if (canMove)
                base.Update();
            ChooseAction();
            ExecuteAction();
        }
    }

    /*
        █▀ ▀█▀ █▀█ ▄▀█ ▀█▀ █▀▀ █▀▀ █▄█
        ▄█ ░█░ █▀▄ █▀█ ░█░ ██▄ █▄█ ░█░
    */

    AgentUnit FindEnemyNear()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange*5);

        foreach (Collider collider in hitColliders)
        {
            AgentUnit agentUnit = collider.gameObject.GetComponent<AgentUnit>();
            if (agentUnit != null && team != agentUnit.team)
                return agentUnit;
        }

        return null;
    }

    void ChooseAction()
    {
        if (state != State.dying && state != State.healing && state != State.routed)
        {
            AgentUnit enemy = FindEnemyNear();//pathFinder.FindEnemy(this);
            // If there is an enemy in sight, attack it
            if (enemy != null) {
                if (state != State.attacking)
                    Attack(enemy);
            }
            // If there is no enemy near, do what the strategy demands
            else {
                switch (strategyMode)
                {
                    case GameManager.StrategyMode.ATTACK:
                    case GameManager.StrategyMode.TOTALWAR:
                        // Move to enemy base
                        Move(enemyBasePosition, State.none);
                        break;
                    case GameManager.StrategyMode.DEFEND:
                        // Go back to defend team base
                        if (state != State.defending)
                        {
                           float distance = (this.position - teamBasePosition).magnitude;
                           if (distance > this.exteriorRadius)
                               Move(teamBasePosition);
                           else
                               Defend();
                        }
                        break;
                    default:
                        // Patrol your zone
                        if (state != State.patrolling) {
                            float distance = (this.position - initialPosition).magnitude;
                            if (distance > this.exteriorRadius)
                                Move(initialPosition);
                            else
                                Patrol();
                        }
                        break;
                }
            }
        }
    }

    public void ExecuteAction()
    {
        if (action != null)
        {
            action.Execute();
            if (action.IsComplete()) {
                action = null;
                canMove = true;
                InitializeSteerings(); // HACK
                // Si ha terminado de hacer respawn, vuelve a la posición donde murió
                if (state == State.dying) {
                    Move(deathPosition, State.none);
                }
                else {
                    state = State.none;
                }
            }
        }
    }

    public bool TakeDamage(int damage, AgentUnit attacker)
    {
        if (damage > 0) {
            hp -= damage;
            // If health is 0, die
            if (hp <= 0) {
                print(attacker.gameObject.name+" killed "+this.gameObject.name);
                Die();
                return true;
            }
            // If attacked, respond to attack
            else if (state != State.attacking) {
                Attack(attacker);
            }
            // If health is low and speed is high, run away
            else if (hp < (maxHP / 2) && hp < attacker.hp && maxSpeed >= attacker.maxSpeed && state != State.healing) {
                print(this.gameObject.name+"'s HP is low");
                Heal();
            }
        }
        return false;
    }
    
    public bool TakeHeal(float heal)
    {
        if (heal > 0)
        {
            hp += heal;
            if (hp >= maxHP) {
                hp = maxHP;
                return true;
            }
        }
        return false;
    }

    /*
        ▄▀█ █▀▀ ▀█▀ █ █▀█ █▄░█ █▀
        █▀█ █▄▄ ░█░ █ █▄█ █░▀█ ▄█
    */

    public void Attack(AgentUnit enemyUnit)
    {
        action = new Attack(this, enemyUnit);
        state = State.attacking;

        // TODO:HACK
        //SetTarget(enemyUnit);
        FollowPathAndStop(enemyUnit.position);
    }
    
    public void Defend()
    {
        action = new Defend(this);
        state = State.defending;
        if (pathFollowing != null) Destroy(pathFollowing);
        pathFollowing = null;
    }

    public void Die()
    {
        action = new Die(this);
        state = State.dying;
        // Respawn
        deathPosition = this.position;
        this.position = teamBasePosition;
        hp = maxHP;
    }
    
    public void Heal()
    {
        action = new Heal(this);
        state = State.healing;
        FollowPathAndStop(healingZonePosition);
    }
    
    //public void Hide(Vector3 targetPoint)
    //{
    //    action = new TakeCover(this, targetPoint);
    //    state = State.fleeing;
    //    FollowPathAndStop(targetPoint);
    //}

    public void Move(Vector3 targetPoint, State s = State.routed)
    {
        action = new Move(this, targetPoint);
        state = s;
        FollowPathAndStop(targetPoint);
    }
    
    public void Patrol()
    {
        action = new Patrol(this);
        state = State.patrolling;
        
        if (pathFollowing != null) Destroy(pathFollowing);
        pathFollowing = new GameObject("PathFollowing").AddComponent<PathFollowing>();
        pathFollowing.path = patrolPath;
        pathFollowing.mode = PathFollowing.Mode.patrol;
        NewTarget(pathFollowing);
    }

    // AUX

    private void FollowPathAndStop(Vector3 target)
    {
        List<Vector3> path = pathFinder.FindPathA_star(this, target);
        // TODO: borrar del Agent el PathFollowing anterior
        if (pathFollowing != null) Destroy(pathFollowing);
        pathFollowing = new GameObject("PathFollowing").AddComponent<PathFollowing>();
        pathFollowing.path = path;
        pathFollowing.mode = PathFollowing.Mode.stay;
        NewTarget(pathFollowing);
    }

    /*
        █▀▀ █ ▀█ █▀▄▀█ █▀█ █▀
        █▄█ █ █▄ █░▀░█ █▄█ ▄█
    */

    public void OnDrawGizmos()
    {
        string text = unitType.ToString()+" / "+terrain.ToString();
        text += "\nHP: "+hp.ToString();
        if (selected)
            text += "\nSELECTED";
        if (action != null)
            text += "\n"+action.ToString();
        
        Handles.Label(transform.position, text);
    }

    private bool selected;
    public LayerMask hitLayers;

    void OnMouseOver()
    {
        // Left click
        if (Input.GetMouseButtonDown(0)) {
            selected = !selected;
        }
        // Right click
        if (Input.GetMouseButtonDown(1)) {
            selected = false;
        }
    }

    protected override void FindPath()
    {
        if (Input.GetKey(KeyCode.Keypad1))
            heuristic = 1;
        if (Input.GetKey(KeyCode.Keypad2))
            heuristic = 2;
        if (Input.GetKey(KeyCode.Keypad3))
            heuristic = 3;

        if (selected) {
            // Left click
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //Si el raycast no golpea en una pared o en el agua
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
                {
                    if (hit.transform != null && hit.transform.tag != "Wall" && hit.transform.tag != "Water")
                        if (hit.transform != this.transform)
                        {
                            FollowPathAndStop(hit.transform.position);
                            selected = false;
                        }
                }
            }
            // Right click
            if (Input.GetMouseButtonDown(1)) {
                selected = false;
            }
        }
    }


    // public Material materialOriginal;
    // public Material materialElegido;
    // //Renderer renderer;


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