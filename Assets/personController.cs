using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class personController : MonoBehaviour
{

    
    public float visibleLength;
    public int numRays;
    [Range(0f,180f)]
    public float visibilityAngle;
    public bool visible;
    public bool detectPeople;
    public bool exponentialColoring;
    public Vector3 initalVelocity;
    [Range(0f,1f)]
    public float turnSpeed;
    [Range(0f,1f)]
    public float acceleration;
    public float interval;
    public int exponent;


    float lineWidth = 0.1f;
    List<GameObject> visibleRays;
    List<Ray> rays;
    Rigidbody body;
    float initialSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        visibleRays = new List<GameObject>();
        rays = new List<Ray>();
        body = GetComponent<Rigidbody>();
        body.velocity = initalVelocity;
        initialSpeed = initalVelocity.magnitude;
        if (numRays % 2 == 0) numRays++; //Rays should be an odd number so there is one dead ahead
    }

    // Update is called once per frame
    void Update()
    {
        acccelerate();
        clearRays();
        generateRays();

        nudge();
    }


    //if the person is not going their initial speed, accelerates them slightly;
    public void acccelerate(){
        body.velocity *= 1 + acceleration*(initialSpeed - body.velocity.magnitude)/body.velocity.magnitude;
    }

    // turns the agent away from upcoming obstacles.
    public void nudge(){
        //~~~~~~~~~~~~~~~~~~~~~~~~ Old Approach ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        float nudgeAngle = 0f;
        float distance;
        foreach (Ray ray in rays){
            distance = detectPeople ? ray.getNearestCollision() : ray.getNearestWallCollision();
            if (distance < 0) continue;
            float turnAngle = -(90f - Mathf.Abs(ray.angle))*Mathf.Sign(ray.angle);
            float scalingFactor = (visibleLength - distance)/visibleLength;
            scalingFactor = Mathf.Pow(scalingFactor,exponent);
            nudgeAngle += turnAngle*scalingFactor*turnSpeed;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //~~~~~~~~~~~~~~~~~~~~~~~ Attempt at new Approach ~~~~~~~~~~~~~~~~~~
        // float maxDistance = 0;
        // Ray maxRay = rays[0];
        // float dist;
        // foreach (Ray ray in rays){
        //     dist = detectPeople ? ray.getNearestCollision() : ray.getNearestWallCollision();
        //     dist = dist < 0 ? visibleLength : dist;
        //     if (dist > maxDistance || (dist == maxDistance && (Mathf.Abs(ray.angle) < Mathf.Abs(maxRay.angle)))){
        //         maxRay = ray;
        //         maxDistance = dist;
        //     }
        // }
        // float nudgeAngle = maxRay.angle*turnSpeed;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // doesn't update rotation every update
        if (Time.frameCount % interval == 0)
        {
            body.velocity = rotate(body.velocity, nudgeAngle);
        }
        
    }

    public void clearRays(){
        foreach (GameObject rayObj in visibleRays){
            Destroy(rayObj);
        }
        visibleRays.Clear();
        rays.Clear();
    }

    //creates rays fanning out from the person
    //appends to the rays list
    //draws the ray if visible set to true
    public void generateRays(){
        for (int i=0;i<numRays;i++){
            float angle = ((float) i)/(numRays-1)*visibilityAngle;
            angle -= visibilityAngle/2;
            Ray ray = makeRay(angle);
            rays.Add(ray);
            if (visible) drawRay(ray);
        }
    }

    //returns a ray object that is ANGLE num of degrees away from
    //the velocity. Positive is counter clockwise and negative is clockwise.
    Ray makeRay(float angle){
        Vector3 direction = rotate(body.velocity,angle);
        direction.Normalize();
        return new Ray(transform.position,direction,angle,maxDist:visibleLength);
    }

    //Draws a line representing the ray on the screen
    //Draws sets the color to blue if it does not have a collison and red otherwise
    //adds ray line renderer object to visbleRays list
    void drawRay(Ray ray){

        Color startingColor = Color.white;
        Color collColor = Color.red;

        GameObject rayObj = new GameObject();
        rayObj.name = "Visible Ray";

        LineRenderer drawLine = rayObj.AddComponent<LineRenderer>();

        drawLine.material = new Material(Shader.Find("Sprites/Default"));
        drawLine.startWidth = lineWidth;
        drawLine.endWidth = lineWidth;
        drawLine.startColor = startingColor;
        drawLine.endColor = startingColor;

        drawLine.positionCount = 2;

        float len = detectPeople ? ray.getNearestCollision() : ray.getNearestWallCollision();
        if (len >= 0){
            float r = (visibleLength - len)/visibleLength;
            r = exponentialColoring ? Mathf.Pow(r,exponent) : r;
            drawLine.startColor = startingColor*(1-r) + collColor*(r);
            drawLine.endColor = startingColor*(1-r) + collColor*(r);
        } else {
            len = visibleLength;
        }

        drawLine.SetPositions(new [] {ray.position, ray.position + ray.direction*len});


        visibleRays.Add(rayObj);
    }

    //returns a vector that is the input vector rotated around the y axis by ANGLE degrees counter clockwise
    public Vector3 rotate(Vector3 currVec, float angle){
        angle *= Mathf.Deg2Rad;
        float newX = currVec.x * Mathf.Cos(angle) - currVec.z * Mathf.Sin(angle);
        float newZ = currVec.x * Mathf.Sin(angle) + currVec.z * Mathf.Cos(angle);
        return new Vector3(newX,currVec.y,newZ);
    }
}
