using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    Grid grid;
    private Queue<PathRequest> pathQueue = new Queue<PathRequest>();
    private bool pathHunt;
    private PathRequest currentPath;

    private static PathRequestManager instance;

    void Awake()
    {
        instance = this;
        grid = GetComponent<Grid>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> trace){
        instance.pathQueue.Enqueue(new PathRequest(pathStart, pathEnd, trace));
        instance.findNext();
    }

    void findNext(){
        if (!pathHunt && pathQueue.Count > 0){
            currentPath = pathQueue.Dequeue();
            pathHunt = true;
            StartCoroutine(FindPath(currentPath.pathStart, currentPath.pathEnd));
        }
    }


    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos){

        Vector3[] pathSteps = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable != true){
            List<Node> neighbors = grid.GetNeighbours(startNode);
            foreach(Node n in neighbors){
                if (n.walkable == true) { 
                    startNode = n;
                    break;
                 }
            }
        }
        
        if (startNode.walkable == true && targetNode.walkable == true){
            Heap<Node> workingHeap = new Heap<Node>(grid.MaxSize);
            HashSet<Node> workingSet = new HashSet<Node>();

            workingHeap.Add(startNode);

            while (workingHeap.Count > 0){
                Node currentNode = workingHeap.RemoveFirst();
                workingSet.Add(currentNode);

                if (currentNode == targetNode){
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (neighbour.walkable != true || workingSet.Contains(neighbour))
                        continue;
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !workingHeap.Contains(neighbour)){
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!workingHeap.Contains(neighbour))
                            workingHeap.Add(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess){
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode){
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            pathSteps = simplifyPath(path);
            Array.Reverse(pathSteps);
        }
        currentPath.trace(pathSteps, pathSuccess);
        pathHunt = false;
        findNext();

    }



    Vector3[] simplifyPath(List<Node> path){
        List<Vector3> pathSteps = new List<Vector3>();
        Vector2 lastPoint = Vector2.zero;
        for (int i = 1; i < path.Count; i++){
            Vector2 newPoint = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (newPoint != lastPoint)
                pathSteps.Add(path[i].worldPos + Vector3.up );
            lastPoint = newPoint;
        }
        return pathSteps.ToArray();
    }

    int GetDistance(Node A, Node B){
        int dstX = Mathf.Abs(A.gridX - B.gridX);
        int dstY = Mathf.Abs(A.gridY - B.gridY);
        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


    struct PathRequest{
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> trace;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _trace){
            pathStart = _start;
            pathEnd = _end;
            trace = _trace;
        }
    }
}
