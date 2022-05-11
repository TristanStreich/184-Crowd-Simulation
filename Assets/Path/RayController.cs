using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayController : MonoBehaviour
{

    public float visibleLength;
    [Range(0,64)]
    public int numRays;
    public float visibilityAngle;
    public bool visible;
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
    List<RaycastHit?> rayHits;
    float hasHitPeopleMin;
    int hasHitPeople;
    public RaycastHit?  peopleHit;
    // Start is called before the first frame update
    void Start()
    {
        visibilityAngle = Random.Range(100f,140f);
        visibleLength = Random.Range(8f,20f);
        visibleRays = new List<GameObject>();
        rays = new List<Ray>();
        rayHits = new List<RaycastHit?>();
        body = GetComponent<Rigidbody>();
        body.velocity = initalVelocity;
        initialSpeed = initalVelocity.magnitude;
        if (numRays % 2 == 0) numRays++; //Rays should be an odd number so there is one dead ahead
    }

    // Update is called once per frame
    void Update()
    {
        
        clearRays();
        generateRays();

    }

    public void clearRays(){
        hasHitPeople = -1;
        hasHitPeopleMin = -1f;
        peopleHit = null;
        foreach (GameObject rayObj in visibleRays){
            Destroy(rayObj);
        }
        visibleRays.Clear();
        rays.Clear();
        rayHits.Clear();
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
            rayHits.Add(null);
            if (visible) drawRay(ray);
        }
    }

    //returns a ray object that is ANGLE num of degrees away from
    //the velocity. Positive is counter clockwise and negative is clockwise.
    Ray makeRay(float angle){
        Vector3 direction = rotate(transform.TransformDirection(Vector3.forward),angle);
        direction.Normalize();
        return new Ray(transform.position,direction,angle,maxDist:visibleLength);
    }

    //Draws a line representing the ray on the screen
    //Draws sets the color to blue if it does not have a collison and red otherwise
    //adds ray line renderer object to visbleRays list
    void drawRay(Ray ray){

        Color startingColor = Color.green;
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


        RaycastHit? len_ph = ray.getNearestPeopleCollision(); 
        //RaycastHit? len_wh = ray.getNearestWallCollision();
        RaycastHit? len_rh = ray.getNearestCollision();  
        float len = -1;
        if(len_rh != null){
            if(len_ph != null){
                len = ((RaycastHit)len_ph).distance;
                hasHitPeopleMin = (hasHitPeopleMin<0) || (len <hasHitPeopleMin)? len:hasHitPeopleMin;
                hasHitPeople = (hasHitPeople<0) || (len <hasHitPeopleMin)? rayHits.Count-1:hasHitPeople;
                rayHits[rayHits.Count-1] = len_rh;
                peopleHit = len_rh;
            }
            len = ((RaycastHit)len_rh).distance;
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
