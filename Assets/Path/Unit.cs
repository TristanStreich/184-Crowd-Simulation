using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{

    public GameObject GridWorld;
    public bool drawGizmos = false;
    public float speed;
    public float rotationSpeed;
    private Quaternion lookLoc;
    private Grid m_grid;
    public Vector2 currentPos = new Vector2(0, 0);
    private Node lastNodePosition;
    private List<Node> lastNodeNeighbors;
    private Vector3 lastPositionVector;
    private Coroutine lastRoutine = null;
    public Transform target;
    public Vector3 lastTargetPos;
    public float unitArea = 5;
    public float stoppingPoint = 5;
    public int travelDist = 0;
    public float actTime = 5f;
    private Vector3[] pathFound;
    private int Index;
    private bool updateBuffer = false;
    private RayController rc;
    private Animator an = null;
    private bool canUpdate = false;
    private int paths = 0;
    private bool isMoving = false;
    private bool isTargetReached = false;

    public virtual void Awake()
    {
        if (GridWorld != null)
            m_grid = GridWorld.GetComponent<Grid>();
    }

    public virtual void Start()
    {
        an = GetComponent<Animator>();
        speed = Random.Range(5, 25);
        rotationSpeed = Random.Range(75, 125);
        rc = GetComponent<RayController>();
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        lastTargetPos = target.position;
    }

    public virtual void Update()
    {
        RaycastHit? hasCollision = rc.peopleHit;

        if (Time.time > actTime){
            actTime += 5f;
            canUpdate = true;
        }
        else
            canUpdate = false;
        

        if (canUpdate || (!isMoving && isTargetReached && !updateBuffer)){
            updateBuffer = true;
            UpdateNodePosition();
        }

        if ((canUpdate && travelDist % 10 == 0) || ( canUpdate && hasCollision != null && (((RaycastHit)hasCollision).transform.gameObject.GetComponent<Unit>() != null) && !(((RaycastHit)hasCollision).transform.gameObject.GetComponent<Unit>().isTargetReached) ))
            UpdatePath();
        else if (target.position != lastTargetPos){
            isMoving = true;
            UpdateNodePosition();
            UpdatePath();
        }

        lastTargetPos = target.position;

        lastPositionVector = target.transform.position;
        lookLoc = Quaternion.LookRotation(lastPositionVector - transform.position);

        if (transform.rotation != lookLoc)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookLoc, rotationSpeed * Time.deltaTime);

        an.SetBool("isRunning", isMoving);

    }

    public void UpdatePath(){
        lastNodePosition.walkable = true;
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public virtual void OnPathFound(Vector3[] newPath, bool fullPath){
        if (fullPath){
            paths++;
            pathFound = newPath;
            Index = 0;
            if (lastRoutine != null)
                StopCoroutine(lastRoutine);
            lastRoutine = StartCoroutine(FollowPath());
        }
    }

    public virtual IEnumerator FollowPath(){
        
        Vector3 unitStep;
        if (pathFound != null && pathFound.Length > 0)
            unitStep = pathFound[0];
        else
            unitStep = transform.position;

        while (true){
            if (Vector3.Distance(transform.position, unitStep) < unitArea){
                Index++;
                if (Index >= pathFound.Length){
                    isMoving = false;
                    yield break;
                }
                unitStep = pathFound[Index];
            }
            Vector3 forward = transform.TransformDirection(Vector3.forward) * stoppingPoint;
            RaycastHit? hasCollision = rc.peopleHit;
            if (hasCollision != null && ( (((RaycastHit)hasCollision).transform == target) || ( ( ((RaycastHit)hasCollision).transform.gameObject.GetComponent<Unit>() != null) &&
            (((RaycastHit)hasCollision).transform.gameObject.GetComponent<Unit>().isTargetReached) ) ) )
            {
                isMoving = false;
                isTargetReached = true;
                pathFound = null;
                yield break;
            }
            else if(!isTargetReached){
                isMoving = true;
                isTargetReached = false;
                transform.position = Vector3.MoveTowards(transform.position,unitStep,speed * Time.deltaTime);

            }
            yield return null;
        } 
    }

    public void UpdateNodePosition(){
        Node node = m_grid.NodeFromWorldPoint(transform.position);
        if (lastNodePosition != null && isMoving == false){
            lastNodeNeighbors = m_grid.GetNeighbours(node);
            foreach (Node n in lastNodeNeighbors){
                if (n.walkable != false)
                    n.walkable = false;
            }
            node.walkable = false;
            lastNodePosition = node;
            currentPos = new Vector2(node.gridX, node.gridY);
        }
        else if (lastNodePosition != null && isMoving){
            updateBuffer = false;
            lastNodeNeighbors = m_grid.GetNeighbours(node);
            lastNodePosition.walkable = true;
            if (lastNodeNeighbors != null)
                foreach (Node n in lastNodeNeighbors){
                    if (n.walkable != false)
                        n.walkable = true;
                }
            if (!node.Equals(lastNodePosition))
                travelDist++;
        }
        else{
            node.walkable = false;
            lastNodePosition = node;
            currentPos = new Vector2(node.gridX, node.gridY);
        }
    }

    public void OnDrawGizmos(){
        if (!drawGizmos)
            return;
        if (pathFound != null){
            for (int i = Index; i < pathFound.Length; i++){
                Gizmos.color = Color.black;
                Gizmos.DrawCube(pathFound[i], Vector3.one);
                if (i == Index)
                    Gizmos.DrawLine(transform.position, pathFound[i]);
                else
                    Gizmos.DrawLine(pathFound[i - 1], pathFound[i]);
            }
        }
    }


}

