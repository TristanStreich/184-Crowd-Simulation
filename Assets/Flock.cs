using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

	public FlockManager myManager;
	float speed;
	Vector3 direction;
	Vector3 targetpos;

	// Use this for initialization
	void Start()
	{
		speed = Random.Range(myManager.minSpeed,
								myManager.maxSpeed);
		targetpos = myManager.target.transform.position.normalized;
	}

	// Update is called once per frame
	void Update()
	{
		transform.Translate(Time.deltaTime * speed, 0, 0);
		//transform.Rotate(direction);
		ApplyRules();

	}
	void ApplyRules()
	{
		GameObject[] gos;
		gos = myManager.allFish;

		Vector3 vcentre = Vector3.zero;
		Vector3 vavoid = Vector3.zero;
		float gSpeed = 0.01f;
		float nDistance;
		int groupSize = 0;

		foreach (GameObject go in gos)
		{
			if (go != this.gameObject)
			{
				nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                if (nDistance <= myManager.neighbourDistance)
                {
					
                    vcentre += go.transform.position;
					groupSize++;

					vavoid =  vavoid +  10 * (this.transform.position - go.transform.position);
					
					Flock anotherFlock = go.GetComponent<Flock>();
					gSpeed = gSpeed + anotherFlock.speed;
                }
            }
		}
		if (groupSize > 0)
		{
			vcentre = vcentre / groupSize;
			speed = gSpeed / groupSize;
			
			Vector3 direction =  - transform.position + (vcentre + vavoid);

			if (direction != Vector3.zero)
				transform.Rotate(direction);
			/*	transform.rotation = Quaternion.Slerp(transform.rotation,
													  Quaternion.LookRotation(direction),
													  myManager.rotationSpeed );*/
				//float rotation = Quaternion.LookRotation(direction);
				print(transform.rotation);
				
		}
	}
}
