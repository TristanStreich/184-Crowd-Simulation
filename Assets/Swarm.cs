using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour
{
    public float alpha = 0.5f, speed = 300f, r = 0.1f, repulsion = 100f;
    public GameObject target;

    [Header("Personality")]
    [Range(0,2)]
    public int personality = 0;

    List<Collider> colliders;
    Vector3 movement;
    Vector3 direction;
    Vector3 updateDirection;
    Quaternion rotation;
    Rigidbody rb;
    Vector3 basePos;
    
    Vector3 targetPos;
    

    // Start is called before the first frame update
    void Start()
    {
        colliders = new List<Collider>();
        movement = new Vector3(0f, 0f, 0f);
        rb = GetComponent<Rigidbody>();
        targetPos = target.transform.position;
        //repulsion = (1 + personality) / 2 * repulsion;
    }

    // Update is called once per frame
    void Update()
    {

        movement += new Vector3(Random.Range(-r, r), 0, Random.Range(-r, r));
        movement = Vector3.Normalize(movement);
        basePos = transform.position + speed * transform.forward * Time.deltaTime;
        basePos[1] = transform.position[1];
        rb.MovePosition(basePos);
        direction = Vector3.Normalize((1f - alpha) * movement);
        //updateDirection = Vector3.zero;
        updateDirection = targetPos - transform.position;
        updateDirection = updateDirection.normalized;
        print(colliders.Count);
        for (int i = 0; i < Mathf.Min(colliders.Count, 5); i++)
        {
            
            //Go the other way
            if (Vector3.Distance(transform.position, colliders[i].transform.position) < repulsion)
                updateDirection += Vector3.Normalize(transform.position - colliders[i].transform.position);
            //go twords others
            else
                updateDirection += colliders[i].transform.forward;
        }

        updateDirection = Vector3.Normalize(updateDirection);
        rotation = Quaternion.LookRotation(0.1f * direction + 0.5f * updateDirection + 0.5f * transform.forward, transform.up);
        rb.MoveRotation(rotation);
    }
}