using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    const float TURN_SPEED = 300;

    [SerializeField]
    Rigidbody body;
    [SerializeField]
    new Collider collider;

    [SerializeField]
    UnitModel model;

    [SerializeField]
    AnimationListener listen;

    [SerializeField]
    Transform armRight;
    [SerializeField]
    Transform armLeft;

    [SerializeField]
    LayerMask actorLayer = -1;

    [SerializeField]
    float atkDuration = 1.8f;

    float atkTimer = 0;

    Vector3 movementDir;
    Vector3[] currentPath = null;
    int currentPathNode = 0;
    [SerializeField]
    State initState;

    public Vector3 MovementDir
    {
        get => movementDir;
        set
        {
            movementDir = value;
            currentPath = null;
            currentPathNode = 0;
        }
    }
    public FSM FSM { get; private set; }
    public float AngularMovementDir { get; private set; }

    public Vector3 CurrentDir => transform.forward.ProjectOnPlane(Vector3.up);
    public float AtkDuration => atkDuration;

    public Vector3? Target { get; set; }
    public bool Attacking => atkTimer > 0;

    Vector3 TargetDir => ((Vector3)Target - transform.position).ProjectOnPlane(Vector3.up).normalized;

    public event Action OnAttackBegin;
    public event Action OnAttackFinished;
    

    private void Awake()
    {
        Instantiate(model.ShieldObj, armLeft);
        Instantiate(model.MainWeapon.WeaponPrefab, armRight);
        listen.OnAttackHit += On_HitFrame;
        model.OnDeath += Model_OnDeath;

        SetupFSM();
    }

    private void SetupFSM()
    {
        FSM = new FSM();

        var states = GetComponentsInChildren<State>();
        for (int i = 0; i < states.Length; i++)
        {
            //if (model.IsLeader)
            //    Debug.Log($"STATE: {states[i].GetType().Name}");
            states[i].Init(FSM);
        }
        FSM.Transition(initState);
        
    }

    private void Model_OnDeath()
    {
        body.isKinematic = true;
        body.velocity = Vector3.zero;
        collider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (Attacking || model.Dead)
        {
            var vel = body.velocity;
            vel.x = 0;
            vel.z = 0;
            body.velocity = vel;
            return;
        }

        if (currentPath is null)
        {
            body.velocity = MovementDir * model.MoveSpeed * Time.fixedDeltaTime * 40;
        }
        else
        {
            Vector3 targetPos = currentPath[currentPathNode];
            movementDir = (targetPos - transform.position).normalized;
            body.velocity = MovementDir * model.MoveSpeed * Time.fixedDeltaTime * 40;
            if (Vector3.Distance(targetPos, transform.position) < 1f)
            {
                currentPathNode++;
                if (currentPathNode >= currentPath.Length)
                {
                    currentPath = null;
                    currentPathNode = 0;
                    MovementDir = Vector3.zero;
                }
            }
        }
    }

    private void Update()
    {
        if (model.Dead)
            return;
        
        FSM.OnUpdate();
        
        float angleToMoveTarget = Vector3.SignedAngle(CurrentDir, MovementDir, transform.up);
        if (Mathf.Abs(angleToMoveTarget) > 10)
        {
            AngularMovementDir = Mathf.Clamp(angleToMoveTarget/2, -1, 1);
            transform.forward = Quaternion.Euler(0, TURN_SPEED * Time.deltaTime * AngularMovementDir, 0) * CurrentDir;
        }
        else
        {
            AngularMovementDir = 0;
            if (MovementDir.magnitude > .1f)
            {
                transform.forward = MovementDir;
            }
        }

        if (Target is object && MovementDir.magnitude < 0.2f)
        {
            //print($"{name} is turning at {Target} - {AngularMovementDir}");
            float angleToViewTarget = Vector3.SignedAngle(CurrentDir, TargetDir, transform.up);
            if (Mathf.Abs(angleToMoveTarget) > 5)
            {
                AngularMovementDir = Mathf.Clamp(angleToViewTarget / 2, -1, 1);
                transform.forward = Quaternion.Euler(0, TURN_SPEED * Time.deltaTime * AngularMovementDir, 0) * CurrentDir;
            }
            else
            {
                AngularMovementDir = 0;
                transform.forward = TargetDir;
            }
            
        }

        //if (Input.GetMouseButtonDown(1))
        //{
        //    Attack(Vector3.zero);
        //}

        if (Attacking)
        {
            atkTimer -= Time.deltaTime;
            if (atkTimer < 0)
            {
                OnAttackFinished?.Invoke();
                atkTimer = 0;
                Target = null;
            }
        }

        //var mouse = Input.mousePosition;
        //var ray = Camera.main.ScreenPointToRay(mouse);
        //if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out var hit))
        //{
        //    MoveTowards(hit.point);
        //}

        #region OLD_CODE
        //var move = new Vector3
        //{
        //    x = Input.GetAxis("Horizontal"),
        //    z = Input.GetAxis("Vertical"),
        //};
        //if (move.magnitude > 0.1f)
        //{
        //    MovementDir = Vector3.ClampMagnitude(move, 1f);
        //}
        //else if (MovementDir.magnitude > 0.1f)
        //{
        //    movementDir = Vector3.zero;
        //}
        #endregion
    }

    public void MoveTowards(Vector3 pos)
    {
        //Debug.Log($"Move to {pos}");
        var array = NodeArray.Ins;

        if (!array.FindClosest(pos, out Vector2Int destinationNode))
        {
            print("DESTINATION NOT FOUND");
            return;
        }
        array.GetPos(destinationNode, out Vector3 destinationPosition);


        array.FindClosest(transform.position, out Vector2Int startNode);
        array.GetPos(startNode, out Vector3 startPosition);

        var path = AStar<Vector3>.Run(
            startPosition,
            Satisfy,
            GetNeighbours,
            GetHeuristic,
            1200
            );

        if (path is object && path.Count > 0)
        {
            currentPath = path.ToArray();
            currentPathNode = 0;
        }
        else
        {
            print($"PATH NOT FOUND: {(path is null ? "path is null" : "path is 0")}");
        }

        bool Satisfy(Vector3 pos) => Vector3.Distance(pos, destinationPosition) < 0.4f;

        Dictionary<Vector3, float> GetNeighbours(Vector3 pos)
        {
            Dictionary<Vector3, float> dic
                = new Dictionary<Vector3, float>();

            foreach (var item in array.GetNeighbours(pos))
            {
                dic.Add(item, 1);
            }

            return dic;
        }

        float GetHeuristic(Vector3 node)
        {
            return 1;
        }
    }
    public void Attack(Vector3 target)
    {
        Target = target;
        atkTimer = atkDuration;
        OnAttackBegin?.Invoke();
    }
    public void StopMove()
    {
        Target = null;
        currentPath = null;
        currentPathNode = 0;
        MovementDir = Vector3.zero;
    }
    private void On_HitFrame()
    {
        Collider[] colition = new Collider[16];
        int col = Physics.OverlapSphereNonAlloc(transform.position, model.AttackRange, colition, actorLayer);
        //print($"HIT FRAME: {col}");
        int c = 0;
        for (int i = 0; i < col; i++)
        {
            Vector3 f = transform.forward;
            Vector3 t = colition[i].transform.position - transform.position;
            var a = Vector3.Angle(f, t);

            if (a < 45)
            {
                colition[i].SendMessage("TakeDamage", new DamageParams(model.AtkDamage, model.CritChance, model.Team));
                c++;
            }
            if (c > 3)
            {
                break;
            }
        }
    }

    private void OnGUI()
    {
        //float a = Vector3.SignedAngle(CurrentDir, MovementDir, transform.up);
        //GUILayout.Label($"ANGULAR MOVEMENT: {AngularMovementDir}");
        //GUILayout.Label($"MOVEMENT: {CurrentDir}");
        //if (model.Team == Team.Blue)
        //GUILayout.Label($"MOVEMENTDIR MAGNITURE: {MovementDir.magnitude}");

        //GUILayout.Label($"CurrentState");
    }

    //private void OnDrawGizmos()
    //{
    //    //Gizmos.color = Color.cyan;
    //    //Gizmos.DrawLine(transform.position, transform.position + MovementDir);
    //    //Gizmos.color = Color.red;
    //    //Gizmos.DrawLine(transform.position, transform.position + CurrentDir);
    //}

}
