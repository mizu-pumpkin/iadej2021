using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor; // Handles

public class AgentUnit : AgentNPC
{
    [SerializeField] private PathFindingAStar pathFinder;
    AgentUnit targetedEnemy;

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
    public GameObject patrolPathObject;
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
    public bool inEnemyBase {
        get {
            return team == 0 && terrain == CombatSystem.TerrainType.BaseB ||
                   team == 1 && terrain == CombatSystem.TerrainType.BaseA;
        }
    }
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

        MakePatrolPath();

        ResetPath();
    }

    private void MakePatrolPath()
    {
        patrolPath = new List<Vector3>();

        if (patrolPathObject != null)
            for (int i = 0; i < patrolPathObject.transform.childCount; i++)
                patrolPath.Add(patrolPathObject.transform.GetChild(i).position);
    }

    private void LeftRightPoint(int n, int steps)
    {
        for (int i=0; i<n; i++)
            patrolPath.Add(initialPosition + Vector3.left * steps);
    }

    private void ResetPath(List<Vector3> path = null, PathFollowing.Mode mode = PathFollowing.Mode.stay)
    {
        if (pathFollowing == null)
            pathFollowing = new GameObject("PathFollowing").AddComponent<PathFollowing>();
        pathFollowing.path = path;
        pathFollowing.currentNode = 0;
        pathFollowing.pathDir = 1;
        pathFollowing.mode = mode;
        AddTarget(pathFollowing);
    }

    public override void AddTarget(SteeringBehaviour sb) {
        if (this.steeringBehaviours == null)
            this.steeringBehaviours = new List<SteeringBehaviour>();
        if (!this.steeringBehaviours.Contains(sb))
            this.steeringBehaviours.Add(sb);
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

    AgentUnit FindClosestEnemy()
    {
        AgentUnit targetedEnemy = null;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange*2.5f);

        float minDistance = Mathf.Infinity; 
        foreach (Collider collider in hitColliders)
        {
            AgentUnit agentUnit = collider.gameObject.GetComponent<AgentUnit>();
            if (agentUnit != null && team != agentUnit.team)
            {
                float distance = (position - agentUnit.position).magnitude;
                if (distance < minDistance) {
                    targetedEnemy = agentUnit;
                    minDistance = distance;
                }
            }
        }

        return targetedEnemy;
    }

    void ChooseAction()
    {
        if (GameManager.IsBaseUnderAttack(team))
        {
            DefendMode();
        }
        else if (state != State.dying && state != State.healing)
        {
            switch (strategyMode)
            {
                case GameManager.StrategyMode.TOTALWAR:
                    AttackMode();
                    break;
                case GameManager.StrategyMode.ATTACK:
                    if (unitType == CombatSystem.UnitType.Ranged)
                        NeutralMode();
                    else
                        AttackMode();
                    break;
                case GameManager.StrategyMode.DEFEND:
                    DefendMode();
                    break;
                default:
                    NeutralMode();
                    break;
            }
        }
    }


    public void AttackMode()
    {
        AgentUnit enemy = FindClosestEnemy();
        // If there is an enemy in sight, attack it
        if (enemy != null) {
            if (targetedEnemy == null) {
                targetedEnemy = enemy;
                Attack(enemy);
            }
            else {
                float d1 = (position - targetedEnemy.position).magnitude;
                float d2 = (position - enemy.position).magnitude;
                if (d2 < d1) {
                    targetedEnemy = enemy;
                    Attack(enemy);
                }
            }
        }
        // Attack enemy base
        else {
            if (state != State.routed) {
                // If team is 0, move to enemy base 1
                if (team == 0 && terrain != CombatSystem.TerrainType.BaseB)
                    Move(enemyBasePosition, CombatSystem.TerrainType.BaseB);
                // If team is 1, move to enemy base 0
                if (team == 1 && terrain != CombatSystem.TerrainType.BaseA)
                    Move(enemyBasePosition, CombatSystem.TerrainType.BaseA);
            }
        }
    }


    public void DefendMode()
    {
        // If I'm already defending the base, find closest enemy
        if (state == State.defending)
        {
            AgentUnit enemy = FindClosestEnemy();
            // If there is an enemy in sight, attack it
            if (enemy != null) {
                targetedEnemy = enemy;
                Attack(enemy);
            }
            // If no enemy and hp not full, go recharge
            else if (hp < maxHP) {
                Heal();
            }
        }
        // Go back to defend team base  
        else if (state != State.attacking)
        {
            if (state != State.routed) {
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
        }
    }


    public void NeutralMode()
    {
        // If I'm already patrolling my zone, find closest enemy
        if (state == State.patrolling)
        {
            AgentUnit enemy = FindClosestEnemy();
            // If there is an enemy in sight, attack it
            if (enemy != null) {
                targetedEnemy = enemy;
                Attack(enemy);
            }
            // If no enemy and hp not full, go recharge
            else if (hp < maxHP) {
                Heal();
            }
        }
        // Go back to patrolling
        else if (state != State.attacking)
        {
            if (state != State.routed) {
                float distance = (this.position - initialPosition).magnitude;
                if (distance > this.exteriorRadius)
                    Move(initialPosition);
                else if (patrolPath.Count > 0)
                    Patrol();
            }
        }
    }

    public void ChangeMode(GameManager.StrategyMode mode)
    {
        strategyMode = mode;
        state = State.none;
        ResetAction();

    }

    public void ResetAction()
    {
        action = null;
        canMove = true;
        color = colorOriginal;
        InitializeSteerings(); // HACK
    }


    public void ExecuteAction()
    {
        if (action != null) {
            action.Execute();
            if (action.IsComplete()) {
                ResetAction();
                if (state == State.dying) {
                    Move(deathPosition);
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
                GameManager.CountKilled(attacker, this);
                Die();
                return true;
            }
            // If health is low and speed is high, find healer
            else if (hp < (maxHP / 2) && hp < attacker.hp && maxSpeed >= attacker.maxSpeed) {
                if (state != State.healing)
                    Heal();
                return true;
            }
            // If attacked, respond to attack
            else {
                if (state != State.attacking)
                    Attack(attacker);
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

}