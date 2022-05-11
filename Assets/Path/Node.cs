using UnityEngine;
using System.Collections;


public class Node : HeapItem<Node>
{

    public Vector3 worldPos;
    public float height;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    private bool isWalkable;
    private int index;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,float _height){
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        height = _height;
    }

    public int fCost{
        get{
            return gCost + hCost;
        }
    }

    public int HeapIndex{
        get{
            return index;
        }
        set{
            index = value;
        }
    }

    public bool walkable{
        get{
            return isWalkable;
        }
        set{
            isWalkable = value;
        }
        
    }

    public int CompareTo(Node nodeToCompare){
        int compare = -fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = -hCost.CompareTo(nodeToCompare.hCost);
        return compare;
    }

    public override bool Equals(object obj){
        return worldPos == ((Node)obj).worldPos;
    }
}
