using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
    public float radius;
    public int power;
    public float damage;
    public int count;
    private Vector3 explosionPos;
    private Collider[] colliders;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void Explosion()
    {
        foreach (Collider hit in colliders)
        {
            if (hit.GetComponent<Rigidbody>() && !hit.CompareTag("Player"))
                hit.transform.GetComponent<Rigidbody>().AddExplosionForce(hit.GetComponent<Rigidbody>().mass * power, explosionPos, radius);
        }
        
        //Destroy(gameObject); НАДО РАЗОБРАТЬСЯ С ДАМАГАМИ
    }
}
