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
    public float getNearestWallCollision(){
        RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit,
                            maxDistance:maxDist,
                            layerMask:LayerMask.NameToLayer("People"))){
            return hit.distance;
        }
        return -1f;
    }

    //returns the distance to the nearest collions.
    //Does collide with people
    //returns -1f if there is no collision
    public float getNearestCollision(){
                RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit, maxDistance:maxDist)){
            return hit.distance;
        }
        return -1f;
    }
}
