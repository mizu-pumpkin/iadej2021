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
    public CombatSystem.TerrainType terrain => pathFinder.WhereAmI(this);

    public Color colorOriginal;
    public Color colorAttack = Color.yellow;
    public Color colorHeal = Color.blue;
    public Color colorDamage = Color.red;
    private bool damaged; // para cambiar el color después de ser herido

    public Color color {
        get { return this.gameObject.GetComponent<Renderer>().material.color; }
        set { this.gameObject.GetComponent<Renderer>().material.color = value; }
    }
    
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
        maxAcceleration = maxSpeed * 8;

        attackSpeed = CombatSystem.AtkSpeed[(int)unitType];
        attackRange = CombatSystem.AtkRange[(int)unitType];

        interiorRadius = gameObject.transform.localScale.x;
        exteriorRadius = interiorRadius * 2;

        maxHP = CombatSystem.MaxHP[(int)unitType];
        hp = maxHP;
        healSpeed = CombatSystem.HealSpeed;
        healPower = CombatSystem.HealPower;

        influence = CombatSystem.Influence[(int)unitType];
        effectRadius = CombatSystem.Radius[(int)unitType];

        colorOriginal = this.color;

        patrolPath = new List<Vector3>();
        //RightPoint(10);
        //LeftPoint(10);
        
        ResetPath();
    }

    private void RightPoint(int n)
    {
        for (int i=0; i<n; i++)
            patrolPath.Add(initialPosition + Vector3.right * 40);
    }

    private void LeftPoint(int n)
    {
        for (int i=0; i<n; i++)
            patrolPath.Add(initialPosition + Vector3.left * 40);
    }

    private void ResetPath(List<Vector3> path = null, PathFollowing.Mode mode = PathFollowing.Mode.stay)
    {
        if (pathFollowing == null)
            pathFollowing = new GameObject("PathFollowing").AddComponent<PathFollowing>();
        pathFollowing.path = path;
        pathFollowing.currentNode = 0;
        pathFollowing.pathDir = 1;
        pathFollowing.mode = mode;
        NewTarget(pathFollowing);
    }

    public override void Update()
    {
        if (!GameManager.CheckVictory()) {
            if (canMove)
                base.Update();
            ChooseAction();
            ExecuteAction();
            if (damaged)
            {
                damaged = false;
                color = colorOriginal;
            }
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
                        // if (state != State.routed) Move(enemyBasePosition);
                        // If team is 0, move to enemy base 1
                        if (team == 0 && terrain != CombatSystem.TerrainType.BaseB)
                            Move(enemyBasePosition, CombatSystem.TerrainType.BaseB);//, State.none);
                        // If team is 1, move to enemy base 0
                        if (team == 1 && terrain != CombatSystem.TerrainType.BaseA)
                            Move(enemyBasePosition, CombatSystem.TerrainType.BaseA);//, State.none);
                        break;
                    case GameManager.StrategyMode.DEFEND:
                        // Go back to defend team base
                        if (state != State.defending)
                        {
                            // If team is 0, move to base 0
                            if (team == 0 && terrain != CombatSystem.TerrainType.BaseA)
                                Move(teamBasePosition, CombatSystem.TerrainType.BaseA);
                            // If team is 1, move to base 1
                            else if (team == 1 && terrain != CombatSystem.TerrainType.BaseB)
                                Move(teamBasePosition, CombatSystem.TerrainType.BaseB);
                            // If it's already in the base, defend it
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
                color = colorOriginal;
                InitializeSteerings(); // HACK
                // Si ha terminado de hacer respawn, vuelve a la posición donde murió
                if (state == State.dying) {
                    Move(deathPosition, CombatSystem.TerrainType.Unknown, State.none);
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
            damaged = true;
            color = colorDamage;
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
        FindPathToTarget(enemyUnit.position);
    }
    
    public void Defend()
    {
        action = new Defend(this);
        state = State.defending;
        ResetPath();
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
        FindPathToTarget(healingZonePosition);
    }
    
    //public void Hide(Vector3 targetPoint)
    //{
    //    action = new TakeCover(this, targetPoint);
    //    state = State.fleeing;
    //    FindPathToTarget(targetPoint);
    //}

    public void Move(Vector3 targetPoint,
                     CombatSystem.TerrainType terrain = CombatSystem.TerrainType.Unknown,
                     State state = State.routed)
    {
        action = new Move(this, targetPoint, terrain);
        this.state = state;
        FindPathToTarget(targetPoint);
    }
    
    public void Patrol()
    {
        action = new Patrol(this);
        state = State.patrolling;
        ResetPath(patrolPath, PathFollowing.Mode.patrol);
    }

    // AUX

    private void FindPathToTarget(Vector3 target)
    {
        List<Vector3> path = pathFinder.FindPathA_star(this, target);
        ResetPath(path);
    }

    /*
        █▀▀ █ ▀█ █▀▄▀█ █▀█ █▀
        █▄█ █ █▄ █░▀░█ █▄█ ▄█
    */

    public void OnDrawGizmos()
    {
        string text = unitType.ToString()+" / "+terrain.ToString();
        text += "\nHP: "+hp.ToString();
        text += "\nstate: "+state.ToString();
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
                            FindPathToTarget(hit.transform.position);
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


    // public void AccionPatrulla()
    // {
    //     AgentUnit enemy = FindEnemyNear();
    //     if (enemy != null)  //Ha detectado a alguien, va a por él
    //     {
    //         if ((nodoActual == null) || (target.position != targetPosicion))
    //         {
    //             FindPathToTarget(transform.position, target.position, mapaCostes);
    //         }
    //         NewMethod();
    //     }
    //     else  //No ha detectado a nadie, patrulla
    //     {
    //         if (nodoActual == null) //Inicio de la patrulla
    //         {
    //             //Debug.Log("kwd " + name);
    //             FindPathToTarget(transform.position, caminoPatrulla[numCaminoPatrulla].vPosition, mapaCostes);
    //         }
    //         NewMethod1();
    //     }
    // }

    // private static void FindPathToTarget(Vector3 start, Vector3 end, float[,] mapaCostes)
    // {
    //     path = controlador.FindPath(start, end, mapaCostes);
    //     if (path.Count > 0)
    //     {
    //         nodoActual = path[0];
    //         numeroCamino = 0;
    //         camino = true;
    //         targetPosicion = end;
    //     }
    //     else
    //     {
    //         camino = false;
    //     }
    // }

    // private static void NewMethod()
    // {
    //     if (camino)
    //     {
    //         //Ahora nos movemos por el camino
    //         Node actual = controlador.GridReference.NodeFromWorldPoint(transform.position);
    //         if (actual == nodoActual) //Avanzo al siguiente nodo del camino
    //         {
    //             numeroCamino++;
    //             if (numeroCamino == path.Count)
    //             {
    //                 //Quedate quieto porque has llegado al final del camino
    //                 transform.position = transform.position;
    //                 llego = true;
    //                 //Debug.Log(actitud);
    //                 StartCoroutine(buscarNuevoObjetivo());
    //             }
    //             else
    //             {
    //                 nodoActual = path[numeroCamino];
    //             }
    //         }
    //         if (numeroCamino < path.Count)
    //         {
    //             //Muevete al siguiente nodo
    //             Vector3 direccion = nodoActual.vPosition - transform.position;
    //             direccion.Normalize();
    //             //Debug.Log(transform.position + " " + direccion + " " + mapaCostes[nodoActual.iGridX, nodoActual.iGridY]);
    //             transform.position = transform.position + direccion * vel * Time.deltaTime / mapaCostes[nodoActual.iGridX, nodoActual.iGridY];
    //         }
    //     }
    // }

    // private static void NewMethod1()
    // {
    //     if (camino)
    //     {
    //         //Ahora nos movemos por el camino
    //         Node actual = controlador.GridReference.NodeFromWorldPoint(transform.position);
    //         if (actual == nodoActual) //Avanzo al siguiente nodo del camino
    //         {
    //             numeroCamino++;
    //             if (numeroCamino == path.Count)
    //             {
    //                 //Quedate quieto porque has llegado al final del camino
    //                 transform.position = transform.position;
    //                 nodoActual = null;
    //                 numCaminoPatrulla++;
    //                 if (numCaminoPatrulla == caminoPatrulla.Length) numCaminoPatrulla = 0; //Si he llegado al final, repetimos
    //             }
    //             else
    //             {
    //                 nodoActual = path[numeroCamino];
    //             }
    //         }
    //         if (numeroCamino < path.Count)
    //         {
    //             //Muevete al siguiente nodo
    //             Vector3 direccion = nodoActual.vPosition - transform.position;
    //             direccion.Normalize();
    //             //Debug.Log(transform.position + " " + direccion + " " + mapaCostes[nodoActual.iGridX, nodoActual.iGridY]);
    //             transform.position = transform.position + direccion * vel * Time.deltaTime / mapaCostes[nodoActual.iGridX, nodoActual.iGridY];
    //         }
    //     }
    // }

    // public void cambioActitud() // TODO: lo llama el A*
    // {
    //     target = controlador.buscarNuevoObjetivo(this, mapaCostes);
    //     cambio = true;
    // }

}