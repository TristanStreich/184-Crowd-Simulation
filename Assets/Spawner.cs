using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int amount = 10;
    public GameObject unit;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        for(int i=0; i<amount; i++)
        {
            Instantiate(unit);//, Vector3.zero, Quaternion.identity);
            yield return null;
        }
    }
}
