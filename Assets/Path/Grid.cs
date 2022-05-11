using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public bool displayGridGizmos;
    public LayerMask unwalkableLayer;
    public LayerMask walkableLayer;
    public Vector2 gridSize;
    public float nodeRadius;
    public float nodeRadiusBuffer = 2;

    public Node[,] grid;

    protected float nodeDiameter;
    protected int gridSizeX, gridSizeY;

    void Awake(){
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }


    void CreateGrid(){
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;
        for (int x = 0; x < gridSizeX; x++){
            for (int y = 0; y < gridSizeY; y++){
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius * nodeRadiusBuffer, unwalkableLayer));
                float height = 0;
                bool isWalkable = walkable? true:false;
                grid[x, y] = new Node(isWalkable, worldPoint, x, y, height);
            }
        }
    }


    public List<Node> GetNeighbours(Node node){
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++){
            for (int y = -1; y <= 1; y++){
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x,  checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    public int MaxSize{
        get{
            return gridSizeX * gridSizeY;
        }
    }
    
    public Node NodeFromWorldPoint(Vector3 worldPos){
        int x = Mathf.RoundToInt((gridSizeX - 1) *  Mathf.Clamp01( (worldPos.x + gridSize.x / 2) / gridSize.x ));
        int y = Mathf.RoundToInt((gridSizeY - 1) *  Mathf.Clamp01( (worldPos.z + gridSize.y / 2) / gridSize.y ));
        return grid[x, y];
    }

    void OnDrawGizmos(){
		Gizmos.DrawWireCube(transform.position,new Vector3(gridSize.x,1,gridSize.y));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter-.1f));
			}
		}
    }

}
