using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int amount = 20;
    public GameObject unit;
    public int interval = 200;
    public int rangeX = 50;
    public int rangeZ = 50;

    private Transform spawnTransform;
    private int numofPeople = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        /* StartCoroutine(Spawn());*/
        spawnTransform.position = new Vector3(-150, 0.7f, -216);
        
    }

    private void Update()
    {
        if (numofPeople < amount)
        {
            if (Time.frameCount % interval == 0)
            {
                float spawnX = Random.Range(transform.position.x - rangeX, transform.position.x + rangeX);
                float spawnZ = Random.Range(transform.position.z - rangeZ, transform.position.z + rangeZ);
                Vector3 spawnPos = new Vector3(spawnX, 0.7f, spawnZ);
                Instantiate(unit, spawnPos, Quaternion.identity);
                numofPeople++;
            }
            
        }
        
    }
    /*
   IEnumerator Spawn()
   {
       


               for(int i=0; i<amount; i++)
               {
                   int spawnZ = Random.Range(-212, -133);
                   Vector3 spawnPos = new Vector3(-91, 0.7f, spawnZ);
                   //new Vector3(UnityEngine.Random.Range(-150, -80), 0.7f,
                   //    UnityEngine.Random.Range(-216,-200));
                   Instantiate(unit, spawnPos, Quaternion.identity);//, Vector3.zero, Quaternion.identity);
                   yield return null;
               
}
    }*/
}
