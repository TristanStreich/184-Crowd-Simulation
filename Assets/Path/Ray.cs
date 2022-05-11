using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray
{
    public Vector3 position;
    public Vector3 direction;

    //angle from current velocity
    public float angle;

    float maxDist;

    public Ray(Vector3 position, Vector3 direction,float angle, float maxDist = 3000f){
        this.position = position;
        this.direction = direction;
        this.angle=angle;
        this.maxDist = maxDist;
    }

    //returns the distance to the nearest collission.
    //Does not collide with people.
    //returns -1f if there is no collision
    public RaycastHit? getNearestWallCollision(){
        RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit,
                            maxDistance:maxDist,
                            layerMask:LayerMask.NameToLayer("People"))){
            return hit;
        }
        return null;
    }

    public RaycastHit? getNearestPeopleCollision(){
        RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit,
                            maxDistance:maxDist,
                            ~(1 << 4))){
            return hit;
        }
        return null;
    }

    //returns the distance to the nearest collions.
    //Does collide with people
    //returns -1f if there is no collision
    public RaycastHit? getNearestCollision(){
                RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit, maxDistance:maxDist)){
            return hit;
        }
        return null;
    }
}
